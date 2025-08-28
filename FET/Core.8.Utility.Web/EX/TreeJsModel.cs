using System.Text.Json.Serialization;

namespace Core.Utility.Web.EX
{
    // TODO
    /// <summary>
    /// 
    /// </summary>
    public class TreeJsStatus
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("opened")]
        public bool Opened { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("selected")]
        public bool Selected { get; set; }
    }
    // TODO
    /// <summary>
    /// 
    /// </summary>
    public class TreeJsFlatModel
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("parent")]
        public string Parent { get; set; }
        /// <summary>
        /// /
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        /// <summary>
        /// /
        /// </summary>
        [JsonPropertyName("status")]
        public TreeJsStatus Status { get; set; }
        /// <summary>
        /// /
        /// </summary>
        [JsonPropertyName("a_attr")]
        public object AAttr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("children")]
        public bool Children { get; set; } = false;

        /// <summary>
        /// 其他屬性
        /// </summary>
        public Dictionary<string, string> OtherAttr { get; set; } = [];
    }
}
