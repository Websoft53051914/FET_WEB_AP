using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace FTT_API.Controllers.InProcess
{
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
    [EnableCors("AllowLocalhost7234")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InProcessController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public InProcessController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize]
        [HttpPost("[action]")]
        public IActionResult GetPageList(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var _InProcessHanlder = new InProcessHandler(_config, HttpContext);
                var pageList = _InProcessHanlder.FindPageList(pageEntity, vm);

                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    var item = pageList.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;

                    item.IsTicket = item.StatusId == "TICKET";
                }

                this.LogSuccess();
                return Json(new DataSourceResult
                {
                    Data = pageList.Results,
                    Total = pageList.DataCount
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
        public IActionResult InsterTrackingForm(v_ftt_form2DTO vm)
        {
            try
            {
                var form_No = vm.form_no;
                var _InProcessHanlder = new InProcessHandler(_config, HttpContext);
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

                    BaseDBHandler baseHandler = new BaseDBHandler();
                    Dictionary<string, object> dic = new();
                    dic.Add("form_no", int.Parse(form_No));
                    var newEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                    MailPoolHandler _MailPoolHandler = new MailPoolHandler();
                    var result = Method.CreateMailPool(form_No, "", "REMINDER", _MailPoolHandler);
                    if (!string.IsNullOrEmpty(result))
                    {
                        this.LogError("CreateMailPool 執行失敗");
                    }

                    //_InProcessHanlder.InsertFTT_FORM_LOG(form_No, _sessionVO.empname, m_FormType);
                    //db.ExecuteNonQuery("INSERT INTO FTT_FORM_LOG (FORM_NO,UPDATE_EMPNO,UPDATETIME,FIELDNAME,ACTION,FORM_TYPE,ROOT_NO) VALUES ('" + Form_No.Text + "','" + Session["empname"].ToString() + "',SYSDATE,'催單','FORM','" + m_FormType + "','" + Form_No.Text + "')");

                    this.LogSuccess("已發送催單通知!!");
                    return JsonSuccess("已發送催單通知!!");
                }
                else
                {
                    this.LogSuccess($"此工單【{form_No}】KPI為" + kpiTime + "天，目前尚未Fail，無法催單!!");
                    return JsonValidFail($"此工單【{form_No}】KPI為" + kpiTime + "天，目前尚未Fail，無法催單!!");
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

    }
}
