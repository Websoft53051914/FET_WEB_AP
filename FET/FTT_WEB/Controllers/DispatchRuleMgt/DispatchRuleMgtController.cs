/**
 * 舊版頁面： "Dispatch/Config_Dispatch.aspx", "/pool/queryDispatch.aspx"
 */
using Const.VO;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.DispatchRuleMgt
{
    /// <summary>
    /// 派工規則維護
    /// </summary>
    public partial class DispatchRuleMgtController : BaseProjectController
    {
        /// <summary>
        /// 派工規則維護
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 派工規則查詢
        /// </summary>
        /// <returns></returns>
        public IActionResult Query()
        {
            return View(new DispatchRuleMgtQueryVO());
        }

        /// <summary>
        /// 派工規則維護-新增
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View("Edit", new DispatchRuleMgtVO());
        }

        /// <summary>
        /// 派工規則維護-編輯
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit(int? id)
        {
            return View(new DispatchRuleMgtVO
            {
                Id = id
            });
        }
    }
}
