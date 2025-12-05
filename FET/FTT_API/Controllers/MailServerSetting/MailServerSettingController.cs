using FTT_API.Models.ViewModel.MailServerSetting;
using Microsoft.AspNetCore.Mvc;
using FTT_API.Models;
using static Const.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using FTT_API.Models.Handler;
using FTT_API.Common.ConfigurationHelper;
using Microsoft.AspNetCore.Cors;


namespace FTT_API.Controllers.MailServerSetting
{
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
    [EnableCors("AllowLocalhost7234")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class MailServerSettingController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        public MailServerSettingController(IWebHostEnvironment hostingEnvironment, ConfigurationHelper configHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _configHelper = configHelper;
        }

        [HttpPost("[action]")]
        public IActionResult Update(MailServerSettingVM vm)
        {
            var MailServerHandler = new MailServerHandler(_configHelper, HttpContext);
            try
            {
                MailServerHandler.Update(vm);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return BadRequest();
            }

            //return BadRequest();
            this.LogSuccess("MailServerSetting儲存成功");
            return JsonSuccess("資料儲存成功");
        }


    }
}
