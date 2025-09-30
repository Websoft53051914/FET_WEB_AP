using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FTT_API.Controllers.SupplierMgt
{
    public partial class SupplierMgtController : BaseProjectController
    {
        [HttpPost("[action]")]
        public IActionResult Delete(string order_id)
        {
            try
            {
                var _SupplierMgtHandler = new SupplierMgtHandler(_config, HttpContext);
                _SupplierMgtHandler.Delete(order_id);
                
                this.LogSuccess("刪除成功");
                return JsonSuccess("刪除成功");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonSuccess("系統異常");
            }
        }
    }
}
