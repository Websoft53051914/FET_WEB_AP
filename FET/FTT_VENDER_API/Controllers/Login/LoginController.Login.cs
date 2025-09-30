using Const.VO;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Models.Handler;
using FTT_VENDER_API.Models.ViewModel.Login;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.Login
{
    /// <summary>
    /// 登入 API
    /// </summary>
    [Route("[controller]")]
    public partial class LoginController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        /// <summary>
        /// Constructor
        /// </summary>
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
                (LoginResultVO resultVO, SessionVO? sessionVO) = loginHanlder.Login(vm);

                if (!string.IsNullOrEmpty(resultVO.ErrorMsg))
                {
                    this.LogSuccess();
                    return JsonValidFail(resultVO.ErrorMsg);
                }

                if (!string.IsNullOrEmpty(resultVO.Token.TokenId))
                {
                    Response.Cookies.Append("Token", resultVO.Token.TokenId, new CookieOptions
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

                    userLoginName = sessionVO.engname;
                }
                Response.Cookies.Append("userLoginName", userLoginName, new CookieOptions { Secure = true, SameSite = SameSiteMode.None });
                Response.Cookies.Append("userrole", sessionVO?.userrole ?? string.Empty, new CookieOptions { Secure = true, SameSite = SameSiteMode.None });

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
