# FTT API 文件

## 概述
FTT_API 是門市報修管理系統的核心 API 服務，提供完整的 RESTful API 端點，支援報修流程的所有業務邏輯以及智能告警通知系統。

## 基礎資訊

### 服務端點
- **開發環境**: https://localhost:50101
- **Swagger UI**: https://localhost:50101/swagger
- **API 版本**: v1

### 認證機制
- **類型**: JWT Bearer Token (HS256 演算法)
- **Access Token 有效期**: 1800 秒 (30 分鐘)
- **Refresh Token 有效期**: 7 天
- **發行者**: FET
- **Token 格式**: `Authorization: Bearer {token}`
- **Cookie 名稱**: `authToken` (HttpOnly, Secure)
- **自動刷新**: Token 過期前 5 分鐘自動更新

### 回應格式
所有 API 回應都遵循統一的 JSON 格式：

```json
{
  "success": true,
  "message": "操作成功",
  "data": {
    // 實際資料內容
  },
  "errors": []
}
```

## 📧 告警通知系統架構

### 自動告警機制
系統採用事件驅動的智能告警機制，在報修流程的關鍵節點自動觸發郵件通知：

```
📧 告警觸發流程:
1. 業務事件發生 → 2. 規則引擎匹配 → 3. 收件人識別 → 4. 範本套用 → 5. 郵件池排程 → 6. 背景發送服務
```

### 核心組件
- **MailPoolHandler**: 負責郵件池管理與收件人識別
- **SendMailHandler**: 背景郵件發送服務（定期排程執行）
- **MailServerSetting**: 郵件伺服器設定管理
- **Hangfire Job**: 定時郵件發送任務調度

### 告警觸發點
```
🚨 系統告警觸發時機:
├── 新報修建立 (NEW → PENDING)
├── 審核通過/駁回 (PENDING → APPROVED/REJECTED)  
├── 派工指派 (APPROVED → ASSIGNED)
├── 維修中狀態 (ASSIGNED → IN_PROCESS)
├── 完工回報 (IN_PROCESS → COMPLETED)
├── 結案確認 (COMPLETED → CLOSED)
├── 逾期提醒 (REMINDER 機制)
└── 系統異常告警 (ERROR/EXCEPTION)
```

## API 端點清單

### 1. 資料查詢相關

#### 1.1 取得自行尋商維修品項分頁資料
```http
POST /Api/GetCiDataSelfVendorPageList
```

**說明**: 取得自行尋商開單的維修品項分頁資料

**請求參數**:
- `DataSourceRequest`: 分頁請求參數
  - `page`: 頁碼 (int)
  - `pageSize`: 每頁筆數 (int)
  - `sort`: 排序條件 (array)
  - `filter`: 篩選條件 (object)

**回應範例**:
```json
{
  "Data": [
    {
      "id": 1,
      "itemName": "維修項目名稱",
      "category": "分類",
      "description": "描述"
    }
  ],
  "Total": 100,
  "AggregateResults": null,
  "Errors": null
}
```

#### 1.2 取得維修品項樹狀結構子項目
```http
GET /Api/GetListTreeChildrenCi
```

**說明**: 根據父項目 ID 取得子項目清單

**查詢參數**:
- `parentId`: 父項目 ID (int, optional)
- `reqSrc`: 請求來源 (string, default: "ALL")
- `acType`: 動作類型 (string, optional)

**回應範例**:
```json
[
  {
    "id": 1,
    "name": "子項目名稱",
    "parentId": 0,
    "hasChildren": true,
    "level": 1
  }
]
```

#### 1.3 取得維修品項樹狀結構指定項目
```http
POST /Api/GetListTreeItemCi
```

**說明**: 根據 ID 清單取得指定的維修品項

**請求參數**:
```json
{
  "idList": [1, 2, 3],
  "reqSrc": "ALL",
  "acType": ""
}
```

