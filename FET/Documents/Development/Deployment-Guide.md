# 部署指南

## 部署概述

FTT 系統支援多種部署方式，包括傳統 IIS 部署、Docker 容器化部署，以及雲端部署。本文件詳細說明各種部署方式的步驟和注意事項。

## 部署環境需求

### 最低硬體需求
- **CPU**: 4 核心 (建議 8 核心)
- **記憶體**: 8GB RAM (建議 16GB)
- **硬碟**: 100GB 可用空間 (建議 SSD)
- **網路**: 100Mbps 頻寬

### 軟體需求
- **作業系統**: Windows Server 2019+ 或 Linux Ubuntu 20.04+
- **.NET Runtime**: .NET 8.0
- **資料庫**: PostgreSQL 12+
- **Web 伺服器**: IIS 10+ 或 Nginx 1.18+
- **反向代理**: Nginx (建議)

## 部署方式 1: IIS 部署 (Windows)

### 1.1 準備工作

#### 安裝必要軟體
```powershell
# 安裝 .NET 8.0 Runtime
choco install dotnet-8.0-runtime

# 安裝 IIS 和 ASP.NET Core Module
dism /online /enable-feature /featurename:IIS-WebServerRole
dism /online /enable-feature /featurename:IIS-WebServer
dism /online /enable-feature /featurename:IIS-CommonHttpFeatures
dism /online /enable-feature /featurename:IIS-HttpErrors
dism /online /enable-feature /featurename:IIS-HttpLogging
dism /online /enable-feature /featurename:IIS-RequestFiltering
dism /online /enable-feature /featurename:IIS-StaticContent
dism /online /enable-feature /featurename:IIS-DefaultDocument

# 下載並安裝 ASP.NET Core Module
# 從 Microsoft 官網下載 dotnet-hosting-8.0.x-win.exe
```

### 1.2 發布應用程式

```powershell
# 發布 FTT_WEB
cd FTT_WEB
dotnet publish -c Release -o C:\inetpub\wwwroot\FTT_WEB

# 發布 FTT_API
cd ..\FTT_API
dotnet publish -c Release -o C:\inetpub\wwwroot\FTT_API

# 發布 FTT_VENDER_WEB
cd ..\FTT_VENDER_WEB
dotnet publish -c Release -o C:\inetpub\wwwroot\FTT_VENDER_WEB

# 發布 FTT_VENDER_API
cd ..\FTT_VENDER_API
dotnet publish -c Release -o C:\inetpub\wwwroot\FTT_VENDER_API
```

### 1.3 建立 IIS 應用程式

```powershell
# 建立應用程式集區
New-WebAppPool -Name "FTT_WEB_Pool" -Force
Set-ItemProperty -Path "IIS:\AppPools\FTT_WEB_Pool" -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
Set-ItemProperty -Path "IIS:\AppPools\FTT_WEB_Pool" -Name "managedRuntimeVersion" -Value ""

New-WebAppPool -Name "FTT_API_Pool" -Force
Set-ItemProperty -Path "IIS:\AppPools\FTT_API_Pool" -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
Set-ItemProperty -Path "IIS:\AppPools\FTT_API_Pool" -Name "managedRuntimeVersion" -Value ""

# 建立網站
New-Website -Name "FTT_WEB" -Port 50102 -PhysicalPath "C:\inetpub\wwwroot\FTT_WEB" -ApplicationPool "FTT_WEB_Pool"
New-Website -Name "FTT_API" -Port 50101 -PhysicalPath "C:\inetpub\wwwroot\FTT_API" -ApplicationPool "FTT_API_Pool"
```

### 1.4 設定 SSL 憑證

```powershell
# 建立自簽憑證 (僅供開發使用)
New-SelfSignedCertificate -DnsName "ftt.local" -CertStoreLocation "cert:\LocalMachine\My"

# 或使用 Let's Encrypt (生產環境建議)
# 安裝 win-acme
choco install win-acme
```

## 部署方式 2: Docker 容器化部署

### 2.1 建立 Dockerfile

#### FTT_WEB Dockerfile
```dockerfile
# FTT_WEB/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FTT_WEB/FTT_WEB.csproj", "FTT_WEB/"]
COPY ["Const/Const.csproj", "Const/"]
COPY ["Core.8.Utility/Core.8.Utility.csproj", "Core.8.Utility/"]
COPY ["Core.8.Utility.Web/Core.8.Utility.Web.csproj", "Core.8.Utility.Web/"]
RUN dotnet restore "FTT_WEB/FTT_WEB.csproj"
COPY . .
WORKDIR "/src/FTT_WEB"
RUN dotnet build "FTT_WEB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FTT_WEB.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FTT_WEB.dll"]
```

