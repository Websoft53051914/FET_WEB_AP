using Core.Utility.Helper.DB.Entity;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using System.Text;

namespace FTT_API.Models.Handler
{
    public class CaseClosedHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public CaseClosedHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }
         
        internal PageResult<v_ftt_form2DTO> FindPageList(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            //ArgumentNullException.ThrowIfNull(SessionVO);
            //SELECT DISTINCT form_no as 工單號碼,tt_category as 報修型態,l2_desc as 報修類別,ciname as 報修品項,to_char(createtime,'yyyy/mm/dd hh24:mi:ss') as 報修日期,shop_name as 店名,statusname as 工單狀態,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as 更新日期 FROM v_ftt_form2 WHERE statusid in ('CLOSE','CANCEL','REJECT') AND (UPDATETIME > SYSDATE-180) AND  form_no in (select form_no from ACCESS_ROLE where user_type=:USERROLE or empno=:EMPNO or deptcode=:IVRCODE)
            StringBuilder condition = new();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("USERROLE", dto.USERROLE);
            paras.Add("EMPNO", dto.EMPNO);
            paras.Add("IVRCODE", dto.IVRCODE);

            // === 修正權限過濾邏輯並加入日誌 ===
            try {
                var logMessage = $"[{DateTime.Now}] CaseClosedHandler - USERROLE: {dto.USERROLE}, EMPNO: {dto.EMPNO}, IVRCODE: {dto.IVRCODE}\n";
                System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", logMessage);
            } catch { }

            if(dto.USERROLE == "MANAGER")
            {
                condition.Append(@"
AND form_no IN (SELECT form_no
                FROM   access_role
                WHERE  user_type = @USERROLE
                        AND empno = @EMPNO
                        AND @IVRCODE IS NOT NULL) 
");
            }
            else if (dto.USERROLE == "ADMIN" || dto.USERROLE == "SECURITY" || dto.USERROLE == "ASSETER" || dto.USERROLE == "ASSISTANT")
            {
                // 管理員角色不加權限過濾，可以看所有資料
                try {
                    System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", $"[{DateTime.Now}] Admin role detected - no access filter\n");
                } catch { }
            }
            else
            {
                // === 修正：非管理員只能看自己有權限的資料，改用 AND 邏輯，並加上 action='Y' ===
                condition.Append(@"
AND form_no IN (SELECT form_no
                FROM   access_role
                WHERE  action = 'Y'
                       AND empno = @EMPNO)
AND EXISTS (SELECT 1 FROM access_role WHERE action = 'Y' AND empno = @EMPNO)
");
                try {
                    System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", $"[{DateTime.Now}] Non-admin role - applying strict empno filter\n");
                } catch { }
            }

            string originSQL = $@"
SELECT DISTINCT form_no                                      AS form_no,
                tt_category                                  AS tt_category,
                l2_desc                                      AS l2_desc,
                ciname                                       AS ciname,
                To_char(createtime, 'yyyy/mm/dd hh24:mi:ss') AS createtime,
                shop_name                                    AS shop_name,
                statusname                                   AS statusname,
                To_char(updatetime, 'yyyy/mm/dd hh24:mi:ss') AS updatetime
FROM   v_ftt_form2
WHERE  statusid IN ( 'CLOSE', 'CANCEL', 'REJECT' )
       AND ( updatetime > SYSDATE - 180 )
       {condition}

";

            // === 記錄最終 SQL 條件 ===
            try {
                System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", $"[{DateTime.Now}] Final SQL condition: {condition}\n");
                System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", $"[{DateTime.Now}] Complete SQL: {originSQL}\n");
            } catch { }
             
            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = dbHelper.FindPageList<v_ftt_form2DTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal PageResult<v_ftt_form2DTO> FindPageListForCount(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            //ArgumentNullException.ThrowIfNull(SessionVO);
            //SELECT DISTINCT form_no as 工單號碼,tt_category as 報修型態,l2_desc as 報修類別,ciname as 報修品項,to_char(createtime,'yyyy/mm/dd hh24:mi:ss') as 報修日期,shop_name as 店名,statusname as 工單狀態,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as 更新日期 FROM v_ftt_form2 WHERE statusid in ('CLOSE','CANCEL','REJECT') AND (UPDATETIME > SYSDATE-180) AND  form_no in (select form_no from ACCESS_ROLE where user_type=:USERROLE or empno=:EMPNO or deptcode=:IVRCODE)
            StringBuilder condition = new();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("USERROLE", dto.USERROLE);
            paras.Add("EMPNO", dto.EMPNO);
            paras.Add("IVRCODE", dto.IVRCODE);

            // === 修正計數查詢的權限過濾邏輯，與主查詢保持一致 ===
            try {
                var logMessage = $"[{DateTime.Now}] CaseClosedHandler.FindPageListForCount - USERROLE: {dto.USERROLE}, EMPNO: {dto.EMPNO}, IVRCODE: {dto.IVRCODE}\n";
                System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", logMessage);
            } catch { }

            if (dto.USERROLE == "MANAGER")
            {
                condition.Append(@"
AND form_no IN (SELECT form_no
                FROM   access_role
                WHERE  user_type = @USERROLE
                        AND empno = @EMPNO
                        AND @IVRCODE IS NOT NULL) 
");
            }
            else if (dto.USERROLE == "ADMIN" || dto.USERROLE == "SECURITY" || dto.USERROLE == "ASSETER" || dto.USERROLE == "ASSISTANT")
            {
                // 管理員角色不加權限過濾，可以看所有資料
                try {
                    System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", $"[{DateTime.Now}] FindPageListForCount - Admin role detected - no access filter\n");
                } catch { }
            }
            else
            {
                // === 修正：非管理員只能看自己有權限的資料，改用 AND 邏輯，並加上 action='Y' ===
                condition.Append(@"
AND form_no IN (SELECT form_no
                FROM   access_role
                WHERE  action = 'Y'
                       AND empno = @EMPNO)
AND EXISTS (SELECT 1 FROM access_role WHERE action = 'Y' AND empno = @EMPNO)
");
                try {
                    System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", $"[{DateTime.Now}] FindPageListForCount - Non-admin role - applying strict empno filter\n");
                } catch { }
            }

            string originSQL = $@"
SELECT DISTINCT form_no                                      AS form_no 
FROM   v_ftt_form2
WHERE  statusid IN ( 'CLOSE', 'CANCEL', 'REJECT' )
       AND ( updatetime > SYSDATE - 180 )
       {condition}

";

            // === 記錄計數查詢的最終 SQL 條件 ===
            try {
                System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", $"[{DateTime.Now}] FindPageListForCount - Final SQL condition: {condition}\n");
                System.IO.File.AppendAllText(@"d:\caseclosed_handler_debug.log", $"[{DateTime.Now}] FindPageListForCount - Complete SQL: {originSQL}\n");
            } catch { }

            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = dbHelper.FindPageList<v_ftt_form2DTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }
    }
}
