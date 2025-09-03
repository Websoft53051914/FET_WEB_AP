/**
 * 舊版頁面： "/pool/query.aspx"
 */
using Const.VO;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.Query
{
    /// <summary>
    /// 門市報修管理-查詢
    /// </summary>
    public class QueryController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View(new QueryIndexVO());
        }
    }
}
