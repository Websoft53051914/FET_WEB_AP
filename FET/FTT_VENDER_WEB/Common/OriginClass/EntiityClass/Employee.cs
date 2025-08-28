using FTT_VENDER_WEB.Models.Handler;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;

namespace FTT_VENDER_WEB.Common.OriginClass.EntiityClass
{
    public partial class Employee : IDisposable
    {

        public enum RefType
        {
            EmpNo,
            AliasName,
            Ext
        }

        private string m_empno;

        private string m_deptcode;

        private string m_deptdesc;

        private string m_deptfulldesc;

        private string m_empname;

        private string m_engname;

        private string m_sex;

        private string m_aliasname;

        private string m_email;

        private string m_mobile;

        private string m_ext;

        private string m_titlename;

        private string m_titleengname;

        private string m_region;

        private string m_regionname;

        private string m_costcenter;

        private string m_locationcode;

        private string m_locationname;

        private DateTime m_entdate;

        private DateTime m_offdate;

        private DateTime m_finaldate;

        private string m_emptype;

        private string m_opid;

        private string m_loginid;

        private string m_repflg;

        private string m_rocid;

        private string m_agent;

        private string m_RetrieveRegion = "";

        private bool m_Result;

        private long m_rowsCount;

        private bool IsDisposed = false;

        private Container components = null;

        private RetrieveData m_retrieveData = new RetrieveEmpData();

        public long RowsCount => m_rowsCount;

        public string EmpNO
        {
            get
            {
                return m_empno;
            }
            set
            {
                if (value != "")
                {
                    m_Result = GetEmployeeData("empno", value);
                }
            }
        }

        public string DeptCode
        {
            get
            {
                return m_deptcode;
            }
            set
            {
                m_deptcode = value;
            }
        }

        public string DeptDesc
        {
            get
            {
                return m_deptdesc;
            }
            set
            {
                m_deptdesc = value;
            }
        }

        public string DeptFullDesc
        {
            get
            {
                return m_deptfulldesc;
            }
            set
            {
                m_deptfulldesc = value;
            }
        }

        public string EmployeeName
        {
            get
            {
                return m_empname;
            }
            set
            {
                m_empname = value;
            }
        }

        public string EnglishName
        {
            get
            {
                return m_engname;
            }
            set
            {
                m_engname = value;
            }
        }

        public string Sex
        {
            get
            {
                return m_sex;
            }
            set
            {
                m_sex = value;
            }
        }

        public string AliasName
        {
            get
            {
                return m_aliasname;
            }
            set
            {
                m_aliasname = value;
            }
        }

        public string eMail
        {
            get
            {
                return m_email;
            }
            set
            {
                m_email = value;
            }
        }

        public string Mobile
        {
            get
            {
                return m_mobile;
            }
            set
            {
                m_mobile = value;
            }
        }

        public string Ext
        {
            get
            {
                return m_ext;
            }
            set
            {
                m_ext = value;
            }
        }

        public string TitleName
        {
            get
            {
                return m_titlename;
            }
            set
            {
                m_titlename = value;
            }
        }

        public string TitleEngName
        {
            get
            {
                return m_titleengname;
            }
            set
            {
                m_titleengname = value;
            }
        }

        public string Region
        {
            get
            {
                return m_region;
            }
            set
            {
                m_region = value;
            }
        }

        public string RegionName
        {
            get
            {
                return m_regionname;
            }
            set
            {
                m_regionname = value;
            }
        }

        public string CostCenter
        {
            get
            {
                return m_costcenter;
            }
            set
            {
                m_costcenter = value;
            }
        }

        public string LocationCode
        {
            get
            {
                return m_locationcode;
            }
            set
            {
                m_locationcode = value;
            }
        }

        public string LocationName
        {
            get
            {
                return m_locationname;
            }
            set
            {
                m_locationname = value;
            }
        }

        public DateTime EntryDate
        {
            get
            {
                return m_entdate;
            }
            set
            {
                m_entdate = value;
            }
        }

        public DateTime OffDate
        {
            get
            {
                return m_offdate;
            }
            set
            {
                m_offdate = value;
            }
        }

        public DateTime FinalDate
        {
            get
            {
                return m_finaldate;
            }
            set
            {
                m_finaldate = value;
            }
        }

        public string EmployeeType
        {
            get
            {
                return m_emptype;
            }
            set
            {
                m_emptype = value;
            }
        }

        public string OpID
        {
            get
            {
                return m_opid;
            }
            set
            {
                m_opid = value;
            }
        }

