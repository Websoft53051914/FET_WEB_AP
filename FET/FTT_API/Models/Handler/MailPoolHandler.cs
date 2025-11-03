using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Controllers;
using FTT_API.Models.ViewModel;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System.ServiceModel;
using System.Text;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class MailPoolHandler : BaseDBHandler
    {
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
                { "DestinationEmail_CC", entity.DestinationEmail_CC},
            };

            string strSql = @"
insert into tb_mailpool
( subject, content, estimatesendtime, realsendtime, sendstatus, errormsg, status, creator, createtime, updater, updatetime, destinationemail,destinationemail_CC)
values
(@Subject, @Content, @EstimateSendTime, @RealSendTime, @SendStatus, @ErrorMsg, @Status, @Creator, @CreateTime, @Updater, @UpdateTime, @DestinationEmail,@DestinationEmail_CC)";

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
                { "DestinationEmail_CC", entity.DestinationEmail_CC},
            };

            string strSql = @"
insert into tb_mailpool
( subject, content, estimatesendtime, realsendtime, sendstatus, errormsg, status, creator, createtime, updater, updatetime, destinationemail,destinationemail_CC)
values
(@Subject, @Content, @EstimateSendTime, @RealSendTime, @SendStatus, @ErrorMsg, @Status, @Creator, @CreateTime, @Updater, @UpdateTime, @DestinationEmail,@DestinationEmail_CC)";

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
