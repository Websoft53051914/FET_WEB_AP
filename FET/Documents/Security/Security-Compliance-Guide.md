# 系統安全性與合規指南

## 📋 目錄
- [安全架構概述](#安全架構概述)
- [身份認證與授權](#身份認證與授權)
- [資料加密與保護](#資料加密與保護)
- [網路安全防護](#網路安全防護)
- [應用程式安全](#應用程式安全)
- [資料庫安全](#資料庫安全)
- [日誌與監控](#日誌與監控)
- [合規要求](#合規要求)
- [安全事件響應](#安全事件響應)
- [安全檢核清單](#安全檢核清單)

---

## 安全架構概述

FTT 系統採用多層防禦架構，確保系統與資料的安全性，符合企業資安要求與法規合規標準。

### 安全架構圖
```
🔒 安全防護層級:
├── 🌐 網路層防護 (防火牆、DDoS 防護)
├── 🏢 應用層防護 (WAF、API Gateway)
├── 🔐 認證層防護 (JWT、Multi-Factor Auth)
├── 📱 應用程式防護 (輸入驗證、CSRF 防護)
├── 🗄️ 資料層防護 (加密、存取控制)
└── 📊 監控層防護 (日誌分析、異常偵測)
```

### 核心安全原則
- **最小權限原則**: 使用者僅獲得執行工作所需的最小權限
- **縱深防禦**: 多層安全控制措施
- **零信任架構**: 不信任任何內部或外部連線
- **資料分類保護**: 依據資料敏感性實施不同保護等級

---

## 身份認證與授權

### JWT Token 認證機制

#### Token 生成與驗證
```csharp
public class JwtTokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly int _expireMinutes;

    public string GenerateToken(UserInfo user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("empNo", user.EmpNo),
                new Claim("empName", user.EmpName),
                new Claim("role", user.Role),
                new Claim("storeCode", user.StoreCode ?? ""),
                new Claim("loginTime", DateTime.UtcNow.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(_expireMinutes),
            Issuer = _issuer,
            Audience = _issuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
```

#### 安全性設定
```json
{
  "JwtSettings": {
    "SecretKey": "[256-bit 隨機金鑰，定期更新]",
    "Issuer": "FTT_System",
    "ExpireMinutes": 30,
    "RequireHttps": true,
    "RefreshTokenExpireDays": 7
  }
}
```

### 密碼安全政策

#### 密碼複雜度要求
```csharp
public class PasswordPolicy
{
    public static readonly PasswordRequirements Requirements = new()
    {
        MinLength = 12,                    // 最少 12 字元
        RequireUppercase = true,           // 必須包含大寫字母
        RequireLowercase = true,           // 必須包含小寫字母
        RequireDigit = true,               // 必須包含數字
        RequireSpecialChar = true,         // 必須包含特殊字元
        MaxConsecutiveChars = 2,           // 最多 2 個連續相同字元
        DisallowCommonPasswords = true,    // 禁止常見密碼
        PasswordHistoryCount = 5           // 記住最近 5 組密碼
    };

    // 密碼強度驗證
    public static PasswordValidationResult ValidatePassword(string password, string username)
    {
        var result = new PasswordValidationResult();
        
        // 長度檢查
        if (password.Length < Requirements.MinLength)
        {
            result.Errors.Add($"密碼長度至少需要 {Requirements.MinLength} 字元");
        }
        
        // 複雜度檢查
        if (Requirements.RequireUppercase && !password.Any(char.IsUpper))
        {
            result.Errors.Add("密碼必須包含至少一個大寫字母");
        }
        
        // 禁止包含使用者名稱
        if (password.ToLower().Contains(username.ToLower()))
        {
            result.Errors.Add("密碼不可包含使用者名稱");
        }
        
        return result;
    }
}
```

### 多因素認證 (MFA)
```csharp
public class MfaService
{
    // TOTP (Time-based One-Time Password) 實作
    public string GenerateTotpSecret()
    {
        var key = new byte[20];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return Base32Encoding.ToString(key);
    }
    
    public bool ValidateTotp(string secret, string code)
    {
        var otp = new Totp(Base32Encoding.ToBytes(secret));
        return otp.VerifyTotp(code, out long timeStepMatched);
    }
}
```

---

## 資料加密與保護

### 資料分類與保護等級

#### 資料分類標準
```sql
-- 資料分類表
CREATE TABLE tb_data_classification (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    dataType NVARCHAR(50) NOT NULL,      -- 資料類型
    classification NVARCHAR(20) NOT NULL, -- 分類等級
    encryptionRequired BIT NOT NULL,      -- 是否需要加密
    retentionPeriod INT NULL,            -- 保存期限(天)
    accessLevel NVARCHAR(20) NOT NULL,   -- 存取等級
    
    CONSTRAINT CK_Classification CHECK (classification IN ('PUBLIC', 'INTERNAL', 'CONFIDENTIAL', 'RESTRICTED'))
);

-- 範例資料分類
INSERT INTO tb_data_classification VALUES
('USER_PASSWORD', 'RESTRICTED', 1, NULL, 'ADMIN_ONLY'),
('USER_EMAIL', 'CONFIDENTIAL', 1, 2555, 'AUTHORIZED_ONLY'),
('REPORT_DATA', 'INTERNAL', 0, 2190, 'ROLE_BASED'),
('SYSTEM_LOG', 'INTERNAL', 0, 365, 'ADMIN_ONLY'),
('PUBLIC_NOTICE', 'PUBLIC', 0, 90, 'ALL_USERS');
```

### 敏感資料加密

#### 資料庫欄位加密
```csharp
public class DataEncryption
{
    private readonly string _encryptionKey;
    
    // AES-256 加密
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;
            
        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_encryptionKey);
            aes.GenerateIV();
            
            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
    
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;
            
        var fullCipher = Convert.FromBase64String(cipherText);
        
        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_encryptionKey);
            
            var iv = new byte[aes.BlockSize / 8];
            var cipher = new byte[fullCipher.Length - iv.Length];
            
            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);
            
            aes.IV = iv;
            
            using (var decryptor = aes.CreateDecryptor())
            using (var ms = new MemoryStream(cipher))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
```

#### 密碼雜湊處理
```csharp
public class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100000;

    public string HashPassword(string password)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);
            
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var key = pbkdf2.GetBytes(KeySize);
                var result = new byte[SaltSize + KeySize];
                
                Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
                Buffer.BlockCopy(key, 0, result, SaltSize, KeySize);
                
                return Convert.ToBase64String(result);
            }
        }
    }
    
    public bool VerifyPassword(string password, string hash)
    {
        var hashBytes = Convert.FromBase64String(hash);
        var salt = new byte[SaltSize];
        
        Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
        
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            var key = pbkdf2.GetBytes(KeySize);
            
            for (int i = 0; i < KeySize; i++)
            {
                if (hashBytes[i + SaltSize] != key[i])
                    return false;
            }
            
            return true;
        }
    }
}
```

---

## 網路安全防護

### HTTPS 強制實施
```csharp
public class SecurityHeadersMiddleware
{
    public async Task Invoke(HttpContext context, RequestDelegate next)
    {
        // 強制 HTTPS
        if (!context.Request.IsHttps)
        {
            var httpsUrl = $"https://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
            context.Response.Redirect(httpsUrl, permanent: true);
            return;
        }
        
        // 設定安全 Headers
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none';"
        );
        
        await next(context);
    }
}
```

### IP 白名單控制
```csharp
public class IpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly List<string> _whitelist;

    public IpWhitelistMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _whitelist = configuration.GetSection("Security:IpWhitelist").Get<List<string>>() ?? new List<string>();
    }

    public async Task Invoke(HttpContext context)
    {
        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        
        // 管理員功能需要 IP 白名單驗證
        if (context.Request.Path.StartsWithSegments("/Admin") && 
            !IsIpWhitelisted(remoteIp))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access denied from your IP address.");
            return;
        }

        await _next(context);
    }

    private bool IsIpWhitelisted(string ip)
    {
        if (string.IsNullOrEmpty(ip))
            return false;
            
        return _whitelist.Any(whitelistIp => 
            IPAddress.Parse(ip).Equals(IPAddress.Parse(whitelistIp)) ||
            IsInRange(ip, whitelistIp));
    }
}
```

---

## 應用程式安全

### 輸入驗證與清理
```csharp
public class InputValidator
{
    // SQL Injection 防護
    public static bool IsSqlInjectionAttempt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;
            
        var sqlKeywords = new[]
        {
            "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER",
            "EXEC", "EXECUTE", "UNION", "SCRIPT", "DECLARE", "--", "/*", "*/"
        };
        
        return sqlKeywords.Any(keyword => 
            input.ToUpper().Contains(keyword));
    }
    
    // XSS 防護
    public static string SanitizeHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
            
        // 移除潛在危險的 HTML 標籤和屬性
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.Add("b", "i", "u", "br", "p");
        sanitizer.AllowedAttributes.Clear();
        
        return sanitizer.Sanitize(input);
    }
    
    // 檔案上傳驗證
    public static ValidationResult ValidateUploadFile(IFormFile file)
    {
        var result = new ValidationResult();
        
        // 檔案大小限制
        if (file.Length > 10 * 1024 * 1024) // 10MB
        {
            result.Errors.Add("檔案大小不可超過 10MB");
        }
        
        // 允許的檔案類型
        var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        
        if (!allowedTypes.Contains(extension))
        {
            result.Errors.Add($"不允許的檔案類型: {extension}");
        }
        
        // 檔案內容驗證 (防止檔案類型偽裝)
        var fileBytes = new byte[file.Length];
        file.OpenReadStream().Read(fileBytes, 0, (int)file.Length);
        
        if (!IsValidFileSignature(fileBytes, extension))
        {
            result.Errors.Add("檔案內容與副檔名不符");
        }
        
        return result;
    }
}
```

### CSRF 防護
```csharp
[AutoValidateAntiforgeryToken]
public class BaseProjectController : Controller
{
    // 所有 POST 請求自動驗證 CSRF Token
    
    protected IActionResult JsonOK(object data = null, string message = "操作成功")
    {
        return Json(new
        {
            success = true,
            message = message,
            data = data,
            timestamp = DateTime.UtcNow
        });
    }
}
```

### Session 安全設定
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.Strict;
        options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
        options.Secure = CookieSecurePolicy.Always;
    });
    
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "__FTT_Session";
    });
}
```

---

## 資料庫安全

### 連線安全設定
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=db-server;Initial Catalog=FTT_DB;User ID=ftt_app;Password=[加密密碼];Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Command Timeout=300"
  }
}
```

### 資料庫使用者權限控制
```sql
-- 建立專用資料庫使用者
CREATE LOGIN ftt_app WITH PASSWORD = '[強密碼]';
CREATE USER ftt_app FOR LOGIN ftt_app;

-- 授予最小必要權限
GRANT SELECT, INSERT, UPDATE ON SCHEMA::dbo TO ftt_app;
DENY DELETE ON tb_user TO ftt_app;  -- 禁止刪除使用者資料
DENY ALTER ON SCHEMA::dbo TO ftt_app; -- 禁止修改結構

-- 建立唯讀使用者 (用於報表查詢)
CREATE LOGIN ftt_reader WITH PASSWORD = '[強密碼]';
CREATE USER ftt_reader FOR LOGIN ftt_reader;
GRANT SELECT ON SCHEMA::dbo TO ftt_reader;

-- 敏感資料存取記錄
CREATE TABLE tb_sensitive_data_access (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    userId NVARCHAR(50) NOT NULL,
    tableName NVARCHAR(100) NOT NULL,
    operation NVARCHAR(20) NOT NULL,
    recordId NVARCHAR(50) NULL,
    accessTime DATETIME2(3) DEFAULT GETDATE(),
    ipAddress NVARCHAR(45) NULL,
    userAgent NVARCHAR(500) NULL
);
```

### 資料遮罩與匿名化
```sql
-- 動態資料遮罩 (Dynamic Data Masking)
CREATE TABLE tb_user_masked (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    empNo NVARCHAR(20) NOT NULL,
    empName NVARCHAR(50) MASKED WITH (FUNCTION = 'partial(1,"***",1)') NOT NULL,
    email NVARCHAR(100) MASKED WITH (FUNCTION = 'email()') NULL,
    phone NVARCHAR(20) MASKED WITH (FUNCTION = 'partial(2,"***",2)') NULL,
    idNumber NVARCHAR(20) MASKED WITH (FUNCTION = 'partial(3,"***",2)') NULL
);

-- 為非管理員使用者建立遮罩檢視
CREATE VIEW vw_user_info AS
SELECT 
    id,
    empNo,
    CASE 
        WHEN IS_MEMBER('admin_role') = 1 THEN empName
        ELSE LEFT(empName, 1) + '***'
    END AS empName,
    CASE 
        WHEN IS_MEMBER('admin_role') = 1 THEN email
        ELSE '***@' + SUBSTRING(email, CHARINDEX('@', email) + 1, LEN(email))
    END AS email
FROM tb_user;
```

---

## 日誌與監控

### 安全事件日誌
```csharp
public class SecurityLogger
{
    private readonly ILogger<SecurityLogger> _logger;
    
    public void LogSecurityEvent(SecurityEventType eventType, string userId, string description, string ipAddress = null)
    {
        var logEntry = new
        {
            EventType = eventType.ToString(),
            UserId = userId,
            Description = description,
            IpAddress = ipAddress ?? "Unknown",
            Timestamp = DateTime.UtcNow,
            Severity = GetSeverityLevel(eventType)
        };
        
        _logger.LogWarning("Security Event: {@SecurityEvent}", logEntry);
        
        // 高風險事件立即發送告警
        if (IsHighRiskEvent(eventType))
        {
            SendSecurityAlert(logEntry);
        }
    }
    
    public enum SecurityEventType
    {
        LoginSuccess,
        LoginFailure,
        LoginLockout,
        PasswordChange,
        PermissionDenied,
        SuspiciousActivity,
        DataExport,
        AdminAccess,
        SystemError
    }
}
```

### 即時威脅偵測
```csharp
public class ThreatDetectionService
{
    private readonly ILogger<ThreatDetectionService> _logger;
    private readonly Dictionary<string, LoginAttemptCounter> _loginAttempts;

    public async Task<bool> AnalyzeLoginAttempt(string username, string ipAddress, bool success)
    {
        var key = $"{username}:{ipAddress}";
        
        if (!_loginAttempts.ContainsKey(key))
        {
            _loginAttempts[key] = new LoginAttemptCounter();
        }
        
        var counter = _loginAttempts[key];
        
        if (success)
        {
            counter.Reset();
            return false; // 無威脅
        }
        
        counter.IncrementFailure();
        
        // 5 次失敗嘗試觸發告警
        if (counter.FailureCount >= 5)
        {
            await AlertBruteForceAttack(username, ipAddress, counter.FailureCount);
            return true; // 偵測到威脅
        }
        
        return false;
    }
    
    // 偵測異常登入模式
    public async Task<bool> DetectAnomalousLogin(string username, string ipAddress, string userAgent)
    {
        var user = await GetUserLoginHistory(username);
        
        // 檢查是否為新的 IP 位址
        if (!user.KnownIpAddresses.Contains(ipAddress))
        {
            await AlertNewLocationLogin(username, ipAddress);
        }
        
        // 檢查登入時間是否異常
        var currentHour = DateTime.Now.Hour;
        if (currentHour < 6 || currentHour > 22)
        {
            if (!user.HasNightShiftAccess)
            {
                await AlertOffHoursLogin(username, currentHour);
                return true;
            }
        }
        
        return false;
    }
}
```

---

## 合規要求

### 個資法合規 (GDPR/PDPA)

#### 個人資料處理記錄
```sql
CREATE TABLE tb_personal_data_processing (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    dataSubjectId NVARCHAR(50) NOT NULL,    -- 資料主體 ID
    processingPurpose NVARCHAR(200) NOT NULL, -- 處理目的
    legalBasis NVARCHAR(100) NOT NULL,       -- 法源依據
    dataCategories NVARCHAR(500) NOT NULL,   -- 資料類別
    retentionPeriod INT NOT NULL,            -- 保存期限
    processingDate DATETIME2(3) DEFAULT GETDATE(),
    consentGiven BIT DEFAULT 0,              -- 是否取得同意
    consentWithdrawn BIT DEFAULT 0,          -- 是否撤回同意
    
    INDEX IX_DataSubject (dataSubjectId),
    INDEX IX_ProcessingDate (processingDate)
);
```

#### 資料主體權利實現
```csharp
public class GdprComplianceService
{
    // 資料可攜權 (Right to Data Portability)
    public async Task<PersonalDataExport> ExportPersonalData(string userId)
    {
        var userData = await _userRepository.GetUserDataAsync(userId);
        var reportData = await _reportRepository.GetUserReportsAsync(userId);
        
        return new PersonalDataExport
        {
            UserId = userId,
            ExportDate = DateTime.UtcNow,
            UserProfile = userData,
            Reports = reportData,
            ProcessingHistory = await GetProcessingHistoryAsync(userId)
        };
    }
    
    // 被遺忘權 (Right to be Forgotten)
    public async Task<bool> AnonymizeUserData(string userId)
    {
        using var transaction = _context.Database.BeginTransaction();
        
        try
        {
            // 匿名化個人識別資訊
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE tb_user SET 
                    empName = 'ANONYMIZED_' + CAST(id AS NVARCHAR),
                    email = NULL,
                    phone = NULL,
                    idNumber = NULL
                WHERE empNo = {0}", userId);
            
            // 記錄匿名化操作
            await LogAnonymizationAsync(userId);
            
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

### SOX 法案合規

#### 內控制度實施
```csharp
public class SoxComplianceAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var userId = context.HttpContext.User.FindFirst("empNo")?.Value;
        var actionName = context.ActionDescriptor.DisplayName;
        var controllerName = context.Controller.GetType().Name;
        
        // 記錄所有財務相關操作
        if (IsFinancialOperation(controllerName, actionName))
        {
            LogFinancialOperation(userId, actionName, context.HttpContext);
        }
        
        // 職責分離檢查
        if (RequiresDualApproval(actionName))
        {
            ValidateDualApproval(context);
        }
        
        base.OnActionExecuted(context);
    }
}
```

---

## 安全事件響應

### 事件分類與響應等級

```csharp
public enum SecurityIncidentLevel
{
    Low = 1,      // 資訊收集、一般掃描
    Medium = 2,   // 登入異常、權限濫用
    High = 3,     // 資料外洩、系統入侵
    Critical = 4  // 大規模攻擊、核心系統癱瘓
}

public class IncidentResponse
{
    public async Task HandleSecurityIncident(SecurityIncident incident)
    {
        switch (incident.Level)
        {
            case SecurityIncidentLevel.Critical:
                await ExecuteCriticalResponse(incident);
                break;
            case SecurityIncidentLevel.High:
                await ExecuteHighLevelResponse(incident);
                break;
            case SecurityIncidentLevel.Medium:
                await ExecuteMediumLevelResponse(incident);
                break;
            case SecurityIncidentLevel.Low:
                await ExecuteLowLevelResponse(incident);
                break;
        }
    }
    
    private async Task ExecuteCriticalResponse(SecurityIncident incident)
    {
        // 1. 立即隔離受影響系統
        await IsolateAffectedSystems(incident.AffectedSystems);
        
        // 2. 通知緊急應變團隊
        await NotifyEmergencyResponseTeam(incident);
        
        // 3. 啟動備援系統
        await ActivateBackupSystems();
        
        // 4. 保存證據
        await PreserveEvidence(incident);
        
        // 5. 通知管理層和法務
        await NotifyManagementAndLegal(incident);
    }
}
```

### 自動化回應機制
```csharp
public class AutomatedSecurityResponse
{
    // 自動封鎖可疑 IP
    public async Task BlockSuspiciousIp(string ipAddress, string reason)
    {
        await _firewallService.AddBlockRule(ipAddress, TimeSpan.FromHours(24));
        await _alertService.SendSecurityAlert($"IP {ipAddress} has been blocked. Reason: {reason}");
        
        _logger.LogWarning("Automatically blocked IP {IpAddress} due to {Reason}", ipAddress, reason);
    }
    
    // 自動鎖定帳號
    public async Task LockUserAccount(string userId, string reason)
    {
        await _userService.LockAccount(userId);
        await _notificationService.NotifySecurityTeam($"User account {userId} locked: {reason}");
        
        _logger.LogWarning("Automatically locked user account {UserId} due to {Reason}", userId, reason);
    }
    
    // 系統完整性檢查
    public async Task<SystemIntegrityReport> PerformIntegrityCheck()
    {
        var report = new SystemIntegrityReport();
        
        // 檢查重要檔案是否被修改
        report.FileIntegrityResults = await CheckFileIntegrity();
        
        // 檢查資料庫完整性
        report.DatabaseIntegrityResults = await CheckDatabaseIntegrity();
        
        // 檢查系統設定
        report.ConfigurationResults = await CheckSystemConfiguration();
        
        return report;
    }
}
```

---

## 安全檢核清單

### 日常安全檢查
```
📋 每日安全檢查項目:
├── ✅ 檢查系統登入記錄異常
├── ✅ 檢視安全事件日誌
├── ✅ 確認備份作業正常執行
├── ✅ 檢查防毒軟體更新狀態
├── ✅ 驗證 SSL 憑證有效性
├── ✅ 檢查系統更新與修補程式
└── ✅ 確認監控系統運作正常
```

### 週期性安全評估
```
📊 每月安全評估項目:
├── 🔍 弱點掃描與評估
├── 👥 使用者權限審核
├── 🔐 密碼政策合規檢查
├── 📊 安全事件統計分析
├── 🗂️ 資料分類與保護檢核
├── 📋 合規要求檢查
└── 🎯 安全培訓與教育執行
```

### 年度安全檢核
```
🎯 年度安全檢核項目:
├── 🔒 全面滲透測試
├── 📋 災難復原計畫測試
├── 👤 社交工程測試
├── 🏛️ 合規稽核與認證
├── 📚 安全政策與程序更新
├── 💼 第三方安全評估
└── 🎓 員工安全意識調查
```

---

## 📞 安全事件聯絡資訊

### 🚨 緊急安全事件 (24/7)
- **資安事件專線**: (02) 1234-5678 #999
- **Email**: security-incident@fet.com.tw
- **Line 群組**: FTT_Security_Team

### 👨‍💻 一般安全諮詢
- **Email**: security@fet.com.tw
- **內部諮詢**: IT Security Portal

### 📝 安全事件回報格式
```
事件類型: [登入異常/資料外洩/系統入侵/其他]
發現時間: YYYY/MM/DD HH:MM
影響範圍: [系統/使用者/資料範圍]
嚴重程度: [低/中/高/緊急]
事件描述: [詳細描述]
初步應變措施: [已採取的行動]
聯絡人: [姓名/分機]
```

---

*本指南遵循 ISO 27001、NIST Cybersecurity Framework 等國際資安標準*

*最後更新: 2024年12月*
