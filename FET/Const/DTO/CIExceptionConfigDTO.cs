namespace Const.DTO
{
    public class CIExceptionConfigDTO
    {
        #region -- 資料庫欄位 --
        /// <summary>
        /// CISID
        /// </summary>
        public string? cisid { get; set; }
        /// <summary>
        /// 廠商代碼
        /// </summary>
        public string? vendor_id { get; set; }
        /// <summary>
        /// IVRCODE
        /// </summary>
        public string? ivrcode { get; set; }
        /// <summary>
        /// 驗收日期
        /// </summary>
        public DateTime? approval_date { get; set; }
        #endregion -- 資料庫欄位 --

        /// <summary>
        /// 報修類別
        /// </summary>
        public string? aciname { get; set; }
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? merchant_name { get; set; }
        /// <summary>
        /// 門市名稱
        /// </summary>
        public string? shop_name { get; set; }
        /// <summary>
        /// 驗收日期
        /// </summary>
        public string? approval_date_text { get; set; }
        /// <summary>
        /// 執行註記
        /// </summary>
        public string? flag { get; set; }

        /// <summary>
        /// 門市名稱
        /// </summary>
        public string? ShopNameLike { get; set; }
    }
}
