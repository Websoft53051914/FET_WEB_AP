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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 檢查是否啟用同步功能
            var isEnabled = _configuration.GetValue<bool>("NSPStoreSync:Enabled", true);
            if (!isEnabled)
            {
                _logger.LogInformation("NSP門市資料同步功能已停用");
                return;
            }

            _logger.LogInformation("NSP門市資料同步背景服務已啟動");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWork();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "NSP門市資料同步時發生錯誤，繼續執行下次同步");
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
                    string result1 = handler.SyncStoreProfileData();
                    _logger.LogInformation($"第一階段同步完成：{result1}");
                    
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

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NSP門市資料同步背景服務正在停止");
            await base.StopAsync(stoppingToken);
        }
    }
}
