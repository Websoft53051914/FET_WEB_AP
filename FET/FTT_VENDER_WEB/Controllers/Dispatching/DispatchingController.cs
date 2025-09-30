/**
 * 舊版頁面： "/pool/process.aspx",
 */
using FTT_VENDER_WEB.Models;
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
