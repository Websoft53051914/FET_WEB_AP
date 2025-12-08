using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FTT_API.Controllers.FTTGroupMgt
{
    [Route("[controller]")]
    //[Common.Attribute.CustomAuthorization]
    [EnableCors("AllowLocalhost7234")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class FTTGroupMgtController : BaseProjectController
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList(DataSourceRequest request, ftt_groupDTO vm)
       {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                ftt_groupSQL _ftt_groupSQL = new ftt_groupSQL();
                var list = _ftt_groupSQL.FindPageList(pageEntity, vm);

                for (int i = 0; i < list.Results.Count; i++)
                {
                    var item = list.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;
                }

                this.LogSuccess();
                return Json(new DataSourceResult
                {
                    Data = list.Results,
                    Total = list.DataCount
                });
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }
    }
}