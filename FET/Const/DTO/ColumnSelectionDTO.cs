namespace Const.DTO
{
    /// <summary>
    /// column_select
    /// </summary>
    public class ColumnSelectionDTO
    {
        #region -- 資料庫欄位 --
        /// <summary>
        /// columnname
        /// </summary>
        public string? columnname { get; set; }
        /// <summary>
        /// selectitem
        /// </summary>
        public string? selectitem { get; set; }
        /// <summary>
        /// selectindex
        /// </summary>
        public int? selectindex { get; set; }
        /// <summary>
        /// selectvalue
        /// </summary>
        public string? selectvalue { get; set; }
        #endregion -- 資料庫欄位 --
    }
}
