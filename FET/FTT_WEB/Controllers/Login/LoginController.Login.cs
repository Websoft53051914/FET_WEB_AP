using Core.Utility.Helper.CaptchaCode;
using FTT_WEB.Common;
using FTT_WEB.Common.Attribute;
using FTT_WEB.Common.ConfigurationHelper;
using FTT_WEB.Models.Handler;
using FTT_WEB.Models.ViewModel.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Const.Enums;

namespace FTT_WEB.Controllers.Login
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
        [Route("/auth/loginFromSP.aspx")]
        public IActionResult IndexSP(string goalURL = "")
        {
            return View();
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

    }
}
