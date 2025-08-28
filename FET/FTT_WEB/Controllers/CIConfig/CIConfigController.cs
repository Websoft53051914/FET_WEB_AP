using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.CIConfig
{
    public partial class CIConfigController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
