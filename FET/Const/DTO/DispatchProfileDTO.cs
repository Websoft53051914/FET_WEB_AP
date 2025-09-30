namespace Const.DTO
{
    /// <summary>
    /// dispatch_profile
    /// </summary>
    public class DispatchProfileDTO
    {
        #region -- 資料庫欄位 --
        /// <summary>
        /// 保固
        /// </summary>
        public string? ifwarrant { get; set; }
        /// <summary>
        /// 編號
        /// </summary>
        public int? id { get; set; }
        #endregion -- 資料庫欄位 --

        /// <summary>
        /// 廠商ID
        /// </summary>
        public string? tvender { get; set; }
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? vender { get; set; }
        /// <summary>
        /// IVR CODE
        /// </summary>
        public string? tivr_code { get; set; }
        /// <summary>
        /// store
        /// </summary>
        public string? store { get; set; }
        /// <summary>
        /// 類別ID
        /// </summary>
        public string? tcisid { get; set; }
        /// <summary>
        /// 類別名稱
        /// </summary>
        public string? ci_name { get; set; }

    }

}
