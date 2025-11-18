using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers
{
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
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
                var loginHanlder = new LoginHandler(_configHelper, HttpContext);
                loginHanlder.Logout(token ?? string.Empty);
                this.LogSuccess("登出成功");
                Response.Cookies.Delete(FTT_API.Common.Const.TOKEN_NAME);
                Response.Cookies.Delete(FTT_API.Common.Const.USER_LOGIN_NAME);
                Response.Cookies.Delete(FTT_API.Common.Const.USER_ROLE);

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
