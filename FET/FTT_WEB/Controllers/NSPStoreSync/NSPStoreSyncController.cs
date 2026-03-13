using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.NSPStoreSync
{
    public partial class NSPStoreSyncController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
