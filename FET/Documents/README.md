# FTT 門市報修管理系統

## 系統概述
FTT (Field Technical Trouble) 門市報修管理系統是一個完整的報修流程管理平台，提供門市端與廠商端的雙向整合服務。

## 系統架構
```
┌─────────────────┐    ┌─────────────────┐
│   FTT_WEB       │    │ FTT_VENDER_WEB  │
│   (門市前端)     │    │   (廠商前端)     │
└─────────────────┘    └─────────────────┘
        │                       │
        │                       │
┌─────────────────┐    ┌─────────────────┐
│   FTT_API       │    │ FTT_VENDER_API  │
│   (門市後端)     │    │   (廠商後端)     │
└─────────────────┘    └─────────────────┘
        │                       │
        └───────┬───────────────┘
                │
    ┌─────────────────┐
    │   PostgreSQL    │
    │   資料庫        │
    └─────────────────┘
```

## 主要功能模組

### 門市端 (FTT_WEB + FTT_API)
- 報修單新增與管理
- 審單流程處理
- 派工管理
- 報價確認
- 完修結案

### 廠商端 (FTT_VENDER_WEB + FTT_VENDER_API)
- 接收派工通知
- 現場處理回報
- 報價作業
- 完修確認
- 廠商管理

## 工作流程狀態
根據 `appsettings.json` 的配置，系統支援以下狀態轉換：

```
NEW → REVIEW → DISPATCH → ASSIGN → TICKET → COMPLETE → CLOSE
     ↓         ↓         ↓        ↓        ↓
   SELF     REVISE    USED    ASSETER   OFFER
     ↓         ↓                  ↓        ↓
   USED     CANCEL              REVIEW2   AGREE/DENY
```

## 技術規格
- **Framework**: ASP.NET Core 
- **Database**: PostgreSQL
- **Authentication**: JWT Token
- **Background Jobs**: Hangfire
- **Documentation**: Swagger UI

## 快速開始
詳見 [開發環境設定指南](Development/Setup-Guide.md)

## 相關文件

### 📋 架構文件
- [系統架構詳細說明](Architecture/System-Architecture.md) - 完整的系統架構、技術棧與設計模式
- [專案結構說明](Architecture/Project-Structure.md) - 詳細的專案結構與模組說明
- [資料庫架構說明](Architecture/Database-Schema.md) - 完整的資料庫設計與資料字典

### 🔧 開發文件
- [開發環境設定指南](Development/Setup-Guide.md) - 詳細的開發環境建置步驟
- [部署指南](Development/Deployment-Guide.md) - 多種部署方式的完整說明
- [開發者指南](Development/Developer-Guide.md) - 開發環境設置、編碼規範、測試與部署流程

### 📡 API 文件
- [FTT API 文件](API-Documentation/FTT-API.md) - 門市系統 API 詳細說明
- [FTT VENDER API 文件](API-Documentation/FTT-VENDER-API.md) - 廠商系統 API 說明

### 🔄 業務流程
- [業務流程與狀態管理](Business-Process/Workflow.md) - 完整的報修流程圖與狀態轉換說明

### 👥 使用手冊
- [門市使用手冊](User-Guide/Store-User-Guide.md) - 門市人員操作指南
- [廠商使用手冊](User-Guide/Vendor-User-Guide.md) - 廠商操作指南
- [系統管理手冊](User-Guide/Admin-Guide.md) - 系統管理員手冊

### 📞 Support (支援與維護)
- [常見問題與故障排除 (FAQ & Troubleshooting)](Support/FAQ-Troubleshooting.md) - 系統操作問題解決方案與故障排除指南

### 🔒 Security (安全性)
- [安全性與合規指南 (Security & Compliance Guide)](Security/Security-Compliance-Guide.md) - 系統安全架構、資料保護、合規要求與安全事件響應
