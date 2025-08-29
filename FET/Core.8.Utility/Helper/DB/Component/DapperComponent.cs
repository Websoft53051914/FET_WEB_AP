using Dapper;
using EnterpriseDB.EDBClient;
using Npgsql;
using System.Data;

namespace Core.Utility.Helper.DB.Component
{
    /// <summary>
    /// Dapper元件
    /// </summary>
    public class DapperComponent : IDBComoponent
    {
        /// <summary>
        /// 連線字串
        /// </summary>
        string _connectionString = null;

        /// <summary>
        /// 建構時設定連線字串
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        public DapperComponent(string connectionString)
        {
            this._connectionString = connectionString;
        }

        #region "連線測試,《測試使用》"
        /// <summary>
        /// 測試用，看連線是否存在
        /// </summary>
        /// <returns>是/否</returns>
        public bool IsConnection()
        {
            bool isConnection = false;
            try
            {
                using IDbConnection sqlConn = new EDBConnection(_connectionString);
                sqlConn.Open();
                isConnection = true;

            }
            catch (Exception)
            {
                return isConnection;
            }

            return isConnection;
        }
        #endregion

        #region "查詢"

        /// <summary>
        /// 依條件查詢多筆Dictionary資料
        /// </summary>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳多筆Dictionary資料</returns>
        public List<Dictionary<string, object>> FindToDictionAry(string _SQLScript, Dictionary<string, object> paras = null)
        {
            List<Dictionary<string, object>> rtnList = new();
            using (EDBConnection _NpgsqlConnection = new(_connectionString))
            {

                using EDBCommand _NpgsqlCommand = new(_SQLScript, _NpgsqlConnection);
                _NpgsqlCommand.CommandType = CommandType.Text;
                if (paras != null && paras.Count > 0)
                {
                    foreach (KeyValuePair<string, object> p in paras)
                    {
                        _NpgsqlCommand.Parameters.AddWithValue(p.Key, p.Value);
                    }
                }
                _NpgsqlConnection.Open();
                using EDBDataReader reader = _NpgsqlCommand.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
                while (reader.Read())
                {
                    Dictionary<string, object> map = new();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i);
                        object value = reader.GetValue(i);
                        map.Add(name, value);
                    }
                    rtnList.Add(map);
                }
            }
            return rtnList;
        }

        /// <summary>
        /// 依條件查詢多筆Dictionary資料
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳多筆 Entity 資料</returns>
        public List<T> FindToList<T>(string _SQLScript, Dictionary<string, object> paras = null)
        {
            using IDbConnection _NpgNpgsqlConnection = new EDBConnection(_connectionString);
            _NpgNpgsqlConnection.Open();
            return _NpgNpgsqlConnection.Query<T>(_SQLScript, paras).ToList();
        }


        /// <summary>
        /// 依條件查詢單筆資料
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">SQL類型</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳指定Entity</returns>
        public T GetEntityBySQLScript<T>(string _SQLScript, CommandType _CommandType, Dictionary<string, object> paras = null)
        {
            using IDbConnection _NpgNpgsqlConnection = new EDBConnection(_connectionString);
            _NpgNpgsqlConnection.Open();
            return _NpgNpgsqlConnection.Query<T>(_SQLScript, paras).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 依條件查詢多筆資料
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">SQL類型</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳指定Entity</returns>
        public List<T> GetEntitiesBySQLScript<T>(string _SQLScript, CommandType _CommandType, Dictionary<string, object> paras = null)
        {
            using IDbConnection _NpgsqlConnection = new EDBConnection(_connectionString);
            _NpgsqlConnection.Open();
            return _NpgsqlConnection.Query<T>(_SQLScript, paras).ToList();
        }

        /// <summary>
        /// 依條件查詢第一筆第一個欄位資料
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">SQL類型</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳指定Entity</returns>
        public T GetScalarBySQLScript<T>(string _SQLScript, CommandType _CommandType, Dictionary<string, object> paras = null)
        {
            using IDbConnection _NpgsqlConnection = new EDBConnection(_connectionString);
            _NpgsqlConnection.Open();
            return _NpgsqlConnection.ExecuteScalar<T>(_SQLScript, paras);
        }
        #endregion

        #region "Private"
        //private static T ToEntity<T>(NpgsqlDataReader reader)
        //{
        //    T entity = (T)Activator.CreateInstance(typeof(T));
        //    for (int i = 0; i < reader.FieldCount; i++)
        //    {
        //        PropertyInfo property = entity.GetType().GetProperty(reader.GetName(i));

        //        if (property != null)
        //        {

        //            Type propType = property.PropertyType;
        //            //如果有nullable的情況沒這段時型別設成nullable會拋exception
        //            if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        //            {
        //                propType = Nullable.GetUnderlyingType(propType);
        //            }


        //            ITypeConverter typeConverter = TypeConverterFactory.GetConvertType(propType);

        //            object value = typeConverter.Convert(reader.GetValue(i));
        //            object safeValue = Convert.ChangeType(value, propType);


        //            property.SetValue(entity, (reader.IsDBNull(i)) ? null : safeValue);
        //        }
        //    }

        //    return entity;
        //}
        #endregion

        #region "StoredProcedure 處理"
        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        public int ExecStoredProcedureWithTransation(string _StoredProcedureName, Dictionary<string, object> paras = null)
        {
            using EDBConnection _objNpgsqlConnection = new(_connectionString);
            _objNpgsqlConnection.Open();
            IDbTransaction tran = _objNpgsqlConnection.BeginTransaction();
            int _ExecResult = -1;
            using EDBCommand _NpgsqlCommand = new(_StoredProcedureName, _objNpgsqlConnection);
            _NpgsqlCommand.CommandType = CommandType.StoredProcedure;
            if (paras != null && paras.Count > 0)
            {
                foreach (KeyValuePair<string, object> p in paras)
                {
                    _NpgsqlCommand.Parameters.AddWithValue(p.Key, p.Value);
                }
            }
            try
            {
                tran.Commit();                              // transaction complete
                _ExecResult = _NpgsqlCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                tran.Rollback();                            // transaction failed
                _ExecResult = -9999; //Exception Error
                throw;
            }
            return _ExecResult;
        }

        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        public int ExecStoredProcedureWithoutTransation(string _StoredProcedureName, Dictionary<string, object> paras = null)
        {
            using EDBConnection _objNpgsqlConnection = new(_connectionString);
            _objNpgsqlConnection.Open();
            using EDBCommand _NpgsqlCommand = new(_StoredProcedureName, _objNpgsqlConnection);
            _NpgsqlCommand.CommandType = CommandType.StoredProcedure;
            if (paras != null && paras.Count > 0)
            {
                foreach (KeyValuePair<string, object> p in paras)
                {
                    _NpgsqlCommand.Parameters.AddWithValue(p.Key, p.Value);
                }
            }
            return _NpgsqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        public object GetExecuteScalarByExecStoredProcedureWithTransation(string _StoredProcedureName, Dictionary<string, object> paras = null)
        {
            using EDBConnection _objNpgsqlConnection = new(_connectionString);
            _objNpgsqlConnection.Open();
            EDBTransaction tran = _objNpgsqlConnection.BeginTransaction();
            object _ExecResult;
            using EDBCommand _NpgsqlCommand = new(_StoredProcedureName, _objNpgsqlConnection);
            _NpgsqlCommand.CommandType = CommandType.StoredProcedure;

            if (paras != null && paras.Count > 0)
            {
                foreach (KeyValuePair<string, object> p in paras)
                {
                    _NpgsqlCommand.Parameters.AddWithValue(p.Key, p.Value);
                }
            }
            try
            {
                tran.Commit();                              // transaction complete
                _ExecResult = _NpgsqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {
                tran.Rollback();                            // transaction failed
                _ExecResult = null; //Exception Error
                throw;
            }
            return _ExecResult;
        }

        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        public object GetExecuteScalarByExecStoredProcedureWithoutTransation(string _StoredProcedureName, Dictionary<string, object> paras = null)
        {
            using EDBConnection _objNpgsqlConnection = new(_connectionString);
            using EDBCommand _NpgsqlCommand = new(_StoredProcedureName, _objNpgsqlConnection);
            _NpgsqlCommand.CommandType = CommandType.StoredProcedure;
            if (paras != null && paras.Count > 0)
            {
                foreach (KeyValuePair<string, object> p in paras)
                {
                    _NpgsqlCommand.Parameters.AddWithValue(p.Key, p.Value);
                }
            }
            return _NpgsqlCommand.ExecuteScalar();
        }
        #endregion

        #region "Batch Execute"

        /// <summary>
        /// 批次執行
        /// </summary>
        /// <param name="sqlList">SQL清單</param>
        public void BatchExecute(List<BatchSqlContainer> sqlList)
        {
            if (sqlList.Count == 0)
            {
                return;
            }

            if (sqlList.Count == 1)
            {
                BatchSqlContainer SqlContainer = sqlList[0];
                this.Execute(SqlContainer.SqlStatement, SqlContainer.Parameter);
                return;
            }

            using IDbConnection conn = new EDBConnection(_connectionString);
            conn.Open();

            using IDbCommand cmd = conn.CreateCommand();
            IDbTransaction trx = conn.BeginTransaction();
            cmd.Transaction = trx;

            try
            {
                foreach (BatchSqlContainer sqlEntity in sqlList)
                {
                    cmd.CommandText = sqlEntity.SqlStatement;
                    cmd.Parameters.Clear();

                    if (sqlEntity.Parameter != null)
                        foreach (KeyValuePair<string, object> p in sqlEntity.Parameter)
                        {
                            IDbDataParameter para = cmd.CreateParameter();
                            para.ParameterName = p.Key;
                            para.Value = p.Value ?? (object)DBNull.Value;

                            cmd.Parameters.Add(para);
                        }

                    cmd.ExecuteNonQuery();

                }

                trx.Commit();
            }
            catch (Exception)
            {
                trx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// SQL執行
        /// </summary>
        /// <param name="executeSQL">SQL</param>
        /// <param name="param">參數</param>
        public void Execute(string executeSQL, Dictionary<string, object> param)
        {
            using IDbConnection conn = new EDBConnection(_connectionString);
            conn.Open();

            conn.Execute(executeSQL, param);
        }

        /// <summary>
        /// 取得指定SQL內容轉換成DataTable
        /// </summary>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">SQL類型</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳DataTable</returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataTable GetDataTableBySQLScript(string _SQLScript, CommandType _CommandType, Dictionary<string, object> paras = null)
        {
            using IDbConnection _NpgNpgsqlConnection = new EDBConnection(_connectionString);
            _NpgNpgsqlConnection.Open();
            IDataReader reader = _NpgNpgsqlConnection.ExecuteReader(_SQLScript, paras);

            DataTable result = new();
            result.Load(reader);

            return result;
        }
        #endregion
    }
}
