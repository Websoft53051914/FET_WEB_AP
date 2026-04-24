using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;

namespace FTT_API.Background
{
    /// <summary>
    /// NSP門市資料同步背景服務
    /// 定期從Oracle同步門市資料到PostgreSQL
    /// </summary>
    public class NSPStoreSyncBackgroundService : BackgroundService
    {
        private readonly ILogger<NSPStoreSyncBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _syncInterval;

        /// <summary>
        /// 初始化 NSPStoreSyncBackgroundService 的新實例。
        /// </summary>
        /// <param name="logger">日誌記錄器</param>
        /// <param name="serviceProvider">服務提供者</param>
        /// <param name="configuration">組態配置</param>
        public NSPStoreSyncBackgroundService(
            ILogger<NSPStoreSyncBackgroundService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            
            // 從設定檔讀取同步間隔時間，預設為每24小時同步一次
            var intervalHours = configuration.GetValue<int>("NSPStoreSync:IntervalHours", 24);
            _syncInterval = TimeSpan.FromHours(intervalHours);
            
            _logger.LogInformation($"NSP門市資料同步背景服務初始化完成，同步間隔: {intervalHours} 小時");
        }

        /// <summary>
        /// 執行店鋪同步的背景核心邏輯。
        /// </summary>
        /// <param name="stoppingToken">用於取消操作的權杖。</param>
        /// <returns>代表非同步操作的 Task。</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 檢查是否啟用同步功能（正式環境測試階段請設定 Enabled: false）
            var isEnabled = _configuration.GetValue<bool>("NSPStoreSync:Enabled", false);
            if (!isEnabled)
            {
                _logger.LogInformation("NSP門市資料同步功能已停用（NSPStoreSync:Enabled = false）");
                return;
            }

            // 啟動延遲，避免系統剛啟動時立即觸發同步，讀取 StartDelayMinutes 設定（預設5分鐘）
            var startDelayMinutes = _configuration.GetValue<int>("NSPStoreSync:StartDelayMinutes", 5);
            _logger.LogInformation($"NSP門市資料同步背景服務已啟動，將於 {startDelayMinutes} 分鐘後開始第一次同步");
            await Task.Delay(TimeSpan.FromMinutes(startDelayMinutes), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                // 每次執行前重新確認 Enabled 設定，支援不重啟服務直接改設定檔生效
                var enabledNow = _configuration.GetValue<bool>("NSPStoreSync:Enabled", false);
                if (!enabledNow)
                {
                    _logger.LogInformation("NSP門市資料同步功能已停用（NSPStoreSync:Enabled = false），本次跳過");
                }
                else
                {
                    try
                    {
                        await DoWork();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "NSP門市資料同步時發生錯誤，繼續執行下次同步");
                    }
                }

                // 等待下次執行
                await Task.Delay(_syncInterval, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            _logger.LogInformation("開始執行NSP門市資料同步作業");

            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var configHelper = scope.ServiceProvider.GetRequiredService<ConfigurationHelper>();
                    var handler = new NSPStoreSyncHandler(configHelper);
                    
                    // 第一階段：從Oracle同步到nsp_store_profile
                    _logger.LogInformation("執行第一階段：Oracle → nsp_store_profile");
                    var (stage1Success, result1) = handler.SyncStoreProfileData();
                    _logger.LogInformation($"第一階段同步結果：{result1}");

                    // 安全防護：第一階段若失敗，不執行第二階段，避免用空資料覆蓋store_profile
                    if (!stage1Success)
                    {
                        _logger.LogWarning($"第一階段未成功，中止第二階段執行。原因：{result1}");
                        return;
                    }

                    // 第二階段：從nsp_store_profile批次同步到store_profile
                    _logger.LogInformation("執行第二階段：nsp_store_profile → store_profile");
                    string result2 = handler.BatchSyncNspToStoreProfile();
                    _logger.LogInformation($"第二階段同步完成：{result2}");
                    
                    _logger.LogInformation("NSP門市資料完整同步作業完成");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "NSP門市資料同步失敗");
                }
            }
        }

        /// <summary>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NSP門市資料同步背景服務正在停止");
            await base.StopAsync(stoppingToken);
        }
    }
}
