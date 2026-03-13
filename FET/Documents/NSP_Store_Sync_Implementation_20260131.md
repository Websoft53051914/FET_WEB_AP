# NSP門市資料同步機制實作文件

## 文件資訊
- **建立日期**: 2026-01-31
- **版本**: 1.0
- **作者**: GitHub Copilot
- **專案**: FTT API
- **功能**: NSP門市資料同步機制

## 概述
本文件記錄了從Oracle VIEW_DP2FTT檢視表同步門市資料到PostgreSQL的完整實作流程，包含兩個主要階段：
1. **第一階段**: 從Oracle `SPADMUSER.VIEW_DP2FTT` 同步資料到 `nsp_store_profile` 表
2. **第二階段**: 從 `nsp_store_profile` 比對並更新到 `store_profile` 表

## 系統架構

### 核心組件
- **NSPStoreSyncHandler**: 主要處理器，負責所有同步邏輯
- **NSPStoreSyncController**: API控制器，提供HTTP端點
- **NSPStoreSyncBackgroundService**: 背景服務，定期自動同步
- **測試頁面**: `nsp-sync-test.html` 用於手動測試和驗證

### 資料流程
```
Oracle (SPADMUSER.VIEW_DP2FTT) 
    ↓ (第一階段同步)
PostgreSQL (nsp_store_profile) 
    ↓ (第二階段比對更新)
PostgreSQL (store_profile)
```

## 第一階段：Oracle到nsp_store_profile同步

### 1.1 Oracle連線設定
```json
{
  "NSPStoreSync": {
    "OracleConnection": {
      "Host": "oracle_host",
      "Port": "1521",
      "ServiceName": "service_name",
      "UserId": "fttuser",
      "Password": "password"
    }
  }
}
```

### 1.2 表格探索與連接測試
由於Oracle權限限制，實作了智能表格探索機制：

```csharp
// 查詢所有可存取的表格和檢視表
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
```

**發現結果**: 找到 `SPADMUSER.VIEW_DP2FTT` 為目標檢視表

### 1.3 資料同步邏輯
```csharp
public string SyncStoreProfileData()
{
    // 1. 從Oracle取得VIEW_DP2FTT資料
    var oracleData = GetOracleViewData();
    
    // 2. 清空現有nsp_store_profile資料
    ClearExistingData();
    
    // 3. 插入新資料到nsp_store_profile
    int insertCount = InsertStoreProfileData(oracleData);
    
    return $"同步完成：處理 {oracleData.Count} 筆資料，成功插入 {insertCount} 筆";
}
```

### 1.4 欄位對應關係
| Oracle VIEW_DP2FTT | nsp_store_profile | 說明 |
|-------------------|------------------|------|
| STORESTYLE | company_leaves | 固定為"FET" |
| STORESTYLE | store_type | 判斷RETAIL/FRANCHISE |
| STORETYPE | channel | 直接對應 |
| REGIONNAME | area | 直接對應 |
| STORENAME | shop_name | 直接對應 |
| STOREID | ivr_code | 主鍵 |
| EMAIL | email | 直接對應 |
| storemanager_empno | owner_empno | 直接對應 |
| sales_empno | as_empno | 直接對應 |
| CONTACTNUM1 | store_tel | 直接對應 |
| FAXNUM | fax_tel | 直接對應 |
| STOREADDRESS | address | 直接對應 |
| 系統時間 | ftt_synctime | 同步時間戳 |
| STOREOPENTM_* | STOREOPENTM_* | 營業時間直接對應 |

## 第二階段：nsp_store_profile到store_profile同步

