/**
 * 舊版頁面： "/pool/newopen.aspx", "/Form/SubmitForm.aspx(.cs), "/Form/StoreInfo.ascx", "/Form/TTInfo.ascx"
 */
using Const.VO;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.NewOrder
{
    /// <summary>
    /// 新開單
    /// </summary>
    public partial class NewOrderController : BaseProjectController
    {
        /// <summary>
        /// 入口頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            DateTime now = DateTime.Now;
            // [SubmitForm.aspx.cs.Page_Load]年節期間暫停設備報修
            if (now < Common.Const.LUNAR_NEW_YEAR_END && now > Common.Const.LUNAR_NEW_YEAR_START)
            {
                return View("NewOrder/StopByLunarNewYear");
            }

            return View(new NewOrderVO());
        }
    }
}
