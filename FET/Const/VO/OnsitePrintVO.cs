namespace Const.ViewModel
{
    public class OnsitePrintVO
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
        /// 報修類別
        /// </summary>
        public string? L2Desc { get; set; }
        /// <summary>
        /// 報修品項
        /// </summary>
        public string? CiName { get; set; }
        /// <summary>
        /// 報修日期
        /// </summary>
        public string? CreateTimeText { get; set; }
        /// <summary>
        /// 廠商
        /// </summary>
        public string? Vender { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        public string? StatusName { get; set; }
        /// <summary>
        /// 廠商到門市日期
        /// </summary>
        public string? VendorArriveDateText { get; set; }
        /// <summary>
        /// 派工日期
        /// </summary>
        public string? AssignDateText { get; set; }
        /// <summary>
        /// 到場最小日期
        /// </summary>
        public string? LimitDateText { get; set; }
        /// <summary>
        /// 廠商到門市日期
        /// </summary>
        public DateTime? VendorArriveDate { get; set; }
        /// <summary>
        /// ivrcode
        /// </summary>
        public string? Ivrcode { get; set; }
    }

    public class OnsitePrintUpdateStatusReqVO
    {
        public List<OnsitePrintVO> DataList { get; set; } = [];
        public List<int> FormNoList { get; set; } = [];
    }
}
