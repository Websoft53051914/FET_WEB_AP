using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FTT_API.Controllers.SupplierMgt
{
    public partial class SupplierMgtController : BaseProjectController
    {
        [Authorize]
        [HttpPost("[action]")]
        public IActionResult Create(store_vender_profileDTO vm)
        {
            try
            {
                var _SupplierMgtHandler = new SupplierMgtHandler(_config, HttpContext);
              _SupplierMgtHandler.Create(vm);
             
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
