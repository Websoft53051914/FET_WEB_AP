using Const;
using Const.DTO;
using Const.VO;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Helper.Excel;
using Core.Utility.Web.EX;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace FTT_API.Controllers.Query
{
    /// <summary>
    /// 門市報修管理-查詢
    /// </summary>
    [Route("[controller]")]



    public partial class QueryController : BaseProjectController
    {
        /// Constructor
        /// </summary>
        public QueryController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        private ConfigurationHelper _configHelper;
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
                    SelectListCompany = commonHandler.GetListStoreType(new Const.DTO.StoreTypeDTO
                    {
                        TypeNameEq = "COMPANY_LEAVES",
                    })
                        .Select(x => new SelectListItem(x.type_value, x.type_value))
                        .ToList(),
                    SelectListStoreType = commonHandler.GetListStoreType(new Const.DTO.StoreTypeDTO
                    {
                        TypeNameEq = "STORE_TYPE",
                    })
                        .Select(x => new SelectListItem(x.type_value, x.type_value))
                        .ToList(),
                    SelectListChannel = commonHandler.GetListStoreType(new Const.DTO.StoreTypeDTO
                    {
                        TypeNameEq = "CHANNEL",
                    })
                        .Select(x => new SelectListItem(x.type_value, x.type_value))
                        .ToList(),
                    SelectListArea = commonHandler.GetListArea()
                        .Select(x => new SelectListItem(x, x))
                        .ToList(),
                    SelectListAsEmp = commonHandler.GetListAsEmp()
                        .Select(x => new SelectListItem(x.as_cname, x.as_empno))
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
                };
                if (_sessionVO.userrole == "VENDOR")
                {
                    searchVO.UserRoleVenderFilter = true;
                }
                else if (_sessionVO.userrole == "ADMIN"
                    || _sessionVO.userrole == "SECURITY"
                    || _sessionVO.userrole == "ASSETER"
                    || _sessionVO.userrole == "ASSISTANT")
                {

                }
                else
                {
                    searchVO.UserRoleOtherFilter = true;
                }

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
                // 記錄環境資訊
                var envInfo = $"OS: {Environment.OSVersion}, Platform: {Environment.OSVersion.Platform}";
                this.LogError($"ExportExcel 開始執行 - {envInfo}");
                
                ArgumentNullException.ThrowIfNullOrWhiteSpace(jsonData);
                this.LogError("JSON 資料驗證通過");
                
                QueryIndexVO? vm = JsonConvert.DeserializeObject<QueryIndexVO>(jsonData);
                DataSourceRequest? request = JsonConvert.DeserializeObject<DataSourceRequest>(jsonData);
                ArgumentNullException.ThrowIfNull(vm);
                ArgumentNullException.ThrowIfNull(request);
                this.LogError("JSON 反序列化完成");
                
                QueryHandler queryHandler = new(_configHelper);
                queryHandler.SessionVO = _sessionVO;
                this.LogError($"QueryHandler 建立完成，使用者角色: {_sessionVO?.userrole ?? "null"}");
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
                };
                if (_sessionVO.userrole == "VENDOR")
                {
                    searchVO.UserRoleVenderFilter = true;
                }
                else if (_sessionVO.userrole == "ADMIN"
                    || _sessionVO.userrole == "SECURITY"
                    || _sessionVO.userrole == "ASSETER"
                    || _sessionVO.userrole == "ASSISTANT")
                {

                }
                else
                {
                    searchVO.UserRoleOtherFilter = true;
                }
                
                this.LogError($"角色過濾設定完成，搜尋條件: VENDOR={searchVO.UserRoleVenderFilter}, OTHER={searchVO.UserRoleOtherFilter}");

                ExcelWriterHelper writer = new();
                this.LogError("ExcelWriterHelper 建立完成");
                
                IWorkbook wb = writer.CreateWorkBook(ExcelType.HSSF);
                this.LogError("Excel Workbook 建立完成");
                
                ISheet sheet = writer.CreateSheet(DateTime.Now.ToString(DbConst.FORMAT_DATETIME));
                this.LogError($"Excel Sheet 建立完成，名稱: {DateTime.Now.ToString(DbConst.FORMAT_DATETIME)}");
                
                writer.SetRowCellIndex(0, 0);
                this.LogError("開始設定 Excel 標題");
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
                writer.SetCellValue("結案日期");
                writer.SetCellValue("完修日期");
                writer.SetCellValue("已派工");
                writer.SetCellValue("確認到場日期");
                writer.SetCellValue("自行尋商日期");
                writer.SetCellValue("派工日期");
                writer.SetCellValue("廠商到門市日期");
                writer.SetCellValue("工單狀態");
                writer.SetCellValue("處理回覆");
                writer.SetCellValue("預計完修日");
                writer.SetCellValue("處理者");
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
                writer.SetCellValue("門市自行尋商");
                writer.SetCellValue("計算派工天數");
                writer.SetCellValue("KPI Day");
                writer.SetCellValue("KPI Result");
                writer.SetCellValue("延遲原因");
                #endregion -- 設定標題 --
                
                this.LogError("Excel 標題設定完成，開始調整欄寬");

                // 調整欄寬
                for (int i = 0; i < writer.GetRow().LastCellNum; i++)
                {
                    writer.SetRowCellIndex(0, i);
                    int lenTitle = Encoding.UTF8.GetByteCount(writer.GetCell().StringCellValue);
                    writer.GetSheet().SetColumnWidth(writer.GetCellIndex(), (lenTitle + 6) * 256);
                }
                
                this.LogError("欄寬調整完成，開始資料分頁處理");

                int totalPage = 1;
                int currentPage = 1;
                int pageDataSize = 100;
                int posRow = 1;
                request.pageSize = pageDataSize;
                
                this.LogError($"分頁參數設定: pageSize={pageDataSize}, 開始第一頁資料查詢");
                do
                {
                    request.pageIndex = currentPage;
                    this.LogError($"處理第 {currentPage} 頁資料");

                    PageEntity pageEntity = GetPageEntity<QueryGridVO>(request);
                    // 取得資料
                    PageResult<VFttForm2DTO> pageList = queryHandler.GetPageListExport(pageEntity, searchVO);
                    
                    this.LogError($"第 {currentPage} 頁查詢完成，資料筆數: {pageList.Results.Count}, 總筆數: {pageList.DataCount}");

                    if (currentPage == 1)
                    {
                        totalPage = (int)Math.Ceiling((double)pageList.DataCount / pageDataSize);
                        this.LogError($"總頁數計算: {totalPage} 頁");
                    }

                    if (pageList.Results.Count == 0)
                    {
                        this.LogError("沒有更多資料，跳出迴圈");
                        break;
                    }

                    foreach (VFttForm2DTO item in pageList.Results)
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
                        writer.SetCellValue(item.closedate_text ?? string.Empty);    // 結案日期
                        writer.SetCellValue(item.completetime_text ?? string.Empty); // 完修日期
                        writer.SetCellValue(item.tickettime_text ?? string.Empty);   // 已派工
                        writer.SetCellValue(item.confirmtime_text ?? string.Empty);  // 確認到場日期
                        writer.SetCellValue(item.usedtime_text ?? string.Empty);     // 自行尋商日期
                        writer.SetCellValue(item.assign_date_text ?? string.Empty);  // 派工日期
                        writer.SetCellValue(item.vendor_arrive_date_text ?? string.Empty); // 廠商到門市日期
                        writer.SetCellValue(item.statusname ?? string.Empty);        // 工單狀態
                        writer.SetCellValue(item.descr ?? string.Empty);             // 處理回覆
                        writer.SetCellValue(item.precompletetime_text ?? string.Empty); // 預計完修日
                        writer.SetCellValue(item.processer ?? string.Empty);         // 處理者
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
                        writer.SetCellValue(item.selfconfig ?? string.Empty);        // 門市自行尋商
                        writer.SetCellValue(item.dispatch_days ?? string.Empty);     // 計算派工天數
                        writer.SetCellValue(item.kpi_days ?? string.Empty);          // KPI Day
                        writer.SetCellValue(item.kpi_result ?? string.Empty);        // KPI Result
                        writer.SetCellValue(item.delay_reason ?? string.Empty);      // 延遲原因

                        posRow++;
                    }

                    currentPage++;
                } while (currentPage <= totalPage);
                
                this.LogError($"所有資料處理完成，總共寫入 {posRow-1} 筆資料，開始產生 Excel 檔案");

                MemoryStream? memoryStream = null;
                using (MemoryStream stream = new())
                {
                    this.LogError("開始將 Excel 寫入 MemoryStream");
                    writer.GetWorkBook().Write(stream, false);
                    this.LogError($"Excel 寫入完成，檔案大小: {stream.Length} bytes");
                    memoryStream = new MemoryStream(stream.ToArray());
                }

                this.LogError("開始關閉 Excel Workbook");
                writer.GetWorkBook().Close();
                this.LogError("Excel Workbook 關閉完成");

                this.LogSuccess();
                return File(memoryStream, "application/vnd.ms-excel", "CodeList.xls");
            }
            catch (Exception ex)
            {
                this.LogError($"ExportExcel 發生錯誤:");
                this.LogError($"錯誤類型: {ex.GetType().FullName}");
                this.LogError($"錯誤訊息: {ex.Message}");
                this.LogError($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    this.LogError($"內部異常類型: {ex.InnerException.GetType().FullName}");
                    this.LogError($"內部異常訊息: {ex.InnerException.Message}");
                }
                
                // 檢查是否為 Linux 環境常見問題
                var errorMsg = ex.Message.ToLower();
                if (errorMsg.Contains("libgdiplus") || errorMsg.Contains("gdi+"))
                {
                    this.LogError("檢測到 GDI+ 相關錯誤，這是 Linux 環境常見問題");
                    return JsonValidFail("Linux 環境缺少 libgdiplus 套件，請執行: sudo apt-get install libgdiplus");
                }
                else if (errorMsg.Contains("font") || errorMsg.Contains("字型"))
                {
                    this.LogError("檢測到字型相關錯誤");
                    return JsonValidFail("Linux 環境字型問題，請安裝字型套件: sudo apt-get install fonts-liberation");
                }
                else if (errorMsg.Contains("memory") || errorMsg.Contains("outofmemory"))
                {
                    this.LogError("檢測到記憶體不足錯誤");
                    return JsonValidFail("記憶體不足，請增加伺服器記憶體或減少資料量");
                }
                
                // 原本的錯誤處理
                return JsonValidFail(ex.ToString());
            }
        }
    }
}
