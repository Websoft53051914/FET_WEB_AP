# NSP門市資料同步功能

## 概述
此功能負責從Oracle資料庫的 `VIEW_DP2FTT` 檢視表同步門市資料到PostgreSQL的 `nsp_store_profile` 表。

## 功能特色
1. **背景自動同步**：定期自動從Oracle同步資料
2. **手動同步API**：提供RESTful API進行手動同步
3. **資料查詢功能**：可查詢單一門市或所有門市資料
4. **同步狀態監控**：可檢查同步狀態和統計資訊
5. **詳細日誌記錄**：記錄同步過程和錯誤資訊

## 檔案說明

### 核心檔案
- `Models/Handler/NSPStoreSyncHandler.cs`：核心同步邏輯處理器
- `Controllers/NSPStoreSyncController.cs`：REST API控制器
- `Background/NSPStoreSyncBackgroundService.cs`：背景服務
- `Common/OriginClass/EntiityClass/nsp_store_profileEntity.cs`：資料實體定義

### 設定檔案
- `appsettings.json`：包含同步間隔時間和Oracle連接資訊

## 資料對應關係

### Oracle VIEW_DP2FTT → PostgreSQL nsp_store_profile
| Oracle 欄位 | PostgreSQL 欄位 | 說明 |
|-------------|-----------------|------|
| N/A | company_leaves | 固定值 'FET' |
| STORESTYLE | store_type | 有值='RETAIL'，無值='FRANCHISE' |
| STORETYPE | channel | 通路 |
| REGIONNAME | area | 區域 |
| STORENAME | shop_name | 門市名稱 |
| STOREID | ivr_code | 門市代碼(主鍵) |
| EMAIL | email | 電子郵件 |
| storemanager_empno | owner_empno | 店長工號 |
| sales_empno | as_empno | 業務工號 |
| CONTACTNUM1 | store_tel | 門市電話 |
| FAXNUM | fax_tel | 傳真電話 |
| STOREADDRESS | address | 地址 |
| N/A | ftt_synctime | 同步時間(自動產生) |
| STOREOPENTM_MON~SUN | STOREOPENTM_MON~SUN | 營業時間 |
| STORECLOSETM_MON~SUN | STORECLOSETM_MON~SUN | 營業時間 |

## API 使用說明

### 1. 手動同步資料
```http
POST /api/NSPStoreSync/sync
```

**回應範例：**
```json
{
  "IsSuccess": true,
  "Message": "同步完成",
  "Data": "同步完成：處理 150 筆資料，成功插入 150 筆"
}
```

### 2. 查詢單一門市資料
```http
GET /api/NSPStoreSync/store/{ivrCode}
```

**參數：**
- `ivrCode`: 門市代碼

### 3. 查詢所有門市資料
```http
GET /api/NSPStoreSync/stores
```

### 4. 檢查同步狀態
```http
GET /api/NSPStoreSync/sync-status
```

**回應範例：**
```json
{
  "IsSuccess": true,
  "Message": "查詢成功",
  "Data": {
    "TotalStores": 150,
    "LastSyncTime": "2026-01-31T10:30:00",
    "SyncedToday": 150
  }
}
```

## 設定說明

### appsettings.json 設定
```json
{
  "NSPStoreSync": {
    "IntervalHours": 1,
    "OracleConnection": {
      "Host": "10.68.24.186",
      "Port": "1560",
      "ServiceName": "NSP",
      "UserId": "fttuser",
      "Password": "Ds!#Hj129s"
    }
  }
}
```

### 設定項目說明
- `IntervalHours`：背景服務同步間隔時間(小時)
- `OracleConnection`：Oracle資料庫連接資訊
  - `Host`：資料庫主機位址
  - `Port`：連接埠
  - `ServiceName`：服務名稱
  - `UserId`：使用者帳號
  - `Password`：密碼

## 部署說明

1. **確認相依套件**：確保專案已安裝Oracle.ManagedDataAccess.Core套件
2. **設定連接資訊**：在appsettings.json中正確設定Oracle連接資訊
3. **建立資料表**：確保PostgreSQL中已建立nsp_store_profile表
4. **啟動服務**：背景服務會在應用程式啟動時自動啟動

## 日誌監控

系統會將同步過程記錄到 `TB_Control_Log` 表中，可透過以下方式監控：
- 查看應用程式日誌
- 查詢TB_Control_Log表
- 使用同步狀態API

## 注意事項

1. **資料庫權限**：確保Oracle和PostgreSQL的連接帳號都有適當權限
2. **網路連接**：確保應用程式能連接到Oracle資料庫
3. **資料一致性**：目前採用全量更新模式，每次同步會清空現有資料
4. **效能考量**：大量資料同步可能影響系統效能，建議在低峰時段執行
5. **錯誤處理**：同步失敗時會記錄錯誤，但不會中斷背景服務

## 故障排除

### 常見問題
1. **Oracle連接失敗**：檢查網路連接和帳號密碼
2. **PostgreSQL寫入失敗**：檢查資料表結構和權限
3. **資料格式錯誤**：檢查Oracle檢視表的資料格式

### 檢查方法
1. 查看應用程式日誌
2. 查詢TB_Control_Log表
3. 使用API檢查同步狀態
4. 直接查詢nsp_store_profile表確認資料
