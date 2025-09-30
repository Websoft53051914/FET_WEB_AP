namespace Const.VO
{
    public class CIConfigIndexVO
    {
        /// <summary>
        /// 門市名稱
        /// </summary>
        public string? ShopNameLike { get; set; }
    }

    /// <summary>
    /// 例外派工維護
    /// </summary>
    public class CIConfigGridVO
    {
        /// <summary>
        /// CISID
        /// </summary>
        public string? Cisid { get; set; }
        /// <summary>
        /// 廠商代碼
        /// </summary>
        public string? VendorId { get; set; }
        /// <summary>
        /// IVRCODE
        /// </summary>
        public string? Ivrcode { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        public string? Aciname { get; set; }
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? MerchantName { get; set; }
        /// <summary>
        /// 門市名稱
        /// </summary>
        public string? ShopName { get; set; }
        /// <summary>
        /// 驗收日期
        /// </summary>
        public string? ApprovalDateText { get; set; }
    }
}
