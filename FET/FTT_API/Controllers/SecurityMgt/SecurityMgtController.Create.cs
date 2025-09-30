using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.SecurityMgt
{
    public partial class SecurityMgtController : BaseProjectController
    {
        [HttpPost("[action]")]
        public IActionResult Create(Store_profileDTO vm)
        {
            try
            { 
                this.LogSuccess("新增完成");
                return JsonSuccess("新增完成");
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統異常");
            }
        }
    }
}
