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
        public string? l1_desc { get; set; }
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
        public int? vender_id { get; set; }
        /// <summary>
        /// 廠商
        /// </summary>
        public string? vender { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        public string? statusname { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        public string? statusid { get; set; }
        /// <summary>
        /// 報修門市
        /// </summary>
        public string? shop_name { get; set; }
        /// <summary>
        /// 報修說明
        /// </summary>
        public string? descr { get; set; }
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
        /// 區經理/業務
        /// </summary>
        public string? as_empno { get; set; }
        /// <summary>
        /// 門市自行尋商
        /// </summary>
        public string? selfconfig { get; set; }
        /// <summary>
        /// 報修日期
        /// </summary>
        public DateTime? createtime { get; set; }
        /// <summary>
        /// 完修日
        /// </summary>
        public DateTime? completetime { get; set; }
        /// <summary>
        /// 結案日
        /// </summary>
        public DateTime? closedate { get; set; }
        /// <summary>
        /// 區經理/業務
        /// </summary>
        public string? as_cname { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string? remark { get; set; }
        /// <summary>
        /// 備註說明
        /// </summary>
        public string? description { get; set; }
        /// <summary>
        /// 一個月內覆修
        /// </summary>
        public string? repair { get; set; }
        /// <summary>
        /// 補單
        /// </summary>
        public string? resupply { get; set; }
        /// <summary>
        /// 檢測故障原因
        /// </summary>
        public string? fault_reason { get; set; }
        /// <summary>
        /// 維修處理動作
        /// </summary>
        public string? repair_action { get; set; }
        /// <summary>
        /// 費用種類
        /// </summary>
        public string? expense_type { get; set; }
        /// <summary>
        /// 維修細項
        /// </summary>
        public string? expense_desc { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public string? qty { get; set; }
        /// <summary>
        /// 單位
        /// </summary>
        public string? unit { get; set; }
        /// <summary>
        /// 金額
        /// </summary>
        public string? price { get; set; }
        /// <summary>
        /// 小計
        /// </summary>
        public string? subtotal { get; set; }
        /// <summary>
        /// 計算派工天數
        /// </summary>
        public string? dispatch_days { get; set; }
        /// <summary>
        /// KPI Day
        /// </summary>
        public string? kpi_days { get; set; }
        /// <summary>
        /// KPI Result
        /// </summary>
        public string? kpi_result { get; set; }
        /// <summary>
        /// 延遲原因
        /// </summary>
        public string? delay_reason { get; set; }
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
        /// <summary>
        /// 派單日期
        /// </summary>
        public string? dispatchtime_text { get; set; }
        /// <summary>
        /// 保固期日期
        /// </summary>
        public string? approval_date_text { get; set; }
        /// <summary>
        /// 結案日期
        /// </summary>
        public string? closedate_text { get; set; }
        /// <summary>
        /// 完修日期
        /// </summary>
        public string? completetime_text { get; set; }
        /// <summary>
        /// 預計完修日
        /// </summary>
        public string? precompletetime_text { get; set; }
        /// <summary>
        /// 自行尋商日期
        /// </summary>
        public string? usedtime_text { get; set; }
        /// <summary>
        /// 已派工
        /// </summary>
        public string? tickettime_text { get; set; }
        /// <summary>
        /// 確認到場日期
        /// </summary>
        public string? confirmtime_text { get; set; }
        /// <summary>
        /// 處理者
        /// </summary>
        public string? processer { get; set; }

        /// <summary>
        /// 報修日
        /// </summary>
        public DateTime? CreateDateGte { get; set; }
        /// <summary>
        /// 報修日
        /// </summary>
        public DateTime? CreateDateLt { get; set; }
        /// <summary>
        /// 完修日
        /// </summary>
        public DateTime? CompleteDateGte { get; set; }
        /// <summary>
        /// 完修日
        /// </summary>
        public DateTime? CompleteDateLt { get; set; }
        /// <summary>
        /// 結案日
        /// </summary>
        public DateTime? CloseDateGte { get; set; }
        /// <summary>
        /// 結案日
        /// </summary>
        public DateTime? CloseDateLt { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        public string? StatusIdEq { get; set; }
        /// <summary>
        /// 工單號碼
        /// </summary>
        public int? FormNoEq { get; set; }
        /// <summary>
        /// 報修型態
        /// </summary>
        public string? TtCategoryEq { get; set; }
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
        /// 區經理/業務
        /// </summary>
        public string? AsEmpNoEq { get; set; }
        /// <summary>
        /// 門市自行尋商
        /// </summary>
        public string? SelfConfigEq { get; set; }
        /// <summary>
        /// 角色為 Vender 時的特殊條件
        /// </summary>
        public bool UserRoleVenderFilter { get; set; } = false;
        /// <summary>
        /// 角色為其他時的特殊條件
        /// </summary>
        public bool UserRoleOtherFilter { get; set; } = false;
    }
}
