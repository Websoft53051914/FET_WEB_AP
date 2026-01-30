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
            // 增加日誌記錄
            Console.WriteLine($"CheckLastLoginTime started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            
            List<store_vender_profileDTO> Store_Vender_ProfileNoLoginOver90Days = new();
            try
            {
                // 修正90天未登入邏輯：直接查詢需要鎖定的帳號
                Store_Vender_ProfileNoLoginOver90Days = GetStoreVendorNoLoginOver90Days();
                
                Console.WriteLine($"Found {Store_Vender_ProfileNoLoginOver90Days.Count} accounts to lock");
                
                foreach (var each in Store_Vender_ProfileNoLoginOver90Days)
                {
                    Console.WriteLine($"Locking account: {each.merchant_login}");
                    LockVenderProfile_ByOrderId(each, LockReason: (int)LockReasonEnum.LockedByNoLoginOver90Days);
                    //InsertVenderPWHistory(each);
                    GetDBHelper().Commit();
                }

                //超過90天未登入，似乎不用寄信
                //InsertPWChangeRemindAndRemindStatus(Store_Vendor_ProfileNoLoginOver90Days);
                
                Console.WriteLine($"CheckLastLoginTime completed successfully");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"CheckLastLoginTime failed: {ex.Message}");
                LogError(ex.Message, "CheckLastLoginTime");
            }
        }

        public List<store_vender_profileDTO> GetStoreVendorNoLoginOver90Days()
        {
            // 寬鬆模式：登入或密碼變更任一活動在90天內都不鎖定
            // 只有當「最後登入」AND「密碼變更」都超過90天才鎖定
            // 注意：pw_chgtime 為 NULL 視為很久以前，應該被鎖定
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
    -- 情況1: 沒有登入記錄且密碼變更時間超過90天(包含NULL)
    (ull.last_login_time IS NULL AND (svp.pw_chgtime IS NULL OR svp.pw_chgtime < NOW() - INTERVAL '90 days'))
    -- 情況2: 有登入記錄但超過90天，且密碼變更也超過90天(包含NULL)
    OR (ull.last_login_time IS NOT NULL 
        AND ull.last_login_time < NOW() - INTERVAL '90 days'
        AND (svp.pw_chgtime IS NULL OR svp.pw_chgtime < NOW() - INTERVAL '90 days'))
  )";

            Dictionary<string, object> Paras = new Dictionary<string, object>();
            Paras.Add("Locked", "Y");

            // 除錯：執行查詢前先記錄相關資訊
            Console.WriteLine($"Debug: Executing query to find accounts over 90 days");
            Console.WriteLine($"Debug: Current time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Debug: 90 days ago: {DateTime.Now.AddDays(-90):yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Debug: SQL Query: {queryForStoreNoLoginOver90Days}");

            // 先執行一些除錯查詢來了解資料狀況
            try
            {
                // 查詢總共有多少個未鎖定的廠商帳號
                string countQuery = "SELECT COUNT(*) FROM store_vender_profile WHERE locked <> @Locked";
                var totalCount = GetDBHelper().Find<int>(countQuery, Paras);
                Console.WriteLine($"Debug: Total unlocked vendor accounts: {totalCount}");

                // 查詢有多少廠商的密碼變更時間超過90天(包含NULL)
                string pwQuery = "SELECT COUNT(*) FROM store_vender_profile WHERE locked <> @Locked AND (pw_chgtime IS NULL OR pw_chgtime < NOW() - INTERVAL '90 days')";
                var pwCount = GetDBHelper().Find<int>(pwQuery, Paras);
                Console.WriteLine($"Debug: Accounts with password changed > 90 days ago (including NULL): {pwCount}");

                // 查詢NULL密碼變更時間的帳號數
                string pwNullQuery = "SELECT COUNT(*) FROM store_vender_profile WHERE locked <> @Locked AND pw_chgtime IS NULL";
                var pwNullCount = GetDBHelper().Find<int>(pwNullQuery, Paras);
                Console.WriteLine($"Debug: Accounts with NULL pw_chgtime: {pwNullCount}");

                // 查詢登入記錄表的一些基本資訊
                string loginQuery = "SELECT COUNT(DISTINCT account) FROM user_login_log WHERE loginstatus in ('True','TRUE')";
                var loginAccounts = GetDBHelper().Find<int>(loginQuery, new Dictionary<string, object>());
                Console.WriteLine($"Debug: Total accounts with successful login records: {loginAccounts}");
            }
            catch (Exception debugEx)
            {
                Console.WriteLine($"Debug query error: {debugEx.Message}");
            }

            List<store_vender_profileDTO> StoreNoLoginOver90Days = GetDBHelper().FindList<store_vender_profileDTO>(queryForStoreNoLoginOver90Days, Paras);

            Console.WriteLine($"Debug: Final query returned {StoreNoLoginOver90Days.Count} accounts");

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

        /// <summary>
        /// 測試方法：只查詢應該被鎖定的帳號，不實際執行鎖定
        /// </summary>
        public void TestCheckLastLoginTime()
        {
            Console.WriteLine($"TestCheckLastLoginTime started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            
            try
            {
                List<store_vender_profileDTO> accountsToLock = GetStoreVendorNoLoginOver90Days();
                
                Console.WriteLine($"Test Result: Found {accountsToLock.Count} accounts that should be locked");
                
                foreach (var account in accountsToLock)
                {
                    Console.WriteLine($"Should lock: {account.merchant_login} ({account.merchant_name})");
                }
                
                Console.WriteLine($"TestCheckLastLoginTime completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TestCheckLastLoginTime failed: {ex.Message}");
                LogError(ex.Message, "TestCheckLastLoginTime");
            }
        }
    }
}
