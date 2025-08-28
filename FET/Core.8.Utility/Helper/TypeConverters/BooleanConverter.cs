using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.TypeConverters
{
    public class BooleanConverter : ITypeConverter
    {
        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <returns>回轉bool型態</returns>
        public object Convert(object ValueToConvert)
        {
            return Convert(ValueToConvert, false);
        }

        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>回轉bool型態</returns>
        public object Convert(object ValueToConvert, object defaultValue)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return defaultValue;

            if (string.IsNullOrEmpty(ValueToConvert.ToString()))
                return false;
            else if (ValueToConvert.ToString() == "0")
                return false;
            else if (ValueToConvert.ToString() == "1")
                return true;
            else
                return System.Convert.ToBoolean(ValueToConvert);
        }
    }
}
