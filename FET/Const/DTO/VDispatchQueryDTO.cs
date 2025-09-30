namespace Const.DTO
{
    /// <summary>
    /// v_dispatch_query
    /// </summary>
    public class VDispatchQueryDTO
    {
        #region -- 資料庫欄位 --
        /// <summary>
        /// ivrcode
        /// </summary>
        public string? ivr_code { get; set; }
        /// <summary>
        /// 報修門市
        /// </summary>
        public string? shop_name { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        public string? l1_desc { get; set; }
        /// <summary>
        /// 報修次類別
        /// </summary>
        public string? l2_desc { get; set; }
        /// <summary>
        /// 報修名稱
        /// </summary>
        public string? ciname { get; set; }
        /// <summary>
        /// 保固內廠商
        /// </summary>
        public string? warrant { get; set; }
        /// <summary>
        /// 保固外廠商
        /// </summary>
        public string? nonwarrant { get; set; }
        /// <summary>
        /// 廠商
        /// </summary>
        public int? vender_id { get; set; }
        /// <summary>
        /// 公司別
        /// </summary>
        public string? company { get; set; }
        /// <summary>
        /// 店格
        /// </summary>
        public string? store_type { get; set; }
        /// <summary>
        /// 通路
        /// </summary>
        public string? channel { get; set; }
        /// <summary>
        /// 區域
        /// </summary>
        public string? area { get; set; }
        /// <summary>
        /// 是否保固
        /// </summary>
        public string? ifwarrant { get; set; }
        #endregion -- 資料庫欄位 --

        /// <summary>
        /// 報修類別
        /// </summary>
        public string? CategoryIdFilter { get; set; }
        /// <summary>
        /// 報修廠商
        /// </summary>
        public string? VenderIdEq { get; set; }
        /// <summary>
        /// 報修門市
        /// </summary>
        public string? IvrCodeEq { get; set; }
        /// <summary>
        /// 公司別
        /// </summary>
        public string? CompanyEq { get; set; }
        /// <summary>
        /// 店格
        /// </summary>
        public string? StoreTypeEq { get; set; }
        /// <summary>
        /// 通路
        /// </summary>
        public string? ChannelEq { get; set; }
        /// <summary>
        /// 區域
        /// </summary>
        public string? AreaEq { get; set; }
        /// <summary>
        /// 是否保固
        /// </summary>
        public string? IfWarrantEq { get; set; }
    }
}
