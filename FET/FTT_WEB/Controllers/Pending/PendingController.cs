using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_WEB.Common;
using FTT_WEB.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;

namespace FTT_WEB.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PendingController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            maintain_configSQL _maintain_configSQL = new maintain_configSQL();
            maintain_configDTO dto = _maintain_configSQL.FindByConfigName("HANDLER");
            if (dto != null)
            {
                ViewData["HandlerDesc"] = dto.config_value;
            }
            return View();
        }
    }
}
