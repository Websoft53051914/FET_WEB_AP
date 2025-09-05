using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using System.Text;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class CaseClosedHanlder : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public CaseClosedHanlder(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }
         
        internal PageResult<v_ftt_form2DTO> FindPageList(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            //SELECT DISTINCT form_no as 工單號碼,tt_category as 報修型態,l2_desc as 報修類別,ciname as 報修品項,to_char(createtime,'yyyy/mm/dd hh24:mi:ss') as 報修日期,shop_name as 店名,statusname as 工單狀態,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as 更新日期 FROM v_ftt_form2 WHERE statusid in ('CLOSE','CANCEL','REJECT') AND (UPDATETIME > SYSDATE-180) AND  form_no in (select form_no from ACCESS_ROLE where user_type=:USERROLE or empno=:EMPNO or deptcode=:IVRCODE)
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("USERROLE", dto.USERROLE);
            paras.Add("EMPNO", dto.EMPNO);
            paras.Add("IVRCODE", dto.IVRCODE);

            string originSQL = @"
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
       AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  user_type = @USERROLE
                               OR empno = @EMPNO
                               OR deptcode = @IVRCODE) 

";
             
            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = baseHandler.GetDBHelper().FindPageList<v_ftt_form2DTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }
         
    }
}
