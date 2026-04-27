using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.ViewModel;
using System.Text;
using static Const.Enums;

namespace FTT_VENDER_API.Models.Handler
{
    public class InProcessHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public InProcessHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal PageResult<v_ftt_form2DTO> FindPageList(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
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
                To_char(updatetime, 'yyyy/mm/dd hh24:mi:ss') AS updatetime,
                @EMPNO
FROM   v_ftt_form2
WHERE  statusid IN ( 'AGREE', 'OFFER', 'COMPLETE', 'CONFIRM' )
       AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  user_type = @USERROLE
                              AND deptcode = @IVRCODE
                              AND @EMPNO IS NOT NULL)
ORDER  BY updatetime DESC 
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

            var result = dbHelper.FindPageList<v_ftt_form2DTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal PageResult<v_ftt_form2DTO> FindPageListForCount(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("USERROLE", dto.USERROLE);
            paras.Add("EMPNO", dto.EMPNO);
            paras.Add("IVRCODE", dto.IVRCODE);

            string originSQL = @"
SELECT DISTINCT form_no                                      AS form_no 
FROM   v_ftt_form2
WHERE  statusid IN ( 'AGREE', 'OFFER', 'COMPLETE', 'CONFIRM' )
       AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  user_type = @USERROLE
                              AND deptcode = @IVRCODE
                              AND action = 'Y'
                              AND @EMPNO IS NOT NULL) 
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

            var result = dbHelper.FindPageList<v_ftt_form2DTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }
    }
}
