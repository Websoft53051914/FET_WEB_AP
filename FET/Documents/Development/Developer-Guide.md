# 開發者指南

## 📋 目錄
- [開發環境設置](#開發環境設置)
- [專案結構說明](#專案結構說明)
- [核心技術架構](#核心技術架構)
- [資料庫設計規範](#資料庫設計規範)
- [API 開發規範](#api-開發規範)
- [前端開發規範](#前端開發規範)
- [測試與偵錯](#測試與偵錯)
- [版本控制規範](#版本控制規範)
- [程式碼審核](#程式碼審核)
- [部署與發佈](#部署與發佈)

---

## 開發環境設置

### 必要軟體
```
🛠️ 開發工具清單:
├── Visual Studio 2022 Professional (17.0+)
├── .NET 8.0 SDK
├── SQL Server 2019 或以上版本
├── SQL Server Management Studio (SSMS)
├── Node.js 18.0+ (用於前端資源建置)
├── Git for Windows
└── Postman (API 測試)
```

### IIS 設定
```powershell
# 啟用 IIS 功能
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirection
Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFxExtensibility45

# 安裝 ASP.NET Core Hosting Bundle
# 下載並安裝: https://dotnet.microsoft.com/download/dotnet/8.0
```

### 專案設定
1. **Clone 專案**
```bash
git clone <repository-url>
cd FET_WEB_AP/FET
```

2. **還原 NuGet 套件**
```bash
dotnet restore FET.sln
```

3. **建置專案**
```bash
dotnet build FET.sln --configuration Debug
```

4. **設定連線字串**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=FTT_DB;Integrated Security=True;TrustServerCertificate=true"
  }
}
```

---

## 專案結構說明

### 解決方案架構
```
📁 FET.sln
├── 📂 Const/ (常數與列舉定義)
│   ├── DbConst.cs (資料庫常數)
│   ├── Enum.cs (系統列舉)
│   ├── DTO/ (資料傳輸物件)
│   └── VO/ (值物件)
├── 📂 Core.8.Utility/ (核心工具類)
│   ├── Common/ (通用工具)
│   ├── Helper/ (輔助類)
│   └── Extensions/ (擴展方法)
├── 📂 Core.8.Utility.Web/ (Web 專用工具)
│   ├── Base/ (基底類)
│   └── HtmlHelperCustom/ (自訂 HTML Helper)
├── 📂 FTT_API/ (API 服務)
│   ├── Controllers/ (API 控制器)
│   ├── Models/ (資料模型)
│   └── Background/ (背景服務)
├── 📂 FTT_WEB/ (主要 Web 應用程式)
│   ├── Controllers/ (MVC 控制器)
│   ├── Views/ (視圖)
│   ├── Models/ (視圖模型)
│   └── wwwroot/ (靜態資源)
├── 📂 FTT_VENDER_API/ (廠商 API)
└── 📂 FTT_VENDER_WEB/ (廠商 Web 系統)
```

### 命名規範
```csharp
// 檔案命名
Controllers: {Name}Controller.cs
Models: {Name}Model.cs, {Name}VO.cs, {Name}DTO.cs
Views: {Action}.cshtml
Services: {Name}Service.cs, {Name}Handler.cs

// 類別命名 (PascalCase)
public class ReportController : BaseProjectController
public class DispatchProfileDTO

// 方法命名 (PascalCase)
public ActionResult GetReportList()
public async Task<bool> SendMailAsync()

// 變數命名 (camelCase)
private readonly IConfiguration _configuration;
public string userName { get; set; }

// 常數命名 (UPPER_CASE)
public const string DEFAULT_PASSWORD = "123456";
```

---

## 核心技術架構

### 後端技術棧
```
🏗️ 後端架構:
├── ASP.NET Core 8.0 MVC
├── Entity Framework Core 8.0
├── AutoMapper (物件對應)
├── Hangfire (背景作業)
├── JWT Bearer Authentication
├── Swagger/OpenAPI (API 文件)
├── NLog (日誌記錄)
└── FluentValidation (資料驗證)
```

### 前端技術棧
```
🎨 前端架構:
├── Razor Views + Bootstrap 5
├── jQuery 3.6+
├── Kendo UI (表格與控制項)
├── Chart.js (圖表)
├── Select2 (下拉選擇器)
├── DatePicker (日期選擇器)
└── Font Awesome (圖示)
```

### 資料存取模式
```csharp
// Repository Pattern 範例
public interface IReportRepository
{
    Task<IEnumerable<ReportVO>> GetReportsAsync(ReportSearchDTO criteria);
    Task<ReportVO> GetReportByIdAsync(int id);
    Task<bool> CreateReportAsync(ReportVO report);
    Task<bool> UpdateReportAsync(ReportVO report);
}

// Service Layer 範例
public class ReportService : IReportService
{
    private readonly IReportRepository _repository;
    private readonly IMapper _mapper;
    
    public ReportService(IReportRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<List<ReportDTO>> GetReportsAsync(ReportSearchDTO criteria)
    {
        var reports = await _repository.GetReportsAsync(criteria);
        return _mapper.Map<List<ReportDTO>>(reports);
    }
}
```

---

## 資料庫設計規範

### 命名規範
```sql
-- 資料表命名 (tb_ 前綴) - 以下為實際存在的資料表範例
tb_report (報修單主檔)
tb_user (使用者)
tb_mailpool (郵件池)
tb_mailserver (郵件伺服器設定)
tb_mailpool_rule (郵件告警規則)
TB_Control_Log (系統操作記錄，注意大小寫)

-- 欄位命名 (camelCase)
id, formNo, createTime, empName
isDeleted, sendStatus, errorMsg

-- 索引命名
IX_{table}_{column} (一般索引)
PK_{table} (主鍵)
FK_{table}_{reference} (外鍵)
```

### 資料型別標準
```sql
-- 主鍵
id BIGINT IDENTITY(1,1) PRIMARY KEY

-- 時間欄位
createTime DATETIME2(3) NOT NULL DEFAULT GETDATE()
updateTime DATETIME2(3) NULL

-- 字串欄位
shortText NVARCHAR(50)    -- 短文字
mediumText NVARCHAR(255)  -- 中等文字
longText NVARCHAR(MAX)    -- 長文字

-- 布林值
isDeleted BIT NOT NULL DEFAULT 0
isActive BIT NOT NULL DEFAULT 1

-- 狀態欄位
status TINYINT NOT NULL DEFAULT 1
```

### 必要欄位
```sql
-- 每個資料表都應包含的基本欄位
CREATE TABLE tb_example (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    -- 業務欄位 --
    createEmp NVARCHAR(50) NOT NULL,     -- 建立人員
    createTime DATETIME2(3) NOT NULL DEFAULT GETDATE(), -- 建立時間
    updateEmp NVARCHAR(50) NULL,         -- 更新人員
    updateTime DATETIME2(3) NULL,        -- 更新時間
    isDeleted BIT NOT NULL DEFAULT 0     -- 邏輯刪除
);
```

---

## API 開發規範

### RESTful API 設計
```csharp
// 控制器基本結構
[ApiController]
[Route("Api/[controller]")]
[Authorize]
public class ReportController : BaseProjectController
{
    // GET: 查詢資源
    [HttpGet]
    public async Task<IActionResult> GetReports([FromQuery] ReportSearchDTO criteria)
    {
        // 實作邏輯
    }
    
    // GET: 取得特定資源
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReport(long id)
    {
        // 實作邏輯
    }
    
    // POST: 建立資源
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportDTO dto)
    {
        // 實作邏輯
    }
    
    // PUT: 更新資源
    [HttpPut("{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReport(long id, [FromBody] UpdateReportDTO dto)
    {
        // 實作邏輯
    }
    
    // DELETE: 刪除資源
    [HttpDelete("{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReport(long id)
    {
        // 實作邏輯
    }
}
```

### 錯誤處理模式
```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleUnauthorizedException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericException(context, ex);
        }
    }
}
```

### 資料驗證
```csharp
public class CreateReportDTOValidator : AbstractValidator<CreateReportDTO>
{
    public CreateReportDTOValidator()
    {
        RuleFor(x => x.StoreCode)
            .NotEmpty().WithMessage("門市代碼不可為空")
            .Length(4, 10).WithMessage("門市代碼長度必須在 4-10 字元之間");
            
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("故障描述不可為空")
            .MaximumLength(1000).WithMessage("故障描述不可超過 1000 字元");
            
        RuleFor(x => x.ContactPhone)
            .NotEmpty().WithMessage("聯絡電話不可為空")
            .Matches(@"^[0-9\-\(\)\s]+$").WithMessage("聯絡電話格式不正確");
    }
}
```

---

## 前端開發規範

### JavaScript 編碼標準
```javascript
// 使用 ES6+ 語法
const API_BASE_URL = '/Api';

// 命名規範
const userName = 'admin';          // camelCase
const USER_ROLE = 'ADMIN';        // 常數 UPPER_CASE
function getUserInfo() { }         // camelCase
class ReportManager { }           // PascalCase

// AJAX 請求標準格式
async function submitReport(data) {
    try {
        showLoading();
        
        const response = await fetch(`${API_BASE_URL}/Report`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getToken()}`,
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(data)
        });
        
        const result = await response.json();
        
        if (result.success) {
            showSuccess(result.message);
            return result.data;
        } else {
            showError(result.message);
            return null;
        }
    } catch (error) {
        console.error('提交報修單失敗:', error);
        showError('系統錯誤，請稍後再試');
        return null;
    } finally {
        hideLoading();
    }
}

// 表單驗證
function validateReportForm() {
    const formData = {
        storeCode: $('#storeCode').val(),
        description: $('#description').val(),
        contactPhone: $('#contactPhone').val()
    };
    
    const errors = [];
    
    if (!formData.storeCode.trim()) {
        errors.push('門市代碼不可為空');
    }
    
    if (!formData.description.trim()) {
        errors.push('故障描述不可為空');
    }
    
    if (!formData.contactPhone.trim()) {
        errors.push('聯絡電話不可為空');
    } else if (!/^[0-9\-\(\)\s]+$/.test(formData.contactPhone)) {
        errors.push('聯絡電話格式不正確');
    }
    
    if (errors.length > 0) {
        showValidationErrors(errors);
        return false;
    }
    
    return true;
}
```

### CSS 編碼規範
```css
/* BEM 命名方式 */
.report-form { }                    /* Block */
.report-form__field { }             /* Element */
.report-form__field--required { }   /* Modifier */

/* 響應式設計 */
.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 15px;
}

@media (max-width: 768px) {
    .container {
        padding: 0 10px;
    }
}

/* 顏色變數 */
:root {
    --primary-color: #007bff;
    --success-color: #28a745;
    --warning-color: #ffc107;
    --danger-color: #dc3545;
    --gray-100: #f8f9fa;
    --gray-600: #6c757d;
}
```

---

## 測試與偵錯

### 單元測試
```csharp
[TestFixture]
public class ReportServiceTests
{
    private Mock<IReportRepository> _mockRepository;
    private Mock<IMapper> _mockMapper;
    private ReportService _service;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IReportRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ReportService(_mockRepository.Object, _mockMapper.Object);
    }

    [Test]
    public async Task GetReportById_ShouldReturnReport_WhenIdExists()
    {
        // Arrange
        var reportId = 1;
        var expectedReport = new ReportVO { Id = reportId, FormNo = "F2024010001" };
        _mockRepository.Setup(r => r.GetReportByIdAsync(reportId))
                      .ReturnsAsync(expectedReport);

        // Act
        var result = await _service.GetReportByIdAsync(reportId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(reportId, result.Id);
    }
}
```

### API 測試
```json
{
  "info": {
    "name": "FTT API Tests",
    "description": "FTT 系統 API 測試集"
  },
  "item": [
    {
      "name": "取得報修單清單",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer {{jwt_token}}"
          }
        ],
        "url": {
          "raw": "{{base_url}}/Api/Report/GetReports",
          "host": ["{{base_url}}"],
          "path": ["Api", "Report", "GetReports"]
        },
        "body": {
          "mode": "raw",
          "raw": "{\n  \"page\": 1,\n  \"pageSize\": 20\n}"
        }
      }
    }
  ]
}
```

### 日誌記錄
```csharp
public class ReportController : BaseProjectController
{
    private readonly ILogger<ReportController> _logger;

    public async Task<IActionResult> CreateReport([FromBody] CreateReportDTO dto)
    {
        try
        {
            _logger.LogInformation("開始建立報修單: {dto}", JsonSerializer.Serialize(dto));
            
            var result = await _reportService.CreateReportAsync(dto);
            
            _logger.LogInformation("成功建立報修單: {reportId}", result.Id);
            return JsonOK(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立報修單時發生錯誤: {dto}", JsonSerializer.Serialize(dto));
            return JsonValidFail("建立報修單失敗");
        }
    }
}
```

---

## 版本控制規範

### Git 分支策略
```
🌳 分支模式:
├── main (主分支 - 生產環境)
├── develop (開發分支)
├── feature/功能名稱 (功能分支)
├── hotfix/修正名稱 (緊急修正分支)
└── release/版本號 (發佈分支)
```

### Commit 訊息規範
```bash
# 格式: <類型>(<範圍>): <描述>
feat(API): 新增報修單查詢 API
fix(UI): 修正日期選擇器顯示問題
docs(README): 更新安裝說明
style(CSS): 調整按鈕樣式
refactor(Service): 重構郵件發送邏輯
test(Unit): 增加報修服務測試案例
chore(Build): 升級 NuGet 套件版本
```

### Git Flow 操作
```bash
# 建立功能分支
git checkout develop
git pull origin develop
git checkout -b feature/report-search

# 提交變更
git add .
git commit -m "feat(Search): 實作報修單搜尋功能"

# 推送到遠端
git push origin feature/report-search

# 建立 Pull Request
# 在 Azure DevOps 或 GitHub 上建立 PR

# 合併到 develop
git checkout develop
git merge feature/report-search
git push origin develop

# 清理分支
git branch -d feature/report-search
git push origin --delete feature/report-search
```

---

## 程式碼審核

### 審核清單
```
✅ 程式碼審核檢查項目:
├── 📋 功能正確性
│   ├── 功能符合需求規格
│   ├── 錯誤處理完整
│   └── 邊界條件考量
├── 🔧 程式碼品質
│   ├── 命名規範一致
│   ├── 程式結構清晰
│   └── 註解說明充足
├── 🚀 效能考量
│   ├── SQL 查詢效能
│   ├── 記憶體使用合理
│   └── 無不必要的計算
├── 🔒 安全性
│   ├── 輸入驗證完整
│   ├── SQL Injection 防護
│   └── XSS 攻擊防護
└── 🧪 測試覆蓋率
    ├── 單元測試充足
    ├── 整合測試完整
    └── 異常情況測試
```

### Code Review 範本
```markdown
## 程式碼審核報告

**審核者**: [審核者姓名]
**審核日期**: [日期]
**分支**: [分支名稱]
**變更範圍**: [變更說明]

### ✅ 通過項目
- [ ] 功能運作正常
- [ ] 程式碼風格符合規範
- [ ] 錯誤處理完整
- [ ] 效能表現良好
- [ ] 安全性檢查通過
- [ ] 測試覆蓋率足夠

### ❌ 需要改善項目
1. [具體問題描述]
2. [改善建議]

### 💡 建議改善
- [具體建議]

### 總體評分: ⭐⭐⭐⭐⭐ (1-5 分)
```

---

## 部署與發佈

### 建置流程
```yaml
# Azure DevOps Pipeline 範例
trigger:
- main
- develop

pool:
  vmImage: 'windows-latest'

variables:
  solution: '**/*.sln'
  buildPlatform: 'Any CPU'
  buildConfiguration: 'Release'

stages:
- stage: Build
  displayName: Build stage
  jobs:
  - job: Build
    displayName: Build
    steps:
    - task: NuGetToolInstaller@1

    - task: NuGetCommand@2
      inputs:
        restoreSolution: '$(solution)'

    - task: VSBuild@1
      inputs:
        solution: '$(solution)'
        msbuildArgs: '/p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:SkipInvalidConfigurations=true /p:PackageLocation="$(build.artifactStagingDirectory)"'
        platform: '$(buildPlatform)'
        configuration: '$(buildConfiguration)'

    - task: VSTest@2
      inputs:
        platform: '$(buildPlatform)'
        configuration: '$(buildConfiguration)'

- stage: Deploy
  displayName: Deploy stage
  dependsOn: Build
  condition: succeeded()
  jobs:
  - deployment: Deploy
    displayName: Deploy
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: IISWebAppDeploymentOnMachineGroup@0
            displayName: 'IIS Web App Deploy'
```

### 部署檢查清單
```
🚀 部署前檢查:
├── ✅ 程式碼測試通過
├── ✅ 資料庫遷移腳本準備
├── ✅ 組態設定檔更新
├── ✅ SSL 憑證有效
├── ✅ 備份計畫準備
├── ✅ 回滾計畫準備
└── ✅ 監控告警設定
```

### 發佈後驗證
```bash
# 健康檢查
curl -f http://localhost/health || exit 1

# API 功能測試
curl -f http://localhost/api/health || exit 1

# 資料庫連線測試
sqlcmd -S localhost -Q "SELECT 1" || exit 1
```

---

## 📚 參考資源

### 開發文件
- [ASP.NET Core 官方文件](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core 文件](https://docs.microsoft.com/ef/core/)
- [Kendo UI 文件](https://docs.telerik.com/kendo-ui/)

### 工具與套件
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [SQL Server Management Studio](https://docs.microsoft.com/sql/ssms/)
- [Postman](https://www.postman.com/)
- [Azure DevOps](https://dev.azure.com/)

### 最佳實務
- [.NET 應用程式架構指南](https://docs.microsoft.com/dotnet/architecture/)
- [ASP.NET Core 效能最佳實務](https://docs.microsoft.com/aspnet/core/performance/performance-best-practices)
- [Entity Framework Core 效能](https://docs.microsoft.com/ef/core/performance/)

---

*最後更新: 2024年12月*
