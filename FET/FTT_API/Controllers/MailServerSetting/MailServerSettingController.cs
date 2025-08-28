using FTT_API.Models.ViewModel.MailServerSetting;
using Microsoft.AspNetCore.Mvc;
using FTT_API.Models;
using static Const.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using FTT_API.Models.Handler;
using FTT_API.Common.ConfigurationHelper;


namespace FTT_API.Controllers.MailServerSetting
{
    public class MailServerSettingController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        public MailServerSettingController(IWebHostEnvironment hostingEnvironment, ConfigurationHelper configHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _configHelper = configHelper;
        }

        public IActionResult Index()
        {
            var MailServerHandler = new MailServerHandler(_configHelper, HttpContext);

            MailServerSettingVM vm = MailServerHandler.GetEdit();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(MailServerSettingVM vm)
        {
            var MailServerHandler = new MailServerHandler(_configHelper, HttpContext);
            try
            {
                MailServerHandler.Update(vm);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            //return BadRequest();
            return JsonSuccess("資料儲存成功");
        }

       
    }
}
