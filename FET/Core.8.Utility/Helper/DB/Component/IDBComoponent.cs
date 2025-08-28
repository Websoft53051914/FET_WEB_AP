using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.DB.Component
{
    public interface IDBComoponent
    {

        #region "連線測試,《測試使用》"
        /// <summary>
        /// 測試用，看連線是否存在
        /// </summary>
        /// <returns></returns>
        bool IsConnection();
        #endregion

        #region "查詢"



        /// <summary>
        /// 依條件查詢多筆Dictionary資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> FindToDictionAry(string _SQLScript, Dictionary<string, object> paras = null);

        /// <summary>
        /// 依條件查詢多筆Dictionary資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        List<T> FindToList<T>(string _SQLScript, Dictionary<string, object> paras = null);

        /// <summary>
        /// 依條件查詢單筆資料
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_SQLScript"></param>
        /// <param name="_CommandType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        T GetEntityBySQLScript<T>(string _SQLScript, CommandType _CommandType, Dictionary<string, object> paras = null);

        /// <summary>
        /// 依條件查詢多筆資料
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">SQL類型</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳指定Entity</returns>
        List<T> GetEntitiesBySQLScript<T>(string _SQLScript, CommandType _CommandType, Dictionary<string, object> paras = null);


        /// <summary>
        /// 依條件查詢第一筆第一個欄位資料
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">SQL類型</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳指定Entity</returns>
        T GetScalarBySQLScript<T>(string _SQLScript, CommandType _CommandType, Dictionary<string, object> paras = null);
        #endregion

        #region "StoredProcedure 處理"

        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        int ExecStoredProcedureWithTransation(string _StoredProcedureName, Dictionary<string, object> paras = null);


        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        int ExecStoredProcedureWithoutTransation(string _StoredProcedureName, Dictionary<string, object> paras = null);


        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        object GetExecuteScalarByExecStoredProcedureWithTransation(string _StoredProcedureName, Dictionary<string, object> paras = null);


        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="_StoredProcedureName">Procedure名稱</param>
        /// <param name="paras">參數</param>
        /// <returns>執行結果</returns>
        object GetExecuteScalarByExecStoredProcedureWithoutTransation(string _StoredProcedureName, Dictionary<string, object> paras = null);

        #endregion

        #region "Batch Execute"

        /// <summary>
        /// 批次執行
        /// </summary>
        /// <param name="sqlList">SQL清單</param>
        void BatchExecute(List<BatchSqlContainer> sqlList);
        #endregion

        /// <summary>
        /// 取得指定SQL內容轉換成DataTable
        /// </summary>
        /// <param name="_SQLScript">SQL</param>
        /// <param name="_CommandType">SQL類型</param>
        /// <param name="paras">參數</param>
        /// <returns>回傳DataTable</returns>
        /// <exception cref="NotImplementedException"></exception>
        DataTable GetDataTableBySQLScript(string _SQLScript, CommandType _CommandType, Dictionary<string, object> paras = null);

    }
}
