using Core.Utility.Helper.CaptchaCode;
using FTT_VENDER_WEB.Common;
using FTT_VENDER_WEB.Common.Attribute;
using FTT_VENDER_WEB.Common.ConfigurationHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Const.Enums;

namespace FTT_VENDER_WEB.Controllers.Login
{
    public partial class LoginController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        public LoginController(IWebHostEnvironment hostingEnvironment, ConfigurationHelper configHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _configHelper = configHelper;
        }
        public IActionResult Index(string goalURL = "")
        {
            if (LoginSession.Current != null && LoginSession.Current.empno != null)
            {
                if (!string.IsNullOrEmpty(goalURL))
                {
                    return Redirect(goalURL);
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }

            return View();
        }
         

        [AllowAnonymous]
        public ActionResult PermissionDenied()
        {
            return View();
        }


        [CustomAuthorization(FuncID.Home_View)]
        public ActionResult CheckLogin()
        {
            if (LoginSession.Current.empno != null)
            {
                return JsonOK();
            }
            return JsonValidFail("逾時");
        }
    }
}
