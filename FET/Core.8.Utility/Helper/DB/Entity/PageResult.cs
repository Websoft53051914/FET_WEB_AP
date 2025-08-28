using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.DB.Entity
{
    /// <summary>
    /// 分頁使用
    /// </summary>
    /// <typeparam name="T">指定Entity</typeparam>
    public class PageResult<T>
    {
        /// <summary>
        /// 查詢結果
        /// </summary>
        public List<T> Results { get; set; }
        /// <summary>
        /// 目前頁數
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 資料總數
        /// </summary>
        public int DataCount { get; set; }
        /// <summary>
        /// 一頁資料量
        /// </summary>
        public int PageDataSize { get; set; }
        /// <summary>
        /// 總頁數
        /// </summary>
        public int PageCount
        {
            get
            {
                int dataMod = DataCount % PageDataSize;
                int page = DataCount / PageDataSize;
                return page + (dataMod > 0 ? 1 : 0);
            }
        }
    }
}