#### 1.4 取得門市資料分頁清單
```http
POST /Api/GetPageListStore
```

**說明**: 取得門市資料的分頁清單

**請求參數**:
- `DataSourceRequest`: 分頁請求參數
- `DialogIvrCodeGridVO`: 門市查詢條件
  - `IvrCodeLike`: IVR代碼 (string)
  - `ShopNameLike`: 店名 (string)
  - `CompanyLeavesLike`: 公司別 (string)
  - `ChannelLike`: 通路 (string)
  - `StoreTypeLike`: 店格 (string)

**回應資料結構**:
```json
{
  "Data": [
    {
      "ivrCode": "店代碼",
      "shopName": "店名",
      "companyLeaves": "公司別",
      "channel": "通路",
      "storeType": "店格",
      "area": "區域",
      "ownerName": "店長/聯絡人",
      "asName": "區經理/業務",
      "ownerTel": "店長電話",
      "urgentTel": "緊急電話",
      "address": "地址"
    }
  ],
  "Total": 150
}
```

#### 1.5 取得廠商資料分頁清單
```http
POST /Api/GetPageListVender
```

**說明**: 取得廠商資料的分頁清單

**請求參數**:
- `DataSourceRequest`: 分頁請求參數
- `DialogVenderGridVO`: 廠商查詢條件

### 2. 系統功能相關

#### 2.1 取得選單資料統計
```http
POST /Api/GetMenuDataCount
```

**說明**: 根據功能 ID 清單取得選單資料統計

**請求參數**:
```json
[1, 2, 3, 4, 5]
```

**回應範例**:
```json
{
  "1": 25,
  "2": 10,
  "3": 5,
  "4": 0,
  "5": 15
}
```

#### 2.2 取得臨時 URL
```http
GET /Api/UrlGetTemp
```

**說明**: 產生臨時存取 URL

## 錯誤處理

### HTTP 狀態碼
- `200`: 成功
- `400`: 請求參數錯誤
- `401`: 未授權
- `403`: 禁止存取
- `404`: 資源不存在
- `500`: 伺服器內部錯誤

### 錯誤回應格式
```json
{
  "success": false,
  "message": "錯誤訊息",
  "data": null,
  "errors": [
    {
      "field": "欄位名稱",
      "message": "錯誤描述"
    }
  ]
}
```

## 認證與授權

### JWT Token 認證機制

#### Token 架構說明
FTT 系統採用 JWT (JSON Web Token) 作為主要的身份認證機制，提供無狀態的安全認證方案。

```json
{
  "header": {
    "typ": "JWT",
    "alg": "HS256"
  },
  "payload": {
    "empNo": "USER001",
    "empName": "張三",
    "role": "STORE_MANAGER",
    "storeCode": "0001",
    "loginTime": "2024-01-15T10:30:00Z",
    "iss": "FET",
    "aud": "FTT_System",
    "exp": 1642252200,
    "iat": 1642250400
  },
  "signature": "HMACSHA256(base64UrlEncode(header) + '.' + base64UrlEncode(payload), secret)"
}
```

### 取得 JWT Token
```http
POST /Auth/Login
Content-Type: application/json

{
  "username": "使用者名稱",
  "password": "密碼"
}
```

**回應範例**:
```json
{
  "success": true,
  "message": "登入成功",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "abc123def456...",
    "expires": "2024-01-15T11:00:00Z",
    "user": {
      "empNo": "USER001",
      "empName": "張三",
      "role": "STORE_MANAGER",
      "storeCode": "0001",
      "permissions": ["REPORT_CREATE", "REPORT_VIEW", "REPORT_UPDATE"]
    }
  }
}
```

### Token 刷新機制
```http
POST /Auth/RefreshToken
Content-Type: application/json
Authorization: Bearer {expired_jwt_token}

{
  "refreshToken": "abc123def456..."
}
```

**回應範例**:
```json
{
  "success": true,
  "message": "Token 刷新成功",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "new123refresh456...",
    "expires": "2024-01-15T12:00:00Z"
  }
}
```

