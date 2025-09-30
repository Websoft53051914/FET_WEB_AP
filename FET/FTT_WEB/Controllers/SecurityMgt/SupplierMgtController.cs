using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.SecurityMgt
{
    public partial class SecurityMgtController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
