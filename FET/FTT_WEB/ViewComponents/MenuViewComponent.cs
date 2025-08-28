using Const;
using FTT_WEB.Common;
using FTT_WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public MenuViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var vm = new HomeMenuVM();

                var filteredTreeData = RoleFunc.GetMenuByFuncIds(RoleFunc.Admin);
                vm.TreeData = filteredTreeData;

                //檢查目前在哪個頁面
                var currentUrl = HttpContext.Request.Path.Value;
                if (currentUrl != null)
                {
                    foreach (var tree in vm.TreeData)
                    {
                        foreach (var menu in tree.Value)
                        {
                            if (currentUrl.Contains(menu.Url, StringComparison.OrdinalIgnoreCase))
                            {
                                menu.IsActive = true;
                            }
                        }
                    }
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                return View(new HomeMenuVM());
            }
        }
    }
}
