using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FTT_API.Common.Attribute
{
    public class AntiforgeryTokenCookieAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var antiforgery = context.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();
            var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
            // 將 Token 存入前端可讀取的 Cookie (HttpOnly 必須為 false)
            context.HttpContext.Response.Cookies.Append("X-CSRF-TOKEN", tokens.RequestToken, 
            new CookieOptions { HttpOnly = false, Secure = true,SameSite = SameSiteMode.None });
        }
    }
}
