namespace Core.Utility.Web.EX
{
    //
    // 摘要:
    //     Provides information about paging, sorting, filtering and grouping of data.
    public class DataSourceRequest
    {
        //
        // 摘要:
        //     The current page.
        public int pageIndex { get; set; }
        //
        // 摘要:
        //     The page size.
        public int pageSize { get; set; }
        /// <summary>
        /// 排序欄位
        /// </summary>
        public string? SortField { get; set; }
        /// <summary>
        /// 排序方向(asc/desc)
        /// </summary>
        public string? SortOrder { get; set; }

        /// <summary>
        /// 取得起始
        /// </summary>
        /// <returns>傳回INT</returns>
        public int GetStartPos()
        {
            return (this.pageIndex - 1) * this.pageSize + 1;
        }
    }
}
