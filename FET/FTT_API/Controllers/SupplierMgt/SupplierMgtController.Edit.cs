using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.SupplierMgt
{
    public partial class SupplierMgtController : BaseProjectController
    {
        
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult Edit(store_vender_profileDTO vm)
        {
            try
            {
                var _SupplierMgtHandler = new SupplierMgtHandler(_config, HttpContext);
                var msg = _SupplierMgtHandler.Edit(vm);
                if (!string.IsNullOrEmpty(msg))
                {
                    return JsonValidFail(msg);
                }

                this.LogSuccess("編輯成功");
                return JsonSuccess("編輯成功");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonSuccess("系統異常");
            }
        }
    }
}
