using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.QuoteMgt
{
    public class QuoteMgtController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
