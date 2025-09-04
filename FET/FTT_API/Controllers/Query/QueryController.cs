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

namespace FTT_API.Controllers.Query
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

                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 取得分頁資料<para/>
        /// [/pool/query.aspx]SearchCode_Click
        /// </summary>
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
                if (_sessionVO.userrole == "Vender")
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
        /// [/pool/printwp.aspx]ExportToExcel_Click()<para/>
        /// 列印維修單
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult ExportExcel(string jsonData)
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
                };
                if (_sessionVO.userrole == "Vender")
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

                // 調整欄寬
                for (int i = 0; i < writer.GetRow().LastCellNum; i++)
                {
                    writer.SetRowCellIndex(0, i);
                    int lenTitle = Encoding.UTF8.GetByteCount(writer.GetCell().StringCellValue);
                    writer.GetSheet().SetColumnWidth(writer.GetCellIndex(), (lenTitle + 6) * 256);
                }

                int totalPage = 1;
                int currentPage = 1;
                int pageDataSize = 100;
                int posRow = 1;
                request.pageSize = pageDataSize;
                do
                {
                    request.pageIndex = currentPage;

                    PageEntity pageEntity = GetPageEntity<QueryGridVO>(request);
                    // 取得資料
                    PageResult<VFttForm2DTO> pageList = queryHandler.GetPageListExport(pageEntity, searchVO);

                    if (currentPage == 1)
                    {
                        totalPage = (int)Math.Ceiling((double)pageList.DataCount / pageDataSize);
                    }

                    if (pageList.Results.Count == 0)
                    {
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

                MemoryStream? memoryStream = null;
                using (MemoryStream stream = new())
                {
                    writer.GetWorkBook().Write(stream, false);
                    memoryStream = new MemoryStream(stream.ToArray());
                }

                writer.GetWorkBook().Close();

                return File(memoryStream, "application/vnd.ms-excel", "CodeList.xls");
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
