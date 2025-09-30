using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.SecurityMgt
{
    public partial class SecurityMgtController : BaseProjectController
    {
        public IActionResult Edit(string order_id)
        {
            ViewData["order_id"] = order_id;
            return View();
        }
        public IActionResult Create()
        {
            return View("Edit");
        }
    }
}
