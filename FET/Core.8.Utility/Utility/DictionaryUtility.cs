using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utility.Utility
{
    public static class DictionaryUtility
    {
        // TODO
        /// <summary>
        /// Gets the value from the key in dictionary.If the key is not exist,then return default value
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict">the dictionary</param>
        /// <param name="key">the key</param>
        /// <param name="dflVal">default value</param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dict, TKey key,
            TValue dflVal = default)
        {
            if (dict == null || key == null || !dict.ContainsKey(key))
            {
                return dflVal;
            }

            return dict[key];
        }

        // TODO
        /// <summary>
        /// 反轉字典(Value當主鍵)
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<TValue, List<TKey>> Inverted<TKey, TValue>(
            this Dictionary<TKey, TValue> source)
        {
            if (source == null)
            {
                ArgumentNullException argumentNullException = new ArgumentNullException("字典不可為空");
                throw argumentNullException;
            }
            return source
                .GroupBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Key).ToList());
        }
    }
}
