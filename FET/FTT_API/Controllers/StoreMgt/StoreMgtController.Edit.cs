using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.StoreMgt
{
    public partial class StoreMgtController : BaseProjectController
    {
        
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult Edit(Store_profileDTO vm)
        {
            try
            {
                var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);
                var msg = _StoreMgtHandler.Edit(vm, _sessionVO.empno);
                if (!string.IsNullOrEmpty(msg))
                {
                    this.LogSuccess();
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
