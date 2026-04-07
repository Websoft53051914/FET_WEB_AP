using Core.Utility.Extensions;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.Json;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    /// <summary>
    /// NSP門市資料同步處理器
    /// 負責從Oracle VIEW_DP2FTT同步資料到PostgreSQL nsp_store_profile
    /// </summary>
    public class NSPStoreSyncHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly string _oracleConnectionString;

        public NSPStoreSyncHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
            
            // 從設定檔讀取Oracle連接資訊
            var oracleConfig = _configHelper.Config.GetSection("NSPStoreSync:OracleConnection");
            var host = oracleConfig["Host"];
            var port = oracleConfig["Port"];
            var serviceName = oracleConfig["ServiceName"];
            var userId = oracleConfig["UserId"];
            var password = oracleConfig["Password"];
            
            // 建立Oracle連接字串
            _oracleConnectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={serviceName})));User Id={userId};Password={password};";
        }

        /// <summary>
        /// 同步VIEW_DP2FTT資料到nsp_store_profile
        /// </summary>
        /// <returns>同步結果訊息</returns>
        public string SyncStoreProfileData()
        {
            try
            {
                LogInfo("開始同步NSP門市資料", "SyncStoreProfileData");
                
                // 1. 從Oracle取得VIEW_DP2FTT資料
                var oracleData = GetOracleViewData();
                
                LogInfo($"從Oracle取得 {oracleData.Count} 筆門市資料", "SyncStoreProfileData");
                
                // 2. 清空現有資料（可依需求調整為增量更新）
                ClearExistingData();
                
                // 3. 插入新資料
                int insertCount = InsertStoreProfileData(oracleData);
                
                string result = $"同步完成：處理 {oracleData.Count} 筆資料，成功插入 {insertCount} 筆";
                LogInfo(result, "SyncStoreProfileData");
                
                return result;
            }
            catch (Exception ex)
            {
                string errorMsg = $"同步失敗：{ex.Message}";
                LogError(ex.Message, "SyncStoreProfileData");
                
                // 發生錯誤時不拋出例外，避免影響其他服務
                return errorMsg;
            }
        }

        /// <summary>
        /// 從Oracle取得VIEW_DP2FTT資料
        /// </summary>
        /// <returns>門市資料列表</returns>
        private List<VIEW_DP2FTTEntity> GetOracleViewData()
        {
            List<VIEW_DP2FTTEntity> result = new List<VIEW_DP2FTTEntity>();
            
            // 先嘗試查詢可用的表格和檢視表
            try
            {
                LogInfo("正在檢查Oracle資料庫中的表格和檢視表", "GetOracleViewData");
                CheckAvailableTables();
            }
            catch (Exception ex)
            {
                LogError($"檢查可用表格時發生錯誤: {ex.Message}", "GetOracleViewData");
            }
            
            // 嘗試不同的表格名稱
            string[] possibleTableNames = {
                "SPADMUSER.VIEW_DP2FTT",  // 從測試結果得知的正確名稱
                "VIEW_DP2FTT",
                "view_dp2ftt",
                "NSP.VIEW_DP2FTT", 
                "NSP.view_dp2ftt",
                "nsp.view_dp2ftt",
                "FTTUSER.VIEW_DP2FTT",
                "FTTUSER.view_dp2ftt",
                "fttuser.view_dp2ftt",
                "DP2FTT",
                "dp2ftt",
                "VIEW_DP2FTT_NSP"
            };
            
            foreach (string tableName in possibleTableNames)
            {
                try
                {
                    LogInfo($"嘗試查詢表格: {tableName}", "GetOracleViewData");
                    result = QueryOracleTable(tableName);
                    
                    if (result.Count > 0)
                    {
                        LogInfo($"成功從 {tableName} 取得 {result.Count} 筆資料", "GetOracleViewData");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    LogInfo($"查詢 {tableName} 失敗: {ex.Message}", "GetOracleViewData");
                    continue;
                }
            }
            
            if (result.Count == 0)
            {
                LogError("所有可能的表格名稱都無法查詢到資料", "GetOracleViewData");
            }
            
            return result;
        }
        
        /// <summary>
        /// 檢查Oracle資料庫中可用的表格和檢視表
        /// </summary>
        private void CheckAvailableTables()
        {
            string sql = @"
                SELECT owner, table_name, table_type 
                FROM (
                    SELECT owner, table_name, 'TABLE' as table_type FROM all_tables
                    UNION ALL
                    SELECT owner, view_name as table_name, 'VIEW' as table_type FROM all_views
                ) 
                WHERE UPPER(table_name) LIKE '%DP2FTT%' 
                   OR UPPER(table_name) LIKE '%STORE%'
                   OR UPPER(table_name) LIKE '%NSP%'
                ORDER BY owner, table_name";

            using (var connection = new OracleConnection(_oracleConnectionString))
            {
                connection.Open();
                using (var command = new OracleCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        LogInfo("=== 可用的表格和檢視表 ===", "CheckAvailableTables");
                        while (reader.Read())
                        {
                            string owner = reader["owner"]?.ToString();
                            string tableName = reader["table_name"]?.ToString();
                            string tableType = reader["table_type"]?.ToString();
                            LogInfo($"{tableType}: {owner}.{tableName}", "CheckAvailableTables");
                        }
                        LogInfo("=== 檢查完畢 ===", "CheckAvailableTables");
                    }
                }
            }
        }
        
        /// <summary>
        /// 查詢指定的Oracle表格
        /// </summary>
        /// <param name="tableName">表格名稱</param>
        /// <returns>門市資料列表</returns>
        private List<VIEW_DP2FTTEntity> QueryOracleTable(string tableName)
        {
            List<VIEW_DP2FTTEntity> result = new List<VIEW_DP2FTTEntity>();
            
            string sql = $@"
                SELECT 
                    STORESTYLE, STORETYPE, REGIONNAME, STORENAME, STOREID, EMAIL,
                    storemanager_empno, sales_empno, CONTACTNUM1, FAXNUM, STOREADDRESS,
                    STOREOPENTM_MON, STORECLOSETM_MON,
                    STOREOPENTM_TUE, STORECLOSETM_TUE,
                    STOREOPENTM_WED, STORECLOSETM_WED,
                    STOREOPENTM_THU, STORECLOSETM_THU,
                    STOREOPENTM_FRI, STORECLOSETM_FRI,
                    STOREOPENTM_SAT, STORECLOSETM_SAT,
                    STOREOPENTM_SUN, STORECLOSETM_SUN
                FROM {tableName}"; // 移除測試限制，取得完整資料

            using (var connection = new OracleConnection(_oracleConnectionString))
            {
                connection.Open();
                using (var command = new OracleCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entity = new VIEW_DP2FTTEntity
                            {
                                STORESTYLE = reader["STORESTYLE"]?.ToString(),
                                STORETYPE = reader["STORETYPE"]?.ToString(),
                                REGIONNAME = reader["REGIONNAME"]?.ToString(),
                                STORENAME = reader["STORENAME"]?.ToString(),
                                STOREID = reader["STOREID"]?.ToString(),
                                EMAIL = reader["EMAIL"]?.ToString(),
                                storemanager_empno = reader["storemanager_empno"]?.ToString(),
                                sales_empno = reader["sales_empno"]?.ToString(),
                                CONTACTNUM1 = reader["CONTACTNUM1"]?.ToString(),
                                FAXNUM = reader["FAXNUM"]?.ToString(),
                                STOREADDRESS = reader["STOREADDRESS"]?.ToString(),
                                STOREOPENTM_MON = reader["STOREOPENTM_MON"]?.ToString(),
                                STORECLOSETM_MON = reader["STORECLOSETM_MON"]?.ToString(),
                                STOREOPENTM_TUE = reader["STOREOPENTM_TUE"]?.ToString(),
                                STORECLOSETM_TUE = reader["STORECLOSETM_TUE"]?.ToString(),
                                STOREOPENTM_WED = reader["STOREOPENTM_WED"]?.ToString(),
                                STORECLOSETM_WED = reader["STORECLOSETM_WED"]?.ToString(),
                                STOREOPENTM_THU = reader["STOREOPENTM_THU"]?.ToString(),
                                STORECLOSETM_THU = reader["STORECLOSETM_THU"]?.ToString(),
                                STOREOPENTM_FRI = reader["STOREOPENTM_FRI"]?.ToString(),
                                STORECLOSETM_FRI = reader["STORECLOSETM_FRI"]?.ToString(),
                                STOREOPENTM_SAT = reader["STOREOPENTM_SAT"]?.ToString(),
                                STORECLOSETM_SAT = reader["STORECLOSETM_SAT"]?.ToString(),
                                STOREOPENTM_SUN = reader["STOREOPENTM_SUN"]?.ToString(),
                                STORECLOSETM_SUN = reader["STORECLOSETM_SUN"]?.ToString()
                            };
                            
                            result.Add(entity);
                        }
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// 清空現有的nsp_store_profile資料
        /// </summary>
        private void ClearExistingData()
        {
            // 先查詢現有資料筆數
            string countSql = "SELECT COUNT(*) FROM nsp_store_profile";
            var existingCountResult = GetDBHelper().FindList<dynamic>(countSql, new Dictionary<string, object>());
            var existingCount = existingCountResult.FirstOrDefault()?.count ?? 0;
            LogInfo($"清空前現有門市資料筆數: {existingCount}", "ClearExistingData");
            
            // 清空資料
            string sql = "DELETE FROM nsp_store_profile";
            GetDBHelper().Execute(sql, new Dictionary<string, object>());
            
            // 確認清空結果
            var afterCountResult = GetDBHelper().FindList<dynamic>(countSql, new Dictionary<string, object>());
            var afterCount = afterCountResult.FirstOrDefault()?.count ?? 0;
            LogInfo($"清空後門市資料筆數: {afterCount}", "ClearExistingData");
        }

        /// <summary>
        /// 插入門市資料到nsp_store_profile
        /// </summary>
        /// <param name="oracleData">Oracle資料列表</param>
        /// <returns>插入筆數</returns>
        private int InsertStoreProfileData(List<VIEW_DP2FTTEntity> oracleData)
        {
            int insertCount = 0;
            var duplicateCheck = new HashSet<string>();
            var skippedDuplicates = new List<string>();
            
            LogInfo($"準備插入 {oracleData.Count} 筆門市資料", "InsertStoreProfileData");
            
            string sql = @"
                INSERT INTO nsp_store_profile (
                    company_leaves, store_type, channel, area, shop_name, ivr_code, email,
                    owner_empno, as_empno, store_tel, fax_tel, address, ftt_synctime,
                    STOREOPENTM_MON, STORECLOSETM_MON,
                    STOREOPENTM_TUE, STORECLOSETM_TUE,
                    STOREOPENTM_WED, STORECLOSETM_WED,
                    STOREOPENTM_THU, STORECLOSETM_THU,
                    STOREOPENTM_FRI, STORECLOSETM_FRI,
                    STOREOPENTM_SAT, STORECLOSETM_SAT,
                    STOREOPENTM_SUN, STORECLOSETM_SUN
                )
                VALUES (
                    @company_leaves, @store_type, @channel, @area, @shop_name, @ivr_code, @email,
                    @owner_empno, @as_empno, @store_tel, @fax_tel, @address, @ftt_synctime,
                    @STOREOPENTM_MON, @STORECLOSETM_MON,
                    @STOREOPENTM_TUE, @STORECLOSETM_TUE,
                    @STOREOPENTM_WED, @STORECLOSETM_WED,
                    @STOREOPENTM_THU, @STORECLOSETM_THU,
                    @STOREOPENTM_FRI, @STORECLOSETM_FRI,
                    @STOREOPENTM_SAT, @STORECLOSETM_SAT,
                    @STOREOPENTM_SUN, @STORECLOSETM_SUN
                )";

            foreach (var data in oracleData)
            {
                try
                {
                    // 檢查 STOREID 是否為空或重複
                    if (string.IsNullOrWhiteSpace(data.STOREID))
                    {
                        LogError($"跳過空的STOREID，門市名稱: {data.STORENAME}", "InsertStoreProfileData");
                        continue;
                    }
                    
                    if (duplicateCheck.Contains(data.STOREID))
                    {
                        skippedDuplicates.Add($"{data.STOREID} ({data.STORENAME})");
                        LogError($"跳過重複的STOREID: {data.STOREID}, 門市名稱: {data.STORENAME}", "InsertStoreProfileData");
                        continue;
                    }
                    
                    duplicateCheck.Add(data.STOREID);

                    // 資料轉換邏輯
                    var parameters = new Dictionary<string, object>
                    {
                        { "company_leaves", "FET" },  // 固定值
                        { "store_type", GetStoreType(data.STORESTYLE) },  // 根據STORESTYLE判斷
                        { "channel", data.STORETYPE },
                        { "area", data.REGIONNAME },
                        { "shop_name", data.STORENAME },
                        { "ivr_code", data.STOREID },  // 主鍵
                        { "email", data.EMAIL },
                        { "owner_empno", data.storemanager_empno },
                        { "as_empno", data.sales_empno },
                        { "store_tel", data.CONTACTNUM1 },
                        { "fax_tel", data.FAXNUM },
                        { "address", data.STOREADDRESS },
                        { "ftt_synctime", DateTime.Now },  // 同步時間
                        { "STOREOPENTM_MON", data.STOREOPENTM_MON },
                        { "STORECLOSETM_MON", data.STORECLOSETM_MON },
                        { "STOREOPENTM_TUE", data.STOREOPENTM_TUE },
                        { "STORECLOSETM_TUE", data.STORECLOSETM_TUE },
                        { "STOREOPENTM_WED", data.STOREOPENTM_WED },
                        { "STORECLOSETM_WED", data.STORECLOSETM_WED },
                        { "STOREOPENTM_THU", data.STOREOPENTM_THU },
                        { "STORECLOSETM_THU", data.STORECLOSETM_THU },
                        { "STOREOPENTM_FRI", data.STOREOPENTM_FRI },
                        { "STORECLOSETM_FRI", data.STORECLOSETM_FRI },
                        { "STOREOPENTM_SAT", data.STOREOPENTM_SAT },
                        { "STORECLOSETM_SAT", data.STORECLOSETM_SAT },
                        { "STOREOPENTM_SUN", data.STOREOPENTM_SUN },
                        { "STORECLOSETM_SUN", data.STORECLOSETM_SUN }
                    };

                    GetDBHelper().Execute(sql, parameters);
                    insertCount++;
                    
                    if (insertCount % 50 == 0) // 每50筆記錄一次進度
                    {
                        LogInfo($"已插入 {insertCount} 筆門市資料", "InsertStoreProfileData");
                    }
                }
                catch (Exception ex)
                {
                    LogError($"插入門市資料失敗 (STOREID: {data.STOREID}, 門市名稱: {data.STORENAME}): {ex.Message}", "InsertStoreProfileData");
                }
            }
            
            // 記錄重複資料統計
            if (skippedDuplicates.Count > 0)
            {
                LogInfo($"跳過的重複資料: {string.Join(", ", skippedDuplicates)}", "InsertStoreProfileData");
            }
            
            GetDBHelper().Commit();
            LogInfo($"插入完成，成功插入 {insertCount} 筆，跳過重複 {skippedDuplicates.Count} 筆", "InsertStoreProfileData");
            return insertCount;
        }

        /// <summary>
        /// 根據STORESTYLE判斷store_type
        /// </summary>
        /// <param name="storeStyle">門市風格</param>
        /// <returns>門市類型</returns>
        private string GetStoreType(string storeStyle)
        {
            return !string.IsNullOrEmpty(storeStyle) ? "RETAIL" : "FRANCHISE";
        }

        /// <summary>
        /// 取得門市資料 (查詢用)
        /// </summary>
        /// <param name="ivrCode">門市代碼</param>
        /// <returns>門市資料</returns>
        public nsp_store_profileDTO GetStoreProfile(string ivrCode)
        {
            string sql = "SELECT * FROM nsp_store_profile WHERE ivr_code = @ivr_code";
            var parameters = new Dictionary<string, object> { { "ivr_code", ivrCode } };
            
            return GetDBHelper().Find<nsp_store_profileDTO>(sql, parameters);
        }

        /// <summary>
        /// 取得所有門市資料列表
        /// </summary>
        /// <returns>門市資料列表</returns>
        public List<nsp_store_profileDTO> GetAllStoreProfiles()
        {
            string sql = "SELECT * FROM nsp_store_profile ORDER BY ivr_code";
            return GetDBHelper().FindList<nsp_store_profileDTO>(sql, new Dictionary<string, object>());
        }

        /// <summary>
        /// 記錄資訊日誌
        /// </summary>
        /// <param name="message">訊息</param>
        /// <param name="actionName">動作名稱</param>
        private void LogInfo(string message, string actionName)
        {
            var entity = new TB_Control_LogEntity()
            {
                LogTime = DateTime.Now,
                IP = "",
                Status = ((int)LogStatusEnum.Success).ToString(),
                ControllerName = "NSPStoreSyncHandler",
                ActionName = actionName,
                Exception = message,
            };

            InsertLog(entity);
        }

        /// <summary>
        /// 記錄錯誤日誌
        /// </summary>
        /// <param name="exception">例外訊息</param>
        /// <param name="actionName">動作名稱</param>
        private void LogError(string exception, string actionName)
        {
            var entity = new TB_Control_LogEntity()
            {
                LogTime = DateTime.Now,
                IP = "",
                Status = ((int)LogStatusEnum.Failed).ToString(),
                ControllerName = "NSPStoreSyncHandler",
                ActionName = actionName,
                Exception = exception,
            };

            InsertLog(entity);
        }

        /// <summary>
        /// 插入日誌
        /// </summary>
        /// <param name="entity">日誌實體</param>
        private void InsertLog(TB_Control_LogEntity entity)
        {
            TB_Control_LogHandler logHandler = new TB_Control_LogHandler();
            logHandler.Insert(entity);
        }

        /// <summary>
        /// 測試Oracle連接並列出可用的表格和檢視表
        /// </summary>
        /// <returns>可用表格清單</returns>
        public string TestOracleConnection()
        {
            try
            {
                LogInfo("開始測試Oracle連接", "TestOracleConnection");
                
                using (var connection = new OracleConnection(_oracleConnectionString))
                {
                    connection.Open();
                    LogInfo("Oracle連接成功", "TestOracleConnection");
                    
                    List<string> allResults = new List<string>();
                    
                    // 1. 查詢所有可存取的表格和檢視表 (包含其他Schema)
                    try
                    {
                        string sql1 = @"
                            SELECT owner, table_name, table_type 
                            FROM (
                                SELECT owner, table_name, 'TABLE' as table_type FROM all_tables
                                UNION ALL
                                SELECT owner, view_name as table_name, 'VIEW' as table_type FROM all_views
                            ) 
                            WHERE UPPER(table_name) LIKE '%DP2FTT%' 
                               OR UPPER(table_name) LIKE '%STORE%'
                               OR UPPER(table_name) LIKE '%NSP%'
                               OR UPPER(owner) LIKE '%NSP%'
                            ORDER BY owner, table_type, table_name";
                        
                        allResults.Add("=== 包含關鍵字的表格/檢視表 ===");
                        using (var command = new OracleCommand(sql1, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                int count = 0;
                                while (reader.Read())
                                {
                                    string owner = reader["owner"]?.ToString();
                                    string tableName = reader["table_name"]?.ToString();
                                    string tableType = reader["table_type"]?.ToString();
                                    allResults.Add($"{owner}.{tableName} ({tableType})");
                                    count++;
                                }
                                if (count == 0)
                                {
                                    allResults.Add("未找到包含關鍵字的表格");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        allResults.Add($"查詢包含關鍵字的表格失敗: {ex.Message}");
                    }
                    
                    allResults.Add("");
                    
                    // 2. 查詢使用者可存取的所有Schema
                    try
                    {
                        string sql2 = @"SELECT DISTINCT owner FROM all_tables ORDER BY owner";
                        allResults.Add("=== 可存取的 Schema ===");
                        using (var command = new OracleCommand(sql2, connection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string owner = reader["owner"]?.ToString();
                                    allResults.Add($"Schema: {owner}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        allResults.Add($"查詢Schema失敗: {ex.Message}");
                    }
                    
                    allResults.Add("");
                    
                    // 3. 嘗試直接查詢可能的表格名稱
                    string[] testTables = {
                        "SPADMUSER.VIEW_DP2FTT",  // 從查詢結果得知的正確名稱
                        "VIEW_DP2FTT",
                        "view_dp2ftt",
                        "NSP.VIEW_DP2FTT",
                        "NSP.view_dp2ftt",
                        "nsp.view_dp2ftt",
                        "SYSTEM.VIEW_DP2FTT",
                        "SYSTEM.view_dp2ftt",
                        "DP2FTT",
                        "dp2ftt",
                        "STORE_PROFILE",
                        "store_profile"
                    };
                    
                    allResults.Add("=== 測試可能的表格名稱 ===");
                    foreach (string testTable in testTables)
                    {
                        try
                        {
                            string testSql = $"SELECT COUNT(*) FROM {testTable} WHERE ROWNUM <= 1";
                            using (var command = new OracleCommand(testSql, connection))
                            {
                                var result = command.ExecuteScalar();
                                allResults.Add($"✅ {testTable} - 可存取 (有資料: {result})");
                            }
                        }
                        catch (Exception ex)
                        {
                            allResults.Add($"❌ {testTable} - {ex.Message}");
                        }
                    }
                    
                    string finalResult = $"Oracle連接測試成功！\n" + string.Join("\n", allResults);
                    LogInfo(finalResult, "TestOracleConnection");
                    return finalResult;
                }
            }
            catch (Exception ex)
            {
                string error = $"Oracle連接測試失敗：{ex.Message}";
                LogError(error, "TestOracleConnection");
                return error;
            }
        }
        
        /// <summary>
        /// 批次同步nsp_store_profile資料到store_profile
        /// 包含新增和更新邏輯
        /// </summary>
        /// <returns>同步結果訊息</returns>
        public string BatchSyncNspToStoreProfile()
        {
            try
            {
                LogInfo("開始批次同步NSP資料到store_profile", "BatchSyncNspToStoreProfile");
                
                // 取得所有nsp_store_profile資料
                var nspData = GetAllStoreProfiles();
                LogInfo($"從nsp_store_profile取得 {nspData.Count} 筆資料", "BatchSyncNspToStoreProfile");
                
                // 記錄所有要處理的 ivr_code
                var nspIvrCodes = nspData.Select(x => x.ivr_code).ToList();
                LogInfo($"NSP資料包含的IVR代碼: [{string.Join(", ", nspIvrCodes)}]", "BatchSyncNspToStoreProfile");
                
                // 特別檢查基隆仁二(2703)是否在NSP資料中
                bool hasKeelung2703 = nspIvrCodes.Contains("2703");
                LogInfo($"基隆仁二(2703)是否在NSP資料中: {hasKeelung2703}", "BatchSyncNspToStoreProfile");
                
                int insertCount = 0;
                int updateCount = 0;
                
                foreach (var nspRecord in nspData)
                {
                    try
                    {
                        LogInfo($"處理門市: {nspRecord.ivr_code} - {nspRecord.shop_name}", "BatchSyncNspToStoreProfile");
                        
                        // 檢查store_profile中是否已存在此ivr_code
                        var existingRecord = GetStoreProfileByIvrCode(nspRecord.ivr_code);
                        
                        if (existingRecord == null)
                        {
                            // 情境1: 新增
                            LogInfo($"門市 {nspRecord.ivr_code} 不存在於store_profile，執行新增", "BatchSyncNspToStoreProfile");
                            InsertNewStoreProfile(nspRecord);
                            insertCount++;
                            LogInfo($"新增門市資料: {nspRecord.ivr_code} - {nspRecord.shop_name}", "BatchSyncNspToStoreProfile");
                        }
                        else
                        {
                            // 情境2: 更新
                            LogInfo($"門市 {nspRecord.ivr_code} 已存在於store_profile，檢查是否需要更新", "BatchSyncNspToStoreProfile");
                            bool hasUpdate = UpdateExistingStoreProfile(nspRecord, existingRecord);
                            if (hasUpdate)
                            {
                                updateCount++;
                                LogInfo($"更新門市資料: {nspRecord.ivr_code} - {nspRecord.shop_name}", "BatchSyncNspToStoreProfile");
                            }
                            else
                            {
                                LogInfo($"門市 {nspRecord.ivr_code} 資料無變化，跳過更新", "BatchSyncNspToStoreProfile");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError($"處理門市資料失敗 (ivr_code: {nspRecord.ivr_code}): {ex.Message}", "BatchSyncNspToStoreProfile");
                    }
                }
                
                GetDBHelper().Commit();
                string result = $"批次同步完成：新增 {insertCount} 筆，更新 {updateCount} 筆";
                LogInfo(result, "BatchSyncNspToStoreProfile");
                
                return result;
            }
            catch (Exception ex)
            {
                string errorMsg = $"批次同步失敗：{ex.Message}";
                LogError(ex.Message, "BatchSyncNspToStoreProfile");
                return errorMsg;
            }
        }

        /// <summary>
        /// 根據ivr_code查詢store_profile資料
        /// </summary>
        /// <param name="ivrCode">門市代碼</param>
        /// <returns>門市資料，若不存在則回傳null</returns>
        private dynamic GetStoreProfileByIvrCode(string ivrCode)
        {
            string sql = "SELECT * FROM store_profile WHERE ivr_code = @ivr_code";
            var parameters = new Dictionary<string, object> { { "ivr_code", ivrCode } };
            
            var result = GetDBHelper().FindList<dynamic>(sql, parameters);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// 新增門市資料到store_profile (情境1)
        /// </summary>
        /// <param name="nspRecord">NSP門市資料</param>
        private void InsertNewStoreProfile(nsp_store_profileDTO nspRecord)
        {
            // 格式化營業時間
            string businessHourRange1 = FormatBusinessHour(nspRecord.STOREOPENTM_MON, nspRecord.STORECLOSETM_MON);
            string businessHourRange2 = FormatBusinessHour(nspRecord.STOREOPENTM_SAT, nspRecord.STORECLOSETM_SAT);
            string businessHourRange3 = FormatBusinessHour(nspRecord.STOREOPENTM_SUN, nspRecord.STORECLOSETM_SUN);
            string businessHourRange4 = FormatBusinessHour(nspRecord.STOREOPENTM_SUN, nspRecord.STORECLOSETM_SUN);

            string sql = @"
                INSERT INTO store_profile (
                    company_leaves, store_type, channel, area, shop_name, ivr_code, email,
                    owner_empno, as_empno, urgent_tel, fax_tel, address, 
                    business_hour_range1, business_hour_range2, business_hour_range3, business_hour_range4,
                    updatetime, sync_nsp, sync_time
                )
                VALUES (
                    @company_leaves, @store_type, @channel, @area, @shop_name, @ivr_code, @email,
                    @owner_empno, @as_empno, @urgent_tel, @fax_tel, @address,
                    @business_hour_range1, @business_hour_range2, @business_hour_range3, @business_hour_range4,
                    @updatetime, @sync_nsp, @sync_time
                )";

            var parameters = new Dictionary<string, object>
            {
                { "company_leaves", nspRecord.company_leaves },
                { "store_type", nspRecord.store_type },
                { "channel", nspRecord.channel },
                { "area", nspRecord.area },
                { "shop_name", nspRecord.shop_name },
                { "ivr_code", nspRecord.ivr_code },
                { "email", nspRecord.email },
                { "owner_empno", nspRecord.owner_empno },
                { "as_empno", nspRecord.as_empno },
                { "urgent_tel", nspRecord.store_tel },
                { "fax_tel", nspRecord.fax_tel },
                { "address", nspRecord.address },
                { "business_hour_range1", businessHourRange1 },
                { "business_hour_range2", businessHourRange2 },
                { "business_hour_range3", businessHourRange3 },
                { "business_hour_range4", businessHourRange4 },
                { "updatetime", DateTime.Now },
                { "sync_nsp", "A" },
                { "sync_time", DateTime.Now }
            };

            GetDBHelper().Execute(sql, parameters);
        }

        /// <summary>
        /// 更新現有門市資料 (情境2)
        /// </summary>
        /// <param name="nspRecord">NSP門市資料</param>
        /// <param name="existingRecord">現有門市資料</param>
        /// <returns>是否有進行更新</returns>
        private bool UpdateExistingStoreProfile(nsp_store_profileDTO nspRecord, dynamic existingRecord)
        {
            bool hasUpdate = false;
            var updateFields = new List<string>();
            var parameters = new Dictionary<string, object> { { "ivr_code", nspRecord.ivr_code } };
            var skippedFields = new List<string>();

            // 比較area (區域不能為空白)
            if (string.IsNullOrWhiteSpace(nspRecord.area))
            {
                skippedFields.Add("area (空值)");
            }
            else if (!string.Equals(nspRecord.area, existingRecord.area?.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                updateFields.Add("area = @area");
                parameters.Add("area", nspRecord.area);
                hasUpdate = true;
            }

            // 比較owner_empno (店長員編不能為空白)
            if (string.IsNullOrWhiteSpace(nspRecord.owner_empno))
            {
                skippedFields.Add("owner_empno (空值)");
            }
            else if (!string.Equals(nspRecord.owner_empno, existingRecord.owner_empno?.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                updateFields.Add("owner_empno = @owner_empno");
                parameters.Add("owner_empno", nspRecord.owner_empno);
                hasUpdate = true;
            }

            // 比較as_empno (區主管員編不能為空白)
            if (string.IsNullOrWhiteSpace(nspRecord.as_empno))
            {
                skippedFields.Add("as_empno (空值)");
            }
            else if (!string.Equals(nspRecord.as_empno, existingRecord.as_empno?.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                updateFields.Add("as_empno = @as_empno");
                parameters.Add("as_empno", nspRecord.as_empno);
                hasUpdate = true;
            }

            // 記錄跳過的空值欄位
            if (skippedFields.Count > 0)
            {
                LogInfo($"門市 {nspRecord.ivr_code} 跳過空值欄位: {string.Join(", ", skippedFields)}", "UpdateExistingStoreProfile");
            }

            // 如果有任何欄位需要更新
            if (hasUpdate)
            {
                updateFields.Add("updatetime = @updatetime");
                updateFields.Add("sync_nsp = @sync_nsp");
                updateFields.Add("sync_time = @sync_time");
                parameters.Add("updatetime", DateTime.Now);
                parameters.Add("sync_nsp", "U");
                parameters.Add("sync_time", DateTime.Now);

                string sql = $@"
                    UPDATE store_profile 
                    SET {string.Join(", ", updateFields)}
                    WHERE ivr_code = @ivr_code";

                GetDBHelper().Execute(sql, parameters);
                LogInfo($"門市 {nspRecord.ivr_code} 更新欄位: {string.Join(", ", updateFields.Take(updateFields.Count - 3))}", "UpdateExistingStoreProfile");
            }

            return hasUpdate;
        }

        /// <summary>
        /// 格式化營業時間為 "開始時間~結束時間" 格式
        /// </summary>
        /// <param name="openTime">開店時間</param>
        /// <param name="closeTime">關店時間</param>
        /// <returns>格式化的營業時間字串</returns>
        private string FormatBusinessHour(string openTime, string closeTime)
        {
            if (string.IsNullOrWhiteSpace(openTime) || string.IsNullOrWhiteSpace(closeTime))
            {
                return string.Empty;
            }

            return $"{openTime.Trim()}~{closeTime.Trim()}";
        }
    }
}
