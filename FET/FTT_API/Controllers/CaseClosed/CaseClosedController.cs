using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.CaseClosed
{
    [Route("[controller]")]
    public class CaseClosedController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CaseClosedController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        
        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPageList(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            // === 確認這個才是真正的查詢方法 ===
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var testMessage = $"=== CASECLOSED CONTROLLER CALLED at {timestamp} ===\n";
                testMessage += $"User: {_sessionVO?.username}, Role: {_sessionVO?.userrole}, EmpNo: {_sessionVO?.empno}\n";
                testMessage += $"Request StatusId: {vm?.StatusId}\n";
                
                // 強制寫入多個位置
                System.IO.File.AppendAllText(@"D:\BACK_OFFICE\FTT\3101N2\github\FET_WEB_AP\FET\FTT_API\CASECLOSED_FOUND.txt", testMessage);
                System.IO.File.AppendAllText(@"D:\BACK_OFFICE\FTT\3101N2\github\FET_WEB_AP\FET\FTT_API\wwwroot\caseclosed_debug.txt", testMessage);
                System.IO.File.AppendAllText(@"d:\caseclosed_debug.log", testMessage);
                
            } catch { }
            
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                CaseClosedHandler _PenddingHanlder = new CaseClosedHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;
                _PenddingHanlder.SessionVO = _sessionVO;

                var list = _PenddingHanlder.FindPageList(pageEntity, vm);

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
    }
}
