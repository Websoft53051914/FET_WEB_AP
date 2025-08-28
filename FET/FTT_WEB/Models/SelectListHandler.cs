
using Core.Utility.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using FTT_WEB.Common;

namespace FTT_WEB.Models
{
    public partial class SelectListHandler
    {


        /// <summary>
        /// 取得 enum 類別項目清單
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<SelectListItem> GetSelectListEnum<T>()
            where T : struct, System.Enum
        {
            return ConvertUtility.Enum2Dictionary<T>()
                .Select(x => new SelectListItem(x.Value, x.Key.ToString()))
                .ToList();
        }

    }

    public class SelectListItemCustom : SelectListItem
    {
        public SelectListItemCustom()
        {

        }

        public SelectListItemCustom(string text, string value) : base(text, value)
        {

        }

        public string? Data { get; set; }
        /// <summary>
        /// 上層值
        /// </summary>
        public string? ParentValue { get; set; }
        /// <summary>
        /// 自定義屬性
        /// </summary>
        public Dictionary<string, string> OtherAttr { get; set; } = new();
    }
}
