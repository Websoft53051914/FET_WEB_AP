using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.CIConfig
{
    public partial class CIConfigController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
