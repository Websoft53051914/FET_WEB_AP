using System.ComponentModel;

namespace Const
{
    public class Enums
    {
        /// <summary>
        /// 值=table內id
        /// </summary>
        public enum FuncID
        {
            /// <summary>
            /// 首頁
            /// </summary>
            Home_View = 0,

            /// <summary>
            /// 
            /// </summary>
            ResetTESTP = 1,

            /// <summary>
            /// 新開單
            /// </summary>
            NewOrder_View = 10001,

            /// <summary>
            /// 自行尋商開單
            /// </summary>
            NewOrderSelfVendor_View = 10002,

            /// <summary>
            /// 待處理
            /// </summary>
            Pending_View = 10003,

            /// <summary>
            /// 列印到場單
            /// </summary>
            OnsitePrint_View = 10004,

            /// <summary>
            /// 處理中
            /// </summary>
            InProcess_View = 10005,

            /// <summary>
            /// 已結案
            /// </summary>
            CaseClosed_View = 10006,

            /// <summary>
            /// 查詢
            /// </summary>
            Query_View = 10007,

            /// <summary>
            /// 報價維護
            /// </summary>
            QuoteMgt_View = 10008,

            /// <summary>
            /// 門市資料維護
            /// </summary>
            StoreMgt_View = 10009,

            /// <summary>
            /// 廠商資料維護
            /// </summary>
            SupplierMgt_View = 10010,

            /// <summary>
            /// 派工規則維護
            /// </summary>
            DispatchRuleMgt_View = 10011,

            /// <summary>
            /// 例外派工維護
            /// </summary>
            CIConfig_View = 10012,

            /// <summary>
            /// 保全廠商維護
            /// </summary>
            SecurityMgt_View = 10013,

            /// <summary>
            /// 角色權限維護
            /// </summary>
            FTTGroupMgt_View = 10014,

            /// <summary>
            /// 維修品項維護
            /// </summary>
            CIMgt_View = 10015,

            /// <summary>
            /// 派工中
            /// </summary>
            Dispatching_View = 10016,

            /// <summary>
            /// 已派工
            /// </summary>
            Dispatched_View = 10017,

            /// <summary>
            /// Mail Server 設定
            /// </summary>
            MailServerSetting_View = 10018,

        }

        public enum StatusEnum
        {
            [Description("啟用")]
            Enabled = 1,

            [Description("停用")]
            Disabled = 8,

            [Description("作廢")]
            Cancel = 9,
        }

        public enum MailSendStatusEnum
        {
            [Description("未寄出")]
            UnSent = 0,

            [Description("已寄出")]
            Sent = 1,

            [Description("錯誤")]
            Error = 2,
        }

        public enum LogStatusEnum
        {
            /// <summary>
            /// 失敗
            /// </summary>
            [Description("失敗")]
            Failed = 0,

            /// <summary>
            /// 成功
            /// </summary>
            [Description("成功")]
            Success = 1,
        }

        public enum LockReasonEnum
        {
            /// <summary>
            /// 未鎖定
            /// </summary>
            [Description("未鎖定")]
            Unlocked = 0,

            /// <summary>
            /// 密碼 3 個月未更換
            /// </summary>
            [Description("密碼3個月未更換")]
            LockedByNochangePWOver90Days = 1,           

            /// <summary>
            /// 密碼 3 個月未更換
            /// </summary>
            [Description("3個月未登入")]
            LockedByNoLoginOver90Days = 2,

            /// <summary>
            /// Admin解鎖
            /// </summary>
            [Description("Admin解鎖")]
            UnlockByAdmin = 3,

            /// <summary>
            /// 廠商自行更換密碼
            /// </summary>
            [Description("廠商自行更換密碼")]
            ChangePwByVender = 4,
        }
    }
}
