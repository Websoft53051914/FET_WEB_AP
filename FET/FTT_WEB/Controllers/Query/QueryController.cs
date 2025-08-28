using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.Query
{
    public class QueryController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
