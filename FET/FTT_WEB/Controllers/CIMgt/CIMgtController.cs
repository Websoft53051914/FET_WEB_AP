using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.CIMgt
{
    public class CIMgtController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
