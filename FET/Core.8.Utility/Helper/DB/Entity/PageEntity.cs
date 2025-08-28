namespace Core.Utility.Helper.DB.Entity
{
    /// <summary>
    /// 分頁Entity
    /// </summary>
    public class PageEntity
    {
        /// <summary>
        /// 建構子預設初始每頁筆數
        /// </summary>
        public PageEntity()
        {
            this.PageDataSize = 10;
        }


        /// <summary>
        /// 當下頁數
        /// </summary>
        public int CurrentPage { set; get; }
        /// <summary>
        /// 頁數資料數量
        /// </summary>
        public int PageDataSize { set; get; }
        /// <summary>
        /// 排序欄位
        /// </summary>
        public string Sort { set; get; }
        /// <summary>
        /// 正序 倒序
        /// </summary>
        public string Asc { set; get; }
    }
}
