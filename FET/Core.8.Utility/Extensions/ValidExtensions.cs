using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Extensions
{
    /// <summary>
    /// 【驗證、檢查】
    /// </summary>
    public static class ValidExtensions
    {
        /// <summary>
        /// 字串是否為null或空字串
        /// </summary>
        /// <param name="value">要驗證的字串</param>
        /// <returns>是/否</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// List是否為null或沒有元素
        /// </summary>
        /// <typeparam name="T">List的Class Model</typeparam>
        /// <param name="value">要驗證的List</param>
        /// <returns>是/否</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value == null || !value.Any();
        }

        /// <summary>
        /// 驗證字串是否有值
        /// </summary>
        /// <param name="value">要驗證的字串</param>
        /// <returns>字串是否有值</returns>
        public static bool WithText(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 驗證集合是否有元素
        /// </summary>
        /// <typeparam name="T">集合的元素型別</typeparam>
        /// <param name="value">要驗證的集合</param>
        /// <returns>集合是否有元素</returns>
        public static bool WithData<T>(this IEnumerable<T> value)
        {
            return value != null && value.Any();
        }

        /// <summary>
        /// 如果該物件為null，回傳空白物件，避免foreach出錯
        /// </summary>
        /// <typeparam name="T">集合的元素型別</typeparam>
        /// <param name="source">要驗證的集合</param>
        /// <returns>集合</returns>
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        // TODO
        /// <summary>
        /// 取得Lambda的屬性名稱
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> expression) where TModel : class
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        /// <summary>
        /// 驗證字串是否在集合或一系列的字串清單param中
        /// </summary>
        /// <param name="source">要驗證的字串</param>
        /// <param name="list">集合或一系列的字串清單param</param>
        /// <returns>是/否</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool In(this string source, params string[] list)
        {
            if (null == source) throw new ArgumentNullException("Extension source不可為空");
            return list.Contains(source, StringComparer.Ordinal);
        }

        /// <summary>
        /// 驗證日期時間是否在給定的範圍中，任一邊皆可為空
        /// </summary>
        /// <param name="value">要驗證的日期時間</param>
        /// <param name="start">開始時間</param>
        /// <param name="end">結束時間</param>
        /// <returns>是/否</returns>
        public static bool IsBetween(this DateTime value, DateTime? start, DateTime? end)
        {
            return (!start.HasValue || value.Date >= start.Value) && (!end.HasValue || value.Date <= end.Value);
        }

        // TODO
        /// <summary>
        /// 若判斷式為真，增加查詢條件
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">要加入查詢條件的集合</param>
        /// <param name="condition">判斷式</param>
        /// <param name="expression">查詢條件lambda運算式</param>
        /// <returns></returns>
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            return source;
        }
    }
}