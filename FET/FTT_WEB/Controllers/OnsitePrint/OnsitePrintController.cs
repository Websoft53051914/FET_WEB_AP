/**
 * 舊版頁面： "/pool/printwp.aspx", "/pool/WP.aspx"
 */
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.OnsitePrint
{
    /// <summary>
    /// 列印到場單
    /// </summary>
    public class OnsitePrintController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