        public string LoginID
        {
            get
            {
                return m_loginid;
            }
            set
            {
                m_loginid = value;
            }
        }

        public string RepFlg
        {
            get
            {
                return m_repflg;
            }
            set
            {
                m_repflg = value;
            }
        }

        public string RocID
        {
            get
            {
                return m_rocid;
            }
            set
            {
                m_rocid = value;
            }
        }

        public string Agent
        {
            get
            {
                return m_agent;
            }
            set
            {
                m_agent = value;
            }
        }
        ~Employee()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (!IsDisposed && Disposing)
            {
                if (m_rowsCount > 0)
                {
                    m_empno = null;
                    m_deptcode = null;
                    m_deptdesc = null;
                    m_deptfulldesc = null;
                    m_empname = null;
                    m_engname = null;
                    m_sex = null;
                    m_aliasname = null;
                    m_email = null;
                    m_mobile = null;
                    m_ext = null;
                    m_titlename = null;
                    m_titleengname = null;
                    m_region = null;
                    m_regionname = null;
                    m_costcenter = null;
                    m_locationcode = null;
                    m_locationname = null;
                    m_entdate = DateTime.MinValue;
                    m_offdate = DateTime.MinValue;
                    m_finaldate = DateTime.MinValue;
                    m_emptype = null;
                    m_opid = null;
                    m_loginid = null;
                    m_repflg = null;
                    m_rocid = null;
                    m_agent = null;
                    m_RetrieveRegion = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            IsDisposed = true;
        }

        public bool hasData()
        {
            if (m_rowsCount > 0)
            {
                return true;
            }
            return false;
        }
    }
    public partial class Employee
    {

        public Employee(RefType DataType, string Data)
        {
            string sType = ConvertEmpType(DataType);
            m_Result = GetEmployeeData(sType, Data);
        }
        public Employee(bool authAD, string acc, string pwd, string region, bool leave, string syscode)
        {
            setEmployee(authAD, acc, pwd, region, leave);
            if (hasData())
            {
                BaseDBHandler baseHandler = new BaseDBHandler();
                if (AliasName.StartsWith("v_") && !baseHandler.CheckDataExist("FET_EXTEND_USER", $"ENABLED='Y' AND SYS_CODE='{syscode}' AND (EMPNO='{EmpNO}' OR DEPTCODE='{DeptCode}')"))
                {
                    m_rowsCount = 0L;
                }
            }
            else
            {
                m_rowsCount = 0L;
            }
        }

