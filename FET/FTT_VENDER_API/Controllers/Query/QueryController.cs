using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.Query
{
    public class QueryController : BaseProjectController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
