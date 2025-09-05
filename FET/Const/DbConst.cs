namespace Const
{
    public class DbConst
    {
        /// <summary>
        /// 已刪除/ disabled
        /// </summary>
        public const string DELETED = "Y";

        /// <summary>
        /// 未刪除/ enabled
        /// </summary>
        public const string ALIVE = "N";

        /// <summary>
        /// 其他選項值
        /// </summary>
        public const string OPTION_OTHER = "999";

        /// <summary>
        /// 資料庫時間格式
        /// </summary>
        public const string FORMAT_DATETIME = "yyyyMMdd HHmmss";

        /// <summary>
        /// 日期時間格式
        /// </summary>
        public const string FORMAT_DATETIME2 = "yyyy/MM/dd HH:mm:ss";

        /// <summary>
        /// 時間格式(預定入國日+航班抵達時間用)
        /// </summary>
        public const string FORMAT_SHORTDATETIME = "yyyyMMddHHmm";

        /// <summary>
        /// 資料庫日期格式
        /// </summary>
        public const string FORMAT_DATE = "yyyyMMdd";
        /// <summary>
        /// 顯示日期格式
        /// </summary>
        public const string FORMAT_DATE2 = "yyyy/MM/dd";

        /// <summary>
        /// 資料庫時段 TimeSpan 轉換格式
        /// </summary>
        public const string FORMAT_TIME = "hhmm";

        /// <summary>
        /// 資料庫時段 TimeSpan 轉換格式
        /// </summary>
        public const string FORMAT_TIME_2 = "hhmmss";

        /// <summary>
        /// 資料庫時段 TimeSpan 轉換格式
        /// </summary>
        public const string FORMAT_TIME3 = "HH:mm";

        /// <summary>
        /// 狀態項目顯示文字參照
        /// </summary>
        public static Dictionary<string, string> GetRefYNDisplayText()
        {
            return new()
            {
                { "Y", "是" },
                { "N", "否" },
            };
        }
    }
}
