using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System.ServiceModel;
using System.Text;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class QuoteMgtHanlder : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public QuoteMgtHanlder(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal System.Data.DataTable GetCategoryConfig()
        {
            string strSql = "SELECT A.CISID AS CISID, A.CINAME AS CINAME, ACINAME AS ACINAME, L1NAME AS L1NAME, L2NAME AS L2NAME, L3NAME AS L3NAME, L4NAME AS L4NAME, B.SELFCONFIG AS 門市可自行尋商 FROM CI_DATA A LEFT JOIN CI_RELATIONS_CATEGORY B ON B.CISID=A.CISID WHERE A.CICATEGORY=1006 ORDER BY ACINAME";

            var dataTable = dbHelper.FindDataTable(strSql, null);

            return dataTable;
        }

        internal System.Data.DataTable GetCategoryData()
        {
            string strSql = "SELECT CISID AS CISID, CINAME AS 報修類別名稱 FROM CI_RELATIONS WHERE PARENTSID=1006 AND DISABLE IS NULL ORDER BY CISID";

            var dataTable = dbHelper.FindDataTable(strSql, null);

            return dataTable;
        }

        internal System.Data.DataTable GetQueryData()
        {
            string strSql = @"SELECT '' AS 更新註記,ID AS ID, CATEGORY_ID AS CISID, CATEGORY_NAME 報修類別名稱, EXPENSE_TYPE AS 費用種類, L1_DESC AS 維修細項_L1, L2_DESC 維修細項_L2, L3_DESC 維修細項_L3, UNIT AS 單位, QTY AS 數量, PRICE AS 單價, REMARK AS 備註 FROM AMOUNT_SELECT WHERE ENABLE='Y' ORDER BY CATEGORY_ID, ID";

            var dataTable = dbHelper.FindDataTable(strSql, null);

            return dataTable;
        }

        internal string? Import(string destFilePath)
        {
            IWorkbook wk;

            string ext = Path.GetExtension(destFilePath).ToLower();

            // 判斷副檔名
            using (FileStream fs = new FileStream(destFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                if (ext == ".xlsx")
                {
                    wk = new XSSFWorkbook(fs); // XSSF 讀取 .xlsx
                }
                else if (ext == ".xls")
                {
                    wk = new HSSFWorkbook(fs); // HSSF 讀取 .xls
                }
                else
                {
                    return "檔案格式錯誤，只能上傳 Excel (.xls / .xlsx)";
                }
            }

            for (int k = 0; k < wk.NumberOfSheets; k++)
            {
                ISheet hst = wk.GetSheetAt(k);

                if (hst.SheetName == "Sheet1")
                {
                    IRow hr = hst.GetRow(0);

                    for (int j = 1; j <= hst.LastRowNum; j++)
                    {
                        int line = j + 1;

                        hr = hst.GetRow(j);

                        string strFlag = hr.GetCell(0) == null ? "0" : hr.GetCell(0).ToString().Trim();
                        string strID = hr.GetCell(1) == null ? "0" : hr.GetCell(1).ToString().Trim();
                        string strCisid = hr.GetCell(2) == null ? "" : hr.GetCell(2).ToString().Trim();
                        string strExpenseType = hr.GetCell(4) == null ? "" : hr.GetCell(4).ToString().Trim();
                        string strL1Desc = hr.GetCell(5) == null ? "" : hr.GetCell(5).ToString().Trim();
                        string strL2Desc = hr.GetCell(6) == null ? "" : hr.GetCell(6).ToString().Trim();
                        string strL3Desc = hr.GetCell(7) == null ? "" : hr.GetCell(7).ToString().Trim();
                        string strUnit = hr.GetCell(8) == null ? "" : hr.GetCell(8).ToString().Trim();
                        string strQty = hr.GetCell(9) == null ? "null" : hr.GetCell(9).ToString().Trim();
                        string strPrice = hr.GetCell(10) == null ? "null" : hr.GetCell(10).ToString().Trim();
                        string strRemark = hr.GetCell(11) == null ? "" : hr.GetCell(11).ToString().Trim();

                        var intCisid = 0;

                        if (int.TryParse(strCisid, out intCisid) == false)
                        {
                            return $"第 {line.ToString()} 列 CISID 應為整數!";
                        }

                        double intQty = 0;

                        if (double.TryParse(strQty, out intQty) == false)
                        {
                            return $"第 {line.ToString()} 列 數量 應為數值!";
                        }

                        int intPrice = 0;

                        if (int.TryParse(strPrice, out intPrice) == false)
                        {
                            return $"第 {line.ToString()} 列 單價 應為整數!";
                        }

                        var paras = new Dictionary<string, object>();

                        if (strFlag == "D")
                        {
                            paras = new Dictionary<string, object>();
                            paras.Add("strID", strID);
                            paras.Add("MODIFY_OPERATOR", LoginSession.Current.empno);

                            //string deleteSql = string.Format("UPDATE AMOUNT_SELECT SET ENABLE='N', UPDATE_TIME=SYSDATE, MODIFY_OPERATOR='{1}' WHERE ENABLE='Y' AND ID={0}", strID, Context.User.Identity.Name);
                            string deleteSql = "UPDATE AMOUNT_SELECT SET ENABLE='N', UPDATE_TIME=SYSDATE, MODIFY_OPERATOR=@MODIFY_OPERATOR WHERE ENABLE='Y' AND ID=@strID ";
                            this.dbHelper.Execute(deleteSql, paras);
                            this.dbHelper.Commit();
                        }
                        else if (strFlag == "A")
                        {
                            paras = new Dictionary<string, object>();
                            paras.Add("strCisid", intCisid);
                            paras.Add("strExpenseType", strExpenseType.Replace("'", "''"));
                            paras.Add("strL1Desc", strL1Desc.Replace("'", "''"));
                            paras.Add("strL2Desc", strL2Desc.Replace("'", "''"));
                            paras.Add("strL3Desc", strL3Desc.Replace("'", "''"));
                            paras.Add("strUnit", strUnit);
                            paras.Add("strQty", intQty);
                            paras.Add("strPrice", intPrice);
                            paras.Add("strRemark", strRemark.Replace("'", "''"));
                            paras.Add("MODIFY_OPERATOR", LoginSession.Current.empno);
                            //string insertSql = string.Format("INSERT INTO AMOUNT_SELECT (CATEGORY_ID, EXPENSE_TYPE, L1_DESC, L2_DESC, L3_DESC, UNIT, QTY, PRICE, REMARK, MODIFY_OPERATOR) VALUES ({0},'{1}','{2}','{3}','{4}','{5}',{6},{7},'{8}','{9}')", strCisid, strExpenseType.Replace("'", "''"), strL1Desc.Replace("'", "''"), strL2Desc.Replace("'", "''"), strL3Desc.Replace("'", "''"), strUnit, strQty, strPrice, strRemark.Replace("'", "''"), Context.User.Identity.Name);
                            string insertSql = @"
INSERT INTO AMOUNT_SELECT 
(CATEGORY_ID, EXPENSE_TYPE, L1_DESC, L2_DESC, L3_DESC, UNIT, QTY, PRICE, REMARK, MODIFY_OPERATOR) 
VALUES 
(@strCisid,@strExpenseType,@strL1Desc,@strL2Desc,@strL3Desc,@strUnit,@strQty,@strPrice,@strRemark,@MODIFY_OPERATOR) ";

                            this.dbHelper.Execute(insertSql, paras);
                            this.dbHelper.Commit();
                        }
                        else
                        {
                            if (strCisid != "0")
                            {
                                if (CheckDataExist("CI_RELATIONS", " CISID=@CISID AND PARENTSID=1006 AND DISABLE IS NULL ", new Dictionary<string, object>() { { "CISID", intCisid } }) == false)
                                {
                                    return $"第 {line.ToString()} 列 無此({strCisid}) CISID!";
                                    //SystemModelClass.RegisterClientScript("Finished", "alert('第 " + line.ToString() + " 列 無此(" + strCisid + ") CISID!');");
                                    //hasErr = true;
                                }
                                else
                                {
                                    if (CheckDataExist("AMOUNT_SELECT", "ID=@strID AND ENABLE='Y'", new Dictionary<string, object>() { { "strID", strID } }) == false)
                                    {
                                        return $"第 {line.ToString()} 列 無此({strID}) ID!";
                                        //SystemModelClass.RegisterClientScript("Finished", "alert('第 " + line.ToString() + " 列 無此(" + strID + ") ID!');");
                                        //hasErr = true;
                                    }
                                    else
                                    {
                                        paras = new Dictionary<string, object>();
                                        paras.Add("strID", strID);
                                        paras.Add("strCisid", intCisid);
                                        paras.Add("strExpenseType", strExpenseType.Replace("'", "''"));
                                        paras.Add("strL1Desc", strL1Desc.Replace("'", "''"));
                                        paras.Add("strL2Desc", strL2Desc.Replace("'", "''"));
                                        paras.Add("strL3Desc", strL3Desc.Replace("'", "''"));
                                        paras.Add("strUnit", strUnit);
                                        paras.Add("strQty", intQty);
                                        paras.Add("strPrice", intPrice);
                                        paras.Add("strRemark", strRemark.Replace("'", "''"));
                                        paras.Add("MODIFY_OPERATOR", LoginSession.Current.empno);

                                        //string updateSql = string.Format("UPDATE AMOUNT_SELECT SET CATEGORY_ID='{1}', EXPENSE_TYPE='{2}', L1_DESC='{3}', L2_DESC='{4}', L3_DESC='{5}', UNIT='{6}', QTY={7}, PRICE={8}, REMARK='{9}', MODIFY_OPERATOR='{10}' WHERE ID={0} AND ENABLE='Y'", strID, strCisid, strExpenseType.Replace("'", "''"), strL1Desc.Replace("'", "''"), strL2Desc.Replace("'", "''"), strL3Desc.Replace("'", "''"), strUnit, strQty, strPrice, strRemark.Replace("'", "''"), Context.User.Identity.Name);
                                        string updateSql = @"
UPDATE AMOUNT_SELECT 
SET 
CATEGORY_ID=@strCisid, 
EXPENSE_TYPE=@strExpenseType, 
L1_DESC=@strL1Desc, 
L2_DESC=@strL2Desc, 
L3_DESC=@strL3Desc, 
UNIT=@strUnit, 
QTY=@strQty, 
PRICE=@strPrice, 
REMARK=@strRemark, 
MODIFY_OPERATOR=@MODIFY_OPERATOR 
WHERE ID=@strID
AND ENABLE='Y'";

                                        this.dbHelper.Execute(updateSql, paras);
                                        this.dbHelper.Commit();
                                    }
                                }
                            }
                        }

                    }
                }
            }

            return "";
        }

        internal string ImportStore(string destFilePath)
        {
            IWorkbook wk;

            string ext = Path.GetExtension(destFilePath).ToLower();

            // 判斷副檔名
            using (FileStream fs = new FileStream(destFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                if (ext == ".xlsx")
                {
                    wk = new XSSFWorkbook(fs); // XSSF 讀取 .xlsx
                }
                else if (ext == ".xls")
                {
                    wk = new HSSFWorkbook(fs); // HSSF 讀取 .xls
                }
                else
                {
                    return "檔案格式錯誤，只能上傳 Excel (.xls / .xlsx)";
                }
            }

            for (int k = 0; k < wk.NumberOfSheets; k++)
            {
                ISheet hst = wk.GetSheetAt(k);

                if (hst.SheetName == "報修類別設定")
                {
                    IRow hr = hst.GetRow(0);

                    for (int j = 1; j <= hst.LastRowNum; j++)
                    {
                        int line = j + 1;
                        hr = hst.GetRow(j);

                        string strCisid = hr.GetCell(0)?.ToString().Trim() ?? "";
                        string strSelfConfig = hr.GetCell(7)?.ToString().Trim() ?? "";

                        var intCisid = 0;

                        if (int.TryParse(strCisid, out intCisid) == false)
                        {
                            return $"第 {line.ToString()} 列 CISID應為整數!";
                        }

                        if (strCisid != "0")
                        {
                            if (!CheckDataExist("CI_RELATIONS", new Dictionary<string, object>() { { "CISID", intCisid } }))
                            {
                                return $"第 {line} 列 無此({strCisid}) CISID!";
                            }
                            else
                            {
                                var paras = new Dictionary<string, object>
                                {
                                    { "strCisid", intCisid },
                                    { "strSelfConfig", strSelfConfig }
                                };

                                string updateSql;

                                if (CheckDataExist("CI_RELATIONS_CATEGORY", new Dictionary<string, object>() { { "CISID", intCisid } }))
                                {
                                    updateSql = "UPDATE CI_RELATIONS_CATEGORY SET SELFCONFIG=@strSelfConfig WHERE CISID=@strCisid";
                                }
                                else
                                {
                                    updateSql = "INSERT INTO CI_RELATIONS_CATEGORY (CISID,SELFCONFIG) VALUES (@strCisid,@strSelfConfig)";
                                }

                                dbHelper.Execute(updateSql, paras);
                                dbHelper.Commit();
                            }
                        }
                    }
                }
            }

            return "";
        }

        internal void SaveMarquee(string content)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("content", content);
            string strSql = "UPDATE MAINTAIN_CONFIG SET CONFIG_VALUE=@content WHERE CONFIG_NAME='MARQUEE'";
            dbHelper.Execute(strSql, paras);
            dbHelper.Commit();
        }
    }
}
