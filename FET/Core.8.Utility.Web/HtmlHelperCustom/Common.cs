using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Core.Utility.Web.HtmlHelperCustom
{
    /// <summary>
    /// RAZOR 靜態共用方法
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 將任意物件轉成Json且以Raw方式輸出，通常是為了讓JS變數使用
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper</param>
        /// <param name="obj">任意物件</param>
        /// <returns>回傳IHtmlContent</returns>
        public static IHtmlContent ToJson(this HtmlHelper htmlHelper, object obj)
        {
            return htmlHelper.Raw(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 取得RouteData內指定key的值
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper</param>
        /// <param name="key">指定key</param>
        /// <returns>傳回string</returns>
        private static string RouteData(this IHtmlHelper htmlHelper, string key)
        {
            return htmlHelper.ViewContext.RouteData.Values[key].ToString();
        }

        /// <summary>
        /// 取得 ControllerName
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper</param>
        /// <returns>傳回string</returns>
        public static string ControllerName(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.RouteData("controller");
        }

        /// <summary>
        /// 取得 ActionName
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper</param>
        /// <returns>傳回string</returns>
        public static string ActionName(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.RouteData("action");
        }

        /// <summary>
        /// 取得 AreaName
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper</param>
        /// <returns>傳回string</returns>
        public static string AreaName(this IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.RouteData.DataTokens["area"] as string;
        }

        /// <summary>
        /// 將字串轉變為 HtmlString
        /// </summary>
        /// <param name="str">當下字串</param>
        /// <returns>傳回HtmlString</returns>
        public static HtmlString ToHtmlString(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            HtmlString htmlStr = new(str);
            return htmlStr;
        }

        /// <summary>
        /// 產生完整的 URL<para/>
        /// https://stackoverflow.com/questions/30755827/getting-absolute-urls-using-asp-net-core
        /// <para/>
        /// 範例：<para/>
        /// Url.AbsoluteAction("ActionName", "ControllerName", new { id = 1 })<para/>
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">Action 名稱</param>
        /// <param name="controllerName">Controller 名稱</param>
        /// <param name="routeValues">路徑參數</param>
        /// <returns>完整 URL</returns>
        public static string AbsoluteAction(
            this IUrlHelper url,
            string actionName,
            string controllerName,
            object? routeValues = null)
        {
            return url.Action(actionName, controllerName, routeValues, url.ActionContext.HttpContext.Request.Scheme);
        }
    }
}
