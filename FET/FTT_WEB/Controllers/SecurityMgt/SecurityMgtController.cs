using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.SecurityMgt
{
    public class SecurityMgtController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
