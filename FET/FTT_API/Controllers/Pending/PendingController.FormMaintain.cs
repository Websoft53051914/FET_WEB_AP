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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;
using System.Linq;

namespace FTT_API.Controllers.Pending
{
    
    public partial class PendingController : BaseProjectController
    {
        
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult UpdateEmpnoDeptCode(FormMaintainVM vm)
        {
            try
            {
                if (vm != null)
                {
                    if (vm.vms != null && vm.vms.Count > 0)
                    {
                        PendingHandler _PenddingHanlder = new PendingHandler(_config, HttpContext);
                        foreach (var item in vm.vms)
                        {
                            _PenddingHanlder.UpdateAccessRole(item.form_no, item.user_type, item.empno, item.deptcode, _sessionVO.empno);
                            if (item.user_type == "VENDOR")
                            {
                                _PenddingHanlder.UpdateFttForm_VENDOR(item.form_no, item.deptcode);
                            }
                        }

                        if (!string.IsNullOrEmpty(vm.StatusId))
                        {
                            _PenddingHanlder.UpdateApproveForm(vm.form_no, vm.StatusId, _sessionVO.empno);
                        }

                        _PenddingHanlder.GetDBHelper().Commit();
                    }
                }

                this.LogSuccess();
                return JsonSuccess("更新Access_Role 成功");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("更新Access_Role 失敗 !!,錯誤訊息為" + ex.Message);
            }
        }

        
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_V_ACCESS_ROLE(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHandler _PenddingHanlder = new PendingHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _PenddingHanlder.GetPageList_V_ACCESS_ROLE(pageEntity, vm);

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
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_Access(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHandler _PenddingHanlder = new PendingHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _PenddingHanlder.GetPageList_Access(pageEntity, vm);

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
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_Vender(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHandler _PenddingHanlder = new PendingHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _PenddingHanlder.GetPageList_Vender(pageEntity, vm);

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