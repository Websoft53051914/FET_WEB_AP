using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.SupplierMgt
{
    public partial class SupplierMgtController : BaseProjectController
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
