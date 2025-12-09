using Const;
using FTT_VENDER_WEB.Common;
using FTT_VENDER_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static Const.Enums;

namespace FTT_VENDER_WEB.ViewComponents
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


        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var roleFunc = RoleFunc.VENDOR;
                var vm = new HomeMenuVM();

                var filteredTreeData = RoleFunc.GetMenuByFuncIds(roleFunc);
                vm.TreeData = filteredTreeData;

                ////直接在此呼叫筆數API
                //var sendPara = filteredTreeData.SelectMany(s => s.Value).ToList().Where(w => w.DataCount != null).Select(s => (int)s.FuncId).ToList();
                //var url = $"{Method.GetAppSettingsDataByName("BackendURL")}/Api/GetMenuDataCount";

                //var jwtToken = Request.Cookies["Token"];

                //using HttpClient client = new HttpClient(new HttpClientHandler { });
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                ////client.DefaultRequestHeaders.Add("Cookie", $"Token={jwtToken}");

                //HttpResponseMessage postResponse = await client.PostAsync(
                //    $"{Method.GetAppSettingsDataByName("BackendURL")}/Api/GetMenuDataCount",
                //    new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(sendPara), Encoding.UTF8, "application/json"));
                //string postResponseData = await postResponse.Content.ReadAsStringAsync();

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
                return View(new HomeMenuVM());
            }
        }
    }
}
