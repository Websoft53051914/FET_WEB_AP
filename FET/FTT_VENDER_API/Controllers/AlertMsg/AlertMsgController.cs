using Core.Utility.Web.Base;
using FTT_VENDER_API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.AlertMsg
{
    [Route("[controller]")]
    [IgnoreAntiforgeryToken]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
