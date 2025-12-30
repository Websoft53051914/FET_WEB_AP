using Core.Utility.Extensions;
using Core.Utility.Helper.Mail;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;
using System.Transactions;
using static Const.Enums;


namespace FTT_VENDER_API.Models.Handler
{
    public partial class CheckVenderPWLoginTimeHandler : BaseDBHandler
    {

        private readonly ConfigurationHelper _configHelper;

        public CheckVenderPWLoginTimeHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }

        /// <summary>
        /// 紀錄失敗訊息於資料庫
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>TB_Control_Log.Id</returns>
        protected void LogError(string exception, string ActionName)
        {
            var entity = new TB_Control_LogEntity()
            {
                IP = "",
                Status = ((int)LogStatusEnum.Failed).ToString(),
                ControllerName = "CheckVenderPWLoginTimeHandler",
                ActionName = ActionName,
                Exception = exception,
            };

            InsertLog(entity);
        }

        protected void InsertLog(TB_Control_LogEntity entity)
        {
            TB_Control_LogHandler _BaseDBHandler = new TB_Control_LogHandler();
            _BaseDBHandler.Insert(entity);
        }

        public void CheckPWChangeTime()
        {

            List<store_vender_profileDTO> Store_Vendor_ProfileChangePwBefore80To90Days = new();
            List<store_vender_profileDTO> Store_Vendor_ProfileChangePwBeforeOver90Days = new();
            TB_Control_LogHandler LogHandler = new();
            try
            {
                // testpwdtime 要改為 pw_chgtime

                Store_Vendor_ProfileChangePwBefore80To90Days = GetNeedRemindChangePwList();

                Store_Vendor_ProfileChangePwBeforeOver90Days = GetChangePwBeforeOver90Days();

                //將 80-90天未改密碼的寫入寄信提醒
                InsertPWChangeRemindAndRemindStatus(Store_Vendor_ProfileChangePwBefore80To90Days);

                foreach (var each in Store_Vendor_ProfileChangePwBeforeOver90Days)
                {
                    LockVenderProfile_ByOrderId(each, LockReason: (int)LockReasonEnum.LockedByNochangePWOver90Days);
                    //InsertVenderPWHistory(each);
                    GetDBHelper().Commit();
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "CheckPWChangeTime");
            }

        }

        public List<store_vender_profileDTO> GetNeedRemindChangePwList()
        {

            string queryForPWChangeRemind = $@"Select * from store_vender_profile where pw_chgtime < @EndTime
            AND pw_chgtime > @StartTime
            and locked = @Locked and (is_pwchange_remind <> 'Y' OR is_pwchange_remind is null)";

            Dictionary<string, object> Paras = new();


            Paras = new Dictionary<string, object>
                        {
                            { "StartTime", DateTime.Now.AddDays(-90)},
                            { "Locked", "N" },
                            { "EndTime", DateTime.Now.AddDays(-80)},
                            { "IsPwChangeRemind", "Y" },

                        };

            List<store_vender_profileDTO> ResultList = GetDBHelper().FindList<store_vender_profileDTO>(queryForPWChangeRemind, Paras);

            return ResultList;
        }

        public List<store_vender_profileDTO> GetChangePwBeforeOver90Days()
        {

            string queryForLock = $@"Select * from store_vender_profile where pw_chgtime < @Time and locked = @Locked ";

            Dictionary<string, object> Paras = new();

            Paras = new Dictionary<string, object>
                        {
                            { "Locked", "N" },
                            { "Time", DateTime.Now.AddDays(-90)},
                        };

            List<store_vender_profileDTO> ResultList = GetDBHelper().FindList<store_vender_profileDTO>(queryForLock, Paras);

            return ResultList;
        }

        public void InsertPWChangeRemindAndRemindStatus(List<store_vender_profileDTO> Store_Vendor_ProfileUnRemindForChangePW)
        {
            DateTime tempDtTime = DateTime.Now;
            foreach (var each in Store_Vendor_ProfileUnRemindForChangePW)
            {
                //產生key
                Guid temp = Guid.NewGuid();

                //更新到資料庫 store_vender_profile  LastUrlTime LastUrlKey
                UpdateLastUrlInfo(each.merchant_login, tempDtTime, temp);

                //寄信通知
                SendReminderMail(MailTo: each.email?.ToString() ?? "", StoreVendorName: each.merchant_name?.ToString() ?? "", temp.ToString());

                UpdateVenderProfileRemindStatus(each);

                GetDBHelper().Commit();
            }
        }

