using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.Handler;
using FTT_VENDER_API.Models.Partial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_VENDER_API.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        
        [HttpPost("[action]")]
        public IActionResult UpdateEmpnoDeptCode(FormMaintainVM vm)
        {
            try
            {
                if (vm != null)
                {
                    if (vm.vms != null && vm.vms.Count > 0)
                    {
                        PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);
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

                this.LogError("更新Access_Role 成功");
                return JsonSuccess("更新Access_Role 成功");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("更新Access_Role 失敗 !!,錯誤訊息為" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_V_ACCESS_ROLE(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_Access(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList_Vender(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

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