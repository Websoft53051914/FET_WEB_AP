/**
 * 舊版頁面： "/pool/process.aspx",
 */
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_WEB.Controllers.Dispatching
{
    /// <summary>
    /// 派工中
    /// </summary>
    public partial class DispatchingController : BaseProjectController
    {
        /// <summary>
        /// 派工中
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
