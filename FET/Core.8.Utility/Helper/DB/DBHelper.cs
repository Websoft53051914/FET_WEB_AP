using Core.Utility.Config;
using Core.Utility.Helper.DB.Component;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Helper.DB.Enums;
using System.Data;

namespace Core.Utility.Helper.DB
{
    public class DBHelper : IDBHelper
    {
        /// <summary>
        /// 
        /// </summary>
        IDBComoponent iDBComoponent = null;
        //TODO 待確認
        /// <summary>
        /// 
        /// </summary>
        private List<BatchSqlContainer> listExecute = new();

        /// <summary>
        /// 建構時決定連線字串以及DB類型
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbType"></param>
        public DBHelper(string connectionString, DBTypeEnums dbType)
        {
            if (dbType == DBTypeEnums.POSTGRESQL)
            {
                iDBComoponent = new DapperComponent(connectionString);
            }

        }

        #region "連線測試,《測試使用》"
        /// <summary>
        /// 測試用，看連線是否存在
        /// </summary>
        /// <returns>是/否</returns>
        public bool IsConnection()
        {
            return this.iDBComoponent.IsConnection();
        }
        #endregion

        /// <summary>
        /// 新刪修查下SQL指令
        /// </summary>
        /// <param name="_SQLScript"></param>
        /// <param name="paras"></param>
        public void Execute(string _SQLScript, Dictionary<string, object> paras)
        {
            listExecute.Add(new BatchSqlContainer(_SQLScript, paras));
        }

        /// <summary>
        /// 執行Transation後下Commit
        /// </summary>
        public void Commit()
        {
            this.iDBComoponent.BatchExecute(listExecute);
            listExecute.Clear();
        }

        /// <summary>
        /// 執行Transation後下Commit
        /// </summary>
        public void Rollback()
        {
            listExecute.Clear();
        }


        #region "查詢多筆 分頁"

        /// <summary>
        /// 依條件查詢多筆資料-分頁
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="currentPage">目前第幾頁</param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public PageResult<T> FindPageList<T>(string _SQLScript, string _countSQL, int currentPage, Dictionary<string, object> paras = null)
        {
            return FindPageList<T>(_SQLScript, _countSQL, currentPage, UtilityConfig.DEFULT_PAGE_SIZE, paras);
        }

        /// <summary>
        /// 依條件查詢多筆資料-分頁
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="currentPage">目前第幾頁</param>
        /// <param name="pageSize">一頁顯示幾筆資料</param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns>分頁清單</returns>
        public PageResult<T> FindPageList<T>(string _SQLScript, string _countSQL, int currentPage, int pageSize, Dictionary<string, object> paras = null)
        {
            PageResult<T> pageResult = new()
            {
                CurrentPage = currentPage,
                PageDataSize = pageSize,
                Results = this.PageList<T>(_SQLScript, currentPage, pageSize, paras),
                DataCount = this.iDBComoponent.GetScalarBySQLScript<int>(_countSQL, CommandType.Text, paras)
            };

            return pageResult;
        }


        /// <summary>
        /// 依條件查詢多筆資料(分頁用)
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="currentPage">目前第幾頁</param>
        /// <param name="pageSize">一頁顯示幾筆資料</param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns>分頁清單</returns>
        public List<T> PageList<T>(string _SQLScript, int currentPage, int pageSize, Dictionary<string, object> paras = null)
        {
            int startRowNum = (currentPage <= 1) ? 1 : 1 + (currentPage - 1) * pageSize;
            int endRowNum = (startRowNum - 1) + pageSize;
            string paggingSQL = @"
select
    *
from 
    ( " + _SQLScript + @" ) as pageData
limit "+ pageSize + @" offset "+ (startRowNum-1) +@" ;

";

            return this.iDBComoponent.GetEntitiesBySQLScript<T>(paggingSQL, CommandType.Text, paras);
        }
        #endregion

        #region "Batch Execute By SQL Script"

        /// <summary>
        /// 批次執行新刪修
        /// </summary>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="paras">參數</param>
        public void BatchExecuteBySQLScript(string _SQLScript, List<Dictionary<string, object>> batchParas)
        {
            foreach (Dictionary<string, object> paras in batchParas)
            {
                listExecute.Add(new BatchSqlContainer(_SQLScript, paras));
            }
        }
        #endregion


