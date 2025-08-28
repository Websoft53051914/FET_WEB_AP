using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.DB.Component
{
    /// <summary>
    /// 批次SQL容器
    /// </summary>
    public class BatchSqlContainer
    {
        /// <summary>
        /// SQL字串
        /// </summary>
        string _SQL;

        /// <summary>
        /// 條件參數
        /// </summary>
        Dictionary<string, object> _para;

        /// <summary>
        /// 設定SQL、參數
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="para"></param>
        public BatchSqlContainer(string SQL, Dictionary<string, object> para)
        {
            this._SQL = SQL;
            this._para = para;
        }

        /// <summary>
        /// 傳回SQL
        /// </summary>
        public string SqlStatement
        {
            get { return _SQL; }
        }

        /// <summary>
        /// 傳回參數
        /// </summary>
        public Dictionary<string, object> Parameter
        {
            get { return _para; }
        }
    }
}
