/**
 * 舊版頁面： "/pool/printwp.aspx", "/pool/WP.aspx"
 */
using FTT_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.OnsitePrint
{
    /// <summary>
    /// 列印到場單
    /// </summary>
    public class OnsitePrintController : BaseProjectController
    {
        /// <summary>
        /// 入口頁
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
