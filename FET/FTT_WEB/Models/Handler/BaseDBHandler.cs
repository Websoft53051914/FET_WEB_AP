using Core.Utility.Helper.DB;
using Core.Utility.Helper.Message;
using DocumentFormat.OpenXml.Presentation;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace FTT_WEB.Models.Handler
{
    public partial class BaseDBHandler
    {
        protected IDBHelper? dbHelper = null;
        public BaseDBHandler()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            var connectionString = config["ConnectionStrings:MainConnection"];
            this.dbHelper = new DBHelper((connectionString), Core.Utility.Helper.DB.Enums.DBTypeEnums.POSTGRESQL);
        }
        public BaseDBHandler(IDBHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public IDBHelper GetDBHelper()
        {
            return this.dbHelper;
        }

        public void Commit()
        {
            this.dbHelper.Commit();
        }

        private MessageHelper? _msgHelper = null;
        /// <summary>
        /// 錯誤訊息資訊
        /// </summary>
        /// <returns></returns>
        public MessageHelper GetMessage()
        {
            _msgHelper ??= new MessageHelper();
            return _msgHelper;
        }
    }

    public partial class BaseDBHandler
    {
        public bool CheckDataExist(string TableName, Dictionary<string, object> Condition)
        {
            string whereClause = string.Join(" AND ", Condition.Select(kv => $"{kv.Key} = @{kv.Key}"));
            Dictionary<string, object> parameters = Condition.ToDictionary(kv => kv.Key, kv => kv.Value);
            string countSql = $"SELECT EXISTS (SELECT 1 FROM {TableName} WHERE {whereClause} ); ";
            int result = this.dbHelper.FindScalar<int>(countSql, parameters);
            return result == 1;
        }
        public bool CheckDataExist(string TableName, string Condition)
        {
            string countSql = $"SELECT EXISTS (SELECT 1 FROM {TableName} {Condition} ); ";
            int result = this.dbHelper.FindScalar<int>(countSql, null);
            return result == 1;
        }

        /// <summary>
        /// 取得資料表中某各欄位的資料型態
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public string GetFieldType(string FieldName, string TableName)
        {
            string text = "";
            string text2 = "SELECT DATA_TYPE FROM USER_TAB_COLUMNS WHERE TABLE_NAME='" + TableName + "' AND COLUMN_NAME='" + FieldName + "'";
            if (!CheckSchemaExist(FieldName.ToUpper(), TableName.ToUpper()))
            {
                return "";
            }

            DataTable dataTable = GetDBHelper().FindDataTable(text, []);
            if (dataTable.Rows.Count > 0)
            {
                text = dataTable.Rows[0][0].ToString();
            }
            else
            {
                text2 = text2.Replace("USER_TAB_COLUMNS", "USER_VIEW_COLUMNS").Replace("TABLE_NAME", "VIEW_NAME");
                dataTable = GetDBHelper().FindDataTable(text2, []);
                text = ((dataTable.Rows.Count <= 0) ? "" : dataTable.Rows[0][0].ToString());
            }

            dataTable.Dispose();
            return text;
        }

        /// <summary>
        /// 確認資料庫中是否含有某個表格的某個欄位。
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool CheckSchemaExist(string ColumnName, string TableName)
        {
            if (TableName.ToLower() == "dual")
            {
                return true;
            }

            string text = "";
            if (TableName.Contains("@"))
            {
                text = TableName.Substring(TableName.IndexOf("@"));
                TableName = TableName.Replace(text, "");
            }

            if (ColumnName.IndexOf("(") > 0 && ColumnName.IndexOf(")") > 0)
            {
                return true;
            }

            bool flag = false;
            string text2 = "SELECT column_name FROM user_tab_columns" + text + " WHERE table_name='" + TableName.ToUpper() + "' AND column_name='" + ColumnName.ToUpper() + "'";
            Trace.WriteLine(text2);
            DataTable dataTable = GetDBHelper().FindDataTable(text2, []);
            if (dataTable.Rows.Count > 0)
            {
                flag = true;
            }
            else
            {
                text2 = text2.Replace("user_tab_columns", "user_view_columns").Replace("table_name", "view_name");
                dataTable = GetDBHelper().FindDataTable(text2, []);
                flag = dataTable.Rows.Count > 0 || (CheckDataExist("user_objects", "object_type='SYNONYM' AND object_name='" + TableName.ToUpper() + "'") ? true : false);
            }

            dataTable.Dispose();
            return flag;
        }

        /// <summary>
        /// 轉換資料為具 Oracle 型別的資料
        /// </summary>
        /// <param name="Data">資料內容</param>
        /// <param name="Type">轉換型別</param>
        /// <returns>轉換後之物件</returns>
        public object ConvertOracleData(string Data, DbType Type)
        {
            switch (Type)
            {
                case DbType.DateTime:
                    if (Data != "" && Data != null && Data != string.Empty)
                    {
                        return Convert.ToDateTime(Data);
                    }

                    return DBNull.Value;
                case DbType.Int32:
                    if (Data != "" && Data != null)
                    {
                        return Convert.ToInt32(Data);
                    }

                    return DBNull.Value;
                case DbType.Double:
                    if (Data != "" && Data != null)
                    {
                        return Convert.ToDouble(Data);
                    }

                    return DBNull.Value;
                case DbType.AnsiString:
                case DbType.String:
                    return string.IsNullOrEmpty(Data) ? ((IConvertible)DBNull.Value) : ((IConvertible)Data);
                default:
                    return Data;
            }
        }

        /// <summary>
        /// 轉換資料為具 Oracle 型別的資料
        /// </summary>
        /// <param name="Data">資料內容</param>
        /// <param name="Type">轉換型別</param>
        /// <returns>轉換後之字串</returns>
        public string ConvertOracleData(string Data, string Type)
        {
            string text = "";
            return Type switch
            {
                "VARCHAR2" => "'" + Data + "'",
                "NUMBER" => Data,
                "DATE" => "to_date('" + Data + "','YYYY/MM/DD HH24:MI:SS')",
                _ => Data,
            };
        }

        /// <summary>
        /// 取得資料表中某個欄位的資料。[使用 Transaction]
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="FieldName"></param>
        /// <param name="TableName"></param>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public string GetFieldData(string FieldName, string TableName, Dictionary<string, object> Condition)
        {
            string text = string.Empty;

            if (!CheckSchemaExist(FieldName, TableName))
            {
                return text;
            }

            string whereClause = string.Join(" AND ", Condition.Select(kv => $"{kv.Key} = @{kv.Key}"));
            Dictionary<string, object> parameters = Condition.ToDictionary(kv => kv.Key, kv => kv.Value);
            string queryString = $"SELECT {FieldName} FROM {TableName} WHERE {whereClause} ";

            BaseDBHandler handler = new();
            DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, parameters);
            text = (dataTable.Rows.Count <= 0) ? string.Empty : dataTable.Rows[0][0].ToString() ?? string.Empty;
            dataTable.Dispose();
            return text;
        }
    }
}
