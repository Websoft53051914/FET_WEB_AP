using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.DB.TypeMapper.MapHandler
{
    public class GuidByteTypeHandler : Dapper.SqlMapper.TypeHandler<Guid>
    {
        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Guid Parse(object value)
        {
            var inVal = (byte[])value;
            return new Guid(inVal);
        }

        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public override void SetValue(System.Data.IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value;
        }
    }
}
