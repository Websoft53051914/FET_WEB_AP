using Const.DTO;
using Const.VO;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                        .Select(x => new SelectListItem(x.status, x.status_name))
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
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult GetPageList(DataSourceRequest request, QueryIndexVO vm)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);

                QueryHandler queryHandler = new(_configHelper);
                // 取得資料，雖然 m_QueryString 會依角色有不同的 SQL，但實際結果相同
                PageResult<VFttForm2DTO> pageList = queryHandler.GetPageList(GetPageEntity(request), new VFttForm2DTO
                {

                });
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
    }
}
