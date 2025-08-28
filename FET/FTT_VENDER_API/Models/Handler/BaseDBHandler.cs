using Core.Utility.Helper.DB;

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

    }
}
