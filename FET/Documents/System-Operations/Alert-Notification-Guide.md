# 系統告警通知機制完整指南

## 📋 目錄
- [告警系統概述](#告警系統概述)
- [告警類型分類](#告警類型分類)
- [通知渠道管理](#通知渠道管理)
- [告警規則設定](#告警規則設定)
- [收件人管理](#收件人管理)
- [郵件範本設計](#郵件範本設計)
- [告警升級機制](#告警升級機制)
- [監控儀表板](#監控儀表板)
- [設定檔管理](#設定檔管理)
- [故障排除](#故障排除)

---

## 告警系統概述

FTT 系統的告警通知機制是一個多層次、多渠道的智能通知系統，確保重要事件能及時傳達給相關人員。

### 系統特色
- 🔔 **即時通知**: 關鍵事件發生後立即發送
- 📱 **多渠道整合**: Email、簡訊、系統推播
- 🎯 **智能分發**: 根據角色和權限自動分發
- 📊 **統計分析**: 完整的發送和開信統計
- 🔄 **失敗重試**: 自動重試失敗的通知

### 架構組件
```
📧 告警系統架構:
├── 🎯 事件觸發器 (Event Triggers)
├── 📋 規則引擎 (Rule Engine)
├── 🔀 通知路由器 (Notification Router)
├── 📨 發送服務 (Delivery Services)
├── 📊 統計監控 (Analytics & Monitoring)
└── 🗄️ 歷史記錄 (History & Logs)
```

---

## 告警類型分類

### 1. 🚨 業務流程告警

#### 1.1 報修流程告警
```
📋 報修相關告警:
├── 🆕 新報修申請
│   ├── 觸發時機: 門市建立新報修單
│   ├── 收件人: 審核人員、相關主管
│   └── 緊急程度: 依報修緊急度而定
├── ⏰ 審核超時告警
│   ├── 觸發時機: 審核時間超過 SLA
│   ├── 收件人: 審核人員主管、系統管理員
│   └── 緊急程度: 中等
├── 🎯 派工通知
│   ├── 觸發時機: 系統派工給廠商
│   ├── 收件人: 指定廠商、派工人員
│   └── 緊急程度: 依報修緊急度而定
└── ✅ 完修確認通知
    ├── 觸發時機: 廠商完修回報
    ├── 收件人: 門市人員、相關主管
    └── 緊急程度: 低
```

#### 1.2 廠商管理告警
```
🔨 廠商相關告警:
├── 🚫 廠商拒絕派工
│   ├── 觸發時機: 廠商拒絕接案
│   ├── 收件人: 派工人員、相關主管
│   └── 緊急程度: 中等
├── ⏰ 廠商回應超時
│   ├── 觸發時機: 超過回應時限未回應
│   ├── 收件人: 廠商聯絡人、派工人員
│   └── 緊急程度: 高
├── 💰 高額報價告警
│   ├── 觸發時機: 報價金額超過設定閾值
│   ├── 收件人: 主管、財務人員
│   └── 緊急程度: 中等
└── ⭐ 服務評價異常
    ├── 觸發時機: 評價低於標準
    ├── 收件人: 廠商管理員、品質管理員
    └── 緊急程度: 中等
```

### 2. 🖥️ 系統運行告警

#### 2.1 系統效能告警
```
⚡ 效能監控告警:
├── 🏃 回應時間異常
│   ├── 觸發條件: API 回應時間 > 5 秒
│   ├── 收件人: 系統管理員、開發團隊
│   └── 緊急程度: 高
├── 💾 記憶體使用異常
│   ├── 觸發條件: 記憶體使用率 > 85%
│   ├── 收件人: 系統管理員
│   └── 緊急程度: 中等
├── 💿 磁碟空間不足
│   ├── 觸發條件: 磁碟使用率 > 90%
│   ├── 收件人: 系統管理員、運維團隊
│   └── 緊急程度: 高
└── 🔗 資料庫連線異常
    ├── 觸發條件: 連線數 > 90% 或連線失敗
    ├── 收件人: DBA、系統管理員
    └── 緊急程度: 非常高
```

#### 2.2 安全相關告警
```
🔒 安全監控告警:
├── 🚪 異常登入告警
│   ├── 觸發條件: 異地登入、多次失敗
│   ├── 收件人: 資安人員、系統管理員
│   └── 緊急程度: 高
├── 🔑 權限異常操作
│   ├── 觸發條件: 越權操作、敏感功能使用
│   ├── 收件人: 資安人員、相關主管
│   └── 緊急程度: 高
├── 📊 資料異常存取
│   ├── 觸發條件: 大量資料下載、異常查詢
│   ├── 收件人: DBA、資安人員
│   └── 緊急程度: 中等
└── 🛡️ 系統攻擊偵測
    ├── 觸發條件: SQL Injection、XSS 攻擊
    ├── 收件人: 資安團隊、系統管理員
    └── 緊急程度: 非常高
```

### 3. 📊 業務統計告警

#### 3.1 KPI 指標告警
```
📈 KPI 監控告警:
├── 📉 完工率下降
│   ├── 觸發條件: 週完工率 < 85%
│   ├── 收件人: 業務主管、營運團隊
│   └── 緊急程度: 中等
├── ⏰ 平均處理時間超標
│   ├── 觸發條件: 平均處理時間超過 SLA 20%
│   ├── 收件人: 流程負責人、相關主管
│   └── 緊急程度: 中等
├── 😞 客戶滿意度下降
│   ├── 觸發條件: 滿意度 < 80%
│   ├── 收件人: 客服主管、品質管理員
│   └── 緊急程度: 中等
└── 💰 成本異常增加
    ├── 觸發條件: 單件平均成本增加 > 15%
    ├── 收件人: 財務主管、採購主管
    └── 緊急程度: 中等
```

---

## 通知渠道管理

### 1. 📧 Email 通知

#### 1.1 SMTP 設定
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.company.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "ftt-system@company.com",
    "Password": "encrypted_password",
    "FromAddress": "FTT系統 <ftt-system@company.com>",
    "ReplyToAddress": "no-reply@company.com",
    "MaxRetryAttempts": 3,
    "RetryInterval": "00:05:00"
  }
}
```

#### 1.2 Email 優先級設定
```
📧 Email 優先級:
├── 🚨 非常緊急 (Critical)
│   ├── 優先級: High
│   ├── 重要性: High
│   └── 立即發送，不等待批次
├── ⚡ 緊急 (High)
│   ├── 優先級: High
│   ├── 重要性: Normal
│   └── 5 分鐘內發送
├── 📋 中等 (Medium)
│   ├── 優先級: Normal
│   ├── 重要性: Normal
│   └── 15 分鐘內批次發送
└── ℹ️ 低 (Low)
    ├── 優先級: Low
    ├── 重要性: Low
    └── 1 小時內批次發送
```

### 2. 📱 簡訊通知

#### 2.1 簡訊服務設定
```json
{
  "SmsSettings": {
    "Provider": "TwilioSMS",
    "ApiKey": "encrypted_api_key",
    "ApiSecret": "encrypted_api_secret",
    "FromNumber": "+886-2-1234-5678",
    "MaxRetryAttempts": 2,
    "RetryInterval": "00:02:00",
    "DailyLimit": 1000,
    "CostPerMessage": 3.5
  }
}
```

#### 2.2 簡訊發送條件
```
📱 簡訊發送條件:
├── 🚨 緊急案件
│   ├── 非常緊急的報修案件
│   ├── 系統嚴重故障
│   └── 安全事件
├── ⏰ 重要超時
│   ├── 廠商回應超時
│   ├── 關鍵審核超時
│   └── SLA 嚴重違反
├── 🚫 通知失敗
│   ├── Email 連續發送失敗
│   ├── 系統推播失敗
│   └── 緊急事件需確保送達
└── 👤 使用者偏好
    └── 使用者設定接收簡訊通知
```

### 3. 🔔 系統推播通知

#### 3.1 即時推播
```javascript
// 即時推播範例
{
  "notification": {
    "title": "新派工通知",
    "body": "您收到一個新的維修派工：FTT20260112001",
    "icon": "/icons/dispatch.png",
    "badge": "/icons/badge.png",
    "tag": "dispatch-FTT20260112001",
    "requireInteraction": true,
    "actions": [
      {
        "action": "accept",
        "title": "接受派工"
      },
      {
        "action": "view",
        "title": "查看詳情"
      }
    ],
    "data": {
      "reportId": "FTT20260112001",
      "type": "dispatch",
      "url": "/dispatch/view/FTT20260112001"
    }
  }
}
```

### 4. 📞 電話通知 (緊急)

#### 4.1 自動語音通知
```
📞 語音通知設定:
├── 觸發條件: 系統嚴重故障、資安事件
├── 語音內容: 預錄制的告警訊息
├── 撥號順序: 依職位和責任區分
├── 確認機制: 按鍵確認收到訊息
└── 升級機制: 無人回應則升級處理
```

---

## 告警規則設定

### 1. 🔧 規則配置引擎

#### 1.1 規則定義格式
```json
{
  "alertRules": [
    {
      "id": "report_overtime_alert",
      "name": "報修審核超時告警",
      "description": "當報修單審核時間超過 SLA 時發送告警",
      "category": "business",
      "priority": "medium",
      "enabled": true,
      "conditions": {
        "eventType": "report_pending_review",
        "threshold": {
          "field": "pending_duration_minutes",
          "operator": ">",
          "value": 480
        },
        "filters": [
          {
            "field": "report_priority",
            "operator": "in",
            "value": ["urgent", "critical"]
          }
        ]
      },
      "actions": {
        "email": {
          "enabled": true,
          "template": "report_overtime_template",
          "recipients": [
            {
              "type": "role",
              "value": "reviewer"
            },
            {
              "type": "role", 
              "value": "supervisor"
            }
          ]
        },
        "sms": {
          "enabled": false
        },
        "push": {
          "enabled": true,
          "template": "simple_alert"
        }
      },
      "cooldown": "00:30:00",
      "maxAlertsPerDay": 10
    }
  ]
}
```

#### 1.2 動態規則調整
```csharp
// 動態調整告警閾值範例
public class AlertRuleManager
{
    public void AdjustThreshold(string ruleId, string field, object newValue)
    {
        var rule = GetRule(ruleId);
        rule.Conditions.Threshold.Value = newValue;
        
        // 記錄變更
        _auditLogger.LogRuleChange(ruleId, field, newValue);
        
        // 即時生效
        _ruleEngine.ReloadRule(rule);
    }
    
    public void EnableRule(string ruleId, bool enabled)
    {
        var rule = GetRule(ruleId);
        rule.Enabled = enabled;
        
        _auditLogger.LogRuleToggle(ruleId, enabled);
        _ruleEngine.ReloadRule(rule);
    }
}
```

### 2. 📊 智能告警抑制

#### 2.1 重複告警抑制
```
🔇 告警抑制機制:
├── 🕐 時間視窗抑制
│   ├── 同類告警 30 分鐘內只發送一次
│   ├── 系統故障告警 10 分鐘內抑制重複
│   └── 業務告警 1 小時內抑制重複
├── 📊 數量閾值抑制
│   ├── 同類告警超過 10 次/小時則升級
│   ├── 批次發送摘要取代個別通知
│   └── 風暴模式：暫停非關鍵告警
├── 🔗 相關性抑制
│   ├── 根本原因告警優先發送
│   ├── 抑制下游影響告警
│   └── 群組相關告警一起發送
└── 🎯 智能分組
    ├── 相同來源的告警分組
    ├── 相同時間窗口的告警合併
    └── 生成綜合告警摘要
```

### 3. ⚖️ 負載均衡發送

#### 3.1 發送速率控制
```csharp
public class NotificationRateLimiter
{
    private readonly Dictionary<string, RateLimitConfig> _rateLimits = new()
    {
        ["email"] = new RateLimitConfig
        {
            MaxPerSecond = 10,
            MaxPerMinute = 300,
            MaxPerHour = 5000,
            BurstSize = 50
        },
        ["sms"] = new RateLimitConfig
        {
            MaxPerSecond = 2,
            MaxPerMinute = 60,
            MaxPerHour = 1000,
            BurstSize = 10
        }
    };
    
    public async Task<bool> CanSendAsync(string channel, string recipient)
    {
        var config = _rateLimits[channel];
        var key = $"{channel}:{recipient}";
        
        // 檢查各級別限制
        return await CheckRateLimit(key, config);
    }
}
```

---

## 收件人管理

### 1. 👥 收件人分類

#### 1.1 角色型收件人
```json
{
  "roleBasedRecipients": {
    "reviewer": {
      "description": "報修審核人員",
      "members": [
        {
          "userId": "user001",
          "email": "reviewer1@company.com",
          "phone": "+886-912-345-678",
          "workingHours": "09:00-18:00",
          "timezone": "Asia/Taipei"
        }
      ],
      "escalationChain": [
        "reviewer",
        "supervisor", 
        "manager"
      ]
    },
    "vendor": {
      "description": "維修廠商",
      "dynamicSelection": true,
      "selectionCriteria": {
        "basedOnAssignment": true,
        "includeBackup": false
      }
    }
  }
}
```

#### 1.2 動態收件人選擇
```csharp
public class DynamicRecipientSelector
{
    public List<Recipient> SelectRecipients(AlertContext context)
    {
        var recipients = new List<Recipient>();
        
        switch (context.AlertType)
        {
            case "dispatch_notification":
                // 選擇被指派的廠商
                recipients.AddRange(GetAssignedVendors(context.ReportId));
                break;
                
            case "approval_required":
                // 根據金額選擇審核層級
                recipients.AddRange(GetApprovalChain(context.Amount));
                break;
                
            case "system_error":
                // 根據錯誤類型選擇技術團隊
                recipients.AddRange(GetTechnicalTeam(context.ErrorType));
                break;
        }
        
        return FilterByWorkingHours(recipients);
    }
}
```

### 2. 📅 時間排程管理

#### 2.1 工作時間設定
```json
{
  "workingTimeProfiles": {
    "standard": {
      "monday": {"start": "09:00", "end": "18:00"},
      "tuesday": {"start": "09:00", "end": "18:00"},
      "wednesday": {"start": "09:00", "end": "18:00"},
      "thursday": {"start": "09:00", "end": "18:00"},
      "friday": {"start": "09:00", "end": "18:00"},
      "saturday": {"enabled": false},
      "sunday": {"enabled": false},
      "holidays": ["2026-01-01", "2026-02-10"]
    },
    "emergency_support": {
      "24x7": true,
      "escalation_after_hours": true,
      "emergency_contacts": ["+886-911-123-456"]
    }
  }
}
```

#### 2.2 假日和值班管理
```csharp
public class OnDutyManager
{
    public List<Recipient> GetOnDutyRecipients(DateTime alertTime, string team)
    {
        // 檢查是否為工作時間
        if (IsWorkingTime(alertTime))
        {
            return GetRegularTeamMembers(team);
        }
        
        // 非工作時間，查找值班人員
        var onDutySchedule = GetOnDutySchedule(alertTime.Date, team);
        return onDutySchedule?.OnDutyPersons ?? GetEmergencyContacts(team);
    }
    
    private bool IsWorkingTime(DateTime time)
    {
        return !IsHoliday(time.Date) && 
               time.Hour >= 9 && time.Hour < 18 && 
               time.DayOfWeek != DayOfWeek.Saturday && 
               time.DayOfWeek != DayOfWeek.Sunday;
    }
}
```

### 3. 🎯 個人化偏好設定

#### 3.1 通知偏好管理
```json
{
  "userPreferences": {
    "userId": "user001",
    "notificationChannels": {
      "email": {
        "enabled": true,
        "priority": "medium_and_above",
        "digest": false,
        "workingHoursOnly": false
      },
      "sms": {
        "enabled": true,
        "priority": "high_and_above",
        "workingHoursOnly": true,
        "maxPerDay": 5
      },
      "push": {
        "enabled": true,
        "priority": "all",
        "quietHours": {
          "start": "22:00",
          "end": "07:00"
        }
      }
    },
    "alertCategories": {
      "business_alerts": true,
      "system_alerts": true,
      "security_alerts": true,
      "performance_alerts": false
    }
  }
}
```

---

## 郵件範本設計

### 1. 📧 HTML 郵件範本

#### 1.1 基礎範本架構
```html
<!DOCTYPE html>
<html lang="zh-TW">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{{alert_title}}</title>
    <style>
        .alert-container {
            max-width: 600px;
            margin: 0 auto;
            font-family: 'Microsoft JhengHei', Arial, sans-serif;
            background: #ffffff;
            border: 1px solid #e0e0e0;
        }
        .alert-header {
            background: {{priority_color}};
            color: white;
            padding: 20px;
            text-align: center;
        }
        .alert-content {
            padding: 30px;
            line-height: 1.6;
        }
        .alert-footer {
            background: #f5f5f5;
            padding: 15px;
            text-align: center;
            font-size: 12px;
            color: #666;
        }
        .action-button {
            display: inline-block;
            padding: 12px 24px;
            background: #007bff;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            margin: 10px 5px;
        }
        .info-table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }
        .info-table th,
        .info-table td {
            padding: 8px 12px;
            border: 1px solid #ddd;
            text-align: left;
        }
        .info-table th {
            background: #f8f9fa;
        }
    </style>
</head>
<body>
    <div class="alert-container">
        <div class="alert-header">
            <h1>🔔 {{alert_title}}</h1>
            <p>{{alert_time}}</p>
        </div>
        
        <div class="alert-content">
            <p>{{greeting}}</p>
            
            <p><strong>告警內容：</strong>{{alert_message}}</p>
            
            {{#if_business_alert}}
            <table class="info-table">
                <tr><th>報修單號</th><td>{{report_id}}</td></tr>
                <tr><th>門市名稱</th><td>{{store_name}}</td></tr>
                <tr><th>緊急程度</th><td>{{priority_level}}</td></tr>
                <tr><th>目前狀態</th><td>{{current_status}}</td></tr>
                <tr><th>責任人員</th><td>{{responsible_person}}</td></tr>
            </table>
            {{/if_business_alert}}
            
            {{#if_system_alert}}
            <table class="info-table">
                <tr><th>系統模組</th><td>{{system_module}}</td></tr>
                <tr><th>錯誤類型</th><td>{{error_type}}</td></tr>
                <tr><th>影響範圍</th><td>{{impact_scope}}</td></tr>
                <tr><th>建議處理</th><td>{{suggested_action}}</td></tr>
            </table>
            {{/if_system_alert}}
            
            <div style="text-align: center; margin: 30px 0;">
                {{#if_action_required}}
                <a href="{{action_url}}" class="action-button">立即處理</a>
                {{/if_action_required}}
                <a href="{{detail_url}}" class="action-button" style="background: #6c757d;">查看詳情</a>
            </div>
            
            <div style="background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0;">
                <strong>⚠️ 重要提醒：</strong>
                <p>{{important_note}}</p>
            </div>
        </div>
        
        <div class="alert-footer">
            <p>此郵件由 FTT 門市報修管理系統自動發送，請勿直接回覆</p>
            <p>如有疑問，請聯繫系統管理員：<a href="mailto:admin@company.com">admin@company.com</a></p>
            <p>發送時間：{{send_time}} | 系統版本：{{system_version}}</p>
        </div>
    </div>
</body>
</html>
```

#### 1.2 不同類型告警範本

##### 📋 業務流程告警範本
```html
<!-- 派工通知範本 -->
<div class="dispatch-notification">
    <h2>🎯 新派工通知</h2>
    <p>親愛的 {{vendor_name}} 您好，</p>
    <p>您收到一個新的維修派工，詳細資訊如下：</p>
    
    <table class="info-table">
        <tr><th>報修單號</th><td>{{report_id}}</td></tr>
        <tr><th>門市名稱</th><td>{{store_name}}</td></tr>
        <tr><th>門市地址</th><td>{{store_address}}</td></tr>
        <tr><th>聯絡人</th><td>{{contact_person}}</td></tr>
        <tr><th>聯絡電話</th><td>{{contact_phone}}</td></tr>
        <tr><th>故障描述</th><td>{{fault_description}}</td></tr>
        <tr><th>緊急程度</th><td><span class="priority-{{priority_class}}">{{priority_text}}</span></td></tr>
        <tr><th>期望完成</th><td>{{expected_completion}}</td></tr>
    </table>
    
    <div class="action-section">
        <p><strong>⏰ 請於 {{response_deadline}} 前回應此派工</strong></p>
        <a href="{{accept_url}}" class="action-button" style="background: #28a745;">接受派工</a>
        <a href="{{reject_url}}" class="action-button" style="background: #dc3545;">拒絕派工</a>
        <a href="{{detail_url}}" class="action-button">查看詳情</a>
    </div>
</div>
```

##### 🚨 系統故障告警範本
```html
<!-- 系統故障範本 -->
<div class="system-error-alert">
    <h2>🚨 系統故障告警</h2>
    <p>系統偵測到以下故障事件：</p>
    
    <table class="info-table">
        <tr><th>故障時間</th><td>{{error_time}}</td></tr>
        <tr><th>故障模組</th><td>{{module_name}}</td></tr>
        <tr><th>錯誤訊息</th><td><code>{{error_message}}</code></td></tr>
        <tr><th>錯誤等級</th><td><span class="severity-{{severity_class}}">{{severity_text}}</span></td></tr>
        <tr><th>影響用戶</th><td>{{affected_users}}</td></tr>
        <tr><th>系統狀態</th><td>{{system_status}}</td></tr>
    </table>
    
    <div class="error-details">
        <h3>📋 詳細資訊：</h3>
        <pre>{{error_stack_trace}}</pre>
    </div>
    
    <div class="action-section">
        <a href="{{monitoring_url}}" class="action-button">查看監控</a>
        <a href="{{logs_url}}" class="action-button">檢視日誌</a>
        <a href="{{incident_url}}" class="action-button" style="background: #dc3545;">建立事件單</a>
    </div>
</div>
```

### 2. 📱 簡訊範本

#### 2.1 簡潔簡訊範本
```
📱 簡訊範本設計原則:
├── 字數限制: 70 字以內 (中文)
├── 包含關鍵資訊: 事件類型、緊急程度、行動連結
├── 清楚明瞭: 避免專業術語
└── 呼籲行動: 明確指示下一步行動
```

#### 2.2 不同類型簡訊範本
```
🎯 派工通知 SMS:
"【FTT派工】您收到新派工 {{report_id}}，{{store_name}} {{fault_type}}，{{priority_level}}，請於{{deadline}}前回應。詳情: {{short_url}}"

🚨 系統故障 SMS:
"【FTT告警】{{system_name}}發生{{error_type}}，影響{{scope}}，請立即處理。監控: {{short_url}}"

⏰ 超時提醒 SMS:
"【FTT提醒】{{task_type}}已超時{{overtime}}，請儘速處理 {{report_id}}。處理: {{short_url}}"
```

---

## 告警升級機制

### 1. 📈 自動升級規則

#### 1.1 時間型升級
```json
{
  "escalationRules": {
    "report_approval_delay": {
      "stages": [
        {
          "level": 1,
          "delay": "01:00:00",
          "recipients": ["reviewer"],
          "actions": ["email", "push"]
        },
        {
          "level": 2, 
          "delay": "02:00:00",
          "recipients": ["reviewer", "supervisor"],
          "actions": ["email", "sms", "push"]
        },
        {
          "level": 3,
          "delay": "04:00:00", 
          "recipients": ["supervisor", "manager"],
          "actions": ["email", "sms", "phone"]
        }
      ],
      "maxLevel": 3,
      "autoResolve": false
    }
  }
}
```

#### 1.2 條件型升級
```csharp
public class ConditionalEscalation
{
    public bool ShouldEscalate(AlertContext context)
    {
        // 基於業務邏輯的升級條件
        return context.Priority == "critical" ||
               context.AffectedUsers > 100 ||
               context.DowntimeMinutes > 60 ||
               context.ConsecutiveFailures > 5;
    }
    
    public EscalationAction GetEscalationAction(AlertContext context)
    {
        if (context.Priority == "critical")
        {
            return new EscalationAction
            {
                Level = EscalationLevel.Emergency,
                Recipients = GetEmergencyContacts(),
                Channels = new[] { "email", "sms", "phone" },
                RequireAcknowledgment = true
            };
        }
        
        return GetStandardEscalation(context);
    }
}
```

### 2. 🎯 智能升級算法

#### 2.1 機器學習升級預測
```csharp
public class IntelligentEscalation
{
    private readonly IMLModel _escalationModel;
    
    public EscalationPrediction PredictEscalation(AlertContext context)
    {
        var features = ExtractFeatures(context);
        var prediction = _escalationModel.Predict(features);
        
        return new EscalationPrediction
        {
            ShouldEscalate = prediction.Probability > 0.7,
            RecommendedLevel = prediction.Level,
            Confidence = prediction.Probability,
            Reasoning = prediction.Factors
        };
    }
    
    private MLFeatures ExtractFeatures(AlertContext context)
    {
        return new MLFeatures
        {
            TimeOfDay = context.Timestamp.Hour,
            DayOfWeek = (int)context.Timestamp.DayOfWeek,
            Priority = MapPriorityToNumeric(context.Priority),
            HistoricalEscalations = GetHistoricalEscalationCount(context.Type),
            SystemLoad = GetCurrentSystemLoad(),
            TeamAvailability = GetTeamAvailabilityScore()
        };
    }
}
```

### 3. 🔔 升級通知管理

#### 3.1 升級狀態追蹤
```csharp
public class EscalationTracker
{
    public void TrackEscalation(string alertId, int level, List<string> recipients)
    {
        var escalation = new EscalationRecord
        {
            AlertId = alertId,
            Level = level,
            EscalatedAt = DateTime.UtcNow,
            Recipients = recipients,
            Status = EscalationStatus.Pending
        };
        
        _repository.SaveEscalation(escalation);
        
        // 設定確認超時
        _scheduler.ScheduleAcknowledgmentTimeout(alertId, TimeSpan.FromMinutes(30));
    }
    
    public void HandleAcknowledgment(string alertId, string userId)
    {
        var escalation = _repository.GetActiveEscalation(alertId);
        escalation.AcknowledgedBy = userId;
        escalation.AcknowledgedAt = DateTime.UtcNow;
        escalation.Status = EscalationStatus.Acknowledged;
        
        // 取消後續升級
        _scheduler.CancelEscalation(alertId);
        
        // 通知其他收件人
        NotifyAcknowledgment(escalation);
    }
}
```

---

## 監控儀表板

### 1. 📊 即時監控面板

#### 1.1 告警統計儀表板
```html
<!-- 告警監控儀表板 -->
<div class="alert-dashboard">
    <div class="stats-row">
        <div class="stat-card critical">
            <h3>🚨 關鍵告警</h3>
            <div class="number">{{critical_count}}</div>
            <div class="trend">{{critical_trend}}</div>
        </div>
        <div class="stat-card warning">
            <h3>⚠️ 警告告警</h3>
            <div class="number">{{warning_count}}</div>
            <div class="trend">{{warning_trend}}</div>
        </div>
        <div class="stat-card info">
            <h3>ℹ️ 資訊告警</h3>
            <div class="number">{{info_count}}</div>
            <div class="trend">{{info_trend}}</div>
        </div>
        <div class="stat-card resolved">
            <h3>✅ 已處理</h3>
            <div class="number">{{resolved_count}}</div>
            <div class="trend">{{resolved_trend}}</div>
        </div>
    </div>
    
    <div class="charts-row">
        <div class="chart-container">
            <canvas id="alertTrendChart"></canvas>
        </div>
        <div class="chart-container">
            <canvas id="alertCategoryChart"></canvas>
        </div>
    </div>
    
    <div class="active-alerts">
        <h2>🔴 活躍告警</h2>
        <table class="alerts-table">
            <thead>
                <tr>
                    <th>時間</th>
                    <th>類型</th>
                    <th>優先級</th>
                    <th>來源</th>
                    <th>狀態</th>
                    <th>操作</th>
                </tr>
            </thead>
            <tbody>
                {{#each active_alerts}}
                <tr class="alert-row priority-{{priority}}">
                    <td>{{timestamp}}</td>
                    <td>{{type}}</td>
                    <td><span class="priority-badge">{{priority}}</span></td>
                    <td>{{source}}</td>
                    <td><span class="status-badge">{{status}}</span></td>
                    <td>
                        <button class="btn-ack" data-id="{{id}}">確認</button>
                        <button class="btn-view" data-id="{{id}}">檢視</button>
                    </td>
                </tr>
                {{/each}}
            </tbody>
        </table>
    </div>
</div>
```

### 2. 📈 效能分析報表

#### 2.1 通知效能統計
```sql
-- 通知發送統計查詢
SELECT 
    notification_type,
    channel,
    DATE(sent_at) as send_date,
    COUNT(*) as total_sent,
    COUNT(CASE WHEN status = 'delivered' THEN 1 END) as delivered,
    COUNT(CASE WHEN status = 'failed' THEN 1 END) as failed,
    COUNT(CASE WHEN opened_at IS NOT NULL THEN 1 END) as opened,
    AVG(EXTRACT(EPOCH FROM (delivered_at - sent_at))) as avg_delivery_time,
    ROUND(
        COUNT(CASE WHEN status = 'delivered' THEN 1 END) * 100.0 / COUNT(*), 2
    ) as delivery_rate,
    ROUND(
        COUNT(CASE WHEN opened_at IS NOT NULL THEN 1 END) * 100.0 / 
        COUNT(CASE WHEN status = 'delivered' THEN 1 END), 2
    ) as open_rate
FROM notification_log
WHERE sent_at >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY notification_type, channel, DATE(sent_at)
ORDER BY send_date DESC, notification_type, channel;
```

### 3. 🔍 告警分析工具

#### 3.1 根因分析
```csharp
public class AlertRootCauseAnalyzer
{
    public RootCauseAnalysis AnalyzeAlert(string alertId)
    {
        var alert = GetAlert(alertId);
        var timeline = GetAlertTimeline(alertId);
        var relatedEvents = FindRelatedEvents(alert.Timestamp, alert.Source);
        
        var analysis = new RootCauseAnalysis
        {
            AlertId = alertId,
            PotentialCauses = IdentifyPotentialCauses(alert, relatedEvents),
            ImpactAssessment = AssessImpact(alert, relatedEvents),
            RecommendedActions = GenerateRecommendations(alert),
            SimilarIncidents = FindSimilarIncidents(alert)
        };
        
        return analysis;
    }
    
    private List<PotentialCause> IdentifyPotentialCauses(Alert alert, List<Event> relatedEvents)
    {
        var causes = new List<PotentialCause>();
        
        // 分析系統事件
        var systemEvents = relatedEvents.Where(e => e.Type == "system").ToList();
        if (systemEvents.Any())
        {
            causes.Add(new PotentialCause
            {
                Type = "SystemEvent",
                Description = "系統事件引發的連鎖反應",
                Confidence = CalculateConfidence(systemEvents),
                Evidence = systemEvents.Select(e => e.Description).ToList()
            });
        }
        
        // 分析業務流程
        var businessEvents = relatedEvents.Where(e => e.Type == "business").ToList();
        if (businessEvents.Any())
        {
            causes.Add(new PotentialCause
            {
                Type = "BusinessProcess",
                Description = "業務流程異常",
                Confidence = CalculateConfidence(businessEvents),
                Evidence = businessEvents.Select(e => e.Description).ToList()
            });
        }
        
        return causes.OrderByDescending(c => c.Confidence).ToList();
    }
}
```

---

## 設定檔管理

### 1. ⚙️ 設定檔結構

#### 1.1 主要設定檔
```json
{
  "AlertingSystem": {
    "Enabled": true,
    "DefaultTimezone": "Asia/Taipei",
    "MaxConcurrentAlerts": 100,
    "AlertRetentionDays": 90,
    "DefaultCooldownMinutes": 30,
    
    "Channels": {
      "Email": {
        "Enabled": true,
        "SmtpSettings": {
          "Host": "smtp.company.com",
          "Port": 587,
          "EnableSsl": true,
          "Username": "{{encrypted_username}}",
          "Password": "{{encrypted_password}}"
        },
        "RateLimits": {
          "PerSecond": 10,
          "PerMinute": 300,
          "PerHour": 5000
        }
      },
      
      "SMS": {
        "Enabled": true,
        "Provider": "TwilioSMS",
        "Settings": {
          "AccountSid": "{{encrypted_sid}}",
          "AuthToken": "{{encrypted_token}}",
          "FromNumber": "+886-2-1234-5678"
        },
        "RateLimits": {
          "PerMinute": 60,
          "PerHour": 1000,
          "DailyCostLimit": 1000
        }
      },
      
      "Push": {
        "Enabled": true,
        "FirebaseSettings": {
          "ProjectId": "ftt-notifications",
          "ServiceAccountKey": "{{encrypted_key}}"
        }
      }
    },
    
    "Templates": {
      "BasePath": "/templates/alerts/",
      "CacheDurationMinutes": 60,
      "SupportedLanguages": ["zh-TW", "en-US"]
    },
    
    "Monitoring": {
      "MetricsEnabled": true,
      "DetailedLogging": true,
      "PerformanceThresholds": {
        "MaxProcessingTimeMs": 5000,
        "MaxQueueSize": 1000
      }
    }
  }
}
```

### 2. 🔧 動態設定更新

#### 2.1 熱更新機制
```csharp
public class AlertConfigurationManager
{
    private readonly IConfigurationRoot _configuration;
    private readonly IOptionsMonitor<AlertingOptions> _options;
    
    public AlertConfigurationManager(
        IConfigurationRoot configuration,
        IOptionsMonitor<AlertingOptions> options)
    {
        _configuration = configuration;
        _options = options;
        
        // 監聽設定變更
        _options.OnChange(OnConfigurationChanged);
    }
    
    private void OnConfigurationChanged(AlertingOptions newOptions)
    {
        _logger.LogInformation("Alert configuration changed, reloading...");
        
        // 重新載入規則引擎
        _ruleEngine.ReloadConfiguration(newOptions.Rules);
        
        // 更新通知渠道設定
        _notificationChannels.UpdateConfiguration(newOptions.Channels);
        
        // 重新初始化範本引擎
        _templateEngine.ReloadTemplates(newOptions.Templates);
        
        _logger.LogInformation("Alert configuration reloaded successfully");
    }
    
    public async Task UpdateConfigurationAsync(string section, object value)
    {
        // 更新設定檔
        _configuration[section] = JsonSerializer.Serialize(value);
        
        // 觸發設定重載
        await _configurationService.ReloadAsync();
        
        // 記錄變更
        _auditLogger.LogConfigurationChange(section, value);
    }
}
```

### 3. 🔐 安全性設定

#### 3.1 敏感資料加密
```csharp
public class SecureConfigurationProvider
{
    private readonly IDataProtector _protector;
    
    public SecureConfigurationProvider(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("AlertConfiguration");
    }
    
    public string EncryptSetting(string plainText)
    {
        return _protector.Protect(plainText);
    }
    
    public string DecryptSetting(string encryptedText)
    {
        try
        {
            return _protector.Unprotect(encryptedText);
        }
        catch (CryptographicException)
        {
            _logger.LogError("Failed to decrypt configuration setting");
            throw new ConfigurationException("Invalid encrypted configuration");
        }
    }
    
    public void RotateEncryptionKeys()
    {
        // 密鑰輪換邏輯
        var newProtector = CreateNewProtector();
        ReencryptAllSettings(newProtector);
    }
}
```

---

## 故障排除

### 1. 🔍 常見問題診斷

#### 1.1 郵件發送失敗
```csharp
public class EmailTroubleshooter
{
    public DiagnosticResult DiagnoseEmailIssue(string recipientEmail)
    {
        var result = new DiagnosticResult();
        
        // 檢查 SMTP 連線
        try
        {
            using var client = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port);
            client.Connect();
            result.SmtpConnection = "OK";
        }
        catch (Exception ex)
        {
            result.SmtpConnection = $"Failed: {ex.Message}";
        }
        
        // 檢查郵件地址格式
        if (!IsValidEmailAddress(recipientEmail))
        {
            result.EmailFormat = $"Invalid: {recipientEmail}";
        }
        
        // 檢查黑名單
        if (IsBlacklisted(recipientEmail))
        {
            result.Blacklist = $"Blacklisted: {recipientEmail}";
        }
        
        // 檢查速率限制
        if (IsRateLimited(recipientEmail))
        {
            result.RateLimit = "Rate limit exceeded";
        }
        
        return result;
    }
}
```

#### 1.2 告警規則除錯
```csharp
public class AlertRuleDebugger
{
    public RuleEvaluationResult DebugRule(string ruleId, AlertContext context)
    {
        var rule = GetRule(ruleId);
        var result = new RuleEvaluationResult
        {
            RuleId = ruleId,
            Context = context,
            Steps = new List<EvaluationStep>()
        };
        
        // 逐步評估條件
        foreach (var condition in rule.Conditions)
        {
            var stepResult = EvaluateCondition(condition, context);
            result.Steps.Add(new EvaluationStep
            {
                Condition = condition.ToString(),
                Result = stepResult.Passed,
                ActualValue = stepResult.ActualValue,
                ExpectedValue = stepResult.ExpectedValue,
                Message = stepResult.Message
            });
            
            if (!stepResult.Passed && rule.LogicOperator == "AND")
            {
                result.FinalResult = false;
                result.FailureReason = stepResult.Message;
                break;
            }
        }
        
        return result;
    }
}
```

### 2. 📊 效能問題診斷

#### 2.1 通知佇列監控
```csharp
public class NotificationQueueMonitor
{
    public QueueHealthReport GetQueueHealth()
    {
        return new QueueHealthReport
        {
            QueueSize = _queue.Count,
            ProcessingRate = CalculateProcessingRate(),
            AverageWaitTime = CalculateAverageWaitTime(),
            FailureRate = CalculateFailureRate(),
            OldestMessage = GetOldestMessageAge(),
            Warnings = GenerateWarnings()
        };
    }
    
    private List<string> GenerateWarnings()
    {
        var warnings = new List<string>();
        
        if (_queue.Count > 1000)
        {
            warnings.Add("Queue size is high, consider scaling up workers");
        }
        
        if (CalculateProcessingRate() < 10)
        {
            warnings.Add("Low processing rate detected");
        }
        
        if (GetOldestMessageAge() > TimeSpan.FromMinutes(30))
        {
            warnings.Add("Messages are aging in queue");
        }
        
        return warnings;
    }
}
```

### 3. 🛠️ 自動修復機制

#### 3.1 自癒功能
```csharp
public class AutoHealingService
{
    public async Task<bool> AttemptAutoHealing(AlertContext context)
    {
        switch (context.ErrorType)
        {
            case "smtp_connection_failed":
                return await RestartSmtpService();
                
            case "queue_overflow":
                return await ScaleUpWorkers();
                
            case "template_not_found":
                return await ReloadTemplates();
                
            case "rate_limit_exceeded":
                return await ImplementCircuitBreaker();
                
            default:
                return false;
        }
    }
    
    private async Task<bool> RestartSmtpService()
    {
        try
        {
            _smtpClient.Disconnect();
            await Task.Delay(5000);
            _smtpClient.Connect();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

---

## 📞 技術支援與維護

### 聯絡資訊
- **告警系統負責人**: alerts@company.com
- **緊急聯絡電話**: 0800-ALERT-24 (0800-253-7824)
- **技術支援時間**: 24x7 (關鍵告警)
- **一般維護時間**: 週一至週五 09:00-18:00

### 維護排程
- **每日**: 系統健康檢查、效能監控
- **每週**: 告警規則檢視、失敗通知分析
- **每月**: 效能調校、範本更新、統計報告
- **每季**: 全面系統檢視、災難復原演練

### 效能指標
- **可用性**: 99.9% 以上
- **通知延遲**: 關鍵告警 < 30 秒，一般告警 < 5 分鐘
- **成功率**: Email > 95%，SMS > 98%
- **確認率**: 關鍵告警 100%，一般告警 > 85%

---

*本文件版本: v1.0 | 最後更新: 2026年1月12日*
