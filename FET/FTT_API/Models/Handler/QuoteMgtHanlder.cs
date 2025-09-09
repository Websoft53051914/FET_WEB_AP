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
                    int dLastNum = hr.LastCellNum;

                    for (int j = 1; j <= hst.LastRowNum; j++)
                    {
                        int line = j + 1;
                        hr = hst.GetRow(j);

                        string strCisid = hr.GetCell(0)?.ToString().Trim() ?? "";
                        string strSelfConfig = hr.GetCell(7)?.ToString().Trim() ?? "";

                        if (strCisid != "0")
                        {
                            if (!CheckDataExist("CI_RELATIONS", new Dictionary<string, object>() { { "CISID", strCisid } }))
                            {
                                return $"第 {line} 列 無此({strCisid}) CISID!";
                            }
                            else
                            {
                                var paras = new Dictionary<string, object>
                        {
                            { "strCisid", strCisid },
                            { "strSelfConfig", strSelfConfig }
                        };

                                string updateSql;

                                if (CheckDataExist("CI_RELATIONS_CATEGORY", new Dictionary<string, object>() { { "CISID", strCisid } }))
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

        internal string ImportStore2(string destFilePath)
        {
            string strFilePath = string.Format(destFilePath);

            XSSFWorkbook wk;
            XSSFSheet hst;
            XSSFRow hr;

            using (FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                wk = new XSSFWorkbook(fs);
            }

            for (int k = 0; k < wk.Count; k++)
            {
                hst = (XSSFSheet)wk.GetSheetAt(k);

                if (hst.SheetName == "報修類別設定")
                {
                    hr = (XSSFRow)hst.GetRow(0);
                    int dLastNum = hr.LastCellNum;

                    bool hasErr = false;

                    for (int j = 1; j <= hst.LastRowNum; j++)
                    {
                        int line = j + 1;

                        hr = (XSSFRow)hst.GetRow(j);

                        string strCisid = hr.GetCell(0) == null ? "" : hr.GetCell(0).ToString().Trim();
                        string strSelfConfig = hr.GetCell(7) == null ? "" : hr.GetCell(7).ToString().Trim();

                        if (strCisid != "0")
                        {
                            //if (db.CheckDataExist("CI_RELATIONS", "CISID='" + strCisid + "'") == false)
                            if (CheckDataExist("CI_RELATIONS", new Dictionary<string, object>() { { "CISID", strCisid } }) == false)
                            {
                                //SystemModelClass.RegisterClientScript("Finished", "alert('第 " + line.ToString() + " 列 無此(" + strCisid + ") CISID!');");
                                return $"第 {line} 列 無此({strCisid}) CISID!');";
                            }
                            else
                            {
                                var paras = new Dictionary<string, object>();
                                paras.Add("strCisid", strCisid);
                                paras.Add("strSelfConfig", strSelfConfig);

                                string updateSql = "";

                                //if (db.CheckDataExist("CI_RELATIONS_CATEGORY", "CISID='" + strCisid + "'") == true)
                                if (CheckDataExist("CI_RELATIONS_CATEGORY", new Dictionary<string, object>() { { "CISID", strCisid } }) == true)
                                {
                                    updateSql = string.Format(@"UPDATE CI_RELATIONS_CATEGORY SET SELFCONFIG=@strSelfConfig WHERE CISID=@strCisid ", strCisid, strSelfConfig);
                                }
                                else
                                {
                                    updateSql = string.Format(@"INSERT INTO CI_RELATIONS_CATEGORY (CISID,SELFCONFIG) VALUES (@strCisid,@strSelfConfig)", strCisid, strSelfConfig);
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
