using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        public IActionResult Edit(int Id)
        {
            return View("~/Views/FormEdit/Index.cshtml");
        }
    }
}
