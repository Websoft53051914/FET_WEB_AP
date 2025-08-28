using Core.Utility.Web.Base;
using Microsoft.AspNetCore.Mvc;
using FTT_WEB.Models;

namespace FTT_WEB.Controllers.AlertMsg
{
    public partial class AlertMsgController : BaseController
    {
        public AlertMsgController()
        {

        }

        public IActionResult Redirection(AlertMsgRedirection vm)
        {
            ViewData["IsShowLayout"] = "false";

            if (!string.IsNullOrEmpty(vm.ParasJson))
            {
                vm.Paras = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(vm.ParasJson);
            }

            return View(vm);
        }
    }

}
