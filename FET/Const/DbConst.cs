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
        /// 介接資料時間格式
        /// </summary>
        public const string FORMAT_ADAPTER_DATETIME = "yyyyMMddHHmmss";
        /// <summary>
        /// 招募時的互動狀態填入的互動類型
        /// </summary>
        public const string RECRUIT_INTERACTION_TYPE = "2";
        /// <summary>
        /// 招募時的互動狀態填入的身分別
        /// </summary>
        public const string RECRUIT_EMPLOYEE_IDENTITY = "111";
        /// <summary>
        /// 等於
        /// </summary>
        public const string OPERATOR_EQUAL = "eq";

        /// <summary>
        /// 不等於
        /// </summary>
        public const string OPERATOR_NOT_EQUAL = "neq";

        /// <summary>
        /// 不在清單之中
        /// </summary>
        public const string OPERATOR_NOT_IN = "notin";

        /// <summary>
        /// 為 null
        /// </summary>
        public const string OPERATOR_IS_NULL = "isnull";

        /// <summary>
        /// 不是 null
        /// </summary>
        public const string OPERATOR_IS_NOT_NULL = "isnotnull";

        /// <summary>
        /// 為空字串
        /// </summary>
        public const string OPERATOR_IS_EMPTY = "isempty";

        /// <summary>
        /// 不是空字串
        /// </summary>
        public const string OPERATOR_IS_NOT_EMPTY = "isnotempty";

        /// <summary>
        /// 大於等於
        /// </summary>
        public const string OPERATOR_GREATER_THAN_OR_EQUAL = "gte";

        /// <summary>
        /// 大於
        /// </summary>
        public const string OPERATOR_GREATER_THAN = "gt";

        /// <summary>
        /// 開頭為
        /// </summary>
        public const string OPERATOR_STARTSWITH = "startswith";

        /// <summary>
        /// 包含
        /// </summary>

        public const string OPERATOR_CONTAINS = "contains";

        /// <summary>
        /// 小於等於
        /// </summary>

        public const string OPERATOR_LESS_THAN_OR_EQUAL = "lte";

        /// <summary>
        /// 小於
        /// </summary>

        public const string OPERATOR_LESS_THAN = "lt";

        /// <summary>
        /// 在清單之中
        /// </summary>
        public const string OPERATOR_IN = "in";

        /// <summary>
        /// 等於或等於空
        /// </summary>
        public const string OPERATOR_EQORNULL = "eqorisnull";

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
