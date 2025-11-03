using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System.ServiceModel;
using System.Text;
using static Const.Enums;

namespace FTT_VENDER_API.Models.Handler
{
    public class MailPoolHandler : BaseDBHandler
    {
        public class access_roleEntity
        {
            public string form_type { get; set; }
            public string user_type { get; set; }
            public string form_no { get; set; }
            public string role { get; set; }
            public string empno { get; set; }
            public string deptcode { get; set; }
            public string action { get; set; }
            public string ifnullskip { get; set; }
            public string updatetime { get; set; }
            public string update_empno { get; set; }
            public string approve_status { get; set; }
            public string priority { get; set; }
            public string root_no { get; set; }
            public string find_self { get; set; }
            public string approve { get; set; }
            public string user_group { get; set; }
            public string find_approve_next { get; set; }
            public string maxlevel { get; set; }
        }

        public class access_roleDTO : access_roleEntity
        {
            public int No { get; set; }
            public string STATUS_NAME { get; set; }
            public string SQL { get; set; }
        }

        public class tb_mailpool_ruleEntity
        {
            /// <summary>
            /// 自動流水號 (Primary Key)
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// 描述
            /// </summary>
            public string description { get; set; }

            /// <summary>
            /// 郵件類型
            /// </summary>
            public string mail_type { get; set; }

            /// <summary>
            /// 收件人
            /// </summary>
            public string mail_reciver { get; set; }

            /// <summary>
            /// 副本收件人
            /// </summary>
            public string mail_reciver_cc { get; set; }

            /// <summary>
            /// 郵件主旨
            /// </summary>
            public string mailsubject { get; set; }

            /// <summary>
            /// 郵件開頭
            /// </summary>
            public string mailhead { get; set; }

            /// <summary>
            /// 郵件內容模板
            /// </summary>
            public string mailcontent { get; set; }

            /// <summary>
            /// 狀態
            /// </summary>
            public int? status { get; set; }

            /// <summary>
            /// 建立者 ID
            /// </summary>
            public int? creator { get; set; }

            /// <summary>
            /// 建立時間
            /// </summary>
            public DateTime? createtime { get; set; }

            /// <summary>
            /// 更新者 ID
            /// </summary>
            public int? updater { get; set; }

            /// <summary>
            /// 更新時間
            /// </summary>
            public DateTime? updatetime { get; set; }

        }

        public class tb_mailpool_ruleDTO : tb_mailpool_ruleEntity
        {

        }


        public class fet_user_profileEntity
        {
            public string aliasname { get; set; }
            public string empno { get; set; }

            public string deptcode { get; set; }

            public string empname { get; set; }

            public string engname { get; set; }

            public string sex { get; set; }
            public string email { get; set; }

            public string mobile { get; set; }

            public string ext { get; set; }

            public string titlename { get; set; }

            public string region { get; set; }

            public string regionname { get; set; }

            public string costcenter { get; set; }

            public string locationcode { get; set; }

            public string locationname { get; set; }

            public DateTime? entdate { get; set; }

            public DateTime? offdate { get; set; }

            public DateTime? finaldate { get; set; }

            public string emptype { get; set; }

            public string opid { get; set; }

            public string loginid { get; set; }

            public string repflg { get; set; }

            public string rocid { get; set; }

            public string agent { get; set; }

            public string titleengname { get; set; }

            public string sr_agent { get; set; }

            public string compname { get; set; }

            public string rigion { get; set; }

            public string rigionname { get; set; }

            public string deptengname { get; set; }

            public string deptchiname { get; set; }

            public string parent { get; set; }

            public string sdeptname { get; set; }

            public string costcenterflg { get; set; }

            public int? compcode { get; set; }

            public string setdate { get; set; }

            public int? deptlevel { get; set; }

            public string deptlevelname { get; set; }

            public string depttype { get; set; }

            public string depttypename { get; set; }
            public string mgr_empno { get; set; }
        }

        public class fet_user_profileDTO : fet_user_profileEntity
        {
            public int No { get; set; }
            public string deptname { get; set; }
        }

        public class MailPoolEntity
        {
            public int Id { get; set; }
            public string? Subject { get; set; }
            public string? Content { get; set; }
            public DateTime EstimateSendTime { get; set; }
            public DateTime? RealSendTime { get; set; }

            public int? SendStatus { get; set; }
            public string? ErrorMsg { get; set; }
            public int? Status { get; set; }

            public string? DestinationEmail { get; set; }
            public string? DestinationEmail_CC { get; set; }
            public int? Creator { get; set; }
            public DateTime? CreateTime { get; set; }

            public int? Updater { get; set; }
            public DateTime? UpdateTime { get; set; }

        }

        internal access_roleDTO FindAccessRole(string form_no, string user_type)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);
            paras.Add("user_type", user_type);

            string strWhere = "";

