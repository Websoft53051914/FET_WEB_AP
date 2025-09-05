using System;
using System.Data;
using Npgsql;   // dotnet add package Npgsql
using System.IO;
using System.Text;

public class FETTaskHelper
{
    private StreamWriter logFile;
    private readonly string logPath;
    private NpgsqlConnection conn;

    private readonly string connStr =
        "Host=10.68.68.236;Port=5445;Database=infwf;Username=ftt;Password=ftt!admin;";

    public FETTaskHelper(string logPath)
    {
        this.logPath = logPath;
        OpenLogFile(logPath);
    }

    public void Open()
    {
        conn = new NpgsqlConnection(connStr);
        conn.Open();
    }

    public void Close()
    {
        conn?.Close();
        CloseLogFile();
    }

    // ========= 主流程 =========
    public void Send_TT_No_RootCause(string groupid)
    {
        string sql = @"SELECT * FROM APPROVE_FORM 
                       WHERE status='DISPATCH' 
                         AND form_no IN (
                             SELECT FORM_NO FROM V_FTT_FORM WHERE STATUSID='DISPATCH'
                         )";

        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                InsertLog("eof get TT data !");
                return;
            }

            InsertLog("get TT data ! SQL=" + sql);

            while (reader.Read())
            {
                string formNo = ValidateField(reader["form_no"]);

                UpdateForm(formNo);
                InsertFormDesc(formNo);
            }
        }
        catch (Exception ex)
        {
            ErrorLog("get TT data fail! " + sql, ex);
        }
    }

    private void UpdateForm(string formNo)
    {
        string sql = @"UPDATE approve_form 
                       SET status='ASSIGN', update_empno='', update_engname='SYSTEM' 
                       WHERE form_no=@formNo";

        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@formNo", formNo);
            cmd.ExecuteNonQuery();

            InsertLog($"update approve_form NO={formNo} Successful!");
        }
        catch (Exception ex)
        {
            ErrorLog($"update approve_form NO={formNo} Fail!", ex);
        }
    }

    private void InsertFormDesc(string formNo)
    {
        string sql = @"INSERT INTO ftt_form_desc 
                       (form_no,user_type,action_name,description,prior_status,status) 
                       VALUES (@formNo,'','SYSTEM','系統自動派單','DISPATCH','ASSIGN')";

        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@formNo", formNo);
            cmd.ExecuteNonQuery();

            InsertLog($"insert into ftt_form_desc NO={formNo} Successful!");
        }
        catch (Exception ex)
        {
            ErrorLog($"insert into ftt_form_desc NO={formNo} Fail!", ex);
        }
    }

    // ========= 輔助函式 =========
    public string ValidateField(object data)
    {
        if (data == DBNull.Value || data == null) return "";
        return data.ToString().Trim();
    }

    public string TransDateStr(DateTime dt)
    {
        return dt.ToString("yyyy/MM/dd HH:mm:ss");
    }

    // ========= Log =========
    private void OpenLogFile(string filename)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filename));
        logFile = new StreamWriter(filename, true, Encoding.UTF8);
    }

    private void CloseLogFile()
    {
        logFile?.Close();
    }

    public void InsertLog(string message)
    {
        logFile.WriteLine($"({TransDateStr(DateTime.Now)}) {message}");
        logFile.Flush();
    }

    public void ErrorLog(string message, Exception ex)
    {
        InsertLog("--- Error Handler ---");
        InsertLog("--- " + message + " ---");
        InsertLog("Error: " + ex.Message);
    }

    // ========= 其他功能 =========
    public string ShowTTStatusDesc(string status)
    {
        return status?.ToLower() switch
        {
            "open" => "新開單",
            "process" => "處理中",
            "pending" => "待料中",
            "待料中" => "待料中",
            "completed" => "處理完成",
            "close" => "結案",
            "退件" => "退件",
            "暫存" => "暫存",
            _ => status
        };
    }

    public string GetEmpDept(string empNo)
    {
        string sql = $"SELECT deptcode FROM fet_user_profile WHERE empno=@empno OR aliasname=@empno";
        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@empno", empNo);
            var result = cmd.ExecuteScalar();
            return ValidateField(result);
        }
        catch
        {
            return "";
        }
    }

    public string GetOpName(string empNoOrAlias)
    {
        string sql = $"SELECT empname, engname FROM fet_user_profile WHERE empno=@data OR aliasname=@data";
        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@data", empNoOrAlias);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return $"{ValidateField(reader["empname"])}({ValidateField(reader["engname"])})";
            }
        }
        catch { }
        return $"({empNoOrAlias})";
    }

    public string GetEmpName(string type, string data)
    {
        string sql;
        if (type.ToLower() == "empno")
            sql = "SELECT empname, engname FROM FET_User_Profile WHERE empno=@data";
        else if (type.ToLower() == "nt")
            sql = "SELECT empname, engname FROM FET_User_Profile WHERE lower(aliasname)=lower(@data) AND finaldate IS NULL";
        else if (type.ToLower() == "fetc")
            sql = "SELECT empname, engname FROM FETC_User_Profile WHERE lower(aliasname)=lower(@data) AND finaldate IS NULL";
        else
            sql = "SELECT empname, engname FROM FET_User_Profile WHERE ext=@data AND finaldate IS NULL";

        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@data", data);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return $"{ValidateField(reader["EmpName"])}({ValidateField(reader["EngName"])})";
            }
        }
        catch { }
        return data;
    }

    public string GetEmpAlias(string empno)
    {
        if (string.IsNullOrEmpty(empno)) return "";
        string sql = "SELECT aliasname FROM fet_user_profile WHERE empno=@empno AND finaldate IS NULL";
        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@empno", empno);
            return ValidateField(cmd.ExecuteScalar());
        }
        catch { return ""; }
    }

    public string GetFullDeptCode(string deptcode, string orgDeptCode = "", int totalLoop = 0)
    {
        totalLoop++;
        if (totalLoop > 20)  // 遞迴保護避免死循環
            return orgDeptCode;

        // 如果 GetEmpDept 有回傳，改用那個部門代碼
        string targetDept = GetEmpDept(deptcode);
        string sql = string.IsNullOrEmpty(targetDept)
            ? "SELECT deptcode, parent, deptlevelname FROM fet_dept_profile WHERE deptcode=@deptcode"
            : "SELECT deptcode, parent, deptlevelname FROM fet_dept_profile WHERE deptcode=@deptcode";

        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@deptcode", string.IsNullOrEmpty(targetDept) ? deptcode : targetDept);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return deptcode; // 找不到就回傳原始代碼

            string currDept = ValidateField(reader["deptcode"]);
            string parent = ValidateField(reader["parent"]);
            string level = ValidateField(reader["deptlevelname"]).ToLower();

            reader.Close();

            if (string.IsNullOrEmpty(orgDeptCode))
                orgDeptCode = currDept;
            else
                orgDeptCode = currDept + "," + orgDeptCode;

            if (level == "chairman" || level == "devision" ||
                level == "president" || level == "vicechairman")
            {
                return orgDeptCode;
            }
            else
            {
                return GetFullDeptCode(parent, orgDeptCode, totalLoop);
            }
        }
        catch
        {
            return deptcode;
        }
    }

    public string GetDeptNameList(string deptcode)
    {
        string sql = "SELECT get_dept_desc(@deptcode) AS deptname";
        try
        {
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@deptcode", deptcode);
            var result = ValidateField(cmd.ExecuteScalar());
            return result.Length < 6 ? "" : result;
        }
        catch { return ""; }
    }

    public string GetGroupListDesc(string groupList)
    {
        if (string.IsNullOrEmpty(groupList)) return "";

        var items = groupList.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        foreach (var item in items)
        {
            string deptName = GetDeptNameList(item);
            if (!string.IsNullOrEmpty(deptName))
            {
                result.Append(deptName + ",");
            }
            else
            {
                string empName = GetEmpName("empno", item);
                if (!string.IsNullOrEmpty(empName))
                    result.Append(empName + ",");
                else
                    result.Append("(" + item + "),");
            }
        }

        return result.ToString().TrimEnd(',');
    }
}
}
