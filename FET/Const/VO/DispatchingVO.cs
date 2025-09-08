namespace Const.VO
{
    public class DispatchingIndexVO
    {

    }

    public class DispatchingGridVO
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
        /// 工單狀態
        /// </summary>
        public string? StatusName { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public string? UpdateTimeText { get; set; }
    }
}
