using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.TypeConverters
{
    public class EnumConverter : ITypeConverter
    {
        //TODO 未製作
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Convert(object ValueToConvert)
        {
            throw new NotImplementedException();
        }

        public object Convert(object ValueToConvert, object defaultValue)
        {
            throw new NotImplementedException();
        }

        public static object Convert(Type EnumType, object ValueToConvert)
        {
            if (!EnumType.IsEnum)
                throw new InvalidOperationException("ERROR_TYPE_IS_NOT_ENUMERATION");

            return System.Convert.ChangeType(Enum.Parse(EnumType, ValueToConvert.ToString()), EnumType);
        }
    }
}
