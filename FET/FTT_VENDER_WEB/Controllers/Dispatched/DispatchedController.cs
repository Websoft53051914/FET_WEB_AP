/**
 * 舊版頁面： "/pool/printwp.aspx",
 */
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_WEB.Controllers.Dispatched
{
    /// <summary>
    /// 已派工
    /// </summary>
    public partial class DispatchedController : BaseProjectController
    {
        /// <summary>
        /// 已派工
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
