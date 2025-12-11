using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.Pending
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class PendingController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        /// <summary>
        /// Constructor
        /// </summary>
        public PendingController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
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
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                v_ftt_form2SQL _v_ftt_form2SQL = new v_ftt_form2SQL();

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _v_ftt_form2SQL.FindPageList(pageEntity, vm);

                for (int i = 0; i < list.Results.Count; i++)
                {
                    var item = list.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;
                }

                this.LogSuccess();
                return Json(new DataSourceResult
                {
                    Data = list.Results,
                    Total = list.DataCount
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