        private void setEmployee(bool authAD, string acc, string pwd, string region, bool leave)
        {
            bool flag = false;
            if (authAD)
            {
                if (!"".Equals(acc) && !"".Equals(pwd))
                {
                    string text = "fareastone.com.tw";
                    try
                    {
                        LdapAuthentication ldapAuthentication = new LdapAuthentication(text);
                        if (ldapAuthentication.IsAuthenticated(text, acc, pwd))
                        {
                            flag = true;
                        }
                        else
                        {
                            m_Result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        m_Result = false;
                    }
                }
                else
                {
                    m_Result = false;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                m_Result = GetEmployeeData(acc, region, leave);
            }
            else
            {
                m_Result = false;
            }
        }

        private bool GetEmployeeData(string sType, string sData)
        {
            string text = "";
            if (m_RetrieveRegion != "")
            {
                text += m_RetrieveRegion;
            }
            DataTable dataTable = m_retrieveData.RetrieveDBData(sType + " = '" + sData + "'" + m_RetrieveRegion);
            m_rowsCount = dataTable.Rows.Count;
            if (m_rowsCount == 0)
            {
                return false;
            }
            m_empno = dataTable.Rows[0]["empno"].ToString();
            m_deptcode = dataTable.Rows[0]["deptcode"].ToString();
            m_deptdesc = dataTable.Rows[0]["deptchiname"].ToString() + "(" + dataTable.Rows[0]["sdeptname"].ToString() + ")";
            m_deptfulldesc = dataTable.Rows[0]["deptnamelist"].ToString();
            m_empname = dataTable.Rows[0]["empname"].ToString();
            m_engname = dataTable.Rows[0]["engname"].ToString();
            m_sex = dataTable.Rows[0]["sex"].ToString();
            m_aliasname = dataTable.Rows[0]["aliasname"].ToString();
            m_email = dataTable.Rows[0]["email"].ToString();
            m_mobile = dataTable.Rows[0]["mobile"].ToString();
            m_ext = dataTable.Rows[0]["ext"].ToString();
            m_titlename = dataTable.Rows[0]["titlename"].ToString();
            m_titleengname = dataTable.Rows[0]["titleengname"].ToString();
            m_region = dataTable.Rows[0]["region"].ToString();
            m_regionname = dataTable.Rows[0]["regionname"].ToString();
            m_costcenter = dataTable.Rows[0]["costcenter"].ToString();
            m_locationcode = dataTable.Rows[0]["locationcode"].ToString();
            m_locationname = dataTable.Rows[0]["locationname"].ToString();
            if (!dataTable.Rows[0]["entdate"].Equals(DBNull.Value))
            {
                m_entdate = Convert.ToDateTime(dataTable.Rows[0]["entdate"]);
            }
            if (!dataTable.Rows[0]["offdate"].Equals(DBNull.Value))
            {
                m_offdate = Convert.ToDateTime(dataTable.Rows[0]["offdate"]);
            }
            if (!dataTable.Rows[0]["finaldate"].Equals(DBNull.Value))
            {
                m_finaldate = Convert.ToDateTime(dataTable.Rows[0]["finaldate"]);
            }
            m_emptype = dataTable.Rows[0]["emptype"].ToString();
            m_opid = dataTable.Rows[0]["opid"].ToString();
            m_loginid = dataTable.Rows[0]["loginid"].ToString();
            m_repflg = dataTable.Rows[0]["repflg"].ToString();
            m_rocid = dataTable.Rows[0]["rocid"].ToString();
            m_agent = dataTable.Rows[0]["agent"].ToString();
          
            dataTable.Dispose();
            return true;
        }
        private bool GetEmployeeData(string acc, string region, bool leave)
        {
            DataTable dataTable = m_retrieveData.RetrieveDBData(acc, region, leave);
            m_rowsCount = dataTable.Rows.Count;
            if (m_rowsCount == 0)
            {
                return false;
            }

            m_empno = dataTable.Rows[0]["empno"].ToString();
            m_deptcode = dataTable.Rows[0]["deptcode"].ToString();
            m_deptdesc = dataTable.Rows[0]["deptchiname"].ToString() + "(" + dataTable.Rows[0]["sdeptname"].ToString() + ")";
            m_deptfulldesc = dataTable.Rows[0]["deptnamelist"].ToString();
            m_empname = dataTable.Rows[0]["empname"].ToString();
            m_engname = dataTable.Rows[0]["engname"].ToString();
            m_sex = dataTable.Rows[0]["sex"].ToString();
            m_aliasname = dataTable.Rows[0]["aliasname"].ToString();
            m_email = dataTable.Rows[0]["email"].ToString();
            m_mobile = dataTable.Rows[0]["mobile"].ToString();
            m_ext = dataTable.Rows[0]["ext"].ToString();
            m_titlename = dataTable.Rows[0]["titlename"].ToString();
            m_titleengname = dataTable.Rows[0]["titleengname"].ToString();
            m_region = dataTable.Rows[0]["region"].ToString();
            m_regionname = dataTable.Rows[0]["regionname"].ToString();
            m_costcenter = dataTable.Rows[0]["costcenter"].ToString();
            m_locationcode = dataTable.Rows[0]["locationcode"].ToString();
            m_locationname = dataTable.Rows[0]["locationname"].ToString();
            if (!dataTable.Rows[0]["entdate"].Equals(DBNull.Value))
            {
                m_entdate = Convert.ToDateTime(dataTable.Rows[0]["entdate"]);
            }
            if (!dataTable.Rows[0]["offdate"].Equals(DBNull.Value))
            {
                m_offdate = Convert.ToDateTime(dataTable.Rows[0]["offdate"]);
            }
            if (!dataTable.Rows[0]["finaldate"].Equals(DBNull.Value))
            {
                m_finaldate = Convert.ToDateTime(dataTable.Rows[0]["finaldate"]);
            }
            m_emptype = dataTable.Rows[0]["emptype"].ToString();
            m_opid = dataTable.Rows[0]["opid"].ToString();
            m_loginid = dataTable.Rows[0]["loginid"].ToString();
            m_repflg = dataTable.Rows[0]["repflg"].ToString();
            m_rocid = dataTable.Rows[0]["rocid"].ToString();
            m_agent = dataTable.Rows[0]["agent"].ToString();
            dataTable.Dispose();
            return true;
        }

        private string ConvertEmpType(RefType DataType)
        {
            string result = "";
            switch (DataType)
            {
                case RefType.EmpNo:
                    result = "empno";
                    break;
                case RefType.AliasName:
                    result = "aliasname";
                    break;
                case RefType.Ext:
                    result = "ext";
                    break;
            }
            return result;
        }
    }


}
