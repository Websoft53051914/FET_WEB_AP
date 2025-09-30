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
                List<string> userRoleList = userRole?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToUpper()).ToList() ?? [];
                if (userRoleList.Contains("ADMIN"))
                {
                    roleFunc = RoleFunc.ADMIN;
                }
                else if (userRoleList.Contains("ASSETER"))
                {
                    roleFunc = RoleFunc.ASSETER;
                }
                else if (userRoleList.Contains("SECURITY"))
                {
                    roleFunc = RoleFunc.SECURITY;
                }
                else if (userRoleList.Contains("ASSISTANT"))
                {
                    roleFunc = RoleFunc.ASSISTANT;
                }
                else if (userRoleList.Contains("MANAGER"))
                {
                    roleFunc = RoleFunc.MANAGER;
                }
                else if (userRoleList.Contains("SUBMITTER") || userRoleList.Contains("EMP") || userRoleList.Contains("STORE"))
                {
                    roleFunc = RoleFunc.EMPLOYEE;
                }
                else
                {
                    roleFunc = RoleFunc.OTHER;
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
