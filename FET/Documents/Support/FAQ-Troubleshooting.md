# 常見問題與故障排除指南

## 📋 目錄
- [系統登入問題](#系統登入問題)
- [報修單操作問題](#報修單操作問題)
- [郵件通知問題](#郵件通知問題)
- [檔案上傳問題](#檔案上傳問題)
- [權限與存取問題](#權限與存取問題)
- [效能與速度問題](#效能與速度問題)
- [瀏覽器相容性問題](#瀏覽器相容性問題)
- [API 錯誤處理](#api-錯誤處理)
- [資料庫連線問題](#資料庫連線問題)
- [系統維護與監控](#系統維護與監控)

---

## 系統登入問題

### ❓ 問題：無法登入系統
**症狀**: 輸入帳號密碼後，系統顯示「帳號或密碼錯誤」

**可能原因與解決方案**:

1. **帳號密碼錯誤**
   ```
   🔍 檢查方式:
   - 確認帳號格式正確（通常為工號）
   - 確認是否有大小寫錯誤
   - 確認數字與英文字母輸入正確
   ```
   
   **解決方法**: 
   - 使用「忘記密碼」功能重設密碼
   - 聯繫系統管理員確認帳號狀態

2. **帳號被鎖定**
   ```sql
   -- 管理員查詢帳號狀態
   SELECT empNo, empName, isLocked, lockTime, errorCount 
   FROM tb_user 
   WHERE empNo = 'USER001'
   ```
   
   **解決方法**: 
   - 管理員執行帳號解鎖
   - 等待自動解鎖時間（通常 30 分鐘）

3. **瀏覽器 Cookie 問題**
   ```javascript
   // 清除瀏覽器快取
   // Chrome: Ctrl+Shift+Delete
   // 或手動清除網站資料
   ```
   
   **解決方法**: 
   - 清除瀏覽器快取和 Cookie
   - 使用無痕視窗測試
   - 嘗試其他瀏覽器

### ❓ 問題：登入後立即被登出
**症狀**: 登入成功但馬上跳回登入頁面

**解決方案**:
```javascript
// 檢查 JWT Token 設定
localStorage.getItem('authToken');
// 檢查 Token 過期時間
```

1. **JWT Token 設定問題**
   - 檢查系統時間是否正確
   - 確認 Token 有效期設定
   - 檢查 Token 簽章金鑰

2. **Session 設定問題**
   - 檢查 Session 逾時設定
   - 確認 Cookie 網域設定
   - 檢查 HTTPS 設定

---

## 報修單操作問題

### ❓ 問題：無法建立報修單
**症狀**: 填寫報修單資料後，點擊儲存沒有反應或出現錯誤

**診斷步驟**:
```javascript
// 開啟瀏覽器開發者工具 (F12)
// 查看 Console 錯誤訊息
console.log('檢查 JavaScript 錯誤');

// 查看 Network 面板
// 檢查 API 請求是否成功
```

**常見原因與解決方案**:

1. **必填欄位未填寫**
   ```html
   <!-- 檢查必填欄位 -->
   <input required data-val="true" data-val-required="此欄位為必填">
   ```
   
   **解決方法**: 
   - 檢查紅色標示的必填欄位
   - 確認所有必填資料都已填寫
   - 查看頁面上方的錯誤提示訊息

2. **資料格式錯誤**
   ```javascript
   // 檢查電話號碼格式
   const phonePattern = /^[0-9\-\(\)\s]+$/;
   if (!phonePattern.test(phoneNumber)) {
       alert('電話號碼格式不正確');
   }
   ```
   
   **解決方法**: 
   - 電話號碼只能包含數字、括號、連字號
   - 日期格式需符合 YYYY/MM/DD
   - 數量欄位只能輸入正整數

3. **檔案上傳問題**
   ```javascript
   // 檢查檔案大小和格式
   const maxSize = 10 * 1024 * 1024; // 10MB
   if (file.size > maxSize) {
       alert('檔案大小不可超過 10MB');
   }
   ```
   
   **解決方法**: 
   - 確認檔案格式為 JPG、PNG、PDF、DOCX
   - 單一檔案不超過 10MB
   - 總上傳容量不超過 100MB

### ❓ 問題：報修單狀態異常
**症狀**: 報修單卡在某個狀態，無法正常流轉

**檢查流程**:
```sql
-- 查詢系統操作記錄 (實際資料表為 TB_Control_Log)
SELECT * FROM TB_Control_Log 
WHERE Account IN (SELECT empNo FROM tb_user WHERE empNo LIKE '%報修相關人員%')
AND LogTime >= DATEADD(day, -7, GETDATE())
ORDER BY LogTime DESC;

-- 檢查當前處理人員
SELECT * FROM tb_report 
WHERE formNo = 'F2024010001';
```

**解決方案**:

1. **派工異常**
   - 確認廠商帳號狀態正常
   - 檢查廠商是否有對應的服務區域
   - 確認派工規則設定正確

2. **審核卡關**
   - 確認審核人員權限正常
   - 檢查是否有代理人設定
   - 查看審核規則是否符合條件

---

## 郵件通知問題

### ❓ 問題：沒有收到系統通知郵件
**症狀**: 系統操作後應該要收到郵件，但信箱中沒有郵件

**診斷流程**:
```sql
-- 查詢郵件池狀態
SELECT * FROM tb_mailpool 
WHERE formNo = 'F2024010001' 
ORDER BY createTime DESC;

-- 檢查郵件發送狀態
SELECT 
    subject,
    destinationEmail,
    sendStatus,
    sendStatusText,
    errorMsg,
    realSendTime
FROM tb_mailpool 
WHERE destinationEmail = 'user@example.com'
AND createTime >= DATEADD(day, -7, GETDATE());
```

**問題排除**:

1. **郵件在垃圾信匣**
   ```
   🔍 檢查項目:
   ├── 垃圾郵件資料夾
   ├── 促銷分類標籤
   ├── 封鎖名單設定
   └── 郵件規則篩選
   ```

2. **郵件伺服器問題**
   ```bash
   # 管理員檢查 SMTP 設定
   telnet smtp.server.com 587
   # 測試連線是否正常
   ```
   
   **解決方法**: 
   - 檢查 SMTP 伺服器設定
   - 確認帳號密碼正確
   - 檢查防火牆設定

3. **收件人設定錯誤**
   ```sql
   -- 檢查使用者郵件地址
   SELECT empNo, empName, email FROM tb_user WHERE empNo = 'USER001';
   
   -- 檢查告警規則設定 (實際資料表為 tb_mailpool_rule)
   SELECT * FROM tb_mailpool_rule WHERE mailType LIKE '%NEW%';
   ```

### ❓ 問題：郵件發送失敗
**症狀**: 系統顯示郵件發送失敗，錯誤訊息不明確

**常見錯誤與解決方案**:

1. **SMTP 認證失敗**
   ```
   錯誤訊息: "535 Authentication failed"
   ```
   
   **解決方法**: 
   - 檢查 SMTP 帳號密碼
   - 確認是否啟用「低安全性應用程式存取」
   - 檢查是否需要應用程式專用密碼

2. **收件人地址無效**
   ```
   錯誤訊息: "550 Mailbox not found"
   ```
   
   **解決方法**: 
   - 檢查收件人郵件地址拼寫
   - 確認網域名稱正確
   - 測試發送到已知有效地址

3. **郵件內容被拒絕**
   ```
   錯誤訊息: "554 Message rejected"
   ```
   
   **解決方法**: 
   - 檢查郵件內容是否包含敏感詞彙
   - 確認附件格式和大小符合規定
   - 檢查 HTML 格式是否正確

---

## 檔案上傳問題

### ❓ 問題：檔案上傳失敗
**症狀**: 選擇檔案後，上傳進度條停止或出現錯誤

**檢查項目**:

1. **檔案格式限制**
   ```javascript
   const allowedTypes = ['image/jpeg', 'image/png', 'application/pdf', 
                        'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
   
   if (!allowedTypes.includes(file.type)) {
       alert('不支援的檔案格式');
   }
   ```

2. **檔案大小限制**
   ```javascript
   const maxSize = 10 * 1024 * 1024; // 10MB
   if (file.size > maxSize) {
       alert('檔案大小超過限制');
   }
   ```

3. **伺服器設定問題**
   ```xml
   <!-- web.config 設定檢查 -->
   <system.web>
     <httpRuntime maxRequestLength="102400" /> <!-- 100MB -->
   </system.web>
   
   <system.webServer>
     <security>
       <requestFiltering>
         <requestLimits maxAllowedContentLength="104857600" /> <!-- 100MB -->
       </requestFiltering>
     </security>
   </system.webServer>
   ```

**解決方案**:
- 確認檔案格式為 JPG、PNG、PDF、DOCX
- 壓縮大型檔案或分割上傳
- 聯繫管理員檢查伺服器設定

### ❓ 問題：上傳的圖片無法顯示
**症狀**: 檔案上傳成功，但在系統中無法正常顯示

**可能原因**:
1. 檔案路徑錯誤
2. 權限設定問題
3. 圖片檔案損壞

**檢查方法**:
```javascript
// 檢查圖片 URL 是否正確
const img = new Image();
img.onload = function() {
    console.log('圖片載入成功');
};
img.onerror = function() {
    console.log('圖片載入失敗');
};
img.src = '/uploads/image.jpg';
```

---

## 權限與存取問題

### ❓ 問題：頁面顯示「無權限存取」
**症狀**: 點擊選單或功能時，系統提示權限不足

**檢查步驟**:

1. **使用者權限檢查**
   ```sql
   -- 查詢使用者權限
   SELECT r.roleName, f.funcName 
   FROM tb_user u
   JOIN tb_user_role ur ON u.empNo = ur.empNo
   JOIN tb_role r ON ur.roleId = r.id
   JOIN tb_role_func rf ON r.id = rf.roleId
   JOIN tb_func f ON rf.funcId = f.id
   WHERE u.empNo = 'USER001';
   ```

2. **功能權限設定**
   ```sql
   -- 檢查特定功能權限
   SELECT * FROM tb_func WHERE funcName LIKE '%報修%';
   ```

**解決方案**:
- 聯繫管理員新增必要權限
- 確認是否需要申請角色變更
- 檢查是否有臨時權限可申請

### ❓ 問題：某些按鈕或功能無法使用
**症狀**: 頁面可以存取，但部分功能按鈕呈現灰色或無法點擊

**原因分析**:
```javascript
// 檢查按鈕狀態
const button = document.getElementById('submitBtn');
if (button.disabled) {
    console.log('按鈕被禁用，原因:', button.getAttribute('data-reason'));
}
```

1. **條件限制**
   - 報修單狀態不符合操作條件
   - 時間限制（如超過修改期限）
   - 資料完整性檢查未通過

2. **權限限制**
   - 使用者角色權限不足
   - 特定功能需要更高權限

---

## 效能與速度問題

### ❓ 問題：系統回應緩慢
**症狀**: 頁面載入時間過長，操作有明顯延遲

**診斷工具**:
```javascript
// 效能監控
performance.mark('start-operation');
// ... 執行操作 ...
performance.mark('end-operation');
performance.measure('operation-time', 'start-operation', 'end-operation');
console.log(performance.getEntriesByName('operation-time')[0].duration);
```

**優化建議**:

1. **瀏覽器快取**
   ```javascript
   // 清除瀏覽器快取
   // Ctrl+F5 強制重新整理
   // 或使用開發者工具禁用快取
   ```

2. **資料分頁**
   ```javascript
   // 增加分頁大小設定
   const pageSize = 50; // 預設值，可調整為 20 或更小
   ```

3. **網路連線**
   ```bash
   # 檢查網路連線速度
   ping google.com
   # 檢查 DNS 解析
   nslookup your-server.com
   ```

### ❓ 問題：大量資料查詢超時
**症狀**: 查詢報表或大量資料時，系統顯示超時錯誤

**解決方案**:

1. **縮小查詢範圍**
   - 限制日期區間（建議不超過 3 個月）
   - 增加篩選條件
   - 使用分頁查詢

2. **優化查詢條件**
   ```sql
   -- 使用索引欄位作為篩選條件
   SELECT * FROM tb_report 
   WHERE createTime >= '2024-01-01' 
   AND status = 1;
   
   -- 避免使用 LIKE '%文字%' 的模糊搜尋
   ```

---

## 瀏覽器相容性問題

### ❓ 問題：IE 瀏覽器功能異常
**症狀**: 在 Internet Explorer 中，部分功能無法正常運作

**解決方案**:
```html
<!-- 檢查瀏覽器相容性模式 -->
<meta http-equiv="X-UA-Compatible" content="IE=edge">
```

**建議瀏覽器**:
- Chrome 80 或以上版本
- Edge 80 或以上版本  
- Firefox 75 或以上版本

### ❓ 問題：行動裝置顯示異常
**症狀**: 在手機或平板上，頁面排版混亂

**檢查項目**:
```html
<!-- 確認 viewport 設定 -->
<meta name="viewport" content="width=device-width, initial-scale=1.0">
```

**建議**:
- 使用桌面版瀏覽器存取
- 將螢幕方向調整為橫向
- 使用平板裝置以獲得更好體驗

---

## API 錯誤處理

### ❓ 常見 HTTP 錯誤碼

**400 Bad Request**
```json
{
  "success": false,
  "message": "請求參數錯誤",
  "errors": [
    {
      "field": "storeCode",
      "message": "門市代碼不可為空"
    }
  ]
}
```
**解決方法**: 檢查請求參數格式和必填欄位

**401 Unauthorized**
```json
{
  "success": false,
  "message": "未授權存取"
}
```
**解決方法**: 重新登入獲取有效 Token

**403 Forbidden**
```json
{
  "success": false,
  "message": "權限不足"
}
```
**解決方法**: 聯繫管理員申請權限

**404 Not Found**
```json
{
  "success": false,
  "message": "資源不存在"
}
```
**解決方法**: 確認 API 路徑和資源 ID 正確

**500 Internal Server Error**
```json
{
  "success": false,
  "message": "伺服器內部錯誤"
}
```
**解決方法**: 聯繫技術支援，提供詳細錯誤資訊

---

## 資料庫連線問題

### ❓ 問題：資料庫連線失敗
**症狀**: 系統無法存取資料，出現資料庫連線錯誤

**檢查步驟**:
```sql
-- 測試資料庫連線
SELECT 1 AS TestConnection;

-- 檢查資料庫狀態
SELECT name, state_desc FROM sys.databases WHERE name = 'FTT_DB';
```

**常見原因**:

1. **連線字串錯誤**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=SERVER;Initial Catalog=FTT_DB;Integrated Security=True"
     }
   }
   ```

2. **網路連線問題**
   ```bash
   # 測試伺服器連線
   telnet database-server 1433
   ```

3. **資料庫伺服器停止**
   - 檢查 SQL Server 服務狀態
   - 確認防火牆設定
   - 檢查資料庫可用空間

---

## 系統維護與監控

### 🔧 定期維護檢查項目

**每日檢查**:
```sql
-- 檢查錯誤日誌
SELECT TOP 10 * FROM tb_system_log 
WHERE logLevel = 'ERROR' 
AND createTime >= DATEADD(day, -1, GETDATE())
ORDER BY createTime DESC;

-- 檢查郵件發送狀況
SELECT 
    COUNT(*) as TotalMails,
    SUM(CASE WHEN sendStatus = 1 THEN 1 ELSE 0 END) as SentMails,
    SUM(CASE WHEN sendStatus = 2 THEN 1 ELSE 0 END) as FailedMails
FROM tb_mailpool 
WHERE createTime >= DATEADD(day, -1, GETDATE());
```

**每週檢查**:
```sql
-- 檢查系統效能
SELECT 
    AVG(responseTime) as AvgResponseTime,
    MAX(responseTime) as MaxResponseTime
FROM tb_performance_log 
WHERE createTime >= DATEADD(day, -7, GETDATE());

-- 檢查磁碟空間
EXEC sp_spaceused;
```

**每月檢查**:
- 檢查伺服器硬體狀態
- 更新系統補丁
- 備份資料庫完整性檢查
- 使用者權限清理

### 📊 系統監控指標

**關鍵效能指標 (KPI)**:
```sql
-- 報修單處理效率
SELECT 
    AVG(DATEDIFF(hour, createTime, updateTime)) as AvgProcessTime,
    COUNT(*) as TotalReports
FROM tb_report 
WHERE status = 'CLOSED' 
AND createTime >= DATEADD(month, -1, GETDATE());

-- 系統可用性
SELECT 
    (COUNT(*) - SUM(CASE WHEN isError = 1 THEN 1 ELSE 0 END)) * 100.0 / COUNT(*) as Availability
FROM tb_health_check 
WHERE createTime >= DATEADD(day, -30, GETDATE());
```

### 🚨 告警閾值設定

```sql
-- 設定系統告警閾值
INSERT INTO tb_alert_threshold VALUES
('RESPONSE_TIME', 5000, 'API 回應時間超過 5 秒'),
('ERROR_RATE', 5, '錯誤率超過 5%'),
('DISK_SPACE', 85, '磁碟使用率超過 85%'),
('MEMORY_USAGE', 90, '記憶體使用率超過 90%');
```

---

## 📞 技術支援聯絡方式

### 🆘 緊急支援 (24/7)
- **電話**: (02) 1234-5678
- **Email**: support@fet.com.tw
- **Line**: @FTT_Support

### 👨‍💻 一般技術支援 (週一至五 09:00-18:00)
- **Email**: tech@fet.com.tw
- **內部系統**: IT Service Portal

### 📝 問題回報格式
```
問題類型: [登入/功能/效能/其他]
發生時間: YYYY/MM/DD HH:MM
使用者帳號: [帳號]
瀏覽器版本: [Chrome 100.0.0.0]
錯誤訊息: [完整錯誤訊息]
重現步驟: 
1. 
2. 
3. 
預期結果: 
實際結果: 
螢幕截圖: [附件]
```

---

## 📄 文件中的資料表說明

### 實際存在的資料表
以下是系統中實際存在並使用的資料表：

```sql
-- 核心業務資料表
tb_report               -- 報修單主檔
tb_user                 -- 使用者資料
tb_mailpool             -- 郵件池 (系統發送的所有郵件)
tb_mailserver           -- 郵件伺服器設定
tb_mailpool_rule        -- 郵件告警規則設定
tb_token                -- JWT Token 管理
TB_Control_Log          -- 系統操作記錄 (注意大小寫)

-- 廠商相關資料表
tb_vender_password_history  -- 廠商密碼變更歷史
tb_vender_pw_history       -- 廠商密碼歷史記錄
```

### 文件中的範例資料表
以下資料表名稱在文件中作為範例或概念說明使用，實際系統中可能不存在或名稱不同：

```sql
-- 以下為文件範例，實際資料表名稱可能不同
tb_system_log           -- 系統日誌 (範例)
tb_performance_log      -- 效能監控記錄 (範例)  
tb_health_check         -- 系統健康檢查 (範例)
tb_user_role           -- 使用者角色關聯 (範例)
tb_role                -- 角色定義 (範例)
tb_func                -- 功能定義 (範例)
tb_role_func           -- 角色功能關聯 (範例)
```

> **注意**: 在實際開發或維護時，請以程式碼中的 Entity 類別和實際資料庫結構為準。上述範例資料表主要用於說明概念和流程。

---

*最後更新: 2024年12月*
