using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.InProcess
{
    public class TaskTestController : Controller
    {
        public IActionResult Index()
        {
            string logPath = $@".\logs\Dispatch_TT_{DateTime.Now:yyyyMMdd}.log";
            var _FETTaskHelper = new FETTaskHelper(logPath);
            _FETTaskHelper.Send_TT_No_RootCause("");
            _FETTaskHelper.Close();
            return View();
        }
    }
}