### 使用 JWT Token
```http
Authorization: Bearer {your_jwt_token}
```

### Token 驗證失敗處理
```json
{
  "success": false,
  "message": "Token 無效或已過期",
  "data": null,
  "errors": [
    {
      "code": "INVALID_TOKEN",
      "message": "請重新登入"
    }
  ]
}
```

### 登出機制
```http
POST /Auth/Logout
Authorization: Bearer {your_jwt_token}
```

**說明**: 將 Token 加入黑名單，確保無法再次使用

### JWT 安全性設定

#### Token 有效期設定
- **Access Token**: 30 分鐘 (1800 秒)
- **Refresh Token**: 7 天
- **Remember Me**: 30 天 (選擇性)

#### 安全性措施
```javascript
// 前端 Token 管理範例
class TokenManager {
    static setToken(token, refreshToken) {
        // 使用 HttpOnly Cookie 儲存 (推薦)
        document.cookie = `authToken=${token}; HttpOnly; Secure; SameSite=Strict; Max-Age=1800`;
        
        // 或使用 localStorage (需注意 XSS 風險)
        // localStorage.setItem('authToken', token);
        // localStorage.setItem('refreshToken', refreshToken);
    }
    
    static getToken() {
        // 從 Cookie 或 localStorage 取得 Token
        return getCookieValue('authToken') || localStorage.getItem('authToken');
    }
    
    static async refreshTokenIfNeeded() {
        const token = this.getToken();
        if (!token) return false;
        
        // 檢查 Token 是否即將過期 (提前 5 分鐘更新)
        const payload = JSON.parse(atob(token.split('.')[1]));
        const expiryTime = payload.exp * 1000;
        const now = Date.now();
        
        if (expiryTime - now < 5 * 60 * 1000) {
            return await this.refreshToken();
        }
        
        return true;
    }
    
    static async refreshToken() {
        try {
            const refreshToken = localStorage.getItem('refreshToken');
            const response = await fetch('/Auth/RefreshToken', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.getToken()}`
                },
                body: JSON.stringify({ refreshToken })
            });
            
            const result = await response.json();
            if (result.success) {
                this.setToken(result.data.token, result.data.refreshToken);
                return true;
            }
        } catch (error) {
            console.error('Token refresh failed:', error);
        }
        
        // 刷新失敗，導向登入頁面
        this.clearTokens();
        window.location.href = '/login';
        return false;
    }
    
    static clearTokens() {
        document.cookie = 'authToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
        localStorage.removeItem('authToken');
        localStorage.removeItem('refreshToken');
    }
}

