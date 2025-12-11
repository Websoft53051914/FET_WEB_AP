using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.StoreMgt
{
    public partial class StoreMgtController : BaseProjectController
    {
        [Authorize]
        [HttpPost("[action]")]
        public IActionResult Create(Store_profileDTO vm)
        {
            try
            {
                var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);
                var msg = _StoreMgtHandler.Create(vm, _sessionVO.empno);
                if (!string.IsNullOrEmpty(msg))
                {
                    this.LogSuccess();
                    return JsonValidFail(msg);
                }

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
