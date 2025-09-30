/**
 * 舊版頁面： "/pool/newopen2.aspx", "/Form/SubmitForm2.aspx(.cs), "/Form/StoreInfo.ascx", "/Form/TTInfo2.ascx"
 */
using Const.VO;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.NewOrderSelfVendor
{
    /// <summary>
    /// 自行尋商開單
    /// </summary>
    public partial class NewOrderSelfVendorController : BaseProjectController
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
