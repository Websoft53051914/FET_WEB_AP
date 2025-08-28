using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.SecurityMgt
{
    public class SecurityMgtController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
