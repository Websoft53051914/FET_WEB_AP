using Core.Utility.Helper.CaptchaCode;
using DocumentFormat.OpenXml.EMMA;
using FTT_API.Common;
using FTT_API.Common.Attribute;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using FTT_API.Models.ViewModel.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Const.Enums;

namespace FTT_API.Controllers.Login
{
    [Route("[controller]")]
    public partial class LoginController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        public LoginController(IWebHostEnvironment hostingEnvironment, ConfigurationHelper configHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _configHelper = configHelper;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login(LoginVM vm)
        {
            try
            {
                var loginHanlder = new LoginHanlder(_configHelper, HttpContext);
                string errorMsg = loginHanlder.Login(vm);

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    return JsonValidFail(errorMsg);
                }

                return JsonOK();
            }
            catch (Exception ex)
            {
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }



        /// <summary>
        /// 畫出 圖形驗證碼
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public ActionResult CaptchaCode()
        {
            //自製的土炮驗證碼
            CaptchaCodeHelper_ImageSharp captchaCode = new()
            {
                Width = 100
            };

            CaptchaResult result = captchaCode.Result();
            TempData[CaptchaCodeHelper.CAPTCHA_CODE] = result.ResultCode;

            return File(result.CaptchaImage, "image/jpeg");
        }

        //[CustomAuthorization(FuncID.Home_View)]
        [HttpGet("[action]")]
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
