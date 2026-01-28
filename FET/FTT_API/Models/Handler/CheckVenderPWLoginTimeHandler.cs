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
            AND pw_chgtime > @StartTime";

            Dictionary<string, object> Paras = new();


            Paras = new Dictionary<string, object>
                        {
                            { "StartTime", DateTime.Now.AddDays(-90)},
                            { "EndTime", DateTime.Now.AddDays(-80)},
                            //{ "Locked", "N" },
                            //{ "IsPwChangeRemind", "Y" },

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

    public partial class CheckVenderPWLoginTimeHandler
    {
        public void CheckLastLoginTime()
        {

            List<store_vender_profileDTO> Store_Vender_ProfileNoLoginOver90Days = new();
            try
            {
                // 修正90天未登入邏輯：直接查詢需要鎖定的帳號
                Store_Vender_ProfileNoLoginOver90Days = GetStoreVendorNoLoginOver90Days();
                foreach (var each in Store_Vender_ProfileNoLoginOver90Days)
                {
                    LockVenderProfile_ByOrderId(each, LockReason: (int)LockReasonEnum.LockedByNoLoginOver90Days);
                    //InsertVenderPWHistory(each);
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

        public List<store_vender_profileDTO> GetStoreVendorNoLoginOver90Days()
        {
            // 寬鬆模式：同時考慮最後登入時間和密碼變更時間
            // 任一活動都算作使用者活動，可重置90天計時器
            string queryForStoreNoLoginOver90Days = $@"
SELECT svp.* 
FROM store_vender_profile svp
LEFT JOIN (
    SELECT 
        account,
        MAX(createtime) as last_login_time
    FROM user_login_log 
    WHERE loginstatus in ('True','TRUE')
    GROUP BY account
) ull ON svp.merchant_login = ull.account
WHERE svp.locked <> @Locked 
  AND (
    -- 情況1: 沒有登入記錄且密碼變更時間超過90天
    (ull.last_login_time IS NULL AND svp.pw_chgtime < NOW() - INTERVAL '90 days')
    -- 情況2: 有登入記錄，但登入和密碼變更都超過90天
    OR (ull.last_login_time IS NOT NULL 
        AND ull.last_login_time < NOW() - INTERVAL '90 days'
        AND svp.pw_chgtime < NOW() - INTERVAL '90 days')
  )";

            Dictionary<string, object> Paras = new Dictionary<string, object>();
            Paras.Add("Locked", "Y");

            List<store_vender_profileDTO> StoreNoLoginOver90Days = GetDBHelper().FindList<store_vender_profileDTO>(queryForStoreNoLoginOver90Days, Paras);

            return StoreNoLoginOver90Days;
        }

        // 保留原方法供參考，但不再使用
        public List<store_vender_profileDTO> GetStoreVendorNoLoginOver90Days_Old(List<user_login_logDTO> User_Login_Log_InLastLogin)
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

        // 保留原方法供參考，但不再使用
        public List<user_login_logDTO> GetLastLoginOver90days_Old()
        {
            // 修正90天未登入SQL邏輯：
            // 1. 先找每個帳號的最後一次登入記錄
            // 2. 再篩選超過90天的帳號
            // 3. 確保與登入時的判斷邏輯一致
            string queryForLastLogin = $@"SELECT * FROM (
    SELECT 
        *, 
        ROW_NUMBER() OVER (PARTITION BY account ORDER BY createtime DESC) AS rn 
    FROM 
        user_login_log
    WHERE 
        loginstatus in ('True','TRUE')
) t 
WHERE 
    rn = 1 
    AND createtime < NOW() - INTERVAL '90 days';";

            List<user_login_logDTO> ResultList = GetDBHelper().FindList<user_login_logDTO>(queryForLastLogin, new Dictionary<string, object>());

            return ResultList;
        }
    }
}
