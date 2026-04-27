using Const;
using DocumentFormat.OpenXml.Drawing.Charts;
using FTT_WEB.Common;
using FTT_WEB.Models;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static Const.Enums;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FTT_WEB.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public MenuViewComponent()
        {
        }
        public class ApiResponse
        {
            public bool Success { get; set; }
            public Dictionary<string, int> Data { get; set; }
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
                //20260407 else if (userRoleList.Contains("SUBMITTER") || userRoleList.Contains("EMP") || userRoleList.Contains("STORE"))
                else if (userRoleList.Contains("SUBMITTER") || userRoleList.Contains("EMP") || userRoleList.Contains("STORE") || userRoleList.Contains("VASS")) // 20260407 新增 VASS：修正前 VASS userrole 誤存為 "STORE"，修正後正確存為 "VASS"，需在此補對應
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
                //vm.Exception += $" userRole={userRole} ";
                ////直接在此呼叫筆數API
                //var sendPara = filteredTreeData.SelectMany(s => s.Value).ToList().Where(w => w.DataCount != null).Select(s => (int)s.FuncId).ToList();
                ////var url = $"{Method.GetAppSettingsDataByName("BackendURL")}/Api/GetMenuDataCount";

                //var jwtToken = Request.Cookies["FTT_Token"];

                //using HttpClient client = new HttpClient(new HttpClientHandler { });
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                //var url = $"{Method.GetAppSettingsDataByName("BackendURL")}/Api/GetMenuDataCount";
                //HttpResponseMessage postResponse = await client.PostAsync(url, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(sendPara), Encoding.UTF8, "application/json"));
                //string postResponseData = await postResponse.Content.ReadAsStringAsync();

                //vm.Exception += $" postResponseData={postResponseData} ";

                //var response = JsonConvert.DeserializeObject<ApiResponse>(postResponseData);
                //Dictionary<string, int> strMenuList = new Dictionary<string, int>();
                //if (response.Success == true && response.Data.Count > 0)
                //{
                //    foreach (var item in response.Data)
                //    {
                //        strMenuList.Add(((FuncID)int.Parse(item.Key)).ToString(), item.Value);
                //    }
                //}

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

                            //if (response.Success == true && strMenuList.ContainsKey(menu.FuncId.ToString()))
                            //{
                            //    menu.DataCount = strMenuList[menu.FuncId.ToString()];
                            //}
                        }
                    }
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                return View(new HomeMenuVM() { Exception = ex.ToString() });
            }
        }
    }
}
