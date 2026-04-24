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

        //20260416 Add begin - IndexSP 加上說明：支援加密 token 參數（資安要求，取代明碼 IVRCODE/EMPNO/RETAILID）
        /// <summary>
        /// SSO 入口（LoginFromSP.aspx）。
        ///
        /// ── 舊版（明碼，資安禁止）：
        ///   ?IVRCODE=2255&EMPNO=67479&RETAILID=2255
        ///
        /// ── 新版（加密，請使用此格式）：
        ///   ?token={DES-ECB-Base64加密字串}
        ///   加密方式：DESCryptoUtility.DESEncrypt_ECB_Base64(
        ///              "IVRCODE=2255&EMPNO=67479&RETAILID=2255",
        ///              SP_decrypt_key  // appsettings.json "SP_decrypt_key"
        ///            )
        ///
        /// View（IndexSP.cshtml）會依 URL 是否含 token 參數
        /// 自動選擇呼叫 CheckSSO_Encrypted 或 CheckSSO。
        /// </summary>
        //20260416 Add end
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
