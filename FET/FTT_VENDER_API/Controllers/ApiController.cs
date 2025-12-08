using Const;
using Const.DTO;
using Const.VO;
using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.Handler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers
{
    /// <summary>
    /// API
    /// </summary>
    [Route("[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiController : BaseProjectController
    {
        private readonly ConfigurationHelper _configHelper;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="env"></param>
        public ApiController(ConfigurationHelper config, IWebHostEnvironment env)
        {
            _configHelper = config;
            _env = env;
        }

        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// 取得維修品項指定 parentId 下的子項目資料
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult GetListTreeChildrenCi(int? parentId, string reqSrc = "ALL", string acType = "")
        {
            try
            {
                ArgumentNullException.ThrowIfNull(parentId);

                CommonHandler commonHandler = new(_configHelper);
                List<CIRelationsDTO> dtoList = commonHandler.GetListCIRelations(parentId.Value, reqSrc, acType);
                List<TreeJsFlatModel> result = [];

                foreach (CIRelationsDTO dto in dtoList)
                {
                    TreeJsFlatModel item = new()
                    {
                        Id = dto.cisid.ToString(),
                        Text = dto.ciname ?? string.Empty,
                        Parent = parentId.Value.ToString(),
                        Children = dto.HasChildren,
                        OtherAttr = new Dictionary<string, string>
                        {
                            { "CATEGORY_ID", dto.cisid.ToString() },
                            { "CATEGORY_NAME_TMP", dto.ciname ?? string.Empty },
                            { "CATEGORY_NAME", dto.fullname ?? string.Empty },
                            { "TT_CATEGORY_NOTE", dto.notes ?? string.Empty },
                            { "TT_CATEGORY_DESC", dto.descr ?? string.Empty },
                        },
                    };

                    if (!dto.HasChildren && !string.IsNullOrWhiteSpace(dto.ciname))
                    {
                        // 只取檔名，去掉路徑
                        var safeFileName = Path.GetFileName(dto.ciname.Trim()) + ".jpg";

                        // 固定存放資料夾
                        var filePath = Path.Combine("Item", safeFileName);
                        var path = Path.Combine(_env.WebRootPath, filePath);

                        if (System.IO.File.Exists(path))
                        {
                            item.OtherAttr.Add("TT_IMAGE", filePath);
                        }
                    }

                    result.Add(item);
                }

                this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                return JsonValidFail("取得報修品項資料發生錯誤：" + _configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 取得維修品項指定 id 下的項目資料與其階層資料
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult GetListTreeItemCi(List<int> idList, string reqSrc = "ALL", string acType = "")
        {
            try
            {
                ArgumentNullException.ThrowIfNull(idList);

                CommonHandler commonHandler = new(_configHelper);
                List<CIRelationsDTO> dtoList = commonHandler.GetListCIRelations(idList, reqSrc, acType);
                List<TreeJsFlatModel> result = [];
                foreach (CIRelationsDTO dto in dtoList)
                {
                    TreeJsFlatModel item = new()
                    {
                        Id = dto.cisid.ToString(),
                        Text = dto.ciname ?? string.Empty,
                        Parent = dto.parentsid?.ToString() ?? string.Empty,
                        Children = dto.HasChildren,
                        OtherAttr = new Dictionary<string, string>
                        {
                            { "CATEGORY_ID", dto.cisid.ToString() },
                            { "CATEGORY_NAME_TMP", dto.ciname ?? string.Empty },
                            { "CATEGORY_NAME", dto.fullname ?? string.Empty },
                            { "TT_CATEGORY_NOTE", dto.notes ?? string.Empty },
                            { "TT_CATEGORY_DESC", dto.descr ?? string.Empty },
                            { "PathCsv", dto.path_csv ?? string.Empty },
                        },
                    };

                    if (!dto.HasChildren && !string.IsNullOrWhiteSpace(dto.ciname))
                    {
                        // 1️⃣ 取安全檔名，去掉路徑
                        var safeFileName = Path.GetFileName(dto.ciname.Trim()) + ".jpg";

                        // 2️⃣ 指定固定資料夾
                        var filePath = Path.Combine("images", "Item", safeFileName);
                        var path = Path.Combine(_env.WebRootPath, filePath);

                        // 3️⃣ 驗證檔案存在
                        if (System.IO.File.Exists(path))
                        {
                            item.OtherAttr.Add("TT_IMAGE", filePath);
                        }
                    }

                    result.Add(item);
                }

                this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                return JsonValidFail("取得報修品項資料發生錯誤：" + _configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 取得門市分頁資料
        /// </summary>
        [HttpPost("[action]")]
        public IActionResult GetPageListStore(DataSourceRequest request, DialogIvrCodeGridVO vm)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);
                CommonHandler onsitePrintHandler = new(_configHelper);
                // 取得資料(應該只有自行尋商開單的單據會顯示(vender_id 為當前門市的 ivrcode))
                PageResult<StoreProfileDTO> pageList = onsitePrintHandler.GetPageListStore(GetPageEntity(request), new StoreProfileDTO
                {
                    IvrCodeLike = vm.IvrCodeLike?.Trim(),
                    ShopNameLike = vm.ShopNameLike?.Trim(),
                    CompanyLeavesLike = vm.CompanyLeavesLike?.Trim(),
                    ChannelLike = vm.ChannelLike?.Trim(),
                    StoreTypeLike = vm.StoreTypeLike?.Trim(),
                });
                // 轉成 ViewModel
                List<DialogIvrCodeGridGridVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    StoreProfileDTO data = pageList.Results[i];

                    DialogIvrCodeGridGridVO item = new()
                    {
                        IvrCode = data.ivr_code,
                        CompanyLeaves = data.company_leaves,
                        StoreType = data.store_type,
                        Channel = data.channel,
                        Area = data.area,
                        ShopName = data.shop_name,
                        OwnerName = data.owner_name,
                        AsName = data.as_name,
                        OwnerTel = data.owner_tel,
                        UrgentTel = data.urgent_tel,
                        Address = data.address,
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
                LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 取得選單待處理筆數資料
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult GetMenuDataCount([FromBody] List<int> funcIdList)
        {
            try
            {
                funcIdList ??= [];
                PageEntity pageEntity = new PageEntity
                {
                    CurrentPage = 1,
                    PageDataSize = 1,
                };

                Dictionary<string, int> result = [];
                foreach (int funcId in funcIdList)
                {
                    if (funcId == Enums.FuncID.InProcess_View.ToInt())
                    {
                        InProcessHandler handler = new(_configHelper, HttpContext);
                        PageResult<v_ftt_form2DTO> pageResult = handler.FindPageListForCount(pageEntity, new v_ftt_form2DTO
                        {
                            USERROLE = _sessionVO.userrole,
                            IVRCODE = _sessionVO.ivrcode,
                            EMPNO = _sessionVO.empno,
                        });
                        result.Add(funcId.ToString(), pageResult.DataCount);
                    }
                    else if (funcId == Enums.FuncID.Dispatching_View.ToInt())
                    {
                        DispatchingHandler handler = new(_configHelper)
                        {
                            SessionVO = _sessionVO
                        };
                        PageResult<VFttForm2DTO> pageResult = handler.GetPageListForCount(pageEntity);
                        result.Add(funcId.ToString(), pageResult.DataCount);
                    }
                    else if (funcId == Enums.FuncID.Dispatched_View.ToInt())
                    {
                        DispatchedHandler handler = new(_configHelper)
                        {
                            SessionVO = _sessionVO
                        };
                        PageResult<VFttForm2DTO> pageResult = handler.GetPageListForCount(pageEntity);
                        result.Add(funcId.ToString(), pageResult.DataCount);
                    }
                    else if (funcId == Enums.FuncID.CaseClosed_View.ToInt())
                    {
                        CaseClosedHandler handler = new(_configHelper, HttpContext) { };
                        PageResult<v_ftt_form2DTO> pageResult = handler.FindPageListForCount(pageEntity, new v_ftt_form2DTO
                        {
                            USERROLE = _sessionVO.userrole,
                            IVRCODE = _sessionVO.ivrcode,
                            EMPNO = _sessionVO.empno,
                        });
                        result.Add(funcId.ToString(), pageResult.DataCount);
                    }
                }

                this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
