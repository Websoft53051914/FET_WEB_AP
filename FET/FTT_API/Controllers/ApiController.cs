using Const.DTO;
using Const.VO;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using FTT_API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers
{
    /// <summary>
    /// API
    /// </summary>
    [Route("[controller]")]
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
        /// 取得自行尋商開單的維修品項分頁資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetCiDataSelfVendorPageList(DataSourceRequest request)
        {
            try
            {
                CommonHandler commonHandler = new(_configHelper);
                // 取得資料
                PageResult<CIRelationsDTO> pageList = commonHandler.GetPageListCiDataSelfVendor(GetPageEntity(request));
                // 轉成 ViewModel
                List<CiDataVM> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    CIRelationsDTO data = pageList.Results[i];

                    CiDataVM item = new()
                    {
                        CATEGORY_ID = data.CISID,
                        CATEGORY_NAME = data.ACINAME,
                        CATEGORY_NAME_TMP = data.CINAME,
                        TT_CATEGORY_NOTE = data.NOTES,
                        TT_CATEGORY_DESC = data.DESCR,
                    };

                    if (!string.IsNullOrWhiteSpace(data.CINAME))
                    {
                        string filePath = $"images/Item/{data.CINAME.Trim()}.jpg";
                        string path = Path.Combine(_env.WebRootPath, filePath);
                        if (System.IO.File.Exists(path))
                        {
                            item.TT_IMAGE = filePath;
                        }
                    }

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
        /// 取得維修品項指定 parentId 下的子項目資料
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetTreeListTest(int? parentId)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(parentId);

                CommonHandler commonHandler = new(_configHelper);
                List<CIRelationsDTO> dtoList = commonHandler.GetListCIRelations(parentId.Value, "ALL");
                List<TreeJsFlatModel> result = [];

                foreach (CIRelationsDTO dto in dtoList)
                {
                    TreeJsFlatModel item = new()
                    {
                        Id = dto.CISID.ToString(),
                        Text = dto.CINAME ?? string.Empty,
                        Parent = parentId.Value.ToString(),
                        Children = dto.HasChildren,
                        OtherAttr = new Dictionary<string, string>
                        {
                            { "CATEGORY_ID", dto.CISID.ToString() },
                            { "CATEGORY_NAME_TMP", dto.CINAME ?? string.Empty },
                            { "CATEGORY_NAME", dto.FULLNAME ?? string.Empty },
                            { "TT_CATEGORY_NOTE", dto.NOTES ?? string.Empty },
                            { "TT_CATEGORY_DESC", dto.DESCR ?? string.Empty },
                        },
                    };

                    if (!dto.HasChildren && !string.IsNullOrWhiteSpace(dto.CINAME))
                    {
                        string filePath = $"images/Item/{dto.CINAME.Trim()}.jpg";
                        string path = Path.Combine(_env.WebRootPath, filePath);
                        if (System.IO.File.Exists(path))
                        {
                            item.OtherAttr.Add("TT_IMAGE", filePath);
                        }
                    }

                    result.Add(item);
                }

                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                LogError(ex);
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
        /// 取得廠商分頁資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult GetPageListVender(DataSourceRequest request, DialogVenderGridVO vm)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);
                CommonHandler onsitePrintHandler = new(_configHelper);
                // 取得資料(應該只有自行尋商開單的單據會顯示(vender_id 為當前門市的 ivrcode))
                PageResult<StoreVenderProfileDTO> pageList = onsitePrintHandler.GetPageListVender(GetPageEntity(request), new StoreVenderProfileDTO
                {
                    MerchantNameLike = vm.MerchantNameLike?.Trim(),
                    CpNameLike = vm.CpNameLike?.Trim(),
                });
                // 轉成 ViewModel
                List<DialogVenderGridGridVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    StoreVenderProfileDTO data = pageList.Results[i];

                    DialogVenderGridGridVO item = new()
                    {
                        OrderId = data.order_id,
                        MerchantName = data.merchant_name,
                        CpName = data.cp_name,
                        CpTel = data.cp_tel,
                        Email = data.email,
                        MerchantLogin = data.merchant_login,
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
    }
}
