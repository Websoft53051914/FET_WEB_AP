using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.SupplierMgt
{
    [Route("[controller]")]


    public partial class SupplierMgtController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public SupplierMgtController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }


        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList(DataSourceRequest request, store_vender_profileDTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                var _SupplierMgHandler = new SupplierMgtHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _SupplierMgHandler.FindPageList(pageEntity, vm);

                for (int i = 0; i < list.Results.Count; i++)
                {
                    var item = list.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;

                    if (item.locked == "Y")
                        switch (item.locked_reason)
                        {

                            //1: 密碼90天未更換(locked: Y)
                            //2: 90天未登入(locked: Y)
                            //3: 15分鐘排程解鎖(locked: N)
                            //4: 廠商更換密碼((locked: N)
                            //5: 密碼輸入錯誤鎖定(locked: Y)
                            case 1:
                                item.lock_reson_str = "密碼90天未更換"; break;
                            case 2:
                                item.lock_reson_str = "90天未登入"; break;
                            case 3:
                                item.lock_reson_str = "15分鐘排程解鎖"; break;
                            case 4:
                                item.lock_reson_str = "廠商更換密碼"; break;
                            case 5:
                                item.lock_reson_str = "密碼輸入錯誤鎖定"; break;
                            default:
                                break;
                        }
                }

                this.LogSuccess();
                return Json(new DataSourceResult
                {
                    Data = list.Results,
                    Total = list.DataCount
                });
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }


        [ValidateAntiForgeryToken]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetDetail(string order_id)
        {
            try
            {
                var _SupplierMgtHandler = new SupplierMgtHandler(_config, HttpContext);
                var result = _SupplierMgtHandler.GetDetail(order_id);
                this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }


        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult SendPWD(string order_id)
        {
            try
            {
                var _SupplierMgtHandler = new SupplierMgtHandler(_config, HttpContext);
                var msg = _SupplierMgtHandler.SendPWD(order_id);

                if (string.IsNullOrEmpty(msg))
                {
                    this.LogSuccess("變更密碼通知信函已寄出完成");
                    return JsonSuccess("變更密碼通知信函已寄出完成");
                }
                else
                {
                    this.LogSuccess(msg);
                    return JsonValidFail(msg);
                }
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統異常");
            }
        }
    }
}
