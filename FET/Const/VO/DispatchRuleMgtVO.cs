using Const.DTO;
using Core.Utility.Web.EX;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Const.VO
{
    /// <summary>
    /// 派工規則維護
    /// </summary>
    public class DispatchRuleMgtGridVO
    {
        /// <summary>
        /// 廠商ID
        /// </summary>
        public string? TVender { get; set; }
        /// <summary>
        /// 廠商名稱
        /// </summary>
        public string? Vender { get; set; }
        /// <summary>
        /// IVR CODE
        /// </summary>
        public string? TIvrCode { get; set; }
        /// <summary>
        /// 類別ID
        /// </summary>
        public string? TCisId { get; set; }
        /// <summary>
        /// 類別名稱
        /// </summary>
        public string? CiName { get; set; }
        /// <summary>
        /// 保固
        /// </summary>
        public string? IfWarrant { get; set; }
        /// <summary>
        /// 編號
        /// </summary>
        public int? Id { get; set; }
    }

    /// <summary>
    /// 派工規則維護
    /// </summary>
    public class DispatchRuleMgtEditVO
    {
        /// <summary>
        /// 編輯資料
        /// </summary>
        public DispatchRuleMgtVO Data { get; set; } = new();
        /// <summary>
        /// IVR CODE 選單資料
        /// </summary>
        public List<SelectListItem> SelectListIvrCode { get; set; } = [];
        /// <summary>
        /// 廠商選單資料
        /// </summary>
        public List<SelectListItem> SelectListVender { get; set; } = [];
    }

    /// <summary>
    /// 派工規則維護
    /// </summary>
    public class DispatchRuleMgtVO
    {
        /// <summary>
        /// 編號
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 保固
        /// </summary>
        public string? IfWarrant { get; set; }
        /// <summary>
        /// IVR CODE
        /// </summary>
        public List<string> IvrCodeList { get; set; } = [];
        /// <summary>
        /// 報修廠商
        /// </summary>
        public List<string> VenderIdList { get; set; } = [];
        /// <summary>
        /// 類別ID
        /// </summary>
        public List<string> CisIdList { get; set; } = [];
    }

    /// <summary>
    /// 派工規則查詢
    /// </summary>
    public class DispatchRuleMgtQueryVO
    {
        /// <summary>
        /// 報修類別
        /// </summary>
        public string? CategoryIdFilter { get; set; }
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
        /// 是否保固
        /// </summary>
        public string? IfWarrantEq { get; set; }
    }

    /// <summary>
    /// 派工規則查詢
    /// </summary>
    public class DispatchRuleMgtQueryGridVO
    {
        /// <summary>
        /// IVR CODE
        /// </summary>
        [SortColumn(nameof(VDispatchQueryDTO.ivr_code),IsDefault = true)]
        public string? IvrCode { get; set; }
        /// <summary>
        /// 報修門市
        /// </summary>
        [SortColumn(nameof(VDispatchQueryDTO.shop_name))]
        public string? ShopName { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        [SortColumn(nameof(VDispatchQueryDTO.l1_desc))]
        public string? L1Desc { get; set; }
        /// <summary>
        /// 報修次類別
        /// </summary>
        [SortColumn(nameof(VDispatchQueryDTO.l2_desc))]
        public string? L2Desc { get; set; }
        /// <summary>
        /// 報修名稱
        /// </summary>
        [SortColumn(nameof(VDispatchQueryDTO.ciname))]
        public string? CiName { get; set; }
        /// <summary>
        /// 保固內廠商
        /// </summary>
        [SortColumn(nameof(VDispatchQueryDTO.warrant))]
        public string? Warrant { get; set; }
        /// <summary>
        /// 保固外廠商
        /// </summary>
        [SortColumn(nameof(VDispatchQueryDTO.nonwarrant))]
        public string? NonWarrant { get; set; }
    }
}
