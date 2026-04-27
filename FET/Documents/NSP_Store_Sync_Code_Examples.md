# NSP門市資料同步 - 核心程式碼範例

## 檔案: NSPStoreSyncHandler.cs (核心處理器)

```csharp
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
        /// 第一階段：同步VIEW_DP2FTT資料到nsp_store_profile
        /// </summary>
        public string SyncStoreProfileData()
        {
            try
            {
                LogInfo("開始同步NSP門市資料", "SyncStoreProfileData");
                
                // 1. 從Oracle取得VIEW_DP2FTT資料
                var oracleData = GetOracleViewData();
                LogInfo($"從Oracle取得 {oracleData.Count} 筆門市資料", "SyncStoreProfileData");
                
                // 2. 清空現有資料
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
                return errorMsg;
            }
        }

        /// <summary>
        /// 第二階段：批次同步nsp_store_profile到store_profile
        /// </summary>
        public string BatchSyncNspToStoreProfile()
        {
            try
            {
                LogInfo("開始批次同步NSP資料到store_profile", "BatchSyncNspToStoreProfile");
                
                // 取得所有nsp_store_profile資料
                var nspData = GetAllStoreProfiles();
                LogInfo($"從nsp_store_profile取得 {nspData.Count} 筆資料", "BatchSyncNspToStoreProfile");
                
                int insertCount = 0;
                int updateCount = 0;
                
                foreach (var nspRecord in nspData)
                {
                    try
                    {
                        // 檢查store_profile中是否已存在此ivr_code
                        var existingRecord = GetStoreProfileByIvrCode(nspRecord.ivr_code);
                        
                        if (existingRecord == null)
                        {
                            // 情境1: 新增
                            InsertNewStoreProfile(nspRecord);
                            insertCount++;
                            LogInfo($"新增門市資料: {nspRecord.ivr_code} - {nspRecord.shop_name}", "BatchSyncNspToStoreProfile");
                        }
                        else
                        {
                            // 情境2: 更新
                            bool hasUpdate = UpdateExistingStoreProfile(nspRecord, existingRecord);
                            if (hasUpdate)
                            {
                                updateCount++;
                                LogInfo($"更新門市資料: {nspRecord.ivr_code} - {nspRecord.shop_name}", "BatchSyncNspToStoreProfile");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError($"處理門市資料失敗 (ivr_code: {nspRecord.ivr_code}): {ex.Message}", "BatchSyncNspToStoreProfile");
                        continue;
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
        /// 智能表格探索 - 解決Oracle權限問題
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
        /// 從Oracle讀取門市資料
        /// </summary>
        private List<VIEW_DP2FTTEntity> GetOracleViewData()
        {
            List<VIEW_DP2FTTEntity> result = new List<VIEW_DP2FTTEntity>();
            
            // 嘗試不同的表格名稱 (SPADMUSER.VIEW_DP2FTT為正確名稱)
            string[] possibleTableNames = {
                "SPADMUSER.VIEW_DP2FTT",  // 從測試結果得知的正確名稱
                "VIEW_DP2FTT",
                "view_dp2ftt",
                "NSP.VIEW_DP2FTT"
                // ... 其他可能名稱
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
            
            return result;
        }

        /// <summary>
        /// 新增門市資料到store_profile
        /// </summary>
        private void InsertNewStoreProfile(nsp_store_profileDTO nspRecord)
        {
            string sql = @"
                INSERT INTO store_profile (
                    company_leaves, store_type, channel, area, shop_name, ivr_code, email,
                    owner_empno, as_empno, store_tel, fax_tel, address, create_time, update_time,
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
                    @owner_empno, @as_empno, @store_tel, @fax_tel, @address, @create_time, @update_time,
                    @STOREOPENTM_MON, @STORECLOSETM_MON,
                    @STOREOPENTM_TUE, @STORECLOSETM_TUE,
                    @STOREOPENTM_WED, @STORECLOSETM_WED,
                    @STOREOPENTM_THU, @STORECLOSETM_THU,
                    @STOREOPENTM_FRI, @STORECLOSETM_FRI,
                    @STOREOPENTM_SAT, @STORECLOSETM_SAT,
                    @STOREOPENTM_SUN, @STORECLOSETM_SUN
                )";

            var parameters = CreateStoreProfileParameters(nspRecord, true);
            GetDBHelper().Execute(sql, parameters);
        }

        /// <summary>
        /// 更新現有門市資料
        /// </summary>
        private bool UpdateExistingStoreProfile(nsp_store_profileDTO nspRecord, store_profileDTO existingRecord)
        {
            List<string> updates = new List<string>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            // 比較並建立更新列表
            if (nspRecord.shop_name != existingRecord.shop_name)
            {
                updates.Add("shop_name = @shop_name");
                parameters.Add("shop_name", nspRecord.shop_name);
            }
            
            if (nspRecord.area != existingRecord.area)
            {
                updates.Add("area = @area");
                parameters.Add("area", nspRecord.area);
            }
            
            if (nspRecord.email != existingRecord.email)
            {
                updates.Add("email = @email");
                parameters.Add("email", nspRecord.email);
            }
            
            // ... 其他欄位比較
            
            if (updates.Count > 0)
            {
                updates.Add("update_time = @update_time");
                parameters.Add("update_time", DateTime.Now);
                parameters.Add("ivr_code", nspRecord.ivr_code);
                
                string sql = $"UPDATE store_profile SET {string.Join(", ", updates)} WHERE ivr_code = @ivr_code";
                GetDBHelper().Execute(sql, parameters);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Oracle連接測試
        /// </summary>
        public string TestOracleConnection()
        {
            try
            {
                using (var connection = new OracleConnection(_oracleConnectionString))
                {
                    connection.Open();
                    LogInfo("Oracle連接成功", "TestOracleConnection");
                    
                    // 測試可能的表格名稱
                    string[] testTables = {
                        "SPADMUSER.VIEW_DP2FTT",  // 正確的表格名稱
                        "VIEW_DP2FTT",
                        "NSP.VIEW_DP2FTT"
                    };
                    
                    List<string> results = new List<string>();
                    results.Add("=== 測試結果 ===");
                    
                    foreach (string testTable in testTables)
                    {
                        try
                        {
                            string testSql = $"SELECT COUNT(*) FROM {testTable} WHERE ROWNUM <= 1";
                            using (var command = new OracleCommand(testSql, connection))
                            {
                                var result = command.ExecuteScalar();
                                results.Add($"✅ {testTable} - 可存取 (有資料: {result})");
                            }
                        }
                        catch (Exception ex)
                        {
                            results.Add($"❌ {testTable} - {ex.Message}");
                        }
                    }
                    
                    return string.Join("\n", results);
                }
            }
            catch (Exception ex)
            {
                string error = $"Oracle連接測試失敗：{ex.Message}";
                LogError(error, "TestOracleConnection");
                return error;
            }
        }
    }
}
```

