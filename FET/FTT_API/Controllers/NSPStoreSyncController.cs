using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers
{
    /// <summary>
    /// NSP門市資料同步API控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NSPStoreSyncController : ControllerBase
    {
        private readonly ConfigurationHelper _configHelper;

        public NSPStoreSyncController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        /// <summary>
        /// 同步NSP門市資料
        /// 從Oracle VIEW_DP2FTT同步資料到PostgreSQL nsp_store_profile
        /// </summary>
        /// <returns>同步結果</returns>
        [HttpPost("sync")]
        [AllowAnonymous] // 可依需求調整權限
        public IActionResult SyncStoreData()
        {
            try
            {
                NSPStoreSyncHandler handler = new NSPStoreSyncHandler(_configHelper);
                var (isSuccess, result) = handler.SyncStoreProfileData();
                
                return Ok(new ResponseModel<string>
                {
                    IsSuccess = isSuccess,
                    Message = isSuccess ? "同步完成" : "同步中止或失敗",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = $"同步失敗：{ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 取得門市資料
        /// </summary>
        /// <param name="ivrCode">門市代碼</param>
        /// <returns>門市資料</returns>
        [HttpGet("store/{ivrCode}")]
        public IActionResult GetStoreProfile(string ivrCode)
        {
            try
            {
                NSPStoreSyncHandler handler = new NSPStoreSyncHandler(_configHelper);
                var result = handler.GetStoreProfile(ivrCode);
                
                if (result != null)
                {
                    return Ok(new ResponseModel<nsp_store_profileDTO>
                    {
                        IsSuccess = true,
                        Message = "查詢成功",
                        Data = result
                    });
                }
                else
                {
                    return NotFound(new ResponseModel<nsp_store_profileDTO>
                    {
                        IsSuccess = false,
                        Message = $"找不到門市代碼：{ivrCode}",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<nsp_store_profileDTO>
                {
                    IsSuccess = false,
                    Message = $"查詢失敗：{ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 取得所有門市資料列表
        /// </summary>
        /// <returns>門市資料列表</returns>
        [HttpGet("stores")]
        public IActionResult GetAllStoreProfiles()
        {
            try
            {
                NSPStoreSyncHandler handler = new NSPStoreSyncHandler(_configHelper);
                var result = handler.GetAllStoreProfiles();
                
                return Ok(new ResponseModel<List<nsp_store_profileDTO>>
                {
                    IsSuccess = true,
                    Message = "查詢成功",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<List<nsp_store_profileDTO>>
                {
                    IsSuccess = false,
                    Message = $"查詢失敗：{ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 檢查同步狀態
        /// </summary>
        /// <returns>同步狀態資訊</returns>
        [HttpGet("sync-status")]
        public IActionResult GetSyncStatus()
        {
            try
            {
                NSPStoreSyncHandler handler = new NSPStoreSyncHandler(_configHelper);
                var stores = handler.GetAllStoreProfiles();
                
                var syncInfo = new
                {
                    TotalStores = stores.Count,
                    LastSyncTime = stores.OrderByDescending(x => x.ftt_synctime).FirstOrDefault()?.ftt_synctime,
                    SyncedToday = stores.Count(x => x.ftt_synctime?.Date == DateTime.Today)
                };
                
                return Ok(new ResponseModel<object>
                {
                    IsSuccess = true,
                    Message = "查詢成功",
                    Data = syncInfo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<object>
                {
                    IsSuccess = false,
                    Message = $"查詢失敗：{ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 測試Oracle連接並列出可用的表格
        /// </summary>
        /// <returns>測試結果</returns>
        [HttpGet("test-oracle")]
        public IActionResult TestOracleConnection()
        {
            try
            {
                NSPStoreSyncHandler handler = new NSPStoreSyncHandler(_configHelper);
                string result = handler.TestOracleConnection();
                
                return Ok(new ResponseModel<string>
                {
                    IsSuccess = true,
                    Message = "Oracle連接測試完成",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = $"Oracle連接測試失敗：{ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 批次同步nsp_store_profile到store_profile
        /// 包含新增和更新邏輯
        /// </summary>
        /// <returns>批次同步結果</returns>
        [HttpPost("batch-sync")]
        [AllowAnonymous] // 可依需求調整權限
        public IActionResult BatchSyncToStoreProfile()
        {
            try
            {
                NSPStoreSyncHandler handler = new NSPStoreSyncHandler(_configHelper);
                string result = handler.BatchSyncNspToStoreProfile();
                
                return Ok(new ResponseModel<string>
                {
                    IsSuccess = true,
                    Message = "批次同步完成",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = $"批次同步失敗：{ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 測試API端點是否可正常存取
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult HealthCheck()
        {
            return Ok(new ResponseModel<string>
            {
                IsSuccess = true,
                Message = "NSPStoreSync API 正常運作",
                Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }
}

/// <summary>
/// API回應模型
/// </summary>
/// <typeparam name="T">資料類型</typeparam>
public class ResponseModel<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}
