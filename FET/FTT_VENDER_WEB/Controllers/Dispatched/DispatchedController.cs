/**
 * 舊版頁面： "/pool/printwp.aspx",
 */
using FTT_VENDER_WEB.Models;
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

        /// <summary>
        /// 維修單明細頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Detail(string formNo)
        {
            ViewData["form_no"] = formNo;

            return View(new FormTableVM());
        }
    }
}
