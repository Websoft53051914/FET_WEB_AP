using Core.Utility.Web.Base;
using Microsoft.AspNetCore.Mvc;
using FTT_VENDER_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace FTT_VENDER_API.Controllers.AlertMsg
{
    [Route("[controller]")]
    public partial class AlertMsgController : BaseController
    {
        public AlertMsgController()
        {

        }

        [Authorize]
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
