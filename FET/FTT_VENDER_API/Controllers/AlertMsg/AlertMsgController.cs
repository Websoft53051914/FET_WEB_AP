using Core.Utility.Web.Base;
using Microsoft.AspNetCore.Mvc;
using FTT_VENDER_API.Models;

namespace FTT_VENDER_API.Controllers.AlertMsg
{
    [Route("[controller]")]
    public partial class AlertMsgController : BaseController
    {
        public AlertMsgController()
        {

        }

        [HttpGet("[action]")]
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
