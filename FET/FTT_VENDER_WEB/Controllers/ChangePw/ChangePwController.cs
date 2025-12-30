using FTT_VENDER_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using static Const.Enums;

namespace FTT_VENDER_WEB.Controllers.ChangePw
{
    public class ChangePwController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Change(string tempID)
        {
            Guid tempGuid;
            if (Guid.TryParse(tempID, out tempGuid))
            {
                ViewData["tempGuid"] = tempGuid;
            }
            else
            {
                return View("Unchange");
            }

            return View();
        }
    }
}