### 2.1 批次同步邏輯
```csharp
public string BatchSyncNspToStoreProfile()
{
    var nspData = GetAllStoreProfiles();
    int insertCount = 0;
    int updateCount = 0;
    
    foreach (var nspRecord in nspData)
    {
        var existingRecord = GetStoreProfileByIvrCode(nspRecord.ivr_code);
        
        if (existingRecord == null)
        {
            // 情境1: 新增
            InsertNewStoreProfile(nspRecord);
            insertCount++;
        }
        else
        {
            // 情境2: 更新
            bool hasUpdate = UpdateExistingStoreProfile(nspRecord, existingRecord);
            if (hasUpdate) updateCount++;
        }
    }
    
    return $"批次同步完成：新增 {insertCount} 筆，更新 {updateCount} 筆";
}
```

### 2.2 新增邏輯（INSERT）
當 `store_profile` 中不存在對應的 `ivr_code` 時：

```csharp
private void InsertNewStoreProfile(nsp_store_profileDTO nspRecord)
{
    string sql = @"
        INSERT INTO store_profile (
            company_leaves, store_type, channel, area, shop_name, 
            ivr_code, email, owner_empno, as_empno, store_tel, 
            fax_tel, address, create_time, update_time,
            STOREOPENTM_MON, STORECLOSETM_MON, -- 週一到週日的營業時間
            STOREOPENTM_TUE, STORECLOSETM_TUE,
            STOREOPENTM_WED, STORECLOSETM_WED,
            STOREOPENTM_THU, STORECLOSETM_THU,
            STOREOPENTM_FRI, STORECLOSETM_FRI,
            STOREOPENTM_SAT, STORECLOSETM_SAT,
            STOREOPENTM_SUN, STORECLOSETM_SUN
        )
        VALUES (/* 對應參數 */)";
}
```

### 2.3 更新邏輯（UPDATE）
比較 `nsp_store_profile` 與 `store_profile` 的差異，僅更新不同的欄位：

```csharp
private bool UpdateExistingStoreProfile(nsp_store_profileDTO nspRecord, store_profileDTO existingRecord)
{
    List<string> updates = new List<string>();
    Dictionary<string, object> parameters = new Dictionary<string, object>();
    
    // 比較各欄位並建立更新SQL
    if (nspRecord.shop_name != existingRecord.shop_name)
    {
        updates.Add("shop_name = @shop_name");
        parameters.Add("shop_name", nspRecord.shop_name);
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
```

## API端點

### 3.1 控制器端點
```csharp
[ApiController]
[Route("api/[controller]")]
public class NSPStoreSyncController : ControllerBase
{
    // Oracle連接測試
    [HttpGet("test-oracle-connection")]
    public IActionResult TestOracleConnection()
    
    // 第一階段同步
    [HttpPost("sync-from-oracle")]
    public IActionResult SyncFromOracle()
    
    // 第二階段批次同步
    [HttpPost("batch-sync-to-store-profile")]
    public IActionResult BatchSyncToStoreProfile()
}
```

### 3.2 API使用範例
```javascript
// 測試Oracle連接
fetch('/api/NSPStoreSync/test-oracle-connection')
    .then(response => response.text())
    .then(data => console.log(data));

// 執行Oracle同步
fetch('/api/NSPStoreSync/sync-from-oracle', { method: 'POST' })
    .then(response => response.text())
    .then(data => console.log(data));

// 執行批次同步
fetch('/api/NSPStoreSync/batch-sync-to-store-profile', { method: 'POST' })
    .then(response => response.text())
    .then(data => console.log(data));
```

## 背景服務設定

### 4.1 服務註冊
```csharp
// Program.cs
builder.Services.AddHostedService<NSPStoreSyncBackgroundService>();
```

### 4.2 設定檔案
```json
{
  "NSPStoreSync": {
    "Enabled": true,
    "IntervalHours": 24,
    "OracleConnection": {
      // Oracle連接設定
    }
  }
}
```

## 錯誤處理與日誌

### 5.1 日誌記錄
所有操作都會記錄到 `TB_Control_Log` 表：
- **成功操作**: Status = "1"
- **失敗操作**: Status = "0"  
- **詳細訊息**: Exception欄位記錄操作詳情

