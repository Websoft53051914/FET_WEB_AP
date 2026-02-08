using Const.DTO;
using Const.VO;
using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Helper.Word;
using Core.Utility.Utility;
using Core.Utility.Web.EX;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace FTT_API.Controllers.OnsitePrint
{
    /// <summary>
    /// 列印到場單 API
    /// </summary>
    [Route("[controller]")]
    public partial class OnsitePrintController : BaseProjectController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OnsitePrintController(ConfigurationHelper configHelper, IWebHostEnvironment env)
        {
            _configHelper = configHelper;
            _env = env;
        }

        private ConfigurationHelper _configHelper;
        private readonly IWebHostEnvironment _env;
    }

    public partial class OnsitePrintController
    {
        /// <summary>
        /// 取得狀態為 PRWP 的分頁資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult GetPageListPrwp(DataSourceRequest request)
        {
            try
            {
                OnsitePrintHandler onsitePrintHandler = new(_configHelper);
                // 取得資料(應該只有自行尋商開單的單據會顯示(vender_id 為當前門市的 ivrcode))
                PageResult<VFttForm2DTO> pageList = onsitePrintHandler.GetPageListPrwp(GetPageEntity(request), _sessionVO.ivrcode);
                // 轉成 ViewModel
                List<OnsitePrintVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    VFttForm2DTO data = pageList.Results[i];

                    OnsitePrintVO item = new()
                    {
                        AssignDateText = data.assign_date_text,
                        CiName = data.ciname,
                        CreateTimeText = data.createtime_text,
                        FormNo = data.form_no,
                        L2Desc = data.l2_desc,
                        LimitDateText = data.limit_date_text,
                        StatusName = data.statusname,
                        TtCategory = data.tt_category,
                        Vender = data.vender,
                        VendorArriveDateText = data.vendor_arrive_date_text,
                        Ivrcode = data.ivrcode,
                    };

                    dataList.Add(item);
                }

                this.LogSuccess();
                return JsonPage(new DataSourceResult
                {
                    Data = dataList,
                    Total = pageList.DataCount,
                });
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 取得狀態為 CONFIRM 的分頁資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult GetPageListConfirm(DataSourceRequest request)
        {
            try
            {
                OnsitePrintHandler onsitePrintHandler = new(_configHelper);
                // 取得資料
                PageResult<VFttForm2DTO> pageList = onsitePrintHandler.GetPageListConfirm(GetPageEntity(request), _sessionVO.ivrcode);
                // 轉成 ViewModel
                List<OnsitePrintVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    VFttForm2DTO data = pageList.Results[i];

                    OnsitePrintVO item = new()
                    {
                        AssignDateText = data.assign_date_text,
                        CiName = data.ciname,
                        CreateTimeText = data.createtime_text,
                        FormNo = data.form_no,
                        L2Desc = data.l2_desc,
                        LimitDateText = data.limit_date_text,
                        StatusName = data.statusname,
                        TtCategory = data.tt_category,
                        Vender = data.vender,
                        VendorArriveDateText = data.vendor_arrive_date_text,
                        Ivrcode = data.ivrcode,
                    };

                    dataList.Add(item);
                }

                this.LogSuccess();
                return JsonPage(new DataSourceResult
                {
                    Data = dataList,
                    Total = pageList.DataCount,
                });
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 列印到場單<para/>
        /// [/pool/printwp.aspx]PrintWP_Click()<para/>
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult PrintWP([FromBody] string jsonData)
        {
            try
            {
                ArgumentNullException.ThrowIfNullOrWhiteSpace(jsonData);
                OnsitePrintUpdateStatusReqVO? req = JsonConvert.DeserializeObject<OnsitePrintUpdateStatusReqVO>(jsonData);
                ArgumentNullException.ThrowIfNull(req);
                if (req.FormNoList.IsNullOrEmpty())
                {
                    throw new ArgumentException(nameof(req.FormNoList) + "_empty");
                }

                OnsitePrintHandler onsitePrintHandler = new(_configHelper);
                DataTable dataTable = onsitePrintHandler.GetDataTablePrintWP(req.FormNoList);
                Dictionary<string, object> wordPara = ToPrintWPWordPara(dataTable);

                MiniWordHelper helper = new();
                string path = Path.Combine(_env.WebRootPath, "Report6.docx");
                byte[] byteWordFile = helper.Print(path, wordPara);

                using MemoryStream msWord = new(byteWordFile);
                LibreOfficeConverter libreOfficeConverter = new(_env, _configHelper.Config);
                byte[] pdfFileBytes = libreOfficeConverter.WordToPdf(msWord);

                // ReportViewerCore.NETCore 在 Linux 環境無法執行
                //using Stream reportFileStream = new FileStream(Path.Combine(_env.WebRootPath, "Report6.rdlc"), FileMode.Open, FileAccess.Read);
                //LocalReport localReport = new();
                //localReport.LoadReportDefinition(reportFileStream);
                //localReport.EnableHyperlinks = true;
                //localReport.DataSources.Add(new ReportDataSource("DataSet1_V_FTT_FORM", dataTable));
                //byte[] pdfFileBytes = localReport.Render("PDF");

                this.LogSuccess();
                return File(pdfFileBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// DataTable 轉為列印維修單的參數
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private Dictionary<string, object> ToPrintWPWordPara(DataTable dt)
        {
            DateTime now = DateTime.Now;
            Dictionary<string, object> result = [];
            List<Dictionary<string, object>> t1 = [];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                Dictionary<string, object> t1Item = [];
                string valFORM_NO = row["FORM_NO"]?.ToString() ?? string.Empty;
                string valL1_DESC = row["L1_DESC"]?.ToString() ?? string.Empty;
                string valL2_DESC = row["L2_DESC"]?.ToString() ?? string.Empty;
                string valCINAME = row["CINAME"]?.ToString() ?? string.Empty;
                string valDESCR = row["DESCR"]?.ToString() ?? string.Empty;

                t1Item["No"] = (i + 1).ToString();
                t1Item["FORM_NO"] = valFORM_NO;
                t1Item["CINAME"] = $"{valL1_DESC} - {valL2_DESC} - {valCINAME}";
                t1Item["DESCR"] = valDESCR;
                t1.Add(t1Item);

                if (i == 0)
                {
                    string valSHOP_NAME = row["SHOP_NAME"]?.ToString() ?? string.Empty;
                    string valVENDER = row["VENDER"]?.ToString() ?? string.Empty;
                    DateTime? valCreateTime = ConvertUtility.ConvertObjectToDateTime(row["CREATETIME"]);

                    result["SHOP_NAME"] = valSHOP_NAME;
                    result["VENDER"] = valVENDER;
                    result["CREATETIME"] = valCreateTime?.ToString("yyyy/M/d tt h:mm:ss", new CultureInfo("zh-TW")) ?? string.Empty;
                    result["NOWTIME"] = now.ToString("yyyy/M/d tt h:mm:ss", new CultureInfo("zh-TW"));
                }
            }

            result["T1"] = t1;

            return result;
        }

        /// <summary>
        /// [/pool/printwp.aspx]NextButton_Click()<para/>
        /// 廠商已到場-Y
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult UpdateStatusToTicket(OnsitePrintUpdateStatusReqVO req)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(req);
                if (req.FormNoList.IsNullOrEmpty())
                {
                    throw new ArgumentException(nameof(req.FormNoList) + "_empty");
                }

                CommonHandler commonHandler = new(_configHelper);
                BaseDBHandler baseHandler = new BaseDBHandler();

                foreach (int formNo in req.FormNoList)
                {
                    string formType = commonHandler.GetFieldData("FORM_TYPE", "APPROVE_FORM", new Dictionary<string, object>
                    {
                        { "FORM_NO", formNo }
                    });

                    //先取得當下的狀態
                    Dictionary<string, object> dic = new();
                    dic.Add("form_no", formNo);
                    var oldEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                    commonHandler.ExecSetStatus(formType, formNo, "TICKET", _sessionVO.empname);

                    //取得更新完的狀態
                    var newEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                    if (newEntity != null && oldEntity != null)
                    {
                        MailPoolHandler _MailPoolHandlerHandler = new MailPoolHandler();
                        var result = Method.CreateMailPool(formNo.ToString(), oldEntity.status, newEntity.status, _MailPoolHandlerHandler);
                        if (!string.IsNullOrEmpty(result))
                        {
                            this.LogError("CreateMailPool 執行失敗");
                        }
                    }
                }

                commonHandler.GetDBHelper().Commit();

                this.LogSuccess();
                return JsonOK();
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// [/pool/printwp.aspx]BackButton_Click()<para/>
        /// 廠商未到場-N
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult UpdateStatusToPrwp(OnsitePrintUpdateStatusReqVO req)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(req);
                if (req.FormNoList.IsNullOrEmpty())
                {
                    throw new ArgumentException(nameof(req.FormNoList) + "_empty");
                }

                CommonHandler commonHandler = new(_configHelper);
                BaseDBHandler baseHandler = new BaseDBHandler();

                foreach (int formNo in req.FormNoList)
                {
                    string formType = commonHandler.GetFieldData("FORM_TYPE", "APPROVE_FORM", new Dictionary<string, object>
                    {
                        { "FORM_NO", formNo }
                    });

                    //先取得當下的狀態
                    Dictionary<string, object> dic = new();
                    dic.Add("form_no", formNo);
                    var oldEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                    commonHandler.ExecSetStatus(formType, formNo, "PRWP", _sessionVO.empname);

                    //取得更新完的狀態
                    var newEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                    if (newEntity != null && oldEntity != null)
                    {
                        MailPoolHandler _MailPoolHandlerHandler = new MailPoolHandler();
                        var result = Method.CreateMailPool(formNo.ToString(), oldEntity.status, newEntity.status, _MailPoolHandlerHandler);
                        if (!string.IsNullOrEmpty(result))
                        {
                            this.LogError("CreateMailPool 執行失敗");
                        }
                    }
                }

                commonHandler.GetDBHelper().Commit();

                this.LogSuccess();
                return JsonOK();
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// [/pool/printwp.aspx]BackButton_Click()<para/>
        /// 廠商未到場-N
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult UpdateStatusToConfirm(OnsitePrintUpdateStatusReqVO req)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(req);
                if (req.DataList.IsNullOrEmpty())
                {
                    throw new ArgumentException(nameof(req.DataList) + "_empty");
                }

                OnsitePrintHandler onsitePrintHandler = new(_configHelper);
                CommonHandler commonHandler = new(_configHelper, onsitePrintHandler.GetDBHelper());
                BaseDBHandler baseHandler = new BaseDBHandler();

                foreach (OnsitePrintVO data in req.DataList)
                {
                    ArgumentNullException.ThrowIfNull(data.FormNo);
                    ArgumentNullException.ThrowIfNull(data.VendorArriveDate);

                    string formType = commonHandler.GetFieldData("FORM_TYPE", "APPROVE_FORM", new Dictionary<string, object>
                    {
                        { "FORM_NO", data.FormNo.Value }
                    });

                    //先取得當下的狀態
                    Dictionary<string, object> dic = new();
                    dic.Add("form_no", data.FormNo.Value);
                    var oldEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                    onsitePrintHandler.UpdateVendorArriveDate(data.FormNo.Value, data.VendorArriveDate.Value);
                    commonHandler.ExecSetStatus(formType, data.FormNo.Value, "CONFIRM", _sessionVO.empname);

                    //取得更新完的狀態
                    var newEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                    if (newEntity != null && oldEntity != null)
                    {
                        MailPoolHandler _MailPoolHandlerHandler = new MailPoolHandler();
                        var result = Method.CreateMailPool(data.FormNo.Value.ToString(), oldEntity.status, newEntity.status, _MailPoolHandlerHandler);
                        if (!string.IsNullOrEmpty(result))
                        {
                            this.LogError("CreateMailPool 執行失敗");
                        }
                    }
                }

                onsitePrintHandler.GetDBHelper().Commit();

                this.LogSuccess();
                return JsonOK();
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
