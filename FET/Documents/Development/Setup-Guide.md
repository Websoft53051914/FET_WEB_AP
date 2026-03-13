# 開發環境設定指南

## 系統需求

### 軟體需求
- **Visual Studio 2022** (或 Visual Studio Code)
- **.NET 8.0 SDK**
- **PostgreSQL 12+**
- **Git**

### 硬體需求
- RAM: 8GB 以上
- 硬碟空間: 5GB 以上
- CPU: Intel i5 或同等級以上

## 環境設定

### 1. 複製專案
```powershell
git clone [repository-url]
cd FET_WEB_AP/FET
```

### 2. 資料庫設定
1. 安裝並啟動 PostgreSQL
2. 建立資料庫 `infwf`
3. 建立使用者 `ftt` (密碼: `ftt123`)
4. 確認資料庫連線設定

### 3. 設定檔修改

#### 各專案的 appsettings.json 需要檢查的設定：

**連線字串**:
```json
"ConnectionStrings": {
  "MainConnection": "Server=localhost;Port=5432;Database=infwf;User Id=ftt;Password=ftt123;CommandTimeout=86400"
}
```

**系統 URL** (開發環境):
```json
"MailURL": "https://localhost:50102/Query?FuncId=Query_View&className=門市報修管理&form_no=",
"MailURL_VENDOR": "https://localhost:50402/Query?FuncId=Query_View&className=門市報修管理&form_no="
```

### 4. NuGet 套件還原
```powershell
dotnet restore
```

### 5. 建置專案
```powershell
dotnet build
```

## 專案啟動順序

### 開發環境啟動順序
1. **資料庫服務** - 確保 PostgreSQL 正在運行
2. **FTT_API** - 門市 API 服務 (Port: 50101)
3. **FTT_VENDER_API** - 廠商 API 服務 (Port: 50401)
4. **FTT_WEB** - 門市前端 (Port: 50102)  
5. **FTT_VENDER_WEB** - 廠商前端 (Port: 50402)

### 啟動命令
```powershell
# 方法1: 使用 Visual Studio
# 設定多重啟動專案，同時啟動所有 Web 專案

# 方法2: 使用命令列
cd FTT_API
dotnet run

cd ../FTT_VENDER_API  
dotnet run

cd ../FTT_WEB
dotnet run

cd ../FTT_VENDER_WEB
dotnet run
```

## 開發工具設定

### Visual Studio 設定
1. 設定多重啟動專案
   - 右鍵點擊解決方案 → 屬性
   - 選擇「多重啟動專案」
   - 設定所有 Web 專案為「啟動」

2. 偵錯設定
   - 確認各專案的 Port 設定
   - 檢查 SSL 憑證設定

### 程式碼規範
- 使用 C# 命名慣例
- 遵循 Microsoft 編碼標準
- 新增適當的 XML 註解

## 常見問題

### 1. 資料庫連線失敗
- 檢查 PostgreSQL 服務是否啟動
- 確認連線字串設定
- 檢查防火牆設定

### 2. Port 衝突
- 檢查 Properties/launchSettings.json
- 確認各專案使用不同 Port

### 3. SSL 憑證問題
```powershell
dotnet dev-certs https --trust
```

### 4. NuGet 套件問題
```powershell
dotnet nuget locals all --clear
dotnet restore
```

## 除錯模式

### API 測試
- **Swagger UI**: 各 API 專案都有整合 Swagger
  - FTT_API: https://localhost:50101/swagger
  - FTT_VENDER_API: https://localhost:50401/swagger

### 資料庫查詢工具
推薦使用：
- pgAdmin 4
- DBeaver
- Azure Data Studio (with PostgreSQL extension)

### 日誌查看
- 檢查各專案的 `logs/` 資料夾
- 使用 Visual Studio 輸出視窗
- 查看 IIS Express 日誌

## 部署前檢查清單
- [ ] 所有專案建置成功
- [ ] 單元測試通過
- [ ] 設定檔正確配置
- [ ] 資料庫連線正常
- [ ] API 端點可正常存取
- [ ] 前端頁面載入正常
