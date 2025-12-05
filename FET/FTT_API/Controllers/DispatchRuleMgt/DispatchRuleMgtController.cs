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

namespace FTT_API.Controllers.DispatchRuleMgt
{
    /// <summary>
    /// 派工規則維護
    /// </summary>
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public partial class DispatchRuleMgtController : BaseProjectController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatchRuleMgtController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        private ConfigurationHelper _configHelper;
    }

    public partial class DispatchRuleMgtController
    {
        /// <summary>
        /// 取得分頁資料<para/>
        /// [/Dispatch/Config_Dispatch.aspx]
        /// </summary>
        [HttpPost("[action]")]
        public IActionResult GetPageList(DataSourceRequest request)
        {
            try
            {
                DispatchRuleMgtHandler dispatchRuleMgtHandler = new(_configHelper);
                // 取得資料
                PageResult<DispatchProfileDTO> pageList = dispatchRuleMgtHandler.GetPageList(GetPageEntity<QueryGridVO>(request));
                // 轉成 ViewModel
                List<DispatchRuleMgtGridVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    DispatchProfileDTO data = pageList.Results[i];

                    DispatchRuleMgtGridVO item = new()
                    {
                        Id = data.id,
                        IfWarrant = data.ifwarrant,
                        TVender = data.tvender,
                        Vender = data.vender,
                        TCisId = data.tcisid,
                        CiName = data.ci_name,
                        TIvrCode = data.tivr_code,
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
        /// 取得頁面資料
        /// </summary>
        [HttpPost("[action]")]
        public IActionResult GetInitEditData(int? id)
        {
            try
            {
                CommonHandler commonHandler = new(_configHelper);
                DispatchRuleMgtHandler dispatchRuleMgtHandler = new(_configHelper);
                // TODO： 編輯資料
                DispatchProfileDTO? dto = null;
                if (id.HasValue)
                {
                    dto = dispatchRuleMgtHandler.GetOneDispatchProfile(id.Value);
                    if (dto == null)
                    {
                        this.LogSuccess();
                        return JsonValidFail("資料不存在");
                    }
                }
                dto ??= new();

                DispatchRuleMgtEditVO result = new()
                {
                    Data = new()
                    {
                        Id = id,
                        CisIdList = dto.tcisid?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? [],
                        IvrCodeList = dto.tivr_code?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? [],
                        VenderIdList = dto.tvender?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? [],
                        IfWarrant = dto.ifwarrant ?? "N",
                    },
                    SelectListIvrCode = commonHandler.GetListCommonGroup()
                        .Select(x => new SelectListItem($"{x.area} {x.shop_name}＊{x.ivr_code}", x.ivr_code))
                        .ToList(),
                    SelectListVender = commonHandler.GetListCommonVender()
                        .Select(x => new SelectListItem(x.merchant_name, x.order_id.ToString()))
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
        /// 取得頁面資料
        /// </summary>
        [HttpPost("[action]")]
        public IActionResult GetInitDataQuery()
        {
            try
            {
                CommonHandler commonHandler = new(_configHelper);
                DispatchRuleMgtQueryVO result = new DispatchRuleMgtQueryVO
                {
                    SelectListCompany = commonHandler.GetListStoreType(new StoreTypeDTO
                    {
                        TypeNameEq = "COMPANY_LEAVES",
                    })
                        .Select(x => new SelectListItem(x.type_value, x.type_value))
                        .ToList(),
                    SelectListStoreType = commonHandler.GetListStoreType(new StoreTypeDTO
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
        /// [/pool/queryDispatch.aspx]SearchCode_Click
        /// </summary>
        [HttpPost("[action]")]
        public IActionResult GetPageListQuery(DataSourceRequest request, DispatchRuleMgtQueryVO vm)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);

                DispatchRuleMgtHandler dispatchRuleMgtHandler = new(_configHelper);
                dispatchRuleMgtHandler.SessionVO = _sessionVO;
                VDispatchQueryDTO searchVO = new()
                {
                    CategoryIdFilter = vm.CategoryIdFilter,
                    VenderIdEq = vm.VenderIdEq,
                    IvrCodeEq = vm.IvrCodeEq,
                    CompanyEq = vm.CompanyEq,
                    StoreTypeEq = vm.StoreTypeEq,
                    ChannelEq = vm.ChannelEq,
                    AreaEq = vm.AreaEq,
                    IfWarrantEq = vm.IfWarrantEq,
                };

                // 取得資料，雖然 m_QueryString 會依角色有不同的 SQL，但實際結果相同
                PageResult<VDispatchQueryDTO> pageList = dispatchRuleMgtHandler.GetPageListQuery(GetPageEntity<DispatchRuleMgtQueryVO>(request), searchVO);
                // 轉成 ViewModel
                List<DispatchRuleMgtQueryGridVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    VDispatchQueryDTO data = pageList.Results[i];

                    DispatchRuleMgtQueryGridVO item = new()
                    {
                        IvrCode = data.ivr_code,
                        ShopName = data.shop_name,
                        L1Desc = data.l1_desc,
                        L2Desc = data.l2_desc,
                        CiName = data.ciname,
                        Warrant = data.warrant,
                        NonWarrant = data.nonwarrant,
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
        /// 匯出Excel<para/>
        /// [/pool/queryDispatch.aspx]ExportToExcel_Click()<para/>
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult ExportExcelQuery(string jsonData)
        {
            try
            {
                ArgumentNullException.ThrowIfNullOrWhiteSpace(jsonData);
                DispatchRuleMgtQueryVO? vm = JsonConvert.DeserializeObject<DispatchRuleMgtQueryVO>(jsonData);
                DataSourceRequest? request = JsonConvert.DeserializeObject<DataSourceRequest>(jsonData);
                ArgumentNullException.ThrowIfNull(vm);
                ArgumentNullException.ThrowIfNull(request);
                DispatchRuleMgtHandler dispatchRuleMgtHandler = new(_configHelper);
                dispatchRuleMgtHandler.SessionVO = _sessionVO;
                VDispatchQueryDTO searchVO = new()
                {
                    CategoryIdFilter = vm.CategoryIdFilter,
                    VenderIdEq = vm.VenderIdEq,
                    IvrCodeEq = vm.IvrCodeEq,
                    CompanyEq = vm.CompanyEq,
                    StoreTypeEq = vm.StoreTypeEq,
                    ChannelEq = vm.ChannelEq,
                    AreaEq = vm.AreaEq,
                    IfWarrantEq = vm.IfWarrantEq,
                };

                ExcelWriterHelper writer = new();
                IWorkbook wb = writer.CreateWorkBook(ExcelType.HSSF);
                ISheet sheet = writer.CreateSheet(DateTime.Now.ToString(DbConst.FORMAT_DATETIME));
                writer.SetRowCellIndex(0, 0);
                #region -- 設定標題 --
                writer.SetCellValue("IVR_CODE");
                writer.SetCellValue("門市名稱");
                writer.SetCellValue("報修類別");
                writer.SetCellValue("報修次類別");
                writer.SetCellValue("報修名稱");
                writer.SetCellValue("保固內廠商");
                writer.SetCellValue("保固外廠商");
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

                    PageEntity pageEntity = GetPageEntity<DispatchRuleMgtQueryVO>(request);
                    // 取得資料
                    PageResult<VDispatchQueryDTO> pageList = dispatchRuleMgtHandler.GetPageListQuery(pageEntity, searchVO);

                    if (currentPage == 1)
                    {
                        totalPage = (int)Math.Ceiling((double)pageList.DataCount / pageDataSize);
                    }

                    if (pageList.Results.Count == 0)
                    {
                        break;
                    }

                    foreach (VDispatchQueryDTO item in pageList.Results)
                    {
                        writer.SetRowCellIndex(posRow, 0);
                        writer.SetCellValue(item.ivr_code ?? string.Empty);          // IVR_CODE
                        writer.SetCellValue(item.shop_name ?? string.Empty);         // 門市名稱
                        writer.SetCellValue(item.l1_desc ?? string.Empty);           // 報修類別
                        writer.SetCellValue(item.l2_desc ?? string.Empty);           // 報修次類別
                        writer.SetCellValue(item.ciname ?? string.Empty);            // 報修名稱
                        writer.SetCellValue(item.warrant ?? string.Empty);           // 保固內廠商
                        writer.SetCellValue(item.nonwarrant ?? string.Empty);        // 保固外廠商

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

                this.LogSuccess();
                return File(memoryStream, "application/vnd.ms-excel", "DispatchList.xls");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 新增<para/>
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult Create(DispatchRuleMgtVO vm)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);
                DispatchRuleMgtHandler dispatchRuleMgtHandler = new(_configHelper);
                vm.VenderIdList ??= [];
                vm.IvrCodeList ??= [];
                vm.CisIdList ??= [];

                dispatchRuleMgtHandler.ExecDispatchInsert(string.Join(",", vm.IvrCodeList), string.Join(",", vm.VenderIdList), vm.IfWarrant ?? "N", string.Join(",", vm.CisIdList));

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
        /// 編輯<para/>
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult Edit(DispatchRuleMgtVO vm)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);
                ArgumentNullException.ThrowIfNull(vm.Id);
                DispatchRuleMgtHandler dispatchRuleMgtHandler = new(_configHelper);
                vm.VenderIdList ??= [];
                vm.IvrCodeList ??= [];
                vm.CisIdList ??= [];

                dispatchRuleMgtHandler.ExecDispatchUpdate(vm.Id.Value, string.Join(",", vm.IvrCodeList), string.Join(",", vm.VenderIdList), vm.IfWarrant ?? "N", string.Join(",", vm.CisIdList));

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
        /// 編輯<para/>
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult Delete(int? id)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(id);
                DispatchRuleMgtHandler dispatchRuleMgtHandler = new(_configHelper);

                dispatchRuleMgtHandler.ExecDispatchDelete(id.Value);

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
