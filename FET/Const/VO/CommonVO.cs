namespace Const.VO
{
    public class CommonPartialViewVO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// _DialogIvrCodeGrid
    /// </summary>
    public class DialogIvrCodeGridVO : CommonPartialViewVO
    {
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
    /// <summary>
    /// _DialogIvrCodeGrid
    /// </summary>
    public class DialogIvrCodeGridGridVO
    {
        /// <summary>
        /// ivrcode
        /// </summary>
        public string? IvrCode { get; set; }
        /// <summary>
        /// 公司別
        /// </summary>
        public string? CompanyLeaves { get; set; }
        /// <summary>
        /// 店格
        /// </summary>
        public string? StoreType { get; set; }
        /// <summary>
        /// 通路
        /// </summary>
        public string? Channel { get; set; }
        /// <summary>
        /// 區域
        /// </summary>
        public string? Area { get; set; }
        /// <summary>
        /// 店名
        /// </summary>
        public string? ShopName { get; set; }
        /// <summary>
        /// 店長/聯絡人
        /// </summary>
        public string? OwnerName { get; set; }
        /// <summary>
        /// 區經理/業務
        /// </summary>
        public string? AsName { get; set; }
        /// <summary>
        /// 店長電話
        /// </summary>
        public string? OwnerTel { get; set; }
        /// <summary>
        /// 緊急電話
        /// </summary>
        public string? UrgentTel { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }
    }
    /// <summary>
    /// _DialogVenderGrid
    /// </summary>
    public class DialogVenderGridVO : CommonPartialViewVO
    {
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? MerchantNameLike { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string? CpNameLike { get; set; }
    }
    /// <summary>
    /// _DialogVenderGrid
    /// </summary>
    public class DialogVenderGridGridVO
    {
        /// <summary>
        /// 編號
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? MerchantName { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string? CpName { get; set; }
        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string? CpTel { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// 登入帳號
        /// </summary>
        public string? MerchantLogin { get; set; }
    }
}
