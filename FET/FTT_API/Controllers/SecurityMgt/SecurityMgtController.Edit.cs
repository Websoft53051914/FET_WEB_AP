using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.SecurityMgt
{
    public partial class SecurityMgtController : BaseProjectController
    {
        
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult Edit(Store_profileDTO vm)
        {
            try
            { 
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
