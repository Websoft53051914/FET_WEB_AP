# 系統管理員手冊

## 📋 目錄
- [系統概述](#系統概述)
- [系統架構管理](#系統架構管理)
- [使用者管理](#使用者管理)
- [權限管理](#權限管理)
- [系統監控](#系統監控)
- [資料庫管理](#資料庫管理)
- [備份與還原](#備份與還原)
- [安全管理](#安全管理)
- [效能調校](#效能調校)
- [故障排除](#故障排除)

---

## 系統概述

FTT 門市報修管理系統是一個分散式的企業級應用系統，需要專業的系統管理來確保穩定運行。

### 管理職責
- 🖥️ 系統環境維護
- 👥 使用者帳號管理
- 🔐 權限與安全管理
- 📊 系統效能監控
- 🗄️ 資料庫維護
- 💾 備份與災難復原
- 🔧 故障診斷與排除

### 系統架構
```
🏗️ 系統組件:
├── 前端應用 (FTT_WEB, FTT_VENDER_WEB)
├── API 服務 (FTT_API, FTT_VENDER_API)  
├── 資料庫 (PostgreSQL)
├── 背景服務 (Hangfire)
├── 檔案存儲 (File System)
└── 反向代理 (Nginx/IIS)
```

---

## 系統架構管理

### 服務管理

#### 1. Windows 服務管理
```powershell
# 查看服務狀態
Get-Service | Where-Object {$_.Name -like "*FTT*"}

# 重啟 IIS 應用程式
iisreset

# 重啟特定應用程式池
Restart-WebAppPool -Name "FTT_WEB_Pool"
Restart-WebAppPool -Name "FTT_API_Pool"

# 查看應用程式池狀態
Get-IISAppPool
```

#### 2. Linux 系統服務
```bash
# 查看服務狀態
systemctl status ftt-api
systemctl status ftt-web

# 重啟服務
sudo systemctl restart ftt-api
sudo systemctl restart ftt-web

# 查看服務日誌
sudo journalctl -u ftt-api -f
sudo journalctl -u ftt-web -f

# 設定開機自動啟動
sudo systemctl enable ftt-api
sudo systemctl enable ftt-web
```

#### 3. Docker 容器管理
```bash
# 查看容器狀態
docker-compose ps

# 重啟特定服務
docker-compose restart ftt-api
docker-compose restart ftt-web

# 查看容器日誌
docker-compose logs -f ftt-api
docker-compose logs -f ftt-web

# 更新服務
docker-compose pull
docker-compose up -d
```

### 環境配置

#### 1. 應用程式設定檔
```json
// appsettings.json 重要設定項目
{
  "ConnectionStrings": {
    "MainConnection": "Server=10.64.35.138;Port=5444;Database=infwf;User Id=ftt;Password=ftt123;CommandTimeout=86400"
  },
  "JwtConfig": {
    "Secret": "your-secret-key",
    "ExpireTimeDuration": "1800",
    "Issuer": "FET"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### 2. 負載均衡設定
```nginx
# Nginx 負載均衡設定
upstream ftt-api-backend {
    server ftt-api-1:80 weight=3;
    server ftt-api-2:80 weight=2;
    server ftt-api-3:80 weight=1 backup;
}

server {
    listen 80;
    location /api/ {
        proxy_pass http://ftt-api-backend/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        
        # 健康檢查
        health_check;
    }
}
```

---

## 使用者管理

### 帳號管理

#### 1. 新增使用者
```sql
-- 門市使用者
INSERT INTO sys_user (
    username, password_hash, display_name, 
    email, role_id, is_active
) VALUES (
    'store001', '$2a$11$...', '信義門市管理員',
    'store001@company.com', 2, 'Y'
);

-- 廠商使用者  
INSERT INTO sys_user (
    username, password_hash, display_name,
    email, role_id, is_active
) VALUES (
    'vendor001', '$2a$11$...', 'ABC維修公司',
    'contact@abc-repair.com', 3, 'Y'  
);
```

#### 2. 密碼重設
```sql
-- 重設密碼 (需要加密)
UPDATE sys_user 
SET password_hash = '$2a$11$newhashedpassword',
    password_expire_date = CURRENT_DATE + INTERVAL '90 days'
WHERE username = 'store001';
```

#### 3. 帳號狀態管理
```sql
-- 停用帳號
UPDATE sys_user SET is_active = 'N' WHERE username = 'store001';

-- 啟用帳號  
UPDATE sys_user SET is_active = 'Y' WHERE username = 'store001';

-- 查看最近登入記錄
SELECT username, display_name, last_login, is_active
FROM sys_user 
ORDER BY last_login DESC 
LIMIT 20;
```

### 批次操作

#### 1. 匯入使用者
```powershell
# PowerShell 腳本範例
$users = Import-Csv "users.csv"
foreach($user in $users) {
    $hashedPassword = Get-HashedPassword $user.Password
    $sql = "INSERT INTO sys_user (username, password_hash, display_name, email, role_id) VALUES ('$($user.Username)', '$hashedPassword', '$($user.DisplayName)', '$($user.Email)', $($user.RoleId))"
    Invoke-SqlCommand $sql
}
```

#### 2. 批次密碼重設
```sql
-- 強制所有使用者更新密碼
UPDATE sys_user 
SET password_expire_date = CURRENT_DATE
WHERE is_active = 'Y';
```

---

## 權限管理

### 角色定義

#### 1. 系統角色
```sql
-- 查看現有角色
SELECT * FROM sys_role ORDER BY role_id;

-- 新增角色
INSERT INTO sys_role (role_name, role_desc, permissions, is_active)
VALUES (
    '區域主管', 
    '負責特定區域的門市管理',
    '{"menus":["report","approve","query"],"actions":["create","read","update"]}',
    'Y'
);
```

#### 2. 權限設定
```json
// 權限設定範例
{
  "menus": [
    "report",      // 報修管理
    "approve",     // 審單作業  
    "dispatch",    // 派工管理
    "quotation",   // 報價管理
    "complete",    // 完修管理
    "query",       // 查詢功能
    "admin"        // 系統管理
  ],
  "actions": [
    "create",      // 新增
    "read",        // 查詢
    "update",      // 修改
    "delete",      // 刪除
    "approve",     // 審核
    "export"       // 匯出
  ]
}
```

### 權限檢查

#### 1. 使用者權限查詢
```sql
-- 查看使用者權限
SELECT 
    u.username,
    u.display_name,
    r.role_name,
    r.permissions
FROM sys_user u
LEFT JOIN sys_role r ON u.role_id = r.role_id
WHERE u.is_active = 'Y'
ORDER BY u.username;
```

#### 2. 權限審計
```sql
-- 查看特定權限的使用者
SELECT u.username, u.display_name
FROM sys_user u
JOIN sys_role r ON u.role_id = r.role_id
WHERE r.permissions::json->'actions' ? 'delete'
AND u.is_active = 'Y';
```

---

## 系統監控

### 效能監控

#### 1. 系統資源監控
```bash
# CPU 和記憶體使用率
top -p $(pgrep -d, dotnet)

# 磁碟使用量
df -h

# 網路連線狀態
netstat -tulpn | grep :80
netstat -tulpn | grep :5444

# 程序記憶體使用
ps aux | grep dotnet
```

#### 2. 應用程式效能
```sql
-- 資料庫連線數
SELECT count(*) as active_connections 
FROM pg_stat_activity 
WHERE state = 'active';

-- 長時間運行的查詢
SELECT 
    pid,
    now() - pg_stat_activity.query_start AS duration,
    query 
FROM pg_stat_activity 
WHERE (now() - pg_stat_activity.query_start) > interval '5 minutes'
ORDER BY duration DESC;

-- 資料庫大小
SELECT 
    pg_database.datname,
    pg_size_pretty(pg_database_size(pg_database.datname)) AS size
FROM pg_database
ORDER BY pg_database_size(pg_database.datname) DESC;
```

### 日誌監控

#### 1. 應用程式日誌
```bash
# 查看錯誤日誌
tail -f /var/log/ftt/api-*.txt | grep -i error

# 統計錯誤次數
grep -c "ERROR" /var/log/ftt/api-$(date +%Y%m%d).txt

# 查看 API 回應時間
grep "Request completed" /var/log/ftt/api-*.txt | \
awk '{print $NF}' | sort -n | tail -20
```

#### 2. 系統日誌
```bash
# 系統錯誤日誌
sudo tail -f /var/log/syslog | grep ftt

# Nginx 存取日誌
sudo tail -f /var/log/nginx/access.log

# Nginx 錯誤日誌  
sudo tail -f /var/log/nginx/error.log
```

### 告警設定

#### 1. 系統告警腳本
```bash
#!/bin/bash
# 系統健康檢查腳本

# 檢查服務狀態
if ! systemctl is-active --quiet ftt-api; then
    echo "FTT API 服務異常" | mail -s "系統告警" admin@company.com
fi

# 檢查資料庫連線
if ! pg_isready -h localhost -p 5444 -U ftt; then
    echo "資料庫連線異常" | mail -s "資料庫告警" admin@company.com
fi

# 檢查磁碟空間
disk_usage=$(df -h / | awk 'NR==2 {print $5}' | sed 's/%//')
if [ $disk_usage -gt 85 ]; then
    echo "磁碟使用率達到 $disk_usage%" | mail -s "磁碟空間告警" admin@company.com
fi
```

#### 2. 自動監控設定
```bash
# 設定 crontab 定期檢查
crontab -e

# 每 5 分鐘檢查一次
*/5 * * * * /usr/local/bin/ftt-health-check.sh

# 每小時產生效能報告
0 * * * * /usr/local/bin/ftt-performance-report.sh
```

---

## 資料庫管理

### 日常維護

#### 1. 統計資訊更新
```sql
-- 更新資料表統計資訊
ANALYZE;

-- 更新特定資料表
ANALYZE ftt_report_main;
ANALYZE ftt_dispatch_log;

-- 檢查統計資訊最後更新時間
SELECT 
    schemaname, tablename, 
    last_analyze, last_autoanalyze
FROM pg_stat_user_tables
WHERE schemaname = 'public'
ORDER BY last_analyze DESC;
```

#### 2. 索引維護
```sql
-- 重建索引
REINDEX INDEX idx_ftt_report_main_form_no;
REINDEX TABLE ftt_report_main;

-- 檢查索引使用率
SELECT 
    t.tablename,
    indexname,
    c.reltuples AS num_rows,
    pg_size_pretty(pg_relation_size(quote_ident(t.tablename)::text)) AS table_size,
    pg_size_pretty(pg_relation_size(quote_ident(indexrelname)::text)) AS index_size,
    CASE WHEN indisunique THEN 'Y' ELSE 'N' END AS unique,
    idx_scan as number_of_scans,
    idx_tup_read as tuples_read,
    idx_tup_fetch as tuples_fetched
FROM pg_tables t
LEFT OUTER JOIN pg_class c ON c.relname=t.tablename
LEFT OUTER JOIN (
    SELECT c.relname AS ctablename, ipg.relname AS indexname,
    x.indnatts AS number_of_columns, idx_scan, idx_tup_read, idx_tup_fetch,
    indexrelname, indisunique FROM pg_index x
    JOIN pg_class c ON c.oid = x.indrelid
    JOIN pg_class ipg ON ipg.oid = x.indexrelid
    JOIN pg_stat_user_indexes psui ON x.indexrelid = psui.indexrelid)
    AS foo
    ON t.tablename = foo.ctablename
WHERE t.schemaname='public'
ORDER BY 1,2;
```

#### 3. 資料清理
```sql
-- 清理過期的狀態日誌 (保留 2 年)
DELETE FROM ftt_status_log 
WHERE action_time < NOW() - INTERVAL '2 years';

-- 清理過期的 JWT Token 記錄
DELETE FROM sys_token_blacklist 
WHERE expire_time < NOW();

-- 清理暫存檔案記錄
DELETE FROM sys_temp_files 
WHERE create_time < NOW() - INTERVAL '7 days';
```

### 效能優化

#### 1. 查詢效能分析
```sql
-- 啟用查詢統計
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

-- 查看最耗時的查詢
SELECT 
    query,
    calls,
    total_time,
    mean_time,
    rows
FROM pg_stat_statements
ORDER BY total_time DESC
LIMIT 10;

-- 分析特定查詢的執行計畫
EXPLAIN (ANALYZE, BUFFERS) 
SELECT * FROM ftt_report_main 
WHERE status = 'NEW' 
AND create_time >= CURRENT_DATE;
```

#### 2. 連線池優化
```bash
# 檢查目前連線數
psql -h localhost -U ftt -c "SELECT count(*) FROM pg_stat_activity;"

# 檢查閒置連線
psql -h localhost -U ftt -c "
SELECT count(*), state 
FROM pg_stat_activity 
WHERE datname = 'infwf' 
GROUP BY state;"
```

---

## 備份與還原

### 自動備份

#### 1. 資料庫備份腳本
```bash
#!/bin/bash
# 自動備份腳本

BACKUP_DIR="/backup/ftt"
DATE=$(date +%Y%m%d_%H%M%S)
DB_HOST="localhost"  
DB_PORT="5444"
DB_USER="ftt"
DB_NAME="infwf"

# 建立備份目錄
mkdir -p $BACKUP_DIR

# 全備份
pg_dump -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME \
  --no-password -Fc -f $BACKUP_DIR/full_backup_$DATE.dump

# 僅備份結構
pg_dump -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME \
  --no-password -s -f $BACKUP_DIR/schema_backup_$DATE.sql

# 壓縮備份檔
gzip $BACKUP_DIR/schema_backup_$DATE.sql

# 清理 30 天前的備份
find $BACKUP_DIR -name "*.dump" -mtime +30 -delete
find $BACKUP_DIR -name "*.sql.gz" -mtime +30 -delete

echo "備份完成: $BACKUP_DIR/full_backup_$DATE.dump"
```

#### 2. 應用程式備份
```bash
#!/bin/bash
# 應用程式備份腳本

APP_BACKUP_DIR="/backup/app"
DATE=$(date +%Y%m%d)

# 備份設定檔
tar -czf $APP_BACKUP_DIR/config_$DATE.tar.gz \
  /var/www/ftt-*/appsettings*.json

# 備份上傳檔案
tar -czf $APP_BACKUP_DIR/uploads_$DATE.tar.gz \
  /var/www/ftt-*/wwwroot/uploads

# 備份日誌檔
tar -czf $APP_BACKUP_DIR/logs_$DATE.tar.gz \
  /var/log/ftt/*.txt

echo "應用程式備份完成"
```

### 災難復原

#### 1. 資料庫還原
```bash
# 完整還原
pg_restore -h localhost -p 5444 -U ftt -d infwf \
  --clean --if-exists /backup/ftt/full_backup_20260112_140000.dump

# 僅還原結構
psql -h localhost -p 5444 -U ftt -d infwf \
  -f /backup/ftt/schema_backup_20260112_140000.sql

# 僅還原資料
pg_restore -h localhost -p 5444 -U ftt -d infwf \
  --data-only /backup/ftt/full_backup_20260112_140000.dump
```

#### 2. 應用程式還原
```bash
# 還原設定檔
tar -xzf /backup/app/config_20260112.tar.gz -C /

# 還原上傳檔案  
tar -xzf /backup/app/uploads_20260112.tar.gz -C /

# 重啟服務
sudo systemctl restart ftt-api
sudo systemctl restart ftt-web
```

---

## 安全管理

### 安全檢查

#### 1. 帳號安全稽核
```sql
-- 檢查弱密碼 (需要定期更新密碼的帳號)
SELECT username, display_name, password_expire_date
FROM sys_user 
WHERE password_expire_date < CURRENT_DATE
AND is_active = 'Y';

-- 檢查長時間未登入的帳號
SELECT username, display_name, last_login
FROM sys_user
WHERE last_login < CURRENT_DATE - INTERVAL '90 days'  
AND is_active = 'Y';

-- 檢查異常登入記錄
SELECT username, login_time, ip_address, user_agent
FROM sys_login_log
WHERE login_time >= CURRENT_DATE - INTERVAL '7 days'
AND (
    ip_address NOT LIKE '10.%' 
    OR user_agent LIKE '%bot%'
)
ORDER BY login_time DESC;
```

#### 2. 系統安全設定
```bash
# 檢查開放的 Port
sudo nmap -sT -O localhost

# 檢查失敗的登入嘗試
sudo grep "authentication failure" /var/log/auth.log | tail -20

# 檢查 sudo 使用記錄
sudo grep sudo /var/log/auth.log | tail -10
```

### 安全加固

#### 1. 資料庫安全
```sql
-- 撤銷 public 角色的權限
REVOKE ALL ON SCHEMA public FROM PUBLIC;

-- 建立唯讀角色
CREATE ROLE readonly;
GRANT CONNECT ON DATABASE infwf TO readonly;
GRANT USAGE ON SCHEMA public TO readonly;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO readonly;

-- 建立報表使用者
CREATE USER report_user PASSWORD 'strong_password';
GRANT readonly TO report_user;
```

#### 2. 應用程式安全
```json
// appsettings.json 安全設定
{
  "JwtConfig": {
    "ExpireTimeDuration": "1800",
    "RequireHttpsMetadata": true,
    "ValidateIssuerSigningKey": true
  },
  "SecurityHeaders": {
    "ContentSecurityPolicy": "default-src 'self'",
    "XFrameOptions": "DENY",
    "XContentTypeOptions": "nosniff"
  }
}
```

---

## 效能調校

### 應用程式調校

#### 1. IIS 調校
```xml
<!-- web.config -->
<system.webServer>
  <httpCompression directory="%SystemDrive%\inetpub\temp\IIS Temporary Compressed Files">
    <scheme name="gzip" dll="%Windir%\system32\inetsrv\gzip.dll" />
    <staticTypes>
      <add mimeType="*/*" enabled="true" />
    </staticTypes>
    <dynamicTypes>
      <add mimeType="*/*" enabled="true" />
    </dynamicTypes>
  </httpCompression>
</system.webServer>
```

#### 2. 連線池調校
```csharp
// Startup.cs 中的連線池設定
services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(300);
    });
}, ServiceLifetime.Scoped);

// 連線池參數
// Maximum Pool Size=100;Minimum Pool Size=10;
```

### 資料庫調校

#### 1. PostgreSQL 參數調校
```sql
-- 記憶體設定
ALTER SYSTEM SET shared_buffers = '512MB';
ALTER SYSTEM SET work_mem = '8MB';  
ALTER SYSTEM SET maintenance_work_mem = '128MB';

-- 連線設定
ALTER SYSTEM SET max_connections = '200';
ALTER SYSTEM SET max_prepared_statements = '100';

-- 日誌設定
ALTER SYSTEM SET log_min_duration_statement = '1000ms';
ALTER SYSTEM SET log_line_prefix = '%t [%p]: [%l-1] user=%u,db=%d ';

-- 套用設定
SELECT pg_reload_conf();
```

#### 2. 慢查詢優化
```sql
-- 找出需要優化的查詢
SELECT 
    query,
    calls,
    total_time,
    mean_time,
    (total_time/calls)::numeric(10,2) as avg_time_ms
FROM pg_stat_statements 
WHERE calls > 100
ORDER BY total_time DESC
LIMIT 10;

-- 建立複合索引優化查詢
CREATE INDEX CONCURRENTLY idx_report_status_date 
ON ftt_report_main(status, create_time) 
WHERE is_deleted = 'N';
```

---

## 故障排除

### 常見問題診斷

#### 1. 服務無法啟動
```bash
# 檢查 Port 是否被占用
sudo netstat -tlnp | grep :80
sudo netstat -tlnp | grep :5444

# 檢查設定檔語法
nginx -t
sudo -u ftt pg_ctl status -D /var/lib/postgresql/data

# 檢查權限
ls -la /var/www/ftt-*
ps aux | grep ftt
```

#### 2. 資料庫連線問題
```sql
-- 檢查資料庫狀態
SELECT version();
SELECT current_database(), current_user;

-- 檢查連線數
SELECT 
    count(*) as connections,
    state,
    wait_event_type,
    wait_event
FROM pg_stat_activity 
WHERE datname = 'infwf'
GROUP BY state, wait_event_type, wait_event;

-- 檢查鎖定狀況
SELECT 
    blocked_locks.pid AS blocked_pid,
    blocked_activity.usename AS blocked_user,
    blocking_locks.pid AS blocking_pid,
    blocking_activity.usename AS blocking_user,
    blocked_activity.query AS blocked_statement,
    blocking_activity.query AS blocking_statement
FROM pg_catalog.pg_locks blocked_locks
JOIN pg_catalog.pg_stat_activity blocked_activity ON blocked_activity.pid = blocked_locks.pid
JOIN pg_catalog.pg_locks blocking_locks ON (blocking_locks.locktype = blocked_locks.locktype
    AND blocking_locks.database IS NOT DISTINCT FROM blocked_locks.database
    AND blocking_locks.relation IS NOT DISTINCT FROM blocked_locks.relation)
JOIN pg_catalog.pg_stat_activity blocking_activity ON blocking_activity.pid = blocking_locks.pid
WHERE NOT blocked_locks.granted;
```

#### 3. 效能問題診斷
```bash
# 檢查系統負載
uptime
iostat -x 1 10

# 檢查記憶體使用
free -h
cat /proc/meminfo

# 檢查磁碟 I/O
iotop -o -d 1

# 檢查網路狀況
iftop -i eth0
ss -tuln
```

### 故障處理流程

#### 1. 緊急故障處理
```bash
#!/bin/bash
# 緊急故障處理腳本

echo "$(date): 開始故障診斷" >> /var/log/emergency.log

# 1. 檢查關鍵服務
for service in ftt-api ftt-web postgresql nginx; do
    if ! systemctl is-active --quiet $service; then
        echo "$(date): $service 服務異常，嘗試重啟" >> /var/log/emergency.log
        systemctl restart $service
        sleep 10
        if systemctl is-active --quiet $service; then
            echo "$(date): $service 重啟成功" >> /var/log/emergency.log
        else
            echo "$(date): $service 重啟失敗" >> /var/log/emergency.log
        fi
    fi
done

# 2. 檢查磁碟空間
df -h | awk '$5 > 90 {print "$(date): 磁碟空間不足: " $0}' >> /var/log/emergency.log

# 3. 清理暫存檔案
find /tmp -type f -atime +7 -delete
find /var/log -name "*.log" -size +100M -exec truncate -s 50M {} \;

echo "$(date): 緊急處理完成" >> /var/log/emergency.log
```

#### 2. 系統復原步驟
1. **評估影響範圍**: 確定故障影響的功能和使用者
2. **隔離問題**: 停止有問題的服務，避免進一步損害
3. **資料保護**: 確保資料完整性，必要時進行備份
4. **修復問題**: 根據診斷結果進行修復
5. **驗證修復**: 測試系統功能是否正常
6. **監控觀察**: 持續監控系統狀況
7. **文件記錄**: 記錄故障原因和處理過程

---

## 📞 技術支援與資源

### 聯絡資訊
- **系統架構師**: architect@company.com
- **資料庫管理員**: dba@company.com  
- **資安團隊**: security@company.com
- **24小時緊急聯絡**: 0800-911-911

### 技術資源
- 📚 技術文件庫
- 🎓 管理員培訓課程
- 🔧 故障排除手冊
- 📊 效能調校指南

### 定期維護檢查清單

#### 每日檢查
- [ ] 系統服務狀態
- [ ] 錯誤日誌檢查
- [ ] 備份執行狀況
- [ ] 磁碟空間使用率

#### 每週檢查  
- [ ] 效能監控報告
- [ ] 安全事件檢查
- [ ] 資料庫統計更新
- [ ] 使用者帳號稽核

#### 每月檢查
- [ ] 系統更新套用
- [ ] 備份還原測試
- [ ] 效能基準測試
- [ ] 安全弱點掃描

---

*本手冊版本: v1.2 | 最後更新: 2026年1月*
