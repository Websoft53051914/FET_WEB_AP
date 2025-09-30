using Const;
using DocumentFormat.OpenXml.Drawing.Charts;
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

        public async Task<IViewComponentResult> InvokeAsync(string userRole)
        {
            var roleFunc = RoleFunc.AdminMax;
            try
            {
                if (false)
                    switch (userRole.ToUpper())
                    {
                        case "ADMIN":
                            roleFunc = RoleFunc.ADMIN;
                            break;
                        case "ASSETER":
                            roleFunc = RoleFunc.ASSETER;
                            break;
                        case "SECURITY":
                            roleFunc = RoleFunc.SECURITY;
                            break;
                        case "ASSISTANT":
                            roleFunc = RoleFunc.ASSISTANT;
                            break;
                        case "MANAGER":
                            roleFunc = RoleFunc.MANAGER;
                            break;
                        case "VENDOR":
                            roleFunc = RoleFunc.VENDOR;
                            break;
                        case "EMPLOYEE":
                            roleFunc = RoleFunc.EMPLOYEE;
                            break;
                        default:
                            roleFunc = RoleFunc.OTHER;
                            break;
                    }

                var vm = new HomeMenuVM();

                var filteredTreeData = RoleFunc.GetMenuByFuncIds(roleFunc);
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