#### FTT_API Dockerfile
```dockerfile
# FTT_API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FTT_API/FTT_API.csproj", "FTT_API/"]
COPY ["Const/Const.csproj", "Const/"]
COPY ["Core.8.Utility/Core.8.Utility.csproj", "Core.8.Utility/"]
RUN dotnet restore "FTT_API/FTT_API.csproj"
COPY . .
WORKDIR "/src/FTT_API"
RUN dotnet build "FTT_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FTT_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FTT_API.dll"]
```

### 2.2 Docker Compose 設定

```yaml
# docker-compose.yml
version: '3.8'

services:
  # PostgreSQL 資料庫
  postgres:
    image: postgres:13
    container_name: ftt-postgres
    environment:
      POSTGRES_DB: infwf
      POSTGRES_USER: ftt
      POSTGRES_PASSWORD: ftt123
      TZ: Asia/Taipei
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./database/init.sql:/docker-entrypoint-initdb.d/init.sql
    ports:
      - "5444:5432"
    networks:
      - ftt-network
    restart: unless-stopped

  # Redis (可選)
  redis:
    image: redis:6-alpine
    container_name: ftt-redis
    ports:
      - "6379:6379"
    networks:
      - ftt-network
    restart: unless-stopped

  # FTT API
  ftt-api:
    build:
      context: .
      dockerfile: FTT_API/Dockerfile
    container_name: ftt-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - TZ=Asia/Taipei
    ports:
      - "50101:80"
    depends_on:
      - postgres
    networks:
      - ftt-network
    volumes:
      - ./logs:/app/logs
      - ./uploads:/app/uploads
    restart: unless-stopped

  # FTT WEB
  ftt-web:
    build:
      context: .
      dockerfile: FTT_WEB/Dockerfile
    container_name: ftt-web
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - TZ=Asia/Taipei
    ports:
      - "50102:80"
    depends_on:
      - ftt-api
    networks:
      - ftt-network
    volumes:
      - ./logs:/app/logs
    restart: unless-stopped

  # FTT VENDER API
  ftt-vender-api:
    build:
      context: .
      dockerfile: FTT_VENDER_API/Dockerfile
    container_name: ftt-vender-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - TZ=Asia/Taipei
    ports:
      - "50401:80"
    depends_on:
      - postgres
    networks:
      - ftt-network
    volumes:
      - ./logs:/app/logs
      - ./uploads:/app/uploads
    restart: unless-stopped

  # FTT VENDER WEB
  ftt-vender-web:
    build:
      context: .
      dockerfile: FTT_VENDER_WEB/Dockerfile
    container_name: ftt-vender-web
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - TZ=Asia/Taipei
    ports:
      - "50402:80"
    depends_on:
      - ftt-vender-api
    networks:
      - ftt-network
    volumes:
      - ./logs:/app/logs
    restart: unless-stopped

  # Nginx 反向代理
  nginx:
    image: nginx:alpine
    container_name: ftt-nginx
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf
      - ./nginx/ssl:/etc/nginx/ssl
    depends_on:
      - ftt-web
      - ftt-api
      - ftt-vender-web
      - ftt-vender-api
    networks:
      - ftt-network
    restart: unless-stopped

volumes:
  postgres_data:

networks:
  ftt-network:
    driver: bridge
```

### 2.3 Nginx 設定

```nginx
# nginx/nginx.conf
events {
    worker_connections 1024;
}

http {
    upstream ftt-web {
        server ftt-web:80;
    }
    
    upstream ftt-api {
        server ftt-api:80;
    }
    
    upstream ftt-vender-web {
        server ftt-vender-web:80;
    }
    
    upstream ftt-vender-api {
        server ftt-vender-api:80;
    }

    server {
        listen 80;
        server_name ftt.local;

        location /api/ {
            proxy_pass http://ftt-api/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }

        location / {
            proxy_pass http://ftt-web/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }

    server {
        listen 80;
        server_name vendor.ftt.local;

        location /api/ {
            proxy_pass http://ftt-vender-api/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }

        location / {
            proxy_pass http://ftt-vender-web/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }
}
```

### 2.4 部署執行

```bash
# 建置並啟動所有服務
docker-compose up -d

# 查看服務狀態
docker-compose ps

# 查看日誌
docker-compose logs -f ftt-api

# 停止服務
docker-compose down

# 清理並重新建置
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

## 部署方式 3: Linux 系統部署

### 3.1 安裝環境 (Ubuntu 20.04)

```bash
# 更新系統
sudo apt update && sudo apt upgrade -y

# 安裝 .NET 8.0
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-runtime-8.0 aspnetcore-runtime-8.0

# 安裝 Nginx
sudo apt install -y nginx

# 安裝 PostgreSQL
sudo apt install -y postgresql postgresql-contrib
```

### 3.2 設定 PostgreSQL

```bash
# 切換到 postgres 使用者
sudo -u postgres psql

