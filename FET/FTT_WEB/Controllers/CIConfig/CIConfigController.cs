/**
 * 舊版頁面： "Storemgt/CIConfig.aspx"
 */
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.CIConfig
{
    /// <summary>
    /// 例外派工維護
    /// </summary>
    public partial class CIConfigController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
