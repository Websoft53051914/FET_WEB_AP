using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Models.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers
{
    [Route("[controller]")]
    public class LogoutController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        public LogoutController(IWebHostEnvironment hostingEnvironment, ConfigurationHelper configHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _configHelper = configHelper;
        }
        [HttpPost("[action]")]
        public IActionResult Logout()
        {
            try
            {
                Request.Cookies.TryGetValue("Token", out string? token);
                var loginHanlder = new LoginHanlder(_configHelper, HttpContext);
                loginHanlder.Logout(token ?? string.Empty);
                this.LogSuccess("登出成功");
                Response.Cookies.Delete("Token");
                Response.Cookies.Delete("userLoginName");
                Response.Cookies.Delete("userrole");

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
