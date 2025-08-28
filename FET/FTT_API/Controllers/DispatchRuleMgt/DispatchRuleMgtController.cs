using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.DispatchRuleMgt
{
    public partial class DispatchRuleMgtController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
