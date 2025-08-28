using Core.Utility.Helper.DB.Entity;
using System.Data;

namespace Core.Utility.Helper.DB
{
    public interface IDBHelper
    {
        #region "連線測試,《測試使用》"
        /// <summary>
        /// 測試用，看連線是否存在
        /// </summary>
        /// <returns>是/否</returns>
        bool IsConnection();
        #endregion


        /// <summary>
        /// 新刪修查下SQL指令
        /// </summary>
        /// <param name="_SQLScript"></param>
        /// <param name="paras"></param>
        void Execute(string _SQLScript, Dictionary<string, object> paras);

        /// <summary>
        /// 執行Transation後下Commit
        /// </summary>
        void Commit();

        /// <summary>
        /// 執行Transation後下Commit
        /// </summary>
        void Rollback();


        #region "查詢多筆 分頁"

        /// <summary>
        /// 依條件查詢多筆資料,分頁筆數預設10筆
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="currentPage">目前第幾頁</param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        PageResult<T> FindPageList<T>(string _SQLScript, string _countSQL, int currentPage, Dictionary<string, object> paras = null);



        /// <summary>
        /// 依條件查詢多筆資料,分頁筆數預設10筆
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="currentPage">目前第幾頁</param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        PageResult<T> FindPageList<T>(string _SQLScript, string _countSQL, int currentPage, int pageSize, Dictionary<string, object> paras = null);
        #endregion

        #region "Batch Execute By SQL Script"

        /// <summary>
        /// 批次執行新刪修
        /// </summary>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="paras">參數</param>
        void BatchExecuteBySQLScript(string _SQLScript, List<Dictionary<string, object>> batchParas);
        #endregion


        #region "查詢基本方法"
        /// <summary>
        /// 依條件查詢多筆Dictionary資料
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> FindToDictionary(string _SQLScript, Dictionary<string, object> paras = null);

        /// <summary>
        /// 依條件查詢多筆資料
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        List<T> FindList<T>(string _SQLScript, Dictionary<string, object> paras = null);

        /// <summary>
        /// 依條件查詢單筆資料
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        T Find<T>(string _SQLScript, Dictionary<string, object> paras = null);
        /// <summary>
        /// 依照SQL取得DATATABLE
        /// </summary>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="paras">參數</param>
        /// <returns>查詢結果</returns>
        DataTable FindDataTable(string strSQL, Dictionary<string, object> paras);
        /// <summary>
        /// 依條件查詢第一筆第一個欄位資料
        /// </summary>
        /// <typeparam name="T">指定Entity</typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        T FindScalar<T>(string _SQLScript, Dictionary<string, object> paras = null);
        #endregion

        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        int ExecStoredProcedureWithTransation(string _StoredProcedureName, Dictionary<string, object> paras = null);

    }
}
