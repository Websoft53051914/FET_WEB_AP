using FTT_VENDER_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using static Const.Enums;

namespace FTT_VENDER_WEB.Controllers.ChangePw
{
    public class ChangePwController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        //20260127 public IActionResult Change(string tempID)
        public IActionResult Change(string tempid)
        {
            Guid tempGuid;
            //20260127 if (Guid.TryParse(tempID, out tempGuid))
            if (Guid.TryParse(tempid , out tempGuid))
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
