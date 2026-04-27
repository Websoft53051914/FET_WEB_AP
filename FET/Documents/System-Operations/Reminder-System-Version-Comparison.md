# 📋 FTT 催單通知系統 - 新舊版本差異分析

## 📖 目錄
- [版本比較概述](#版本比較概述)
- [核心差異分析](#核心差異分析)
- [功能對比表](#功能對比表)
- [技術實作差異](#技術實作差異)
- [使用者體驗差異](#使用者體驗差異)
- [建議改進措施](#建議改進措施)
- [版本遷移指南](#版本遷移指南)

---

## 🔄 版本比較概述

### 版本資訊
- **舊版本**：觀察顯示每日限制一次催單的版本
- **新版本**：目前運行的版本（2026年3月）
- **主要差異**：防重複催單機制的移除

### 🚨 關鍵發現
**新版本移除了舊版本的防重複催單保護機制，導致同一工單可在一天內被多次催單。**

---

## 🔍 核心差異分析

### 1. 催單頻率控制

#### 舊版本行為
```
✅ 每日限制一次催單
✅ 自動檢查當日是否已催單
✅ 防止重複郵件騷擾
✅ 合理的催單頻率
```

#### 新版本行為
```
❌ 無催單頻率限制
❌ 可一日內多次催單
❌ 無重複檢查機制
❌ 可能造成郵件氾濫
```

### 2. 技術實作差異

#### 舊版本可能的實作方式
根據觀察到的行為推測，舊版本可能包含以下機制之一：

**方案A：程式碼檢查**
```csharp
// 推測的舊版本邏輯
internal bool HasReminderToday(string formNo)
{
    string sql = @"
        SELECT COUNT(*) 
        FROM FTT_FORM_LOG 
        WHERE FORM_NO = @formNo 
        AND FIELDNAME = '催單' 
        AND TRUNC(UPDATETIME) = TRUNC(SYSDATE)";
    
    var count = GetDBHelper().FindSingle<int>(sql, paras);
    return count > 0;
}

// 催單前檢查
if (overKPI == true && !HasReminderToday(form_No))
{
    // 執行催單邏輯
}
```

**方案B：資料庫約束**
```sql
-- 推測的舊版本資料庫約束
ALTER TABLE FTT_FORM_LOG 
ADD CONSTRAINT uk_daily_reminder 
UNIQUE (FORM_NO, TRUNC(UPDATETIME), FIELDNAME) 
WHERE FIELDNAME = '催單';
```

**方案C：前端控制**
```javascript
// 推測的舊版本前端邏輯
function disableReminderButton(formNo, hours = 24) {
    localStorage.setItem(`reminder_${formNo}`, Date.now());
    $("#reminderBtn").prop('disabled', true);
    
    setTimeout(() => {
        $("#reminderBtn").prop('disabled', false);
    }, hours * 60 * 60 * 1000);
}
```

#### 新版本實作
```csharp
// 目前的新版本邏輯（無檢查機制）
if (overKPI == true)
{
    // 直接執行催單，沒有重複檢查
    var result = Method.CreateMailPool(form_No, "", "REMINDER", _MailPoolHandler);
    return JsonSuccess("已發送催單通知!!");
}
```

### 3. 資料記錄差異

#### 舊版本
- 每個工單每日最多一筆催單記錄
- 資料庫記錄相對乾淨
- 郵件佇列記錄適量

#### 新版本
- 同一工單可產生多筆催單記錄
- 可能產生大量重複記錄
- 郵件佇列可能堆積重複郵件

---

## 📊 功能對比表

| 功能項目 | 舊版本 | 新版本 | 影響評估 |
|---------|--------|--------|----------|
| **催單頻率限制** | ✅ 每日一次 | ❌ 無限制 | 🔴 高風險 |
| **重複檢查機制** | ✅ 有檢查 | ❌ 無檢查 | 🔴 高風險 |
| **郵件佇列管理** | ✅ 合理數量 | ⚠️ 可能過量 | 🟡 中風險 |
| **使用者體驗** | ✅ 清楚限制 | ❌ 容易誤用 | 🟡 中風險 |
| **系統負載** | ✅ 可控制 | ⚠️ 可能過重 | 🟡 中風險 |
| **承辦人體驗** | ✅ 適量通知 | ❌ 可能騷擾 | 🔴 高風險 |
| **KPI 檢查** | ✅ 正常 | ✅ 正常 | ✅ 無影響 |
| **郵件發送** | ✅ 正常 | ✅ 正常 | ✅ 無影響 |
| **權限控制** | ✅ 正常 | ✅ 正常 | ✅ 無影響 |
| **日誌記錄** | ✅ 正常 | ✅ 正常 | ✅ 無影響 |

### 風險等級說明
- 🔴 **高風險**：可能嚴重影響使用者體驗或系統穩定性
- 🟡 **中風險**：可能影響系統效能或使用體驗
- ✅ **無影響**：功能正常運作，無明顯差異

---

## 💻 技術實作差異

### 程式碼結構變化

#### 舊版本（推測）
```csharp
[HttpPost("[action]")]
public IActionResult InsterTrackingForm(v_ftt_form2DTO vm)
{
    var form_No = vm.form_no;
    var _InProcessHanlder = new InProcessHandler(_config, HttpContext);
    
    // KPI 檢查
    string kpiTime = _InProcessHanlder.GetKPITime(form_No);
    bool overKPI = _InProcessHanlder.CheckDataExist_APPROVE_FORM(form_No, kpiTime);
    
    if (overKPI == true)
    {
        // 舊版本：檢查今日是否已催過單
        if (_InProcessHanlder.HasReminderToday(form_No))
        {
            return JsonValidFail("今日已催過單，請明日再試！");
        }
        
        // 執行催單邏輯
        var result = Method.CreateMailPool(form_No, "", "REMINDER", _MailPoolHandler);
        return JsonSuccess("已發送催單通知!!");
    }
    else
    {
        return JsonValidFail($"此工單【{form_No}】尚未超過KPI時間，無法催單!!");
    }
}
```

#### 新版本（目前）
```csharp
[HttpPost("[action]")]
public IActionResult InsterTrackingForm(v_ftt_form2DTO vm)
{
    var form_No = vm.form_no;
    var _InProcessHanlder = new InProcessHandler(_config, HttpContext);
    
    // KPI 檢查
    string kpiTime = _InProcessHanlder.GetKPITime(form_No);
    bool overKPI = _InProcessHanlder.CheckDataExist_APPROVE_FORM(form_No, kpiTime);
    
    if (overKPI == true)
    {
        // 新版本：直接執行催單，無重複檢查
        var result = Method.CreateMailPool(form_No, "", "REMINDER", _MailPoolHandler);
        return JsonSuccess("已發送催單通知!!");
    }
    else
    {
        return JsonValidFail($"此工單【{form_No}】尚未超過KPI時間，無法催單!!");
    }
}
```

### 資料庫查詢差異

#### 舊版本可能的查詢
```sql
-- 檢查今日是否已催單
SELECT COUNT(*) 
FROM FTT_FORM_LOG 
WHERE FORM_NO = @formNo 
AND FIELDNAME = '催單' 
AND TRUNC(UPDATETIME) = TRUNC(SYSDATE);

-- 或檢查最近24小時
SELECT COUNT(*) 
FROM FTT_FORM_LOG 
WHERE FORM_NO = @formNo 
AND FIELDNAME = '催單' 
AND UPDATETIME >= SYSDATE - 1;
```

#### 新版本查詢
```sql
-- 只檢查 KPI 超時，無重複檢查
SELECT * FROM APPROVE_FORM 
WHERE FORM_NO = @FORM_NO 
AND CHK_WORKING_DAY2(UPDATETIME, SYSDATE, 'S') > @kpiTime;
```

---

## 👥 使用者體驗差異

### 管理員角度

#### 舊版本體驗
```
✅ 系統郵件量可預測
✅ 催單記錄乾淨有序
✅ 承辦人員不會被騷擾
✅ 系統負載穩定
```

#### 新版本體驗
```
❌ 可能收到用戶抱怨重複郵件
❌ 資料庫記錄可能混亂
❌ 需要手動處理郵件問題
❌ 系統負載可能增加
```

### 一般用戶角度

#### 舊版本體驗
```
✅ 清楚知道催單限制
✅ 不會意外重複催單
✅ 系統回應明確
```

#### 新版本體驗
```
❌ 不知道是否已催過單
❌ 可能多次點擊催單按鈕
❌ 容易造成誤操作
```

### 承辦人員角度

#### 舊版本體驗
```
✅ 每日最多收到一次催單
✅ 郵件數量合理
✅ 不會被過度催促
```

#### 新版本體驗
```
❌ 可能一日內收到多次催單
❌ 郵件可能被視為垃圾信
❌ 工作效率可能受影響
```

---

## 💡 建議改進措施

### 立即改進（高優先級）

#### 1. 恢復防重複機制
```csharp
// 建議實作的檢查方法
internal bool HasReminderToday(string formNo)
{
    Dictionary<string, object> paras = new() { {"formNo", formNo} };
    
    string sql = @"
        SELECT COUNT(*) 
        FROM FTT_FORM_LOG 
        WHERE FORM_NO = @formNo 
        AND FIELDNAME = '催單' 
        AND TRUNC(UPDATETIME) = TRUNC(SYSDATE)";
    
    var count = GetDBHelper().FindSingle<int>(sql, paras);
    return count > 0;
}
```

#### 2. 更新催單邏輯
```csharp
if (overKPI == true)
{
    // 檢查今日是否已催過單
    if (_InProcessHanlder.HasReminderToday(form_No))
    {
        return JsonValidFail("此工單今日已催過單，請明日再試！");
    }
    
    // 原有催單邏輯...
    var result = Method.CreateMailPool(form_No, "", "REMINDER", _MailPoolHandler);
}
```

### 中期改進（中優先級）

#### 1. 前端介面改善
```javascript
// 顯示上次催單時間
function showLastReminderInfo(formNo) {
    // Ajax 查詢上次催單時間
    // 在按鈕旁顯示提示資訊
}

// 催單後禁用按鈕
function disableReminderButton() {
    $("#reminderBtn").prop('disabled', true).text('今日已催單');
}
```

#### 2. 設定參數化
```json
{
  "ReminderSettings": {
    "EnableDuplicateCheck": true,
    "MinIntervalHours": 24,
    "MaxDailyReminders": 1
  }
}
```

### 長期改進（低優先級）

#### 1. 智能催單建議
- 根據工單類型調整催單頻率
- 根據承辦人員回應速度調整策略
- 提供催單效果統計分析

#### 2. 多元化通知方式
- 整合即時通訊工具
- 提供 SMS 通知選項
- 支援 LINE 或其他平台通知

---

## 🚀 版本遷移指南

### 遷移步驟

#### 步驟 1：備份現有資料
```sql
-- 備份催單相關記錄
CREATE TABLE FTT_FORM_LOG_BACKUP AS 
SELECT * FROM FTT_FORM_LOG WHERE FIELDNAME = '催單';

-- 備份郵件佇列
CREATE TABLE tb_mailpool_BACKUP AS 
SELECT * FROM tb_mailpool WHERE subject LIKE '%催單%';
```

#### 步驟 2：實作防重複機制
1. 在 `InProcessHandler.cs` 新增 `HasReminderToday` 方法
2. 修改 `InsterTrackingForm` 控制器方法
3. 更新前端介面提示

#### 步驟 3：測試驗證
```sql
-- 測試同一工單多次催單
-- 應該只有第一次成功，後續被拒絕

-- 測試跨日催單
-- 隔日應該可以重新催單
```

#### 步驟 4：部署上線
1. 部署新版程式碼
2. 監控系統日誌
3. 收集使用者回饋

### 回滾計畫

如果新版本出現問題，可以：
1. 移除防重複檢查邏輯
2. 恢復為目前版本行為
3. 從備份資料中恢復記錄

---

## 📈 效益評估

### 預期改善效果

#### 系統效能
- 減少不必要的郵件佇列記錄
- 降低資料庫寫入操作
- 提升整體系統穩定性

#### 使用者體驗
- 明確的催單限制說明
- 減少誤操作發生
- 提升系統可信度

#### 承辦人員滿意度
- 避免重複郵件騷擾
- 維持合理的工作節奏
- 提升郵件重視程度

### 風險控制
- 新增功能開關，可隨時啟用/停用
- 保留詳細日誌，便於問題追蹤
- 提供緊急回滾機制

---

## 📞 技術支援

### 聯絡資訊
- **系統負責人**：FTT 開發團隊
- **緊急聯絡**：系統維運團隊
- **文檔維護**：技術文件團隊

### 相關文檔
- [催單通知機制說明](./Reminder-Notification-Mechanism.md)
- [系統升級指南](../Development/System-Upgrade-Guide.md)
- [問題回報流程](../Support/Issue-Reporting-Process.md)

---

**文檔建立：2026年3月9日**  
**文檔版本：v1.0**  
**維護人員：FTT 系統開發團隊**  
**最後更新：2026年3月9日**

---

## 🔍 舊版本程式碼分析結果

### 發現舊版本催單機制

經過對 `d:\BACK_OFFICE\FTT\AP` 目錄的詳細程式碼分析，我發現了舊版本系統的催單通知機制實作：

#### 關鍵檔案位置
- **主程式**：`AP\FTTTask\ReNotify.cs`
- **執行入口**：`AP\FTTTask\Program.cs`
- **排程邏輯**：透過參數 `case "5"` 執行催單通知

#### 舊版本催單機制技術分析

**1. 執行方式**：
```csharp
// Program.cs - 排程執行催單
case "5":
    mReNotify.Send_RE_Notify();
    mReNotifyVendor.SendVendor();
    break;
```

**2. KPI 檢查邏輯**：
```csharp
// ReNotify.cs - 檢查 KPI 超時
string kpiTime = DBtable.GetFieldData("category.kpitime", 
    "FTT_FORM form, CI_RELATIONS_CATEGORY category", 
    "category.CISID=form.CATEGORY_ID AND form.FORM_NO='" + FORMNO + "'");

if (kpiTime == "") kpiTime = "3";

bool flag = DBtable.CheckDataExist("APPROVE_FORM", 
    "FORM_NO='" + FORMNO + "' AND CHK_WORKING_DAY2(UPDATETIME,SYSDATE,'S') > " + kpiTime);
```

**3. 催單郵件產生**：
```csharp
// ReNotify.cs - 插入催單通知到佇列
if (recivename != "")
{
    query.Add("insert into notify_profile_new (cisid,receiver,receiver_cc,subject,alerttype,description,status) values (([FORM_NO]),'" + recivename + "','" + recivename_cc + "','" + mailsubject + "','2','" + mailcontent + "','O')");
}
```

### 🔑 防重複催單機制的關鍵發現

**重要發現：舊版本的「每日限制一次」並非透過程式邏輯實現，而是透過以下機制：**

#### 方案一：排程執行頻率控制
- 舊版本使用 **FTTTask 排程程式**，由系統管理員控制執行頻率
- 可能設定為每日執行一次（如：每天早上 09:00）
- 排程控制在作業系統層級，而非程式邏輯層級

#### 方案二：notify_profile_new 表格機制
- 舊版本使用 `notify_profile_new` 表格作為郵件佇列
- 可能存在資料庫層級的約束條件防止重複記錄
- 或者有清理機制在處理完郵件後刪除記錄

#### 方案三：NOTIFY_RULE 規則控制
- 透過 `NOTIFY_RULE` 表格的 `PRIORITY` 和狀態控制
- 可能有額外的日期檢查邏輯在規則層級

### 新舊版本架構差異

| 項目 | 舊版本 (AP\FTTTask) | 新版本 (FTT_API) |
|------|---------------------|------------------|
| **執行方式** | 排程批次處理 | 即時 Web API |
| **觸發機制** | 系統排程自動觸發 | 使用者手動點擊 |
| **郵件佇列** | notify_profile_new | tb_mailpool |
| **防重複機制** | 排程頻率 + 可能的 DB 約束 | 無防護 |
| **發送頻率** | 可控制（通常每日一次） | 無限制 |

### 建議遷移方案

基於舊版本分析，建議新版本採用以下改進：

**1. 恢復排程控制模式**
```csharp
// 選項A：改回批次排程模式（類似舊版本）
// 優點：完全可控的發送頻率
// 缺點：失去即時催單的彈性

// 選項B：混合模式 - 保持即時觸發但加入防重複檢查
if (overKPI == true)
{
    // 檢查今日是否已催過單
    bool hasToday = CheckReminderToday(form_No);
    if (hasToday)
    {
        return JsonValidFail("此工單今日已催過單，請明日再試！");
    }
    
    // 執行催單邏輯...
}
```

**2. 模擬舊版本行為**
```csharp
internal bool CheckReminderToday(string formNo)
{
    // 檢查今日是否已有催單記錄（模擬舊版本的日期控制）
    string sql = @"
        SELECT COUNT(*) 
        FROM FTT_FORM_LOG 
        WHERE FORM_NO = @formNo 
        AND FIELDNAME = '催單' 
        AND TRUNC(UPDATETIME) = TRUNC(SYSDATE)";
    
    return GetDBHelper().FindSingle<int>(sql, paras) > 0;
}
```

### 結論

舊版本的「每天只寄發一次催單」機制主要依靠：
1. **排程執行頻率控制**（最可能的原因）
2. **資料庫表格設計**（notify_profile_new 的使用方式）
3. **系統管理政策**（而非程式邏輯限制）

新版本若要恢復此行為，建議實作日期檢查邏輯，確保同一工單在同一天內最多只能催單一次。
