using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FTT_API.Controllers.CIMgt
{
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
    [EnableCors("AllowLocalhost7234")]
    public partial class  CIMgtController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CIMgtController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
    }
}
