namespace Const.DTO
{
    public class VFttForm2DTO
    {
        #region -- 資料庫欄位 --
        /// <summary>
        /// 工單號碼
        /// </summary>
        public int? form_no { get; set; }
        /// <summary>
        /// ivrcode
        /// </summary>
        public string? ivrcode { get; set; }
        /// <summary>
        /// 報修型態
        /// </summary>
        public string? tt_category { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        public string? l2_desc { get; set; }
        /// <summary>
        /// 報修品項
        /// </summary>
        public string? ciname { get; set; }
        /// <summary>
        /// 廠商
        /// </summary>
        public string? vender { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        public string? statusname { get; set; }

        #endregion -- 資料庫欄位 --
        /// <summary>
        /// 報修日期
        /// </summary>
        public string? createtime_text { get; set; }
        /// <summary>
        /// 廠商到門市日期
        /// </summary>
        public string? vendor_arrive_date_text { get; set; }
        /// <summary>
        /// 派工日期
        /// </summary>
        public string? assign_date_text { get; set; }
        /// <summary>
        /// 到場最小日期
        /// </summary>
        public string? limit_date_text { get; set; }
    }
}
