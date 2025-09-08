using FTT_VENDER_API.Common.ConfigurationHelper;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.Pending
{
    [Route("[controller]")]
    public partial class PendingController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PendingController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
    }
}
