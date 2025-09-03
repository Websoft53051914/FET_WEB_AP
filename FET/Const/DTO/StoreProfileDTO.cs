namespace Const.DTO
{
    /// <summary>
    /// store_profile
    /// </summary>
    public class StoreProfileDTO
    {
        #region -- 資料庫欄位 --
        /// <summary>
        /// 公司別
        /// </summary>
        public string? company_leaves { get; set; }
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
        /// 店名
        /// </summary>
        public string? shop_name { get; set; }
        /// <summary>
        /// ivrcode
        /// </summary>
        public string? ivr_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? owner_cname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? owner_ename { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? as_empno { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? as_cname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? as_ename { get; set; }
        /// <summary>
        /// 店長電話
        /// </summary>
        public string? owner_tel { get; set; }
        /// <summary>
        /// 緊急電話
        /// </summary>
        public string? urgent_tel { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string? address { get; set; }
        #endregion -- 資料庫欄位 --

        /// <summary>
        /// 店長/聯絡人
        /// </summary>
        public string? owner_name { get; set; }
        /// <summary>
        /// 區經理/業務
        /// </summary>
        public string? as_name { get; set; }
        /// <summary>
        /// ivrcode
        /// </summary>
        public string? IvrCodeLike { get; set; }
        /// <summary>
        /// 店名
        /// </summary>
        public string? ShopNameLike { get; set; }
        /// <summary>
        /// 公司別
        /// </summary>
        public string? CompanyLeavesLike { get; set; }
        /// <summary>
        /// 通路
        /// </summary>
        public string? ChannelLike { get; set; }
        /// <summary>
        /// 店格
        /// </summary>
        public string? StoreTypeLike { get; set; }
    }
}
