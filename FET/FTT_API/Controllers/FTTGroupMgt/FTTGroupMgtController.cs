using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FTT_API.Controllers.FTTGroupMgt
{
    public partial class FTTGroupMgtController : BaseProjectController
    {
        public IActionResult Index()
        {
            ftt_groupSQL _ftt_groupSQL = new ftt_groupSQL();
            var dtos = _ftt_groupSQL.GetGroupList();
            ViewData["FTT_GroupList"] = dtos.Select(s => new SelectListItem() { Value = s.FTT_Group, Text = s.FTT_Group }).ToList();
            return View();
        }

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

                return Json(new DataSourceResult
                {
                    Data = list.Results,
                    Total = list.DataCount
                });
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }
    }
}