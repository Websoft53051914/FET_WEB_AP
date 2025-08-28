
using Microsoft.AspNetCore.Mvc;
using FTT_VENDER_WEB.Common;

namespace FTT_VENDER_WEB.ViewComponents
{
    public class BreadCrumbsViewComponent : ViewComponent
    {
        public BreadCrumbsViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(string deatail)
        {
            //string funcId = HttpContext.Request.Query["FuncId"];
            //MenuBL menuBL = BLFactory.GetInstance<MenuBL>();
            //if (!string.IsNullOrEmpty(funcId))
            //{
            //    SysFuncDM dm = menuBL.GetInfoByFuncId(funcId);
            //    if (dm != null)
            //    {
            //        ViewData["FuncName"] = dm.Name;
            //        ViewData["FuncClassName"] = dm.ClassName;
            //    }
            //}

            //ViewData["deatail"] = deatail;
            //ViewData["funcId"] = funcId; 

            return View();
        }
    }
}
