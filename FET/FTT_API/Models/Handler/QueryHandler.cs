using Const.DTO;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using System.Text;
using System.IO;

namespace FTT_API.Models.Handler
{
    /// <summary>
    /// 門市報修管理-查詢
    /// </summary>
    public class QueryHandler : BaseDBHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QueryHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }
        private readonly ConfigurationHelper _configHelper;

        /// <summary>
        /// 取得分頁資料
        /// </summary>
        public PageResult<VFttForm2DTO> GetPageList(PageEntity pageEntity, VFttForm2DTO searchVO)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];
            if (string.IsNullOrWhiteSpace(pageEntity.Sort))
            {
                pageEntity.Sort = nameof(VFttForm2DTO.form_no);
            }

            if (searchVO.CreateDateGte.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.createtime)} >= @{nameof(searchVO.CreateDateGte)} ");
                paras.Add(nameof(searchVO.CreateDateGte), searchVO.CreateDateGte);
            }
            if (searchVO.CreateDateLt.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.createtime)} < @{nameof(searchVO.CreateDateLt)} ");
                paras.Add(nameof(searchVO.CreateDateLt), searchVO.CreateDateLt);
            }
            if (searchVO.CompleteDateGte.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.completetime)} >= @{nameof(searchVO.CompleteDateGte)} ");
                paras.Add(nameof(searchVO.CompleteDateGte), searchVO.CompleteDateGte);
            }
            if (searchVO.CompleteDateLt.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.completetime)} < @{nameof(searchVO.CompleteDateLt)} ");
                paras.Add(nameof(searchVO.CompleteDateLt), searchVO.CompleteDateLt);
            }
            if (searchVO.CloseDateGte.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.closedate)} >= @{nameof(searchVO.CloseDateGte)} ");
                paras.Add(nameof(searchVO.CloseDateGte), searchVO.CloseDateGte);
            }
            if (searchVO.CloseDateLt.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.closedate)} < @{nameof(searchVO.CloseDateLt)} ");
                paras.Add(nameof(searchVO.CloseDateLt), searchVO.CloseDateLt);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.StatusIdEq))
            {
                // 除錯：記錄狀態過濾條件 - 寫入檔案
                try { 
                    var logPath = Environment.OSVersion.Platform == PlatformID.Win32NT 
                        ? @"d:\debug_query.log" 
                        : "/tmp/debug_query.log";
                    System.IO.File.AppendAllText(logPath, $"[{DateTime.Now}] GetPageList - StatusIdEq filter: {searchVO.StatusIdEq}\n"); 
                } catch (Exception ex) { 
                    // 如果檔案寫入失敗，嘗試寫入應用程式目錄
                    try {
                        var fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug_query_fallback.log");
                        System.IO.File.AppendAllText(fallbackPath, $"[{DateTime.Now}] GetPageList - StatusIdEq filter: {searchVO.StatusIdEq} (Fallback: {ex.Message})\n");
                    } catch { }
                }
                condition.Append($"AND {nameof(VFttForm2DTO.statusid)} = @{nameof(searchVO.StatusIdEq)} ");
                paras.Add(nameof(searchVO.StatusIdEq), searchVO.StatusIdEq);
            }
            if (searchVO.FormNoEq.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.form_no)} = @{nameof(searchVO.FormNoEq)} ");
                paras.Add(nameof(searchVO.FormNoEq), searchVO.FormNoEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.TtCategoryEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.tt_category)} = @{nameof(searchVO.TtCategoryEq)} ");
                paras.Add(nameof(searchVO.TtCategoryEq), searchVO.TtCategoryEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.CategoryIdFilter))
            {
                condition.Append($@"AND category_id IN (SELECT cisid FROM ci_data
WHERE instr('-' || acisid || '-', '-' || @{nameof(searchVO.CategoryIdFilter)} || '-') > 0) ");
                paras.Add(nameof(searchVO.CategoryIdFilter), searchVO.CategoryIdFilter);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.VenderIdEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.vender_id)} = @{nameof(searchVO.VenderIdEq)} ");
                paras.Add(nameof(searchVO.VenderIdEq), searchVO.VenderIdEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.IvrCodeEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.ivrcode)} = @{nameof(searchVO.IvrCodeEq)} ");
                paras.Add(nameof(searchVO.IvrCodeEq), searchVO.IvrCodeEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.CompanyEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.company)} = @{nameof(searchVO.CompanyEq)} ");
                paras.Add(nameof(searchVO.CompanyEq), searchVO.CompanyEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.StoreTypeEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.store_type)} = @{nameof(searchVO.StoreTypeEq)} ");
                paras.Add(nameof(searchVO.StoreTypeEq), searchVO.StoreTypeEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.ChannelEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.channel)} = @{nameof(searchVO.ChannelEq)} ");
                paras.Add(nameof(searchVO.ChannelEq), searchVO.ChannelEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.AreaEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.area)} = @{nameof(searchVO.AreaEq)} ");
                paras.Add(nameof(searchVO.AreaEq), searchVO.AreaEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.AsEmpNoEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.as_empno)} = @{nameof(searchVO.AsEmpNoEq)} ");
                paras.Add(nameof(searchVO.AsEmpNoEq), searchVO.AsEmpNoEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.SelfConfigEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.selfconfig)} = @{nameof(searchVO.SelfConfigEq)} ");
                paras.Add(nameof(searchVO.SelfConfigEq), searchVO.SelfConfigEq);
            }
            if (searchVO.UserRoleVenderFilter)
            {
                if (SessionVO != null)
                {
                    condition.Append($@"
AND statusid IN ( 'AGREE', 'OFFER', 'PRWP', 'COMPLETE', 'CLOSE' )
AND form_no IN (SELECT form_no
                FROM   access_role
                WHERE  action = 'Y'
                       AND user_type = @user_type
                       AND deptcode = @deptcode)
");
                    paras.Add("user_type", SessionVO.userrole);
                    paras.Add("deptcode", SessionVO.ivrcode);
                }
            }
            else if (searchVO.UserRoleOtherFilter)
            {
                if (SessionVO != null)
                {
                    // 除錯：記錄使用者權限查詢條件 - 寫入檔案
                    try { 
                        // 跨平台日誌路徑：Windows固定d:\，Linux使用/tmp/
                        var logPath = Environment.OSVersion.Platform == PlatformID.Win32NT 
                            ? @"d:\debug_query.log" 
                            : "/tmp/debug_query.log";
                        System.IO.File.AppendAllText(logPath, $"[{DateTime.Now}] GetPageList - UserRoleOtherFilter empno: {SessionVO.empno}, usertype: {SessionVO.usertype}, ivrcode: {SessionVO.ivrcode}\n"); 
                    } catch { }
                    
                    // 針對門市人員(RETAIL 或 STORE)使用不同的查詢邏輯
                    if (SessionVO.usertype == "RETAIL" || SessionVO.usertype == "STORE")
                    {
                        // 除錯：記錄門市人員查詢參數
                        try { 
                            var logPath2 = Environment.OSVersion.Platform == PlatformID.Win32NT 
                                ? @"d:\debug_query.log" 
                                : "/tmp/debug_query.log";
                            System.IO.File.AppendAllText(logPath2, $"[{DateTime.Now}] GetPageList - STORE/RETAIL Query - empno: {SessionVO.empno}, deptcode: {SessionVO.ivrcode}\n"); 
                        } catch { }
                        
                        condition.Append($@"
AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  action = 'Y'
                              AND (empno = @empno OR deptcode = @deptcode))
");
                        paras.Add("empno", SessionVO.empno);
                        paras.Add("deptcode", SessionVO.ivrcode);
                    }
                    else
                    {
                        condition.Append($@"
AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  action = 'Y'
                              AND empno = @empno)
AND EXISTS (SELECT 1 FROM access_role WHERE action = 'Y' AND empno = @empno)
");
                        paras.Add("empno", SessionVO.empno);
                    }
                }
            }
            
            // 除錯：記錄最終的 SQL 條件 - 寫入檔案
            try { 
                // 跨平台日誌路徑：Windows固定d:\，Linux使用/tmp/
                var logPath = Environment.OSVersion.Platform == PlatformID.Win32NT 
                    ? @"d:\debug_query.log" 
                    : "/tmp/debug_query.log";
                System.IO.File.AppendAllText(logPath, $"[{DateTime.Now}] GetPageList - Final SQL condition: {condition}\n"); 
            } catch { }

            // 直接在 SQL 內嵌 ORDER BY / OFFSET / LIMIT，確保 PostgreSQL 只對當頁
            // 實際回傳的行數呼叫 FIND_ACTION_NAME，而非全量資料集
            int offset = pageEntity.CurrentPage <= 1 ? 0 : (pageEntity.CurrentPage - 1) * pageEntity.PageDataSize;
            string sqlData = $@"
SELECT  form_no
                , tt_category
                , ciname
                , TO_CHAR(createtime, 'YYYY/MM/DD HH24:MI:SS')   AS createtime_text
                , shop_name
                , statusname
                , TO_CHAR(dispatchtime, 'YYYY/MM/DD HH24:MI:SS') AS dispatchtime_text
                , vender
                , descr
                , FIND_ACTION_NAME(form_no)                      AS processer
FROM v_ftt_form2
WHERE  1 = 1
{condition}
ORDER BY {pageEntity.Sort} {pageEntity.Asc}
OFFSET {offset} LIMIT {pageEntity.PageDataSize}
";
            // 獨立計數 SQL，不呼叫 FIND_ACTION_NAME，避免對每筆資料執行函數造成效能瓶頸
            string sqlCount = $@"
SELECT COUNT(*)
FROM v_ftt_form2
WHERE 1 = 1
{condition}
";

            var dbHelper = GetDBHelper();
            return new PageResult<VFttForm2DTO>
            {
                CurrentPage = pageEntity.CurrentPage,
                PageDataSize = pageEntity.PageDataSize,
                Results = dbHelper.FindList<VFttForm2DTO>(sqlData, paras),
                DataCount = dbHelper.FindScalar<int>(sqlCount, paras),
            };
        }

        /// <summary>
        /// 取得匯出用的分頁資料
        /// </summary>
        /// <returns></returns>
        public List<VFttForm2DTO> GetPageListExport(VFttForm2DTO searchVO)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            if (searchVO.CreateDateGte.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.createtime)} >= @{nameof(searchVO.CreateDateGte)} ");
                paras.Add(nameof(searchVO.CreateDateGte), searchVO.CreateDateGte);
            }
            if (searchVO.CreateDateLt.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.createtime)} < @{nameof(searchVO.CreateDateLt)} ");
                paras.Add(nameof(searchVO.CreateDateLt), searchVO.CreateDateLt);
            }
            if (searchVO.CompleteDateGte.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.completetime)} >= @{nameof(searchVO.CompleteDateGte)} ");
                paras.Add(nameof(searchVO.CompleteDateGte), searchVO.CompleteDateGte);
            }
            if (searchVO.CompleteDateLt.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.completetime)} < @{nameof(searchVO.CompleteDateLt)} ");
                paras.Add(nameof(searchVO.CompleteDateLt), searchVO.CompleteDateLt);
            }
            if (searchVO.CloseDateGte.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.closedate)} >= @{nameof(searchVO.CloseDateGte)} ");
                paras.Add(nameof(searchVO.CloseDateGte), searchVO.CloseDateGte);
            }
            if (searchVO.CloseDateLt.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.closedate)} < @{nameof(searchVO.CloseDateLt)} ");
                paras.Add(nameof(searchVO.CloseDateLt), searchVO.CloseDateLt);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.StatusIdEq))
            {
                // 除錯：記錄狀態過濾條件 - 寫入檔案
                try { 
                    var logPath = Environment.OSVersion.Platform == PlatformID.Win32NT 
                        ? @"d:\debug_query.log" 
                        : "/tmp/debug_query.log";
                    System.IO.File.AppendAllText(logPath, $"[{DateTime.Now}] GetPageListExport - StatusIdEq filter: {searchVO.StatusIdEq}\n"); 
                } catch (Exception ex) { 
                    // 如果檔案寫入失敗，嘗試寫入應用程式目錄
                    try {
                        var fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug_query_fallback.log");
                        System.IO.File.AppendAllText(fallbackPath, $"[{DateTime.Now}] GetPageListExport - StatusIdEq filter: {searchVO.StatusIdEq} (Fallback: {ex.Message})\n");
                    } catch { }
                }
                condition.Append($"AND {nameof(VFttForm2DTO.statusid)} = @{nameof(searchVO.StatusIdEq)} ");
                paras.Add(nameof(searchVO.StatusIdEq), searchVO.StatusIdEq);
            }
            if (searchVO.FormNoEq.HasValue)
            {
                condition.Append($"AND {nameof(VFttForm2DTO.form_no)} = @{nameof(searchVO.FormNoEq)} ");
                paras.Add(nameof(searchVO.FormNoEq), searchVO.FormNoEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.TtCategoryEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.tt_category)} = @{nameof(searchVO.TtCategoryEq)} ");
                paras.Add(nameof(searchVO.TtCategoryEq), searchVO.TtCategoryEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.CategoryIdFilter))
            {
                condition.Append($@"AND category_id IN (SELECT cisid FROM ci_data
WHERE instr('-' || acisid || '-', '-' || @{nameof(searchVO.CategoryIdFilter)} || '-') > 0) ");
                paras.Add(nameof(searchVO.CategoryIdFilter), searchVO.CategoryIdFilter);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.VenderIdEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.vender)} = @{nameof(searchVO.VenderIdEq)} ");
                paras.Add(nameof(searchVO.VenderIdEq), searchVO.VenderIdEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.IvrCodeEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.ivrcode)} = @{nameof(searchVO.IvrCodeEq)} ");
                paras.Add(nameof(searchVO.IvrCodeEq), searchVO.IvrCodeEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.CompanyEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.company)} = @{nameof(searchVO.CompanyEq)} ");
                paras.Add(nameof(searchVO.CompanyEq), searchVO.CompanyEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.StoreTypeEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.store_type)} = @{nameof(searchVO.StoreTypeEq)} ");
                paras.Add(nameof(searchVO.StoreTypeEq), searchVO.StoreTypeEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.ChannelEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.channel)} = @{nameof(searchVO.ChannelEq)} ");
                paras.Add(nameof(searchVO.ChannelEq), searchVO.ChannelEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.AreaEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.area)} = @{nameof(searchVO.AreaEq)} ");
                paras.Add(nameof(searchVO.AreaEq), searchVO.AreaEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.AsEmpNoEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.as_empno)} = @{nameof(searchVO.AsEmpNoEq)} ");
                paras.Add(nameof(searchVO.AsEmpNoEq), searchVO.AsEmpNoEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.SelfConfigEq))
            {
                condition.Append($"AND {nameof(VFttForm2DTO.selfconfig)} = @{nameof(searchVO.SelfConfigEq)} ");
                paras.Add(nameof(searchVO.SelfConfigEq), searchVO.SelfConfigEq);
            }
            if (searchVO.UserRoleVenderFilter)
            {
                if (SessionVO != null)
                {
                    condition.Append($@"
AND statusid IN ( 'AGREE', 'OFFER', 'PRWP', 'COMPLETE', 'CLOSE' )
AND form_no IN (SELECT form_no
                FROM   access_role
                WHERE  action = 'Y'
                       AND user_type = @user_type
                       AND deptcode = @deptcode)
");
                    paras.Add("user_type", SessionVO.userrole);
                    paras.Add("deptcode", SessionVO.ivrcode);
                }
            }
            else if (searchVO.UserRoleOtherFilter)
            {
                if (SessionVO != null)
                {
                    // 除錯：記錄使用者權限查詢條件 - 寫入檔案
                    try { 
                        var logPath = Environment.OSVersion.Platform == PlatformID.Win32NT 
                            ? @"d:\debug_query.log" 
                            : "/tmp/debug_query.log";
                        System.IO.File.AppendAllText(logPath, $"[{DateTime.Now}] GetPageListExport - UserRoleOtherFilter empno: {SessionVO.empno}, usertype: {SessionVO.usertype}, ivrcode: {SessionVO.ivrcode}\n"); 
                    } catch (Exception ex) { 
                        // 如果檔案寫入失敗，嘗試寫入應用程式目錄
                        try {
                            var fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug_query_fallback.log");
                            System.IO.File.AppendAllText(fallbackPath, $"[{DateTime.Now}] GetPageListExport - UserRoleOtherFilter empno: {SessionVO.empno} (Fallback: {ex.Message})\n");
                        } catch { }
                    }
                    
                    // 針對門市人員(RETAIL 或 STORE)使用不同的查詢邏輯
                    if (SessionVO.usertype == "RETAIL" || SessionVO.usertype == "STORE")
                    {
                        condition.Append($@"
AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  action = 'Y'
                              AND (empno = @empno OR deptcode = @deptcode))
");
                        paras.Add("empno", SessionVO.empno);
                        paras.Add("deptcode", SessionVO.ivrcode);
                    }
                    else
                    {
                        condition.Append($@"
AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  action = 'Y'
                              AND empno = @empno)
AND EXISTS (SELECT 1 FROM access_role WHERE action = 'Y' AND empno = @empno)
");
                        paras.Add("empno", SessionVO.empno);
                    }
                }
            }
            
            // 除錯：記錄最終的 SQL 條件 - 寫入檔案
            try { 
                var logPath = Environment.OSVersion.Platform == PlatformID.Win32NT 
                    ? @"d:\debug_query.log" 
                    : "/tmp/debug_query.log";
                System.IO.File.AppendAllText(logPath, $"[{DateTime.Now}] GetPageListExport - Final SQL condition: {condition}\n"); 
            } catch (Exception ex) { 
                // 如果檔案寫入失敗，嘗試寫入應用程式目錄
                try {
                    var fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug_query_fallback.log");
                    System.IO.File.AppendAllText(fallbackPath, $"[{DateTime.Now}] GetPageListExport - Final SQL condition: {condition} (Fallback: {ex.Message})\n");
                } catch { }
            }

            string sql = $@"
SELECT company
       , store_type
       , channel
       , area
       , shop_name
       , ivrcode
       , as_cname
       , TO_CHAR(createtime, 'YYYY/MM/DD HH24:MI:SS')      AS createtime_text
       , tt_category
       , l1_desc
       , l2_desc
       , ciname
       , TO_CHAR(approval_date + 365, 'YYYY/MM/DD')        AS approval_date_text
       , vender
       , remark
       , form_no
       , TO_CHAR(closedate, 'YYYY/MM/DD HH24:MI:SS')       AS closedate_text
       , TO_CHAR(completetime, 'YYYY/MM/DD HH24:MI:SS')    AS completetime_text
       , statusname
       , descr
       , TO_CHAR(precompletetime, 'YYYY/MM/DD HH24:MI:SS') AS precompletetime_text
       , FIND_ACTION_NAME(form_no)                         AS processer
       , description
       , repair
       , resupply
       , fault_reason
       , repair_action
       , expense_type
       , expense_desc
       , qty
       , unit
       , price
       , subtotal
       , (SELECT MIN(TO_CHAR(updatetime, 'yyyy/mm/dd hh24:mi:ss'))
          FROM   ftt_form_log
          WHERE  form_no = v_ftt_form2.form_no
                 AND fieldname = 'STATUS'
                 AND newvalue = 'USED')                    AS usedtime_text
       , (SELECT MIN(TO_CHAR(updatetime, 'yyyy/mm/dd hh24:mi:ss'))
          FROM   ftt_form_log
          WHERE  form_no = v_ftt_form2.form_no
                 AND fieldname = 'STATUS'
                 AND newvalue = 'ASSIGN')                  AS assign_date_text
       , TO_CHAR(vendor_arrive_date, 'yyyy/mm/dd')         AS vendor_arrive_date_text
       , (SELECT MIN(TO_CHAR(updatetime, 'yyyy/mm/dd hh24:mi:ss'))
          FROM   ftt_form_log
          WHERE  form_no = v_ftt_form2.form_no
                 AND fieldname = 'STATUS'
                 AND newvalue = 'TICKET')                  AS tickettime_text
       , (SELECT MIN(TO_CHAR(updatetime, 'yyyy/mm/dd hh24:mi:ss'))
          FROM   ftt_form_log
          WHERE  form_no = v_ftt_form2.form_no
                 AND fieldname = 'STATUS'
                 AND newvalue = 'CONFIRM')                 AS confirmtime_text
       , selfconfig
       , EXTRACT(day FROM dispatch_days)                   AS dispatch_days
       , EXTRACT(day FROM kpi_days)                        AS kpi_days
       , kpi_result
       , delay_reason
FROM   v_ftt_form2 
WHERE 1 = 1
{condition}
ORDER BY form_no ASC
";

            return GetDBHelper().FindList<VFttForm2DTO>(sql, paras);
        }
    }
}
