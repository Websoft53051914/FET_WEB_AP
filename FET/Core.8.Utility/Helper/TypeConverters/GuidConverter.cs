using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.TypeConverters
{
    public class GuidConverter : ITypeConverter
    {
        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <returns>回傳GUID</returns>
        public object Convert(object ValueToConvert)
        {
            return Convert(ValueToConvert, Guid.Empty);
        }

        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>回傳GUID</returns>
        public object Convert(object ValueToConvert, object defaultValue)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return defaultValue;

            return Guid.Parse(ValueToConvert.ToString());
        }
    }

    // TODO
    /// <summary>
    /// 
    /// </summary>
    public class MySqlGuidTypeHandler : Dapper.SqlMapper.TypeHandler<Guid>
    {
        // TODO
        /// <summary>
        /// 設定值
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="guid"></param>
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        // TODO
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Guid Parse(object value)
        {
            return new Guid((string)value);
        }
    }
}