// 自動 Token 刷新 Interceptor
axios.interceptors.request.use(async (config) => {
    await TokenManager.refreshTokenIfNeeded();
    const token = TokenManager.getToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// 處理 401 未授權回應
axios.interceptors.response.use(
    (response) => response,
    async (error) => {
        if (error.response?.status === 401) {
            const success = await TokenManager.refreshToken();
            if (success) {
                // 重試原請求
                return axios.request(error.config);
            }
        }
        return Promise.reject(error);
    }
);
```

### 權限驗證機制

#### 角色權限對應
```json
{
  "roles": {
    "STORE_USER": {
      "permissions": ["REPORT_CREATE", "REPORT_VIEW"],
      "description": "門市使用者"
    },
    "STORE_MANAGER": {
      "permissions": ["REPORT_CREATE", "REPORT_VIEW", "REPORT_APPROVE"],
      "description": "門市主管"
    },
    "VENDOR_USER": {
      "permissions": ["REPORT_VIEW", "REPORT_ACCEPT", "REPORT_UPDATE"],
      "description": "廠商使用者"
    },
    "VENDOR_MANAGER": {
      "permissions": ["REPORT_VIEW", "REPORT_ACCEPT", "REPORT_UPDATE", "QUOTE_CREATE"],
      "description": "廠商主管"
    },
    "SYSTEM_ADMIN": {
      "permissions": ["*"],
      "description": "系統管理員"
    }
  }
}
```

#### API 權限驗證
```http
GET /Api/Report/GetList
Authorization: Bearer {token_with_REPORT_VIEW_permission}
```

**權限不足回應**:
```json
{
  "success": false,
  "message": "權限不足",
  "data": null,
  "errors": [
    {
      "code": "INSUFFICIENT_PERMISSION",
      "message": "您沒有檢視報修單的權限"
    }
  ]
}
```

## 📧 告警通知 API

### 1. 郵件池管理

#### 1.1 觸發告警通知
系統會在報修流程狀態變更時自動觸發郵件通知，無需手動呼叫。告警邏輯內建於以下業務 API 中：

- `POST /Api/NewOrder/Submit` - 新建報修後觸發
- `POST /Api/InProcess/UpdateStatus` - 狀態更新後觸發  
- `POST /Api/OnsitePrint/UpdateProgress` - 現場處理進度更新後觱發

#### 1.2 郵件發送狀態查詢
```http
GET /Api/MailPool/GetSendStatus?formNo={form_no}
```

**說明**: 查詢指定報修單的郵件發送狀態

**查詢參數**:
- `formNo`: 報修單號 (string, required)

**回應範例**:
```json
{
  "success": true,
  "data": [
    {
      "id": 123,
      "subject": "【FTT報修系統】新報修單 - F2024010001",
      "destinationEmail": "user@example.com",
      "destinationEmailCC": "manager@example.com,admin@example.com",
      "sendStatus": 1,
      "sendStatusText": "已發送",
      "estimateSendTime": "2024-01-15T10:30:00",
      "realSendTime": "2024-01-15T10:30:15",
      "errorMsg": null
    }
  ]
}
```

#### 1.3 重新發送失敗郵件
```http
POST /Api/MailPool/Resend
```

**說明**: 重新發送失敗的告警郵件

**請求參數**:
```json
{
  "mailPoolIds": [123, 456, 789]
}
```

**權限要求**: 需要系統管理員權限

### 2. 告警規則配置

#### 2.1 取得告警規則清單
```http
GET /Api/AlertRule/GetList
```

**說明**: 取得所有告警規則設定

**回應範例**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "mailType": "NEW,PENDING",
      "mailReciver": "REVIEWER",
      "mailReciverCC": "STORE_MANAGER,AREA_MANAGER",
      "mailSubject": "【FTT報修系統】新報修單 - ([FORM_NO])",
      "mailHead": "親愛的 ([REVIVERNAME]) 您好：",
      "mailContent": "有新的報修單需要您處理...",
      "status": 1
    }
  ]
}
```

#### 2.2 更新告警規則
```http
PUT /Api/AlertRule/Update/{id}
```

**說明**: 更新指定的告警規則

**路徑參數**:
- `id`: 規則 ID (int, required)

**請求參數**:
```json
{
  "mailType": "NEW,PENDING",
  "mailReciver": "REVIEWER", 
  "mailReciverCC": "STORE_MANAGER",
  "mailSubject": "【FTT報修系統】新報修單 - ([FORM_NO])",
  "mailHead": "親愛的 ([REVIVERNAME]) 您好：",
  "mailContent": "有新的報修單 ([FORM_NO]) 需要您處理...",
  "status": 1
}
```

**權限要求**: 需要系統管理員權限

### 3. 郵件伺服器設定

#### 3.1 取得郵件伺服器設定
```http
GET /Api/MailServer/GetSettings
```

**說明**: 取得目前的郵件伺服器設定

**回應範例**:
```json
{
  "success": true,
  "data": {
    "server": "smtp.gmail.com",
    "port": "587",
    "senderAddress": "noreply@fet.com.tw",
    "password": "********",
    "enableSsl": true,
    "status": 1
  }
}
```

#### 3.2 更新郵件伺服器設定
```http
PUT /Api/MailServer/UpdateSettings
```

**說明**: 更新郵件伺服器設定

**請求參數**:
```json
{
  "server": "smtp.gmail.com",
  "port": "587", 
  "senderAddress": "noreply@fet.com.tw",
  "password": "your_password",
  "enableSsl": true,
  "status": 1
}
```

**權限要求**: 需要系統管理員權限

#### 3.3 測試郵件伺服器連線
```http
POST /Api/MailServer/TestConnection
```

**說明**: 測試郵件伺服器連線狀態

**請求參數**:
```json
{
  "testEmail": "admin@example.com"
}
```

**回應範例**:
```json
{
  "success": true,
  "message": "郵件伺服器連線測試成功，測試郵件已發送",
  "data": {
    "connectionTime": "2024-01-15T10:30:00",
    "responseTime": 1250
  }
}
```

### 4. 告警統計與監控

#### 4.1 取得告警發送統計
```http
GET /Api/MailPool/GetStatistics?startDate={start}&endDate={end}
```

**說明**: 取得指定時間範圍的告警發送統計

**查詢參數**:
- `startDate`: 開始日期 (datetime, required)
- `endDate`: 結束日期 (datetime, required)

**回應範例**:
```json
{
  "success": true,
  "data": {
    "totalSent": 1250,
    "totalFailed": 15,
    "successRate": 98.8,
    "averageDeliveryTime": 2.3,
    "byStatus": [
      { "status": "已發送", "count": 1250 },
      { "status": "發送失敗", "count": 15 },
      { "status": "待發送", "count": 5 }
    ],
    "byType": [
      { "type": "新建報修", "count": 450 },
      { "type": "狀態變更", "count": 600 },
      { "type": "逾期提醒", "count": 200 }
    ]
  }
}
```

#### 4.2 取得失敗郵件清單
```http
GET /Api/MailPool/GetFailedMails?page={page}&pageSize={pageSize}
```

**說明**: 取得發送失敗的郵件清單

**查詢參數**:
- `page`: 頁碼 (int, default: 1)
- `pageSize`: 每頁筆數 (int, default: 20)

**回應範例**:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 123,
        "formNo": "F2024010001", 
        "subject": "【FTT報修系統】新報修單 - F2024010001",
        "destinationEmail": "invalid@example.com",
        "errorMsg": "SMTP Error: 550 Mailbox not found",
        "createTime": "2024-01-15T10:30:00",
        "retryCount": 3
      }
    ],
    "totalCount": 15,
    "currentPage": 1,
    "pageSize": 20
  }
}
```

### 5. 告警範本變數

系統支援以下範本變數，會在發送時自動替換：

| 變數名稱 | 說明 | 範例值 |
|---------|------|--------|
| `([FORM_NO])` | 報修單號 | F2024010001 |
| `([STORE])` | 門市名稱 | 台北信義店 |
| `([VENDOR])` | 廠商名稱 | ABC維修公司 |
| `([REVIVERNAME])` | 收件人名稱 | 張三 |
| `([EMPNAME])` | 申請人姓名 | 李四 |
| `([CREATETIME])` | 建立時間 | 2024/01/15 10:30:00 |
| `([CATEGORY_NAME])` | 報修分類 | 電腦設備 |
| `([MailURL])` | 系統網址 | https://ftt.fet.com.tw |
| `([MailURL_VENDOR])` | 廠商系統網址 | https://vendor.fet.com.tw |

### 6. 背景服務與排程

#### 6.1 Hangfire 排程設定
```csharp
// 每分鐘執行一次郵件發送檢查
RecurringJob.AddOrUpdate<SendMailHandler>(
    nameof(SendMailHandler.Send),
    x => x.Send(),
    "*/1 * * * *"  // 每分鐘執行
);
```

#### 6.2 郵件發送流程
```
📧 郵件發送流程:
1. 業務邏輯觸發 CreateMailPool()
2. 插入郵件至 tb_mailpool 表
3. Hangfire 定期執行 SendMailHandler.Send()
4. 查詢未發送郵件 (SendStatus = 0)
5. 透過 MailHelper 發送郵件
6. 更新發送狀態與結果
```

#### 6.3 失敗重試機制
- **自動重試**: 發送失敗會標記錯誤訊息，等待下次排程重試
- **最大重試**: 系統預設不限制重試次數，直到手動處理
- **錯誤日誌**: 所有發送錯誤都會記錄在 ErrorMsg 欄位

---

## 📊 效能建議與最佳實務

### JWT 常見問題與故障排除

#### 問題 1: Token 過期錯誤
**錯誤訊息**: `Token has expired`
```json
{
  "success": false,
  "message": "Token 已過期",
  "data": null,
  "errors": [
    {
      "code": "TOKEN_EXPIRED",
      "message": "請重新登入或使用 Refresh Token"
    }
  ]
}
```
**解決方案**:
1. 使用 Refresh Token 自動更新
2. 重新登入取得新的 Token
3. 檢查系統時間是否正確

#### 問題 2: Token 格式錯誤
**錯誤訊息**: `Invalid token format`
```json
{
  "success": false,
  "message": "Token 格式錯誤",
  "data": null,
  "errors": [
    {
      "code": "INVALID_TOKEN_FORMAT",
      "message": "Token 必須為 Bearer 格式"
    }
  ]
}
```
**解決方案**:
1. 確認 Header 格式: `Authorization: Bearer {token}`
2. 檢查 Token 是否包含三個部分 (header.payload.signature)
3. 確認 Token 中沒有多餘的空格或換行

#### 問題 3: Token 簽章驗證失敗
**錯誤訊息**: `Invalid token signature`
```json
{
  "success": false,
  "message": "Token 簽章無效",
  "data": null,
  "errors": [
    {
      "code": "INVALID_SIGNATURE",
      "message": "Token 可能已被篡改"
    }
  ]
}
```
**解決方案**:
1. 重新登入取得有效 Token
2. 檢查是否使用正確的環境 (開發/測試/正式)
3. 確認 Token 沒有在傳輸過程中被修改

#### 問題 4: 權限不足
**錯誤訊息**: `Insufficient permissions`
```json
{
  "success": false,
  "message": "權限不足",
  "data": null,
  "errors": [
    {
      "code": "INSUFFICIENT_PERMISSION",
      "message": "您沒有執行此操作的權限"
    }
  ]
}
```
**解決方案**:
1. 聯繫管理員申請必要權限
2. 確認使用者角色是否正確
3. 檢查 Token 中的權限資訊

#### Token 除錯工具
```javascript
// 解析 JWT Token 的工具函數
function parseJwtToken(token) {
    try {
        const parts = token.split('.');
        if (parts.length !== 3) {
            throw new Error('Invalid token format');
        }
        
        const header = JSON.parse(atob(parts[0]));
        const payload = JSON.parse(atob(parts[1]));
        
        return {
            header,
            payload,
            isExpired: payload.exp * 1000 < Date.now(),
            expiresAt: new Date(payload.exp * 1000),
            issuedAt: new Date(payload.iat * 1000)
        };
    } catch (error) {
        console.error('Failed to parse JWT token:', error);
        return null;
    }
}

// 使用範例
const tokenInfo = parseJwtToken('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...');
console.log('Token 資訊:', tokenInfo);
```

#### 最佳實務建議
1. **安全儲存**: 使用 HttpOnly Cookie 而非 localStorage
2. **自動刷新**: 實作 Token 自動刷新機制
3. **錯誤處理**: 統一處理認證錯誤並導向登入頁
4. **權限檢查**: 前端也要檢查權限以提升使用體驗
5. **登出清理**: 確實清除所有 Token 相關資料
