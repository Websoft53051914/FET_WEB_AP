using Const.DTO;

namespace Const.VO
{
    public class CommonPartialViewVO
    {
        /// <summary>
        /// 元件 ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// 表單欄位名稱
        /// </summary>
        public string? PropertyName { get; set; }
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
    /// <summary>
    /// _ItemsTreeMuti.cshtml
    /// </summary>
    public class ItemTreeMultiVO : CommonPartialViewVO
    {

    }
    /// <summary>
    /// _ItemsTree.cshtml
    /// </summary>
    public class ItemsTreeVO : CommonPartialViewVO
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public class StoreVO
    {
        public StoreVO()
        {

        }

        public StoreVO(StoreProfileDTO dto)
        {
            Company = dto.company_leaves ?? string.Empty;
            //20260325與FTT Admin討論後不顯示 StoreType = dto.store_type ?? string.Empty;
            Channel = dto.channel ?? string.Empty;
            Area = dto.area ?? string.Empty;
            StoreName = dto.shop_name ?? string.Empty;
            EMail = dto.email ?? string.Empty;
            //Owner = dto.owner_name ?? string.Empty;
            Owner = dto.owner_cname ?? string.Empty;
            Manager = dto.as_cname ?? string.Empty;
            ManagerEmpno = dto.as_empno ?? string.Empty;
            //20260325與FTT Admin討論後不顯示Phone = dto.owner_tel ?? string.Empty;
            PhoneUrgent = dto.urgent_tel ?? string.Empty;
            PhoneFax = dto.fax_tel ?? string.Empty;
            Address = dto.address ?? string.Empty;
            BusinessTime1 = dto.business_hour_range1 ?? string.Empty;
            BusinessTime2 = dto.business_hour_range2 ?? string.Empty;
            BusinessTime3 = dto.business_hour_range3 ?? string.Empty;
            BusinessTime4 = dto.business_hour_range4 ?? string.Empty;
            //20260325與FTT Admin討論後不顯示DecorationCondition = dto.decoration_condition ?? string.Empty;
            Note = dto.note ?? string.Empty;
            ApprovalDate = dto.approval_date?.ToString(DbConst.FORMAT_DATE2) ?? string.Empty;
            IVRCode = dto.ivr_code ?? string.Empty;
        }

        /// <summary>
        /// 公司別
        /// </summary>
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// 門市類型
        /// </summary>
        public string StoreType { get; set; } = string.Empty;

        /// <summary>
        /// 通路
        /// </summary>
        public string Channel { get; set; } = string.Empty;

        /// <summary>
        /// 區域
        /// </summary>
        public string Area { get; set; } = string.Empty;

        /// <summary>
        /// 店名
        /// </summary>
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// IVR Code
        /// </summary>
        public string IVRCode { get; set; } = string.Empty;

        /// <summary>
        /// eMail
        /// </summary>
        public string EMail { get; set; } = string.Empty;

        /// <summary>
        /// 店長
        /// </summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        /// 區經理
        /// </summary>
        public string Manager { get; set; } = string.Empty;

        /// <summary>
        /// 區經理員編
        /// </summary>
        public string ManagerEmpno { get; set; } = string.Empty;

        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 緊急聯絡電話
        /// </summary>
        public string PhoneUrgent { get; set; } = string.Empty;

        /// <summary>
        /// 傳真電話
        /// </summary>
        public string PhoneFax { get; set; } = string.Empty;

        /// <summary>
        /// 住址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 週一至週五
        /// </summary>
        public string BusinessTime1 { get; set; } = string.Empty;

        /// <summary>
        /// 週六
        /// </summary>
        public string BusinessTime2 { get; set; } = string.Empty;

        /// <summary>
        /// 週日
        /// </summary>
        public string BusinessTime3 { get; set; } = string.Empty;

        /// <summary>
        /// 國定假日
        /// </summary>
        public string BusinessTime4 { get; set; } = string.Empty;

        /// <summary>
        /// 裝潢型態
        /// </summary>
        public string DecorationCondition { get; set; } = string.Empty;

        /// <summary>
        /// 備註
        /// </summary>
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// 驗收日
        /// </summary>
        public string ApprovalDate { get; set; } = string.Empty;
    }
    /// <summary>
    /// 報修項目
    /// </summary>
    public class CiDataVO
    {
        public int CATEGORY_ID { get; set; }
        public string? CATEGORY_NAME_TMP { get; set; }
        public string? CATEGORY_NAME { get; set; }
        public string? TT_CATEGORY_NOTE { get; set; }
        public string? TT_CATEGORY_DESC { get; set; }
        public string? TT_IMAGE { get; set; }
    }
}