## 檔案: NSPStoreSyncController.cs (API控制器)

```csharp
[ApiController]
[Route("api/[controller]")]
public class NSPStoreSyncController : ControllerBase
{
    private readonly ConfigurationHelper _configHelper;

    public NSPStoreSyncController(ConfigurationHelper configHelper)
    {
        _configHelper = configHelper;
    }

    /// <summary>
    /// 測試Oracle連接
    /// </summary>
    [HttpGet("test-oracle-connection")]
    public IActionResult TestOracleConnection()
    {
        try
        {
            var handler = new NSPStoreSyncHandler(_configHelper);
            string result = handler.TestOracleConnection();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"測試失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 第一階段：從Oracle同步到nsp_store_profile
    /// </summary>
    [HttpPost("sync-from-oracle")]
    public IActionResult SyncFromOracle()
    {
        try
        {
            var handler = new NSPStoreSyncHandler(_configHelper);
            string result = handler.SyncStoreProfileData();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"同步失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 第二階段：批次同步到store_profile
    /// </summary>
    [HttpPost("batch-sync-to-store-profile")]
    public IActionResult BatchSyncToStoreProfile()
    {
        try
        {
            var handler = new NSPStoreSyncHandler(_configHelper);
            string result = handler.BatchSyncNspToStoreProfile();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"批次同步失敗: {ex.Message}");
        }
    }
}
```

## 檔案: NSPStoreSyncBackgroundService.cs (背景服務)

```csharp
public class NSPStoreSyncBackgroundService : BackgroundService
{
    private readonly ILogger<NSPStoreSyncBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _syncInterval;

    public NSPStoreSyncBackgroundService(
        ILogger<NSPStoreSyncBackgroundService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        
        // 從設定檔讀取同步間隔時間，預設為每24小時同步一次
        var intervalHours = configuration.GetValue<int>("NSPStoreSync:IntervalHours", 24);
        _syncInterval = TimeSpan.FromHours(intervalHours);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 檢查是否啟用同步功能
        var isEnabled = _configuration.GetValue<bool>("NSPStoreSync:Enabled", true);
        if (!isEnabled)
        {
            _logger.LogInformation("NSP門市資料同步功能已停用");
            return;
        }

        _logger.LogInformation("NSP門市資料同步背景服務已啟動");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DoWork();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NSP門市資料同步時發生錯誤，繼續執行下次同步");
            }

            // 等待下次執行
            await Task.Delay(_syncInterval, stoppingToken);
        }
    }

    private async Task DoWork()
    {
        _logger.LogInformation("開始執行NSP門市資料同步作業");

        using (var scope = _serviceProvider.CreateScope())
        {
            try
            {
                var configHelper = scope.ServiceProvider.GetRequiredService<ConfigurationHelper>();
                var handler = new NSPStoreSyncHandler(configHelper);
                
                // 執行兩階段同步
                string result1 = handler.SyncStoreProfileData();
                _logger.LogInformation($"第一階段同步完成：{result1}");
                
                string result2 = handler.BatchSyncNspToStoreProfile();
                _logger.LogInformation($"第二階段同步完成：{result2}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NSP門市資料同步失敗");
            }
        }
    }
}
```

## 設定檔案範例

```json
{
  "NSPStoreSync": {
    "Enabled": true,
    "IntervalHours": 24,
    "OracleConnection": {
      "Host": "your-oracle-host",
      "Port": "1521",
      "ServiceName": "your-service-name",
      "UserId": "fttuser",
      "Password": "your-password"
    }
  }
}
```
