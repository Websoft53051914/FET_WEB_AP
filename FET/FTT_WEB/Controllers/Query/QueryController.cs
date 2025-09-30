/**
 * 舊版頁面： "/pool/query.aspx"
 */
using Const.VO;
using FTT_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.Query
{
    /// <summary>
    /// 門市報修管理-查詢
    /// </summary>
    public class QueryController : BaseProjectController
    {
        /// <summary>
        /// 入口頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View(new QueryIndexVO());
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
