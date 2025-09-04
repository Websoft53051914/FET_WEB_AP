using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.InProcess
{
    [Route("[controller]")]
    public class InProcessController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public InProcessController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("[action]")]
        public IActionResult GetPageList(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                vm.USERROLE = LoginSession.Current.userrole;
                vm.IVRCODE = LoginSession.Current.ivrcode;
                vm.EMPNO = LoginSession.Current.empno;

                var _InProcessHanlder = new InProcessHanlder(_config, HttpContext);
                var pageList = _InProcessHanlder.FindPageList(pageEntity, vm);

                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    var item = pageList.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;

                    item.IsTicket = item.StatusId == "TICKET";
                }

                return Json(new DataSourceResult
                {
                    Data = pageList.Results,
                    Total = pageList.DataCount
                });
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }


        [HttpPost("[action]")]
        public IActionResult InsterTrackingForm(v_ftt_form2DTO vm)
        {
            try
            {
                var form_No = vm.form_no;
                var _InProcessHanlder = new InProcessHanlder(_config, HttpContext);
                string kpiTime = _InProcessHanlder.GetKPITime(form_No);

                if (kpiTime == "") kpiTime = "3";

                bool overKPI = false;

                overKPI = _InProcessHanlder.CheckDataExist_APPROVE_FORM(form_No, kpiTime);

                if (overKPI == true)
                {
                    string m_FormType = _InProcessHanlder.GetFORM_TYPE(form_No);
                    //string m_FormType = db.GetFieldData("FORM_TYPE", "APPROVE_FORM", "FORM_NO='" + Form_No.Text + "'");

                    if (m_FormType == "")
                        m_FormType = "FTT_FORM";
                    _InProcessHanlder.InsertFTT_FORM_LOG(form_No, LoginSession.Current.empname, m_FormType);
                    //db.ExecuteNonQuery("INSERT INTO FTT_FORM_LOG (FORM_NO,UPDATE_EMPNO,UPDATETIME,FIELDNAME,ACTION,FORM_TYPE,ROOT_NO) VALUES ('" + Form_No.Text + "','" + Session["empname"].ToString() + "',SYSDATE,'催單','FORM','" + m_FormType + "','" + Form_No.Text + "')");
                    return JsonSuccess("已發送催單通知!!");
                }
                else
                {
                    return JsonValidFail("此工單KPI為" + kpiTime + "天，目前尚未Fail，無法催單!!");
                }
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }

    }
}
