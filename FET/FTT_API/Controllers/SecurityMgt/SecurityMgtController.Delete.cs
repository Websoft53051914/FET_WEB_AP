using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FTT_API.Controllers.SecurityMgt
{
    public partial class SecurityMgtController : BaseProjectController
    {
        [Authorize]
        [HttpPost("[action]")]
        public IActionResult Delete(string ivrcode)
        {
            try
            {
                SecurityMgtHandler _SecurityMgtHanlder = new SecurityMgtHandler(_config, HttpContext);
                _SecurityMgtHanlder.Delete(ivrcode, _sessionVO.empno);
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
