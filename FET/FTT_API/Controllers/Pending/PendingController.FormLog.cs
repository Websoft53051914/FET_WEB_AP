using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models;
using FTT_API.Models.Handler;
using FTT_API.Models.Partial;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;
using System.Linq;

namespace FTT_API.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_Log(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

                vm.USERROLE = LoginSession.Current.userrole;
                vm.IVRCODE = LoginSession.Current.ivrcode;
                vm.EMPNO = LoginSession.Current.empno;

                var list = _PenddingHanlder.GetPageList_Log(pageEntity, vm);

                for (int i = 0; i < list.Results.Count; i++)
                {
                    var item = list.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;
                }

                return Json(new DataSourceResult
                {
                    Data = list.Results,
                    Total = list.DataCount
                });
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_Desc(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

                vm.USERROLE = LoginSession.Current.userrole;
                vm.IVRCODE = LoginSession.Current.ivrcode;
                vm.EMPNO = LoginSession.Current.empno;

                var list = _PenddingHanlder.GetPageList_Desc(pageEntity, vm);

                for (int i = 0; i < list.Results.Count; i++)
                {
                    var item = list.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;
                }

                return Json(new DataSourceResult
                {
                    Data = list.Results,
                    Total = list.DataCount
                });
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }
    }
}