# 建立資料庫和使用者
CREATE USER ftt WITH PASSWORD 'ftt123';
CREATE DATABASE infwf OWNER ftt;
GRANT ALL PRIVILEGES ON DATABASE infwf TO ftt;
\q

# 設定 PostgreSQL 允許外部連線
sudo nano /etc/postgresql/12/main/postgresql.conf
# 修改: listen_addresses = '*'

sudo nano /etc/postgresql/12/main/pg_hba.conf
# 加入: host all all 0.0.0.0/0 md5

sudo systemctl restart postgresql
```

### 3.3 部署應用程式

```bash
# 建立應用程式目錄
sudo mkdir -p /var/www/ftt-web
sudo mkdir -p /var/www/ftt-api
sudo mkdir -p /var/www/ftt-vender-web
sudo mkdir -p /var/www/ftt-vender-api

# 複製發布檔案
sudo cp -r ./publish/FTT_WEB/* /var/www/ftt-web/
sudo cp -r ./publish/FTT_API/* /var/www/ftt-api/
sudo cp -r ./publish/FTT_VENDER_WEB/* /var/www/ftt-vender-web/
sudo cp -r ./publish/FTT_VENDER_API/* /var/www/ftt-vender-api/

# 設定權限
sudo chown -R www-data:www-data /var/www/ftt-*
sudo chmod -R 755 /var/www/ftt-*
```

### 3.4 建立 systemd 服務

```bash
# 建立 FTT API 服務
sudo nano /etc/systemd/system/ftt-api.service
```

```ini
[Unit]
Description=FTT API Service
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/ftt-api/FTT_API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=ftt-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5001

[Install]
WantedBy=multi-user.target
```

```bash
# 建立其他服務 (類似方式)
# 啟動服務
sudo systemctl daemon-reload
sudo systemctl enable ftt-api
sudo systemctl start ftt-api
sudo systemctl status ftt-api
```

## 生產環境設定

### 1. 安全性設定

#### SSL/TLS 設定
```nginx
server {
    listen 443 ssl http2;
    server_name ftt.yourdomain.com;
    
    ssl_certificate /etc/ssl/certs/ftt.crt;
    ssl_certificate_key /etc/ssl/private/ftt.key;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE+AESGCM:ECDHE+AES256:!aNULL:!MD5:!DSS;
    
    # 其他設定...
}
```

#### 防火牆設定
```bash
# Ubuntu UFW
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 5444/tcp  # PostgreSQL (僅內部網路)
sudo ufw enable

# CentOS/RHEL firewalld
sudo firewall-cmd --permanent --add-service=http
sudo firewall-cmd --permanent --add-service=https
sudo firewall-cmd --permanent --add-port=5444/tcp
sudo firewall-cmd --reload
```

### 2. 監控與日誌

#### 日誌設定
```json
// appsettings.Production.json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.File", "Serilog.Sinks.Console"],
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/ftt/api-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

#### 健康檢查
```bash
# 建立健康檢查腳本
nano /usr/local/bin/ftt-health-check.sh
```

```bash
#!/bin/bash
# FTT 系統健康檢查

echo "檢查服務狀態..."
systemctl is-active ftt-api
systemctl is-active ftt-web

echo "檢查資料庫連線..."
pg_isready -h localhost -p 5444 -U ftt

echo "檢查網站回應..."
curl -I http://localhost:5001/health
curl -I http://localhost:5002/health
```

### 3. 備份與還原

#### 自動備份腳本
```bash
#!/bin/bash
# 建立備份目錄
BACKUP_DIR="/backup/ftt"
DATE=$(date +%Y%m%d_%H%M%S)

mkdir -p $BACKUP_DIR

# 備份資料庫
pg_dump -h localhost -p 5444 -U ftt -d infwf > $BACKUP_DIR/db_$DATE.sql

# 備份應用程式設定
tar -czf $BACKUP_DIR/config_$DATE.tar.gz /var/www/ftt-*/appsettings.json

# 清理 7 天前的備份
find $BACKUP_DIR -name "*.sql" -mtime +7 -delete
find $BACKUP_DIR -name "*.tar.gz" -mtime +7 -delete
```

## 部署檢查清單

### 部署前檢查
- [ ] 所有專案建置成功
- [ ] 單元測試通過
- [ ] 整合測試通過
- [ ] 設定檔環境變數正確
- [ ] 資料庫遷移腳本準備完成
- [ ] SSL 憑證有效

### 部署後檢查
- [ ] 所有服務正常啟動
- [ ] 資料庫連線正常
- [ ] API 端點回應正常
- [ ] 前端頁面載入正常
- [ ] 日誌記錄正常
- [ ] 監控告警正常

### 效能調校
- [ ] 資料庫索引優化
- [ ] 應用程式池設定
- [ ] 快取策略配置
- [ ] 負載均衡設定
- [ ] CDN 配置 (如適用)
