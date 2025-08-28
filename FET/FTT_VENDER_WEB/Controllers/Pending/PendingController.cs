using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_WEB.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
