using Const.DTO;
using Const.VO;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.Dispatching
{
    /// <summary>
    /// 派工中 API
    /// </summary>
    [Route("[controller]")]
    public partial class DispatchingController : BaseProjectController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatchingController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        private ConfigurationHelper _configHelper;
    }

    public partial class DispatchingController
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
                DispatchingHandler dispatchingHandler = new(_configHelper);
                dispatchingHandler.SessionVO = _sessionVO;
                // 取得資料(應該只有自行尋商開單的單據會顯示(vender_id 為當前門市的 ivrcode))
                PageResult<VFttForm2DTO> pageList = dispatchingHandler.GetPageList(GetPageEntity(request));
                // 轉成 ViewModel
                List<DispatchingGridVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    VFttForm2DTO data = pageList.Results[i];

                    DispatchingGridVO item = new()
                    {
                        CiName = data.ciname,
                        CreateTimeText = data.createtime_text,
                        FormNo = data.form_no,
                        L2Desc = data.l2_desc,
                        StatusName = data.statusname,
                        TtCategory = data.tt_category,
                        UpdateTimeText = data.updatetime_text,
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
