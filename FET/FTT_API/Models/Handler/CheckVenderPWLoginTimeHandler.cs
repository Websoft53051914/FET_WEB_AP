using Core.Utility.Extensions;
using Core.Utility.Helper.Mail;
using DocumentFormat.OpenXml.Office2013.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;
using System.Transactions;
using static Const.Enums;


namespace FTT_API.Models.Handler
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
                    InsertVenderPWHistory(each);
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
            and locked = @Locked and is_pwchange_remind <> @IsPwChangeRemind";

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
            foreach (var each in Store_Vendor_ProfileUnRemindForChangePW)
            {
                //寄信通知
                SendReminderMail(MailTo: each.email?.ToString() ?? "", StoreVendorName: each.merchant_name?.ToString() ?? "");

                UpdateVenderProfileRemindStatus(each);

                GetDBHelper().Commit();
            }
        }

        public void SendReminderMail(string MailTo, string StoreVendorName)
        {
            string SmtpServer = _configHelper.Config["GmailConfig:SmtpServer"];
            int SmtpPort = int.Parse(_configHelper.Config["GmailConfig:SmtpPort"]);
            string MailAccount = _configHelper.Config["GmailConfig:MailUserID"];
            string MailUserPwd = _configHelper.Config["GmailConfig:MailUserPwd"];
            bool.TryParse(_configHelper.Config["GmailConfig:EnableSsl"], out bool EnableSSL);


            string MailSubject = _configHelper.Config["RemindChangePWMail:Subject"];
            string ResetPasswordUrl = _configHelper.Config["RemindChangePWMail:ResetPasswordUrl"]; //目前是空字串，再更換成廠商更改密碼的連結
            //MailTo = "ray@websoft.com.tw"; //測試用

            string TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailTemplate", "RemindChangePW.json");
            var TemplateText = File.ReadAllText(TemplatePath);
            var jsonDoc = JsonDocument.Parse(TemplateText);
            string MailContent = jsonDoc.RootElement.GetProperty("PasswordResetEmailTemplate").GetString();
            MailContent = MailContent.Replace("{{VendorName}}", StoreVendorName);
            MailContent = MailContent.Replace("{{ResetPasswordUrl}}", ResetPasswordUrl);


            MailHelper mailHelper = new(SmtpServer, SmtpPort, MailAccount, MailUserPwd);
            mailHelper.Subject = MailSubject;
            mailHelper.Body = MailContent;
            mailHelper.MailFrom = MailAccount;
            mailHelper.MailTos = new string[] { MailTo };
            mailHelper.EnableSsl = EnableSSL;

            string InsertMailPoolCmd = $@"INSERT INTO tb_mailpool
(Subject, Content, EstimateSendTime, RealSendTime, SendStatus, ErrorMsg, Status, Creator, CreateTime, Updater, UpdateTime, DestinationEmail)
VALUES(@Subject, @Content, @EstimateSendTime, @RealSendTime, @SendStatus, @ErrorMsg, @Status, @Creator, @CreateTime, @Updater, @UpdateTime, @DestinationEmail)";

          

            Dictionary<string, object> Paras = new Dictionary<string, object>
            {
                { "Subject", MailSubject},
                { "Content", MailContent},
                { "EstimateSendTime",  DateTime.Now },
                { "RealSendTime",  DateTime.Now  },
                { "Status",(int)StatusEnum.Cancel },
                { "Creator", -1 },
                { "CreateTime",  DateTime.Now },
                { "Updater",  -1},
                 { "UpdateTime",  DateTime.Now },
                 { "DestinationEmail", MailTo },        
            };


            try
            {
                mailHelper.Send();
                Paras.Add("SendStatus",  (int)MailSendStatusEnum.Sent);
                Paras.Add("ErrorMsg", "");

            }
            catch (Exception ex)
            {
                Paras.Add("SendStatus", (int)MailSendStatusEnum.Error);
                Paras.Add("ErrorMsg", ex.Message);
              
            }
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

            var UpdateSql = @"
            UPDATE store_vender_profile
            SET                      
              locked                = @locked,
              locked_reason         = @locked_reason             
            WHERE
              order_id = @order_id;";           

            Dictionary<string, object> Paras = new Dictionary<string, object>
            {
                { "locked", "Y"},
                { "locked_reason", LockReason},
                { "order_id", StoreVenderProfile.order_id},
            };

            GetDBHelper().Execute(UpdateSql, Paras);

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

    public partial class CheckVenderPWLoginTimeHandler
    {
        public void CheckLastLoginTime()
        {
         
            
            List<user_login_logDTO> User_Login_Log_InLastLoginOver90days = new();
            List<store_vender_profileDTO> Store_Vender_ProfileNoLoginOver90Days = new();
            try
            {                

                User_Login_Log_InLastLoginOver90days = GetLastLoginOver90days();
                Store_Vender_ProfileNoLoginOver90Days = GetStoreVendorNoLoginOver90Days(User_Login_Log_InLastLoginOver90days);
                foreach (var each in Store_Vender_ProfileNoLoginOver90Days)
                {
                    LockVenderProfile_ByOrderId(each, LockReason: (int)LockReasonEnum.LockedByNoLoginOver90Days);
                    InsertVenderPWHistory(each);
                    GetDBHelper().Commit();
                }

                //超過90天未登入，似乎不用寄信
                //InsertPWChangeRemindAndRemindStatus(Store_Vendor_ProfileNoLoginOver90Days);
                
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "CheckLastLoginTime");
            }
        }

        public List<store_vender_profileDTO> GetStoreVendorNoLoginOver90Days(List<user_login_logDTO> User_Login_Log_InLastLogin)
        {
            List<string> StoreVendorAccount = new List<string>();
            foreach (var each in User_Login_Log_InLastLogin)
            {
                if (!each.account.IsNullOrEmpty())
                {
                    StoreVendorAccount.Add(each.account);
                }
            }

             string queryForStoreNoLoginOver90Days = "";

            List<store_vender_profileDTO> StoreNoLoginOver90Days = new();
            if (StoreVendorAccount.Count > 0)
            {
                queryForStoreNoLoginOver90Days = $@"Select * from store_vender_profile where locked <> @Locked and merchant_login in @StoreVendorAccount";
              
                Dictionary<string, object> Paras = new Dictionary<string, object>();
                Paras.Add("Locked", "Y");
                Paras.Add("StoreVendorAccount", StoreVendorAccount);

                StoreNoLoginOver90Days = GetDBHelper().FindList<store_vender_profileDTO>(queryForStoreNoLoginOver90Days, Paras);
            }

            return StoreNoLoginOver90Days;

        }

        public List<user_login_logDTO> GetLastLoginOver90days()
        {
            string queryForLastLogin = $@"SELECT * FROM (
    SELECT 
        *, 
        ROW_NUMBER() OVER (PARTITION BY account ORDER BY createtime DESC) AS rn 
    FROM 
        user_login_log
    WHERE
        createtime < NOW() - INTERVAL '90 days'
) t 
WHERE 
    rn = 1;";

            List<user_login_logDTO> ResultList = GetDBHelper().FindList<user_login_logDTO>(queryForLastLogin, new Dictionary<string, object>());

            return ResultList;
        }
    }
}
