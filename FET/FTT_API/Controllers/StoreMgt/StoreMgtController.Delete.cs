using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.StoreMgt
{
    public partial class StoreMgtController : BaseProjectController
    {
        [HttpPost("[action]")]
        public IActionResult Delete(string ivr_code)
        {
            try
            {
                var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);
                _StoreMgtHandler.Delete(ivr_code);
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
