using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.SupplierMgt
{
    public partial class SupplierMgtController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
