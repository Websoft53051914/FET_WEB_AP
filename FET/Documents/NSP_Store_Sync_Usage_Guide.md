# NSP門市資料同步 - 使用說明

## 快速開始

### 1. 設定檔案配置
在 `appsettings.json` 中添加Oracle連線設定：

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

### 2. 註冊服務
在 `Program.cs` 中註冊背景服務：

```csharp
builder.Services.AddHostedService<NSPStoreSyncBackgroundService>();
```

### 3. 手動測試

#### 方法一：使用測試頁面
1. 啟動API服務
2. 瀏覽器開啟：`https://localhost:50302/nsp-sync-test.html`
3. 點擊相關按鈕進行測試

#### 方法二：使用API端點
```bash
# 測試Oracle連接
curl -X GET "https://localhost:50302/api/NSPStoreSync/test-oracle-connection"

# 執行第一階段同步（Oracle → nsp_store_profile）
curl -X POST "https://localhost:50302/api/NSPStoreSync/sync-from-oracle"

# 執行第二階段同步（nsp_store_profile → store_profile）
curl -X POST "https://localhost:50302/api/NSPStoreSync/batch-sync-to-store-profile"
```

### 4. 同步流程

#### 完整同步流程：
1. **Oracle連接測試** → 確認可存取 `SPADMUSER.VIEW_DP2FTT`
2. **第一階段同步** → 從Oracle同步到 `nsp_store_profile`
3. **第二階段同步** → 從 `nsp_store_profile` 比對更新到 `store_profile`

#### 資料流向：
```
Oracle (SPADMUSER.VIEW_DP2FTT) 
    ↓ 
PostgreSQL (nsp_store_profile) 
    ↓ 
PostgreSQL (store_profile)
```

## 監控與維護

### 檢查同步狀態
查詢 `TB_Control_Log` 表格：
```sql
SELECT * FROM TB_Control_Log 
WHERE ControllerName = 'NSPStoreSyncHandler' 
ORDER BY ID DESC 
LIMIT 10;
```

### 檢查資料完整性
```sql
-- 檢查nsp_store_profile筆數
SELECT COUNT(*) FROM nsp_store_profile;

-- 檢查store_profile筆數
SELECT COUNT(*) FROM store_profile;

-- 檢查最近同步時間
SELECT MAX(ftt_synctime) FROM nsp_store_profile;
SELECT MAX(update_time) FROM store_profile;
```

### 常見問題排除

**問題1：Oracle連接失敗**
- 檢查網路連線和防火牆設定
- 確認Oracle服務是否運行
- 驗證使用者名稱和密碼

**問題2：找不到VIEW_DP2FTT**
- 確認使用完整表格名稱：`SPADMUSER.VIEW_DP2FTT`
- 檢查使用者權限

**問題3：背景服務未啟動**
- 檢查 `NSPStoreSync:Enabled` 設定
- 查看應用程式日誌

## 進階設定

### 調整同步頻率
修改 `appsettings.json`：
```json
{
  "NSPStoreSync": {
    "IntervalHours": 12  // 改為12小時同步一次
  }
}
```

### 停用自動同步
```json
{
  "NSPStoreSync": {
    "Enabled": false  // 停用背景自動同步
  }
}
```

## 安全注意事項

1. **資料庫密碼加密**：建議使用 Azure Key Vault 或環境變數
2. **網路安全**：確保Oracle和PostgreSQL連線使用SSL
3. **權限最小化**：使用者僅給予必要的讀取/寫入權限
4. **日誌管理**：定期清理 `TB_Control_Log` 表避免過大

## 性能調整

1. **批次大小**：可調整單次處理的資料筆數
2. **索引優化**：在 `ivr_code` 欄位上建立索引
3. **連線池**：調整資料庫連線池大小
4. **記憶體管理**：處理大量資料時注意記憶體使用

---

更多詳細資訊請參考：
- `NSP_Store_Sync_Implementation_20260131.md` - 完整實作文件
- `NSP_Store_Sync_Code_Examples.md` - 程式碼範例
