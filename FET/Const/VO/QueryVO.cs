using Microsoft.AspNetCore.Mvc.Rendering;

namespace Const.VO
{
    /// <summary>
    /// Query/Index.cshtml
    /// </summary>
    public class QueryIndexVO
    {
        /// <summary>
        /// 報修日
        /// </summary>
        public DateTime? CreateDateGte { get; set; }
        /// <summary>
        /// 報修日
        /// </summary>
        public DateTime? CreateDateLte { get; set; }
        /// <summary>
        /// 完修日
        /// </summary>
        public DateTime? CompleteDateGte { get; set; }
        /// <summary>
        /// 完修日
        /// </summary>
        public DateTime? CompleteDateLte { get; set; }
        /// <summary>
        /// 結案日
        /// </summary>
        public DateTime? CloseDateGte { get; set; }
        /// <summary>
        /// 結案日
        /// </summary>
        public DateTime? CloseDateLte { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        public string? StatusIdEq { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        public List<SelectListItem> SelectListStatus { get; set; } = [];
        /// <summary>
        /// 工單號碼
        /// </summary>
        public string? FormNoEq { get; set; }
        /// <summary>
        /// 報修型態
        /// </summary>
        public string? TtCategoryEq { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        public string? CategoryIdEq { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        public string? CategoryName { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        public string? CategoryDesc { get; set; }
        /// <summary>
        /// 報修廠商
        /// </summary>
        public string? VenderIdEq { get; set; }
        /// <summary>
        /// 報修廠商
        /// </summary>
        public string? VendorName { get; set; }
        /// <summary>
        /// 報修門市
        /// </summary>
        public string? IvrCodeEq { get; set; }
        /// <summary>
        /// 報修門市
        /// </summary>
        public string? StoreName { get; set; }
        /// <summary>
        /// 公司別
        /// </summary>
        public string? CompanyEq { get; set; }
        /// <summary>
        /// 公司別選單資料
        /// </summary>
        public List<SelectListItem> SelectListCompany { get; set; } = [];
        /// <summary>
        /// 店格
        /// </summary>
        public string? StoreTypeEq { get; set; }
        /// <summary>
        /// 店格選單資料
        /// </summary>
        public List<SelectListItem> SelectListStoreType { get; set; } = [];
        /// <summary>
        /// 通路
        /// </summary>
        public string? ChannelEq { get; set; }
        /// <summary>
        /// 通路選單資料
        /// </summary>
        public List<SelectListItem> SelectListChannel { get; set; } = [];
        /// <summary>
        /// 區域
        /// </summary>
        public string? AreaEq { get; set; }
        /// <summary>
        /// 區域選單資料
        /// </summary>
        public List<SelectListItem> SelectListArea { get; set; } = [];
        /// <summary>
        /// 區經理/業務
        /// </summary>
        public string? AsEmpNoEq { get; set; }
        /// <summary>
        /// 區經理/業務選單資料
        /// </summary>
        public List<SelectListItem> SelectListAsEmp { get; set; } = [];
        /// <summary>
        /// 門市自行尋商
        /// </summary>
        public string? SelfConfigEq { get; set; }
    }
    /// <summary>
    /// Query/Index.cshtml
    /// </summary>
    public class QueryGridVO
    {
        /// <summary>
        /// 工單號碼
        /// </summary>
        public int? FormNo { get; set; }
        /// <summary>
        /// 報修型態
        /// </summary>
        public string? TtCategory { get; set; }
        /// <summary>
        /// 報修品項
        /// </summary>
        public string? Ciname { get; set; }
        /// <summary>
        /// 報修日期
        /// </summary>
        public string? CreateTimeText { get; set; }
        /// <summary>
        /// 報修門市
        /// </summary>
        public string? ShopName { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        public string? StatusName { get; set; }
        /// <summary>
        /// 派單日期
        /// </summary>
        public string? DispatchTimeText { get; set; }
        /// <summary>
        /// 廠商
        /// </summary>
        public string? Vender { get; set; }
        /// <summary>
        /// 報修說明
        /// </summary>
        public string? Descr { get; set; }
        /// <summary>
        /// 處理者
        /// </summary>
        public string? Processer { get; set; }
    }
}