### 5.2 例外處理
- Oracle連接失敗：記錄錯誤但不中斷服務
- 資料轉換錯誤：跳過問題記錄，繼續處理其他資料
- PostgreSQL操作失敗：回滾交易，記錄詳細錯誤

## 測試與驗證

### 6.1 測試頁面
位置：`https://localhost:50302/nsp-sync-test.html`

功能：
- Oracle連接測試
- 手動觸發第一階段同步
- 手動觸發第二階段批次同步
- 即時查看執行結果

### 6.2 測試流程
1. **Oracle連接測試**: 確認可存取 `SPADMUSER.VIEW_DP2FTT`
2. **第一階段同步**: 驗證資料從Oracle正確同步到 `nsp_store_profile`
3. **第二階段同步**: 驗證資料正確比對並更新到 `store_profile`
4. **資料驗證**: 檢查欄位對應關係和資料完整性

## 故障排除

### 7.1 常見問題
**問題**: Oracle連接成功但找不到表格
**解決**: 使用 `all_tables` 和 `all_views` 查詢，並確認正確的Schema名稱

**問題**: 權限不足無法存取VIEW_DP2FTT  
**解決**: 使用完整的表格名稱 `SPADMUSER.VIEW_DP2FTT`

**問題**: 資料同步時發生欄位對應錯誤
**解決**: 檢查欄位名稱大小寫和資料型別匹配

### 7.2 監控建議
- 定期檢查背景服務運行狀態
- 監控 `TB_Control_Log` 表的錯誤記錄
- 驗證同步後的資料完整性
- 設定同步失敗的告警機制

## 未來改進建議

1. **增量同步**: 實作基於時間戳的增量同步機制
2. **資料驗證**: 加強資料完整性和一致性檢查
3. **性能優化**: 對大量資料的批次處理優化
4. **監控告警**: 整合系統監控和告警機制
5. **配置管理**: 提供更靈活的同步策略配置

## 附錄

### A.1 相關檔案清單
- `Models/Handler/NSPStoreSyncHandler.cs` - 主要處理器
- `Controllers/NSPStoreSyncController.cs` - API控制器  
- `Background/NSPStoreSyncBackgroundService.cs` - 背景服務
- `wwwroot/nsp-sync-test.html` - 測試頁面

### A.2 資料庫表結構
#### nsp_store_profile
```sql
CREATE TABLE nsp_store_profile (
    company_leaves VARCHAR(10),
    store_type VARCHAR(20),
    channel VARCHAR(50),
    area VARCHAR(100),
    shop_name VARCHAR(200),
    ivr_code VARCHAR(20) PRIMARY KEY,
    email VARCHAR(100),
    owner_empno VARCHAR(20),
    as_empno VARCHAR(20),
    store_tel VARCHAR(50),
    fax_tel VARCHAR(50),
    address TEXT,
    ftt_synctime TIMESTAMP,
    -- 營業時間欄位
    STOREOPENTM_MON VARCHAR(10),
    STORECLOSETM_MON VARCHAR(10),
    -- ... 其他星期
);
```

#### store_profile  
```sql
CREATE TABLE store_profile (
    company_leaves VARCHAR(10),
    store_type VARCHAR(20),
    channel VARCHAR(50),
    area VARCHAR(100),
    shop_name VARCHAR(200),
    ivr_code VARCHAR(20) PRIMARY KEY,
    email VARCHAR(100),
    owner_empno VARCHAR(20),
    as_empno VARCHAR(20),
    store_tel VARCHAR(50),
    fax_tel VARCHAR(50),
    address TEXT,
    create_time TIMESTAMP,
    update_time TIMESTAMP,
    -- 營業時間欄位
    STOREOPENTM_MON VARCHAR(10),
    STORECLOSETM_MON VARCHAR(10),
    -- ... 其他星期
);
```

---
**文件結束**
