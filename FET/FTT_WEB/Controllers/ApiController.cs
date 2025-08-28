using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_WEB.Common.ConfigurationHelper;
using FTT_WEB.Common.OriginClass.EntiityClass;
using FTT_WEB.Models.Handler;
using FTT_WEB.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers
{
    public class ApiController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        public ApiController(ConfigurationHelper config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        private readonly IWebHostEnvironment _env;

        [HttpPost]
        public IActionResult GetCiDataSelfVendorPageList(DataSourceRequest request)
        {
            try
            {
                CommonHandler commonHandler = new(_config);
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
                return JsonValidFail(_config.GetMessage("SystemErrorMsg"));
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

                CommonHandler commonHandler = new(_config);
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
                return JsonValidFail("取得報修品項資料發生錯誤：" + _config.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
