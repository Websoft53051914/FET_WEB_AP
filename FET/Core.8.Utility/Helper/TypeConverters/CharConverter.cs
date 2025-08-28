
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.TypeConverters
{
    public class CharConverter : ITypeConverter
    {
        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <returns>回轉char型態</returns>
        public object Convert(object ValueToConvert)
        {
            return Convert(ValueToConvert, (char)0x0);
        }

        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>回轉char型態</returns>
        public object Convert(object ValueToConvert, object defaultValue)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return defaultValue;

            return System.Convert.ToChar(ValueToConvert);
        }

    }
}
