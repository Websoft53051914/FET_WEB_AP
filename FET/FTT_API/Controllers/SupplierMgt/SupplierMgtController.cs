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
                    this.LogSuccess("密碼通知信函已寄出完成");
                    return JsonSuccess("密碼通知信函已寄出完成");
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
