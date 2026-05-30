using Const;
using Const.DTO;
using Const.VO;
using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Helper.Excel;
using Core.Utility.Helper.Word;
using Core.Utility.Utility;
using Core.Utility.Web.EX;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System.Data;
using System.Globalization;
using System.Text;

namespace FTT_VENDER_API.Controllers.Query
{
    /// <summary>
    /// 門市報修管理-查詢
    /// </summary>
    [Route("[controller]")]

    public partial class QueryController : BaseProjectController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QueryController(ConfigurationHelper configHelper, IWebHostEnvironment env)
        {
            _configHelper = configHelper;
            _env = env;
        }

        private ConfigurationHelper _configHelper;
        private readonly IWebHostEnvironment _env;
    }

    public partial class QueryController
    {
        /// <summary>
        /// 取得頁面資料
        /// </summary>

        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult GetInitData()
        {
            try
            {
                CommonHandler commonHandler = new(_configHelper);
                QueryIndexVO result = new QueryIndexVO
                {
                    SelectListStatus = commonHandler.GetListFormAccessStatus()
                        .Select(x => new SelectListItem(x.status_name, x.status))
                        .ToList(),
                };

                this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 取得分頁資料<para/>
        /// [/pool/query.aspx]SearchCode_Click
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult GetPageList(DataSourceRequest request, QueryIndexVO vm)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);

                QueryHandler queryHandler = new(_configHelper);
                queryHandler.SessionVO = _sessionVO;
                VFttForm2DTO searchVO = new()
                {
                    CreateDateGte = vm.CreateDateGte,
                    CreateDateLt = vm.CreateDateLte?.AddDays(1).Date,
                    CompleteDateGte = vm.CompleteDateGte,
                    CompleteDateLt = vm.CompleteDateLte?.AddDays(1).Date,
                    CloseDateGte = vm.CloseDateGte,
                    CloseDateLt = vm.CloseDateLte?.AddDays(1).Date,
                    StatusIdEq = vm.StatusIdEq,
                    FormNoEq = vm.FormNoEq,
                    TtCategoryEq = vm.TtCategoryEq,
                    CategoryIdFilter = vm.CategoryIdFilter,
                    VenderIdEq = vm.VenderIdEq,
                    IvrCodeEq = vm.IvrCodeEq,
                    CompanyEq = vm.CompanyEq,
                    StoreTypeEq = vm.StoreTypeEq,
                    ChannelEq = vm.ChannelEq,
                    AreaEq = vm.AreaEq,
                    AsEmpNoEq = vm.AsEmpNoEq,
                    SelfConfigEq = vm.SelfConfigEq,
                    UserRoleVenderFilter = true
                };

                // 取得資料，雖然 m_QueryString 會依角色有不同的 SQL，但實際結果相同
                PageResult<VFttForm2DTO> pageList = queryHandler.GetPageList(GetPageEntity<QueryGridVO>(request), searchVO);
                // 轉成 ViewModel
                List<QueryGridVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    VFttForm2DTO data = pageList.Results[i];

                    QueryGridVO item = new()
                    {
                        CreateTimeText = data.createtime_text,
                        FormNo = data.form_no,
                        StatusName = data.statusname,
                        TtCategory = data.tt_category,
                        Vender = data.vender,
                        Ciname = data.ciname,
                        ShopName = data.shop_name,
                        DispatchTimeText = data.dispatchtime_text,
                        Descr = data.descr,
                        Processer = data.processer,
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
        /// [/pool/printwp.aspx]ExportToExcel_Click()<para/>
        /// 列印維修單
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult ExportExcel([FromBody] string jsonData)
        {
            try
            {
                ArgumentNullException.ThrowIfNullOrWhiteSpace(jsonData);
                QueryIndexVO? vm = JsonConvert.DeserializeObject<QueryIndexVO>(jsonData);
                DataSourceRequest? request = JsonConvert.DeserializeObject<DataSourceRequest>(jsonData);
                ArgumentNullException.ThrowIfNull(vm);
                ArgumentNullException.ThrowIfNull(request);
                QueryHandler queryHandler = new(_configHelper);
                queryHandler.SessionVO = _sessionVO;
                VFttForm2DTO searchVO = new()
                {
                    CreateDateGte = vm.CreateDateGte,
                    CreateDateLt = vm.CreateDateLte?.AddDays(1).Date,
                    CompleteDateGte = vm.CompleteDateGte,
                    CompleteDateLt = vm.CompleteDateLte?.AddDays(1).Date,
                    CloseDateGte = vm.CloseDateGte,
                    CloseDateLt = vm.CloseDateLte?.AddDays(1).Date,
                    StatusIdEq = vm.StatusIdEq,
                    FormNoEq = vm.FormNoEq,
                    TtCategoryEq = vm.TtCategoryEq,
                    CategoryIdFilter = vm.CategoryIdFilter,
                    VenderIdEq = vm.VenderIdEq,
                    IvrCodeEq = vm.IvrCodeEq,
                    CompanyEq = vm.CompanyEq,
                    StoreTypeEq = vm.StoreTypeEq,
                    ChannelEq = vm.ChannelEq,
                    AreaEq = vm.AreaEq,
                    AsEmpNoEq = vm.AsEmpNoEq,
                    SelfConfigEq = vm.SelfConfigEq,
                    UserRoleVenderFilter = true,
                };

                ExcelWriterHelper writer = new();
                IWorkbook wb = writer.CreateWorkBook(ExcelType.HSSF);
                ISheet sheet = writer.CreateSheet(DateTime.Now.ToString(DbConst.FORMAT_DATETIME));
                writer.SetRowCellIndex(0, 0);
                #region -- 設定標題 --
                writer.SetCellValue("公司別");
                writer.SetCellValue("店格");
                writer.SetCellValue("通路");
                writer.SetCellValue("區域");
                writer.SetCellValue("店名");
                writer.SetCellValue("IVR_CODE");
                writer.SetCellValue("區經理/業務");
                writer.SetCellValue("報修日期");
                writer.SetCellValue("報修型態");
                writer.SetCellValue("報修類別");
                writer.SetCellValue("報修類別");
                writer.SetCellValue("報修品項");
                writer.SetCellValue("保固期日期");
                writer.SetCellValue("報修廠商");
                writer.SetCellValue("備註");
                writer.SetCellValue("工單號碼");
                writer.SetCellValue("廠商到場日期");
                writer.SetCellValue("已派工日期");
                writer.SetCellValue("結案日期");
                writer.SetCellValue("完修日期");
                writer.SetCellValue("工單狀態");
                writer.SetCellValue("處理回覆");
                writer.SetCellValue("預計完修日");
                writer.SetCellValue("備註說明");
                writer.SetCellValue("一個月內覆修");
                writer.SetCellValue("補單");
                writer.SetCellValue("檢測故障原因");
                writer.SetCellValue("維修處理動作");
                writer.SetCellValue("費用種類");
                writer.SetCellValue("維修細項");
                writer.SetCellValue("數量");
                writer.SetCellValue("單位");
                writer.SetCellValue("金額");
                writer.SetCellValue("小計");
                #endregion -- 設定標題 --

                // 調整欄寬
                for (int i = 0; i < writer.GetRow().LastCellNum; i++)
                {
                    writer.SetRowCellIndex(0, i);
                    int lenTitle = Encoding.UTF8.GetByteCount(writer.GetCell().StringCellValue);
                    writer.GetSheet().SetColumnWidth(writer.GetCellIndex(), (lenTitle + 6) * 256);
                }

                // 取得全部資料（一次查詢，避免 OFFSET 分頁重複揃描造成逾時）
                List<VFttForm2DTO> exportList = queryHandler.GetPageListExport(searchVO);
                int posRow = 1;
                foreach (VFttForm2DTO item in exportList)
                {
                    writer.SetRowCellIndex(posRow, 0);
                    writer.SetCellValue(item.company ?? string.Empty);           // 公司別
                        writer.SetCellValue(item.store_type ?? string.Empty);        // 店格
                        writer.SetCellValue(item.channel ?? string.Empty);           // 通路
                        writer.SetCellValue(item.area ?? string.Empty);              // 區域
                        writer.SetCellValue(item.shop_name ?? string.Empty);         // 店名
                        writer.SetCellValue(item.ivrcode ?? string.Empty);           // IVR_CODE
                        writer.SetCellValue(item.as_cname ?? string.Empty);          // 區經理/業務
                        writer.SetCellValue(item.createtime_text ?? string.Empty);   // 報修日期
                        writer.SetCellValue(item.tt_category ?? string.Empty);       // 報修型態
                        writer.SetCellValue(item.l1_desc ?? string.Empty);           // 報修類別
                        writer.SetCellValue(item.l2_desc ?? string.Empty);           // 報修類別
                        writer.SetCellValue(item.ciname ?? string.Empty);            // 報修品項
                        writer.SetCellValue(item.approval_date_text ?? string.Empty); // 保固期日期
                        writer.SetCellValue(item.vender ?? string.Empty);            // 報修廠商
                        writer.SetCellValue(item.remark ?? string.Empty);            // 備註
                        writer.SetCellValue(item.form_no?.ToString() ?? string.Empty); // 工單號碼
                        writer.SetCellValue(item.vendor_arrive_date_text ?? string.Empty); // 廠商到門市日期
                        writer.SetCellValue(item.assign_date_text ?? string.Empty);  // 派工日期
                        writer.SetCellValue(item.closedate_text ?? string.Empty);    // 結案日期
                        writer.SetCellValue(item.completetime_text ?? string.Empty); // 完修日期
                        writer.SetCellValue(item.statusname ?? string.Empty);        // 工單狀態
                        writer.SetCellValue(item.descr ?? string.Empty);             // 處理回覆
                        writer.SetCellValue(item.precompletetime_text ?? string.Empty); // 預計完修日
                        writer.SetCellValue(item.description ?? string.Empty);       // 備註說明
                        writer.SetCellValue(item.repair ?? string.Empty);            // 一個月內覆修
                        writer.SetCellValue(item.resupply ?? string.Empty);          // 補單
                        writer.SetCellValue(item.fault_reason ?? string.Empty);      // 檢測故障原因
                        writer.SetCellValue(item.repair_action ?? string.Empty);     // 維修處理動作
                        writer.SetCellValue(item.expense_type ?? string.Empty);      // 費用種類
                        writer.SetCellValue(item.expense_desc ?? string.Empty);      // 維修細項
                        writer.SetCellValue(item.qty ?? string.Empty);               // 數量
                        writer.SetCellValue(item.unit ?? string.Empty);              // 單位
                        writer.SetCellValue(item.price ?? string.Empty);             // 金額
                        writer.SetCellValue(item.subtotal ?? string.Empty);          // 小計

                    posRow++;
                }

                MemoryStream? memoryStream = null;
                using (MemoryStream stream = new())
                {
                    writer.GetWorkBook().Write(stream, false);
                    memoryStream = new MemoryStream(stream.ToArray());
                }

                writer.GetWorkBook().Close();

                this.LogSuccess();
                return File(memoryStream, "application/vnd.ms-excel", $"excel_out_{DateTime.Now:yyyyMMddHHmm}.xls");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 列印到場單
        /// [/pool/printwp.aspx]PrintWP_Click()<para/>
        /// 對應[FTT_API/OnsitePrintController]
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

                QueryHandler queryHandler = new(_configHelper);
                DataTable dataTable = queryHandler.GetDataTablePrintWP(req.FormNoList);
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
    }
}
