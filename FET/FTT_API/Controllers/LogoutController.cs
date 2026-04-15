using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers
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
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            try
            {
                Request.Cookies.TryGetValue("Token", out string? token);

                // 20260414 修正：相容舊版瀏覽器 Session 中可能殘留 URL 編碼過的 Token（如 %2E）。
                // 原因：舊版 LoginController 使用 HttpUtility.UrlEncode 將 Token 存入 Cookie，
                //       導致登出時傳入的 Token 與 tb_token 資料庫中的原始 JWT 不符，Status 無法設為 Cancel。
                // 20260414 若需還原：移除下方 if 區塊即可，但需同步還原 LoginController.CookieSafeEncode 為 HttpUtility.UrlEncode。
                if (!string.IsNullOrEmpty(token) && token.Contains('%'))
                {
                    try { token = Uri.UnescapeDataString(token); } catch { /* 解碼失敗則保留原值 */ }
                }

                var loginHanlder = new LoginHandler(_configHelper, HttpContext);
                loginHanlder.Logout(token ?? string.Empty);
                this.LogSuccess("登出成功");
                // 20260407 修正：刪除 Cookie 時須帶入與寫入時相同的 SameSite/Secure 選項，
                // 否則跨來源情境下瀏覽器視為不同 Cookie，刪除無效，導致 VASS 登出後 Cookie 殘留。
                var deleteCookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddDays(-1)  // 明確設定過期使瀏覽器立即刪除
                };
                Response.Cookies.Delete(FTT_API.Common.Const.TOKEN_NAME, deleteCookieOptions);
                Response.Cookies.Delete(FTT_API.Common.Const.USER_LOGIN_NAME, deleteCookieOptions);
                Response.Cookies.Delete(FTT_API.Common.Const.USER_ROLE, deleteCookieOptions);

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
