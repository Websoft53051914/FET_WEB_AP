namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class CIRelationsDTO
    {
        #region -- 資料庫欄位 --
        /// <summary>
        /// 
        /// </summary>
        public int cisid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ciname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? aciname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? cicategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? fullname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? notes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? descr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? parentsid { get; set; }
        #endregion -- 資料庫欄位 --
        /// <summary>
        /// 階層路徑
        /// </summary>
        public string? path_csv { get; set; }
        /// <summary>
        /// 是否有子節點
        /// </summary>
        public bool HasChildren { get; set; } 
    }
}
