using Const.DTO;
using Const.VO;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.Dispatched
{
    /// <summary>
    /// 已派工 API
    /// </summary>
    [Route("[controller]")]
    public partial class DispatchedController : BaseProjectController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatchedController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        private ConfigurationHelper _configHelper;
    }

    public partial class DispatchedController
    {
        /// <summary>
        /// 取得分頁資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult GetPageList(DataSourceRequest request)
        {
            try
            {
                DispatchedHandler dispatchedHandler = new(_configHelper);
                dispatchedHandler.SessionVO = _sessionVO;
                // 取得資料(應該只有自行尋商開單的單據會顯示(vender_id 為當前門市的 ivrcode))
                PageResult<VFttForm2DTO> pageList = dispatchedHandler.GetPageList(GetPageEntity<DispatchedGridVO>(request));
                // 轉成 ViewModel
                List<DispatchedGridVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    VFttForm2DTO data = pageList.Results[i];

                    DispatchedGridVO item = new()
                    {
                        CiName = data.ciname,
                        CreateTimeText = data.createtime_text,
                        FormNo = data.form_no,
                        StatusName = data.statusname,
                        TtCategory = data.tt_category,
                        ShopName = data.shop_name,
                        Vender = data.vender,
                        Flag1 = data.flag1,
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
