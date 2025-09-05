using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.InProcess
{
    public class TaskTestController : Controller
    {
        public IActionResult Index()
        {
            string logPath = $@".\logs\Dispatch_TT_{DateTime.Now:yyyyMMdd}.log";
            var db = new FETTaskHelper(logPath);
            //db.Open();
            db.Send_TT_No_RootCause("");

            return View();
        }
    }
}
