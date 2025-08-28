using FTT_API.Common.OriginClass.EntiityClass;

namespace FTT_API.Models.ViewModel
{
    public class StoreVM
    {
        public StoreVM()
        {

        }

        public StoreVM(StoreDTO dto)
        {
            Company = dto.COMPANY_LEAVES;
            StoreType = dto.STORE_TYPE;
            Channel = dto.CHANNEL;
            Area = dto.AREA;
            StoreName = dto.SHOP_NAME;
            EMail = dto.EMAIL;
            Owner = dto.OWNER_CNAME;
            Manager = dto.AS_CNAME;
            ManagerEmpno = dto.AS_EMPNO;
            Phone = dto.OWNER_TEL;
            PhoneUrgent = dto.URGENT_TEL;
            PhoneFax = dto.FAX_TEL;
            Address = dto.ADDRESS;
            BusinessTime1 = dto.BUSINESS_HOUR_RANGE1;
            BusinessTime2 = dto.BUSINESS_HOUR_RANGE2;
            BusinessTime3 = dto.BUSINESS_HOUR_RANGE3;
            BusinessTime4 = dto.BUSINESS_HOUR_RANGE4;
            DecorationCondition = dto.DECORATION_CONDITION;
            Note = dto.NOTE;
            ApprovalDate = dto.APPROVAL_DATE;
            IVRCode = dto.IVR_CODE;
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
    public class CiDataVM
    {
        public int CATEGORY_ID { get; set; }
        public string? CATEGORY_NAME_TMP { get; set; }
        public string? CATEGORY_NAME { get; set; }
        public string? TT_CATEGORY_NOTE { get; set; }
        public string? TT_CATEGORY_DESC { get; set; }
        public string? TT_IMAGE { get; set; }
    }
}
