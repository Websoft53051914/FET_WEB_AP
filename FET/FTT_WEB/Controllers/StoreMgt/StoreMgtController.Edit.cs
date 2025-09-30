using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.StoreMgt
{
    public partial class StoreMgtController : BaseProjectController
    {
        public IActionResult Edit(string ivrcode)
        {
            ViewData["ivrcode"] = ivrcode;
            return View();
        }
        public IActionResult Create()
        {
            return View("Edit");
        }
    }
}
