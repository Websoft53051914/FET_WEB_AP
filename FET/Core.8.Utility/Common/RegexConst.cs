namespace Core.Utility.Common
{
    /// <summary>
    /// 正規表示式、規律表達式、正規表達式、正規表示法、規則運算式、常規表示法
    /// </summary>
    public class RegexConst
    {
        /// <summary>
        /// 手機格式
        /// </summary>
        public const string PHONE = @"^[0-9*+#\-\s]*$";
        //TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        public const string CODE = @"^[0-9A-Za-z]+$";
        /// <summary>
        /// 郵遞區號
        /// </summary>
        public const string ZIPCODE = @"^[1-9]{1}[0-9]{2}$";
    }
}
