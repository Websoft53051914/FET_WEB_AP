namespace Const.DTO
{
    /// <summary>
    /// store_vender_profile
    /// </summary>
    public class StoreVenderProfileDTO
    {
        #region -- 資料庫欄位 --
        /// <summary>
        /// 編號
        /// </summary>
        public int order_id { get; set; }
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? merchant_name { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string? cp_name { get; set; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string? cp_tel { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string? email { get; set; }
        /// <summary>
        /// 登入帳號
        /// </summary>
        public string? merchant_login { get; set; }
        #endregion -- 資料庫欄位 --

        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? MerchantNameLike { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string? CpNameLike { get; set; }
    }
}
