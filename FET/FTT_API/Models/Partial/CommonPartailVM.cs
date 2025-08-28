namespace FTT_API.Models.Partial
{
    public class CommonPartailVM
    {
        public string ComponentId { get; set; }

        /// <summary>
        /// 是否必填，0為否，1為是
        /// </summary>
        public int required { get; set; }
        /// <summary>
        /// 屬性
        /// 設定後建立以此為 name 的 hidden input ，選擇後自動寫入 value
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;
        public string Id { get; set; }
    }
}
