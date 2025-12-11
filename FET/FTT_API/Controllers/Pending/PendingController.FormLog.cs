using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_Log(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHandler _PenddingHanlder = new PendingHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _PenddingHanlder.GetPageList_Log(pageEntity, vm);

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

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_Desc(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHandler _PenddingHanlder = new PendingHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _PenddingHanlder.GetPageList_Desc(pageEntity, vm);

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