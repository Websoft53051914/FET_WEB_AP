using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.InProcess
{
    public class InProcessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