        public void UpdateLastUrlInfo(string merchant_login, DateTime tempDtTime, Guid tempKey)
        {
            var UpdateSql = @"
UPDATE store_vender_profile
SET 
  LastUrlTime    = @tempDtTime  
  ,LastUrlKey    = @tempKey  
WHERE
  merchant_login = @merchant_login";

            Dictionary<string, object> Paras = new Dictionary<string, object>
            {
                { "merchant_login", merchant_login},
                { "tempDtTime", tempDtTime},
                { "tempKey", tempKey.ToString()},

            };

            GetDBHelper().Execute(UpdateSql, Paras);
        }

        public void SendReminderMail(string MailTo, string StoreVendorName, string key)
        {
            string MailSubject = _configHelper.Config["RemindChangePWMail:Subject"];
            string ResetPasswordUrl = _configHelper.Config["RemindChangePWMail:ResetPasswordUrl"]; //目前是空字串，再更換成廠商更改密碼的連結
            ResetPasswordUrl += "/ChangePw/Change?tempid=" + key;
            //MailTo = "ray@websoft.com.tw"; //測試用

            string TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailTemplate", "RemindChangePW.json");
            var TemplateText = File.ReadAllText(TemplatePath);
            var jsonDoc = JsonDocument.Parse(TemplateText);
            string MailContent = jsonDoc.RootElement.GetProperty("PasswordResetEmailTemplate").GetString();
            MailContent = MailContent.Replace("{{VendorName}}", StoreVendorName);
            MailContent = MailContent.Replace("{{ResetPasswordUrl}}", ResetPasswordUrl);

            string InsertMailPoolCmd = $@"INSERT INTO tb_mailpool
(Subject, Content, EstimateSendTime, RealSendTime, SendStatus, ErrorMsg, Status, Creator, CreateTime, Updater, UpdateTime, DestinationEmail)
VALUES(@Subject, @Content, @EstimateSendTime, @RealSendTime, @SendStatus, @ErrorMsg, @Status, @Creator, @CreateTime, @Updater, @UpdateTime, @DestinationEmail)";

            Dictionary<string, object> Paras = new Dictionary<string, object>
            {
                { "Subject", MailSubject},
                { "Content", MailContent},
                { "EstimateSendTime",  DateTime.Now },
               { "SendStatus",  (int)MailSendStatusEnum.UnSent },
                { "Status",(int)StatusEnum.Enabled },
                { "RealSendTime",  null },
                 { "ErrorMsg", "" },
                { "Creator", -1 },
                { "CreateTime",  DateTime.Now },
                { "Updater",  -1},
                  { "UpdateTime",  DateTime.Now },
                 { "DestinationEmail", MailTo },
            };

            GetDBHelper().Execute(InsertMailPoolCmd, Paras);
        }

        public void UpdateVenderProfileRemindStatus(store_vender_profileDTO StoreVenderProfile)
        {
            var UpdateSql = @"
UPDATE store_vender_profile
SET 
  is_pwchange_remind    = @is_pwchange_remind  
WHERE
  order_id = @order_id;";

            Dictionary<string, object> Paras = new Dictionary<string, object>
            {
                { "is_pwchange_remind", "Y"},
                { "order_id", StoreVenderProfile.order_id},

            };

            GetDBHelper().Execute(UpdateSql, Paras);
        }

        public void LockVenderProfile_ByOrderId(store_vender_profileDTO StoreVenderProfile, int LockReason)
        {
            DateTime dtTime = DateTime.Now;
            var UpdateSql = @"
            UPDATE store_vender_profile
            SET                      
              locked                = @locked,
              locked_reason         = @locked_reason,
                locked_time=@locked_time
            WHERE
              order_id = @order_id;";

            Dictionary<string, object> Paras = new Dictionary<string, object>
            {
                { "locked", "Y"},
                { "locked_reason", LockReason},
                { "order_id", StoreVenderProfile.order_id},
                { "locked_time",dtTime}
            };

            GetDBHelper().Execute(UpdateSql, Paras);

            Dictionary<string, object> paras2 = new Dictionary<string, object>()
                                            {
                                                { "account",StoreVenderProfile.merchant_login},
                                                { "createtime",dtTime},
                                                { "locked_reason",LockReason},
                                            };
            GetDBHelper().Execute(@"
INSERT INTO tb_vender_password_history(
	account, pw, createtime, locked_reason)
	VALUES
(@account, null, @createtime, @locked_reason);

", paras2);

        }

        public void InsertVenderPWHistory(store_vender_profileDTO StoreVenderProfile)
        {
            string InsertVenderPWHistoryCmd = $@"INSERT INTO tb_vender_pw_history
(account, pw) VALUES (@account, @pw)";

            Dictionary<string, object> Paras = new Dictionary<string, object>
            {
                { "account", StoreVenderProfile.merchant_login},
                { "pw", StoreVenderProfile.merchant_password},
            };

            GetDBHelper().Execute(InsertVenderPWHistoryCmd, Paras);
        }
    }
     
}
