using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.InProcess
{
    public class InProcessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
