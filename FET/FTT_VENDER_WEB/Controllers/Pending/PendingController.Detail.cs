using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_WEB.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        public IActionResult Detail(string formNo)
        {
            ViewData["form_no"] = formNo; ;

            return View();
        }
    }
}
