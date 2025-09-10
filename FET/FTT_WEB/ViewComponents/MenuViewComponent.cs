using Const;
using FTT_WEB.Common;
using FTT_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models.Security;

namespace FTT_WEB.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public MenuViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var roleFunc = RoleFunc.AdminMax;
            try
            {
                if (LoginSession.Current != null && !string.IsNullOrEmpty(LoginSession.Current.userrole))
                    switch (LoginSession.Current.userrole.ToUpper())
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
                            break;
                    }

                var vm = new HomeMenuVM();

                var filteredTreeData = RoleFunc.GetMenuByFuncIds(RoleFunc.AdminMax);
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
