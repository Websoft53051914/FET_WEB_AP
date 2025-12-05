using FTT_WEB.Models.ViewModel.MailServerSetting;
using Microsoft.AspNetCore.Mvc;
using FTT_WEB.Models;
using static Const.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using FTT_WEB.Models.Handler;
using FTT_WEB.Common.ConfigurationHelper;


namespace FTT_WEB.Controllers.MailServerSetting
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
            return View(new MailServerSettingVM());
        }
    }
}
