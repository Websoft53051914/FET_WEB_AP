using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.CaseClosed
{
    public class CaseClosedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(string formNo)
        {
            ViewData["form_no"] = formNo; ;

            return View();
        }
    }
}
