using Core.Utility.Helper.DB;
using System.Data;
using System.Diagnostics;

namespace FTT_VENDER_API.Models.Handler
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
    }

    public partial class BaseDBHandler
    {
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

        public bool CheckDataExist(string TableName, Dictionary<string,object> Condition)
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

        public bool CheckDataExist(string TableName, string Condition, Dictionary<string, object> dicCondition)
        {
            string countSql = $"SELECT EXISTS (SELECT 1 FROM {TableName} where {Condition} ); ";
            int result = this.dbHelper.FindScalar<int>(countSql, dicCondition);
            return result == 1;
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
