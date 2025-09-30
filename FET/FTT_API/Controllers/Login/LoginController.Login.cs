using Const.VO;
using Core.Utility.Helper.CaptchaCode;
using DocumentFormat.OpenXml.EMMA;
using FTT_API.Common;
using FTT_API.Common.Attribute;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Common.OriginClass.ModelClass;
using FTT_API.Models.Handler;
using FTT_API.Models.ViewModel.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
                var loginHanlder = new LoginHandler(_configHelper, HttpContext);
                (LoginResultVO resultVO, SessionVO? sessionVO) = loginHanlder.Login(vm);

                if (!string.IsNullOrEmpty(resultVO.ErrorMsg))
                {
                    this.LogSuccess();
                    return JsonValidFail(resultVO.ErrorMsg);
                }

                this.LogSuccess("登入成功");
                if (!string.IsNullOrEmpty(resultVO.Token.TokenId))
                {
                    Response.Cookies.Append(FTT_API.Common.Const.TOKEN_NAME, resultVO.Token.TokenId, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                    });
                }

                string userLoginName = string.Empty;
                if (sessionVO != null)
                {
                    string userType = sessionVO.usertype;

                    switch (userType)
                    {
                        case "RETAIL":
                        case "EMPLOYEE":
                            CEDS ceds = new CEDS();
                            userLoginName = ceds.GetEmpName(Employee.RefType.EmpNo, User.Identity.Name);
                            if (userType == "RETAIL")
                                userLoginName += $"IVR Code：{sessionVO.ivrcode} ";
                            ceds.Dispose();
                            break;
                        case "VASS":
                            userLoginName = $"IVR Code - {sessionVO.ivrcode} ";
                            break;
                        case "VENDOR":
                            userLoginName = sessionVO.engname;
                            break;
                        default:
                            break;
                    }
                }
                Response.Cookies.Append(FTT_API.Common.Const.USER_LOGIN_NAME, userLoginName, new CookieOptions { Secure = true, SameSite = SameSiteMode.None });
                Response.Cookies.Append(FTT_API.Common.Const.USER_ROLE, sessionVO?.userrole ?? string.Empty, new CookieOptions { Secure = true, SameSite = SameSiteMode.None });

                this.LogSuccess();
                return JsonOK();
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
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

            this.LogSuccess();
            return File(result.CaptchaImage, "image/jpeg");
        }

        //[CustomAuthorization(FuncID.Home_View)]
        [HttpGet("[action]")]
        public ActionResult CheckLogin()
        {
            if (LoginSession.Current.empno != null)
            {
                this.LogSuccess();
                return JsonOK();
            }
            this.LogSuccess();
            return JsonValidFail("逾時");
        }
    }
}
