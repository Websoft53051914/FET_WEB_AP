using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers
{
    public class LogoutController : BaseProjectController
    {
        public IActionResult Index()
        {
            HttpContext.SignOutAsync();

            //清除所有的 session
            Common.HttpContext.Current.Session.Clear();
            Response.Cookies.Delete("__MySession__");

            return RedirectToAction("Index", "Login");
        }


    }
}
