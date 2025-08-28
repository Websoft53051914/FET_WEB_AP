using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.TypeConverters
{
    public class DecimalConverter : ITypeConverter
    {
        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <returns>回傳Decimal</returns>
        public object Convert(object ValueToConvert)
        {
            return Convert(ValueToConvert, 0.0m);
        }

        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>回傳Decimal</returns>
        public object Convert(object ValueToConvert, object defaultValue)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return defaultValue;

            return System.Convert.ToDecimal(ValueToConvert);
        }
    }
}