            string originSQL = $@"

SELECT *
	FROM access_role
	where form_no=@form_no
        and user_type=@user_type

";

            var result = dbHelper.Find<access_roleDTO>(originSQL, paras);
            return result;
        }

        internal List<tb_mailpool_ruleDTO> FindMailPoolRuleList(string mail_type)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("mail_type", mail_type);

            string strWhere = "";

            string originSQL = $@"

SELECT *
	FROM tb_mailpool_rule
	where mail_type=@mail_type

";

            var result = dbHelper.FindList<tb_mailpool_ruleDTO>(originSQL, paras);
            return result;
        }

        internal List<fet_user_profileDTO> GetEmailListByRole(string ftt_group)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("ftt_group", ftt_group);

            string strWhere = "";

            string originSQL = $@"

SELECT b.*
	FROM ftt_group a
	join  fet_user_profile b on a.empno=b.empno
	where ftt_group=@ftt_group

";

            var result = dbHelper.FindList<fet_user_profileDTO>(originSQL, paras);
            return result;
        }

        internal store_profileEntity GetStoreProfile(string ivr_code)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("ivr_code", ivr_code);

            string strWhere = "";

            string originSQL = $@"

SELECT *
	FROM store_profile
	where ivr_code=@ivr_code

";

            var result = dbHelper.Find<store_profileEntity>(originSQL, paras);
            return result;
        }

        internal store_vender_profileDTO GetStoreVenderProfile(string order_id)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("order_id", order_id);

            string strWhere = "";

            string originSQL = $@"

SELECT *
	FROM store_vender_profile
	where order_id=@order_id

";

            var result = dbHelper.Find<store_vender_profileDTO>(originSQL, paras);
            return result;
        }

        internal fet_user_profileDTO GetFetUserProfile(string empno)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("empno", empno);

            string strWhere = "";

            string originSQL = $@"

SELECT *
	FROM fet_user_profile
	where empno=@empno

";

            var result = dbHelper.Find<fet_user_profileDTO>(originSQL, paras);
            return result;
        }

        //private readonly ConfigurationHelper _configHelper;
        //private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        //public Control_LogHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        //{
        //    _configHelper = confighelper;
        //    _httpContext = httpContext;
        //}
        internal void Insert(MailPoolEntity entity)
        {
            Dictionary<string, object> paras = new()
            {
                { "Subject", entity.Subject},
                { "Content", entity.Content},
                { "EstimateSendTime", entity.EstimateSendTime},
                { "RealSendTime", entity.RealSendTime},
                { "SendStatus", entity.SendStatus},
                { "ErrorMsg", entity.ErrorMsg},
                { "Status", entity.Status},
                { "Creator", entity.Creator},
                { "CreateTime", entity.CreateTime},
                { "Updater", entity.Updater},
                { "UpdateTime", entity.UpdateTime},
                { "DestinationEmail", entity.DestinationEmail},
            };

            string strSql = @"
insert into tb_mailpool
( subject, content, estimatesendtime, realsendtime, sendstatus, errormsg, status, creator, createtime, updater, updatetime, destinationemail)
values
(@Subject, @Content, @EstimateSendTime, @RealSendTime, @SendStatus, @ErrorMsg, @Status, @Creator, @CreateTime, @Updater, @UpdateTime, @DestinationEmail)";

            try
            {
                base.dbHelper.Execute(strSql, paras);
                base.dbHelper.Commit();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal void InsertAction(MailPoolEntity entity)
        {
            Dictionary<string, object> paras = new()
            {
                { "Subject", entity.Subject},
                { "Content", entity.Content},
                { "EstimateSendTime", entity.EstimateSendTime},
                { "RealSendTime", entity.RealSendTime},
                { "SendStatus", entity.SendStatus},
                { "ErrorMsg", entity.ErrorMsg},
                { "Status", entity.Status},
                { "Creator", entity.Creator},
                { "CreateTime", entity.CreateTime},
                { "Updater", entity.Updater},
                { "UpdateTime", entity.UpdateTime},
                { "DestinationEmail", entity.DestinationEmail},
            };

            string strSql = @"
insert into tb_mailpool
( subject, content, estimatesendtime, realsendtime, sendstatus, errormsg, status, creator, createtime, updater, updatetime, destinationemail)
values
(@Subject, @Content, @EstimateSendTime, @RealSendTime, @SendStatus, @ErrorMsg, @Status, @Creator, @CreateTime, @Updater, @UpdateTime, @DestinationEmail)
";

            try
            {
                base.dbHelper.Execute(strSql, paras);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal ftt_formEntity GetFttForm(string form_no)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string strWhere = "";

            string originSQL = $@"

SELECT *
	FROM ftt_form
	where form_no=@form_no

";

            var result = dbHelper.Find<ftt_formEntity>(originSQL, paras);
            return result;
        }
    }
}
