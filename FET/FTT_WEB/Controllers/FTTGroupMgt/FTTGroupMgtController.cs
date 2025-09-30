using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_WEB.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FTT_WEB.Controllers.FTTGroupMgt
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
    }
}