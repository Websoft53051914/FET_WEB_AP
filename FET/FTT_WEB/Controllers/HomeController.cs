using FTT_WEB.Common.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Const.Enums;

namespace FTT_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [CustomAuthorizationAttribute(FuncID.Home_View)]
        public IActionResult Index()
        {
            return View();
        }



        [AllowAnonymous]
        public ActionResult PermissionDenied()
        {
            return View();
        }

    }
}
