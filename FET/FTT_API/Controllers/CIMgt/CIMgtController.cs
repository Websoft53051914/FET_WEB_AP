using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.CIMgt
{
    public class CIMgtController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
