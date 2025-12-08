using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.Handler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.InProcess
{
    /// <summary>
    /// 處理中 API
    /// </summary>
    [Route("[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InProcessController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        /// <summary>
        /// Constructor
        /// </summary>
        public InProcessController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult GetPageList(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var _InProcessHanlder = new InProcessHandler(_config, HttpContext);
                var pageList = _InProcessHanlder.FindPageList(pageEntity, vm);

                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    var item = pageList.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;

                    item.IsTicket = item.StatusId == "TICKET";
                }

                this.LogSuccess();
                return Json(new DataSourceResult
                {
                    Data = pageList.Results,
                    Total = pageList.DataCount
                });
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }
    }
}
