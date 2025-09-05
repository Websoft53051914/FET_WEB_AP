using Const.DTO;
using Const.VO;
using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System.Data;

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
        [HttpPost("[action]")]
        public IActionResult GetPageListPrwp(DataSourceRequest request)
        {
            try
            {
                OnsitePrintHandler onsitePrintHandler = new(_configHelper);
                // 取得資料(應該只有自行尋商開單的單據會顯示(vender_id 為當前門市的 ivrcode))
                // TODO： 目前寫死IVRCode
                PageResult<VFttForm2DTO> pageList = onsitePrintHandler.GetPageListPrwp(GetPageEntity(request), "29");
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

                return JsonPage(new DataSourceResult
                {
                    Data = dataList,
                    Total = pageList.DataCount,
                });
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 取得狀態為 CONFIRM 的分頁資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult GetPageListConfirm(DataSourceRequest request)
        {
            try
            {
                OnsitePrintHandler onsitePrintHandler = new(_configHelper);
                // 取得資料 
                // TODO： 目前寫死IVRCode
                PageResult<VFttForm2DTO> pageList = onsitePrintHandler.GetPageListConfirm(GetPageEntity(request), "29");
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

                return JsonPage(new DataSourceResult
                {
                    Data = dataList,
                    Total = pageList.DataCount,
                });
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// [/pool/printwp.aspx]PrintWP_Click()<para/>
        /// 列印維修單
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult PrintWP(string jsonData)
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
                using Stream reportFileStream = new FileStream(Path.Combine(_env.WebRootPath, "Report6.rdlc"), FileMode.Open, FileAccess.Read);
                LocalReport localReport = new();
                localReport.LoadReportDefinition(reportFileStream);
                localReport.EnableHyperlinks = true;
                localReport.DataSources.Add(new ReportDataSource("DataSet1_V_FTT_FORM", dataTable));
                byte[] pdfFileBytes = localReport.Render("PDF");

                return File(pdfFileBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// [/pool/printwp.aspx]NextButton_Click()<para/>
        /// 廠商已到場-Y
        /// </summary>
        /// <returns></returns>
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

                foreach (int formNo in req.FormNoList)
                {
                    string formType = commonHandler.GetFieldData("FORM_TYPE", "APPROVE_FORM", new Dictionary<string, object>
                    {
                        { "FORM_NO", formNo }
                    });

                    commonHandler.ExecSetStatus(formType, formNo, "TICKET", _sessionVO.empname);
                }

                commonHandler.GetDBHelper().Commit();

                return JsonOK();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// [/pool/printwp.aspx]BackButton_Click()<para/>
        /// 廠商未到場-N
        /// </summary>
        /// <returns></returns>
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

                foreach (int formNo in req.FormNoList)
                {
                    string formType = commonHandler.GetFieldData("FORM_TYPE", "APPROVE_FORM", new Dictionary<string, object>
                    {
                        { "FORM_NO", formNo }
                    });

                    commonHandler.ExecSetStatus(formType, formNo, "PRWP", _sessionVO.empname);
                }

                commonHandler.GetDBHelper().Commit();

                return JsonOK();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// [/pool/printwp.aspx]BackButton_Click()<para/>
        /// 廠商未到場-N
        /// </summary>
        /// <returns></returns>
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

                foreach (OnsitePrintVO data in req.DataList)
                {
                    ArgumentNullException.ThrowIfNull(data.FormNo);
                    ArgumentNullException.ThrowIfNull(data.VendorArriveDate);

                    string formType = commonHandler.GetFieldData("FORM_TYPE", "APPROVE_FORM", new Dictionary<string, object>
                    {
                        { "FORM_NO", data.FormNo.Value }
                    });

                    onsitePrintHandler.UpdateVendorArriveDate(data.FormNo.Value, data.VendorArriveDate.Value);
                    commonHandler.ExecSetStatus(formType, data.FormNo.Value, "CONFIRM", _sessionVO.empname);
                }

                onsitePrintHandler.GetDBHelper().Commit();

                return JsonOK();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
