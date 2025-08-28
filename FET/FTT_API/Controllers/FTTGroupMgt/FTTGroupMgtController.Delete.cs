using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.FTTGroupMgt
{
    public partial class FTTGroupMgtController : BaseProjectController
    {
        [HttpPost]
        public IActionResult Delete(string empno)
        {
            try
            {
                ftt_groupSQL _ftt_groupSQL = new ftt_groupSQL();
                _ftt_groupSQL.Delete(empno);

                return JsonSuccess("刪除完成");
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統異常");
            }
        }

    }
}