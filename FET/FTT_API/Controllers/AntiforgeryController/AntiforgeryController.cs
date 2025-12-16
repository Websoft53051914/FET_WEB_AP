using Core.Utility.Web.Base;
using FTT_API.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.AntiforgeryController
{
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
    [EnableCors("AllowLocalhost7234")]
    public partial class AntiforgeryController : BaseController
    {
        private readonly IAntiforgery _antiforgery;

        // 透過 DI 注入 IAntiforgery 服務
        public AntiforgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        /// <summary>
        /// 獲取 Anti-Forgery Token。這個方法會設置必要的 Anti-Forgery Cookie，並返回 Request Token。
        /// </summary>
        
        [HttpGet("token")]
        [IgnoreAntiforgeryToken] // 這個接口本身不需要驗證 (因為它是用來獲取 Token 的)
        public IActionResult GetAntiforgeryToken()
        {
            // 核心方法：
            // 1. 在 Response 中設置必要的 Anti-Forgery Cookie (e.g., "CSRF-COOKIE")
            // 2. 生成並返回 Request Token
            AntiforgeryTokenSet tokens = _antiforgery.GetAndStoreTokens(HttpContext);

            // 將 Request Token 值作為 JSON 返回給前端
            return Ok(new { token = tokens.RequestToken });
        }
    }

}
