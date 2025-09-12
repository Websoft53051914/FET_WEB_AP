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
            Home_View = 0,
            ResetTESTP = 1,

            //新開單
            NewOrder_View = 10001,
            //自行尋商開單
            NewOrderSelfVendor_View = 10002,
            //待處理
            Pending_View = 10003,
            //列印到場單
            OnsitePrint_View = 10004,
            //處理中
            InProcess_View = 10005,
            //已結案
            CaseClosed_View = 10006,
            //查詢
            Query_View = 10007,


            //報價維護
            QuoteMgt_View = 10008,


            //門市資料維護
            StoreMgt_View = 10009,
            //廠商資料維護
            SupplierMgt_View = 10010,
            //派工規則維護
            DispatchRuleMgt_View = 10011,
            //例外派工維護
            CIConfig_View = 10012,
            //保全廠商維護
            SecurityMgt_View = 10013,
            //角色權限維護
            FTTGroupMgt_View = 10014,
            //維修品項維護
            CIMgt_View = 10015,
            //派工中
            Dispatching_View = 10016,
            //已派工
            Dispatched_View = 10017,

            //Mail Server 設定
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
    }
}
