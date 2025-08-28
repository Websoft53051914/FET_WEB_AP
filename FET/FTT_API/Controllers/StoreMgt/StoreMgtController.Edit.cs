using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.StoreMgt
{
    public partial class StoreMgtController : BaseProjectController
    {
        public IActionResult Edit()
        {
            return View();
        }
    }
}
