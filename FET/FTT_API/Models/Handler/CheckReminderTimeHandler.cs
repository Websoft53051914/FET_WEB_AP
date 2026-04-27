using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;

namespace FTT_API.Models.Handler
{
    /// <summary>
    /// 催單檢查處理器 - 模擬舊版本 FTTTask 每日催單行為
    /// </summary>
    public class CheckReminderTimeHandler : BaseDBHandler
    {
        private readonly ILogger<CheckReminderTimeHandler> _logger;
        private readonly ConfigurationHelper _configHelper;

        public CheckReminderTimeHandler(
            ConfigurationHelper configHelper,
            ILogger<CheckReminderTimeHandler> logger)
        {
            _configHelper = configHelper;
            _logger = logger;
        }

        /// <summary>
        /// 每日催單檢查任務（模擬舊版本 FTTTask 行為）
        /// </summary>
        public async Task CheckReminderTask()
        {
            try
            {
                _logger.LogInformation($"[Hangfire] 開始執行每日催單檢查 - {DateTime.Now}");

                // 1. 查詢所有處理中且超過 KPI 時間的工單
                var overdueFormList = GetOverdueFormList();

                _logger.LogInformation($"[Hangfire] 發現 {overdueFormList.Count} 筆超時工單");

                int successCount = 0;
                int skipCount = 0;

                foreach (var form in overdueFormList)
                {
                    try
                    {
                        // 2. 檢查今天是否已催過單（避免重複）
                        if (HasReminderToday(form.form_no.ToString()))
                        {
                            _logger.LogDebug($"[Hangfire] 工單 {form.form_no} 今日已催過單，跳過");
                            skipCount++;
                            continue;
                        }

                        // 3. 發送催單通知
                        var mailPoolHandler = new MailPoolHandler();
                        var result = Method.CreateMailPool(form.form_no.ToString(), "", "REMINDER", mailPoolHandler);

                        // 4. 記錄日誌
                        LogReminderAction(form.form_no.ToString(), "SYSTEM_HANGFIRE");

                        _logger.LogDebug($"[Hangfire] 工單 {form.form_no} 催單成功");
                        successCount++;

                        // 避免瞬間大量請求，稍作延遲
                        await Task.Delay(100);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[Hangfire] 工單 {form.form_no} 催單失敗：{ex.Message}");
                    }
                }

                _logger.LogInformation($"[Hangfire] 每日催單檢查完成 - 成功:{successCount}, 跳過:{skipCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Hangfire] 每日催單檢查發生錯誤：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 查詢所有超過 KPI 時間的處理中工單
        /// </summary>
        private List<dynamic> GetOverdueFormList()
        {
            // 【原始版本 - CHK_WORKING_DAY2 函數有問題時暫時保留】
            // string sql = @"
            //     SELECT DISTINCT f.form_no, f.category_id, NVL(c.kpitime, 3) as kpitime
            //     FROM FTT_FORM f
            //     JOIN APPROVE_FORM af ON f.form_no = af.form_no
            //     LEFT JOIN CI_RELATIONS_CATEGORY c ON f.category_id = c.cisid
            //     WHERE f.statusid IN (2, 3, 4, 5)  -- 處理中狀態
            //     AND CHK_WORKING_DAY2(af.UPDATETIME, SYSDATE, 'S') > NVL(c.kpitime, 3)
            //     ORDER BY f.form_no";
            
            // 【替代方案 - 簡單工作日計算，排除週末但不含國定假日】
            // 【20260408 EDB PostgreSQL 修正】
            // Oracle: SYSDATE - UPDATETIME 回傳數值(天數)，TRUNC(天數/7) 合法
            // EDB PostgreSQL: SYSDATE - UPDATETIME 回傳 interval，TRUNC(interval) 不支援
            // 修正: 使用 EXTRACT(EPOCH FROM ...) 將 interval 轉換為秒數，再除以 86400 得到天數
            string sql = @"
                SELECT DISTINCT f.form_no, f.category_id, NVL(c.kpitime, 3) as kpitime
                FROM FTT_FORM f
                JOIN APPROVE_FORM af ON f.form_no = af.form_no
                LEFT JOIN CI_RELATIONS_CATEGORY c ON f.category_id = c.cisid
                WHERE f.statusid IN (2, 3, 4, 5)  -- 處理中狀態
                AND EXTRACT(EPOCH FROM (SYSDATE - af.UPDATETIME)) / 86400.0 - (FLOOR(EXTRACT(EPOCH FROM (SYSDATE - af.UPDATETIME)) / 604800.0) * 2) > NVL(c.kpitime, 3)
                ORDER BY f.form_no";

            return GetDBHelper().FindList<dynamic>(sql, null);
        }

        /// <summary>
        /// 檢查工單今天是否已催過單
        /// </summary>
        private bool HasReminderToday(string formNo)
        {
            Dictionary<string, object> paras = new()
            {
                {"formNo", formNo}
            };

            string sql = @"
                SELECT COUNT(*) 
                FROM FTT_FORM_LOG 
                WHERE FORM_NO = @formNo 
                AND FIELDNAME = '催單' 
                -- 【20260408 EDB PostgreSQL 修正】Oracle: TRUNC(timestamp) → PostgreSQL: DATE_TRUNC('day', timestamp)
                AND DATE_TRUNC('day', UPDATETIME) = DATE_TRUNC('day', SYSDATE)";

            var count = GetDBHelper().FindScalar<int>(sql, paras);
            return count > 0;
        }

        /// <summary>
        /// 記錄催單操作日誌
        /// </summary>
        private void LogReminderAction(string formNo, string empNo)
        {
            Dictionary<string, object> paras = new()
            {
                {"formNo", formNo},
                {"empNo", empNo},
                {"updateTime", DateTime.Now}
            };

            string sql = @"
                INSERT INTO FTT_FORM_LOG 
                (FORM_NO, UPDATE_EMPNO, UPDATETIME, FIELDNAME, ACTION, FORM_TYPE, ROOT_NO) 
                VALUES (@formNo, @empNo, @updateTime, '催單', 'SYSTEM_AUTO', 'REMINDER', @formNo)";

            GetDBHelper().Execute(sql, paras);
        }
    }
}
