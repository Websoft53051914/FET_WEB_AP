using Core.Utility.Helper.DB.TypeMapper.MapHandler;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.DB.TypeMapper
{
    public class DapperAddMapper
    {
        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        public static void OracleGuid()
        {
            SqlMapper.AddTypeHandler<Guid>(new GuidByteTypeHandler());
        }
        // TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        public static void SQLiteGuid()
        {
            SqlMapper.AddTypeHandler<Guid>(new GuidByteTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));
        }
    }
}