        #region "查詢基本方法"
        /// <summary>
        /// 依條件查詢多筆Dictionary資料
        /// </summary>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳多筆Dictionary資料</returns>
        public List<Dictionary<string, object>> FindToDictionary(string _SQLScript, Dictionary<string, object> paras = null)
        {
            return iDBComoponent.FindToDictionAry(_SQLScript, paras);
        }
        /// <summary>
        /// 依條件查詢多筆Dictionary資料
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳多筆 Entity 資料</returns>
        public List<T> FindList<T>(string _SQLScript, Dictionary<string, object> paras = null)
        {
            return iDBComoponent.FindToList<T>(_SQLScript, paras);
        }

        /// <summary>
        /// 依條件查詢單筆資料
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">SQL類型</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳指定Entity</returns>
        public T Find<T>(string _SQLScript, Dictionary<string, object> paras = null)
        {
            return iDBComoponent.GetEntityBySQLScript<T>(_SQLScript, CommandType.Text, paras);
        }

        /// <summary>
        /// 依條件查詢第一筆資料
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">command類型</param>
        /// <param name="paras">參數</param>
        /// <returns>指定Entity</returns>
        public T FindScalar<T>(string _SQLScript, Dictionary<string, object> paras = null)
        {
            return iDBComoponent.GetScalarBySQLScript<T>(_SQLScript, CommandType.Text, paras);
        }

        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        public int ExecStoredProcedureWithTransation(string _StoredProcedureName, Dictionary<string, object> paras = null)
        {
            return iDBComoponent.ExecStoredProcedureWithTransation(_StoredProcedureName, paras);
        }

        /// <summary>
        /// 依照SQL取得DATATABLE
        /// </summary>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="paras">參數</param>
        /// <returns>查詢結果</returns>
        public DataTable FindDataTable(string _SQLScript, Dictionary<string, object> paras)
        {
            return iDBComoponent.GetDataTableBySQLScript(_SQLScript, CommandType.Text, paras);
        }

        /// <summary>
        /// 依條件查詢多筆資料-分頁
        /// </summary>
        /// <typeparam name="T">指定的Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_countSQL">總筆數</param>
        /// <param name="currentPage">當下頁數</param>
        /// <param name="pageSize">每頁幾筆</param>
        /// <param name="paras">參數</param>
        /// <param name="orderColumn">排序欄位</param>
        /// <returns>查詢結果</returns>
        public PageResult<T> FindPageList<T>(string _SQLScript, string _countSQL, int currentPage, int pageSize, Dictionary<string, object> paras, string orderColumn)
        {
            PageResult<T> pageResult = new()
            {
                CurrentPage = currentPage,
                PageDataSize = pageSize,
                Results = this.PageList<T>(_SQLScript, currentPage, pageSize, paras, orderColumn),
                DataCount = this.iDBComoponent.GetScalarBySQLScript<int>(_countSQL, CommandType.Text, paras)
            };

            return pageResult;
        }

        /// <summary>
        /// 組成分頁SQL並執行
        /// </summary>
        /// <typeparam name="T">指定的Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="currentPage">當下頁數</param>
        /// <param name="pageSize">每頁幾筆</param>
        /// <param name="paras">參數</param>
        /// <param name="orderColumn">排序欄位</param>
        /// <returns>查詢結果</returns>
        private List<T> PageList<T>(string _SQLScript, int currentPage, int pageSize, Dictionary<string, object> paras, string orderColumn)
        {
            int startRowNum = (currentPage <= 1) ? 1 : 1 + (currentPage - 1) * pageSize;
            int endRowNum = (startRowNum - 1) + pageSize;
            string paggingSQL = @"
select * from
(
select
    ROW_NUMBER() OVER(ORDER BY " + orderColumn + @") AS RowNum,pageData.*
from
    (" + _SQLScript + @") as pageData
)pageData
where 
    pageData.RowNum >= " + startRowNum + @" AND pageData.RowNum <= " + endRowNum + @"
";

            return this.iDBComoponent.GetEntitiesBySQLScript<T>(paggingSQL, CommandType.Text, paras);
        }
        #endregion

    }
}
