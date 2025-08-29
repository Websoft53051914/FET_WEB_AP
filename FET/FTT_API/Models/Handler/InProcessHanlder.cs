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
    public class InProcessHanlder : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public InProcessHanlder(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal bool CheckDataExist_APPROVE_FORM(string form_no, string kpiTime)
        {
            Dictionary<string, object> paras = new()
            {
                { "FORM_NO", form_no },
                { "kpiTime", kpiTime }
            };

            string tableName = " APPROVE_FORM ";
            string strWhere = " FORM_NO=@FORM_NO AND CHK_WORKING_DAY2(UPDATETIME,SYSDATE,'S') > @kpiTime ";

            return CheckDataExist(tableName, strWhere, paras);
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
                Find_action_name(form_no)                    AS CurrentInchargeName,

                statusid
FROM   v_ftt_form2
WHERE  statusid NOT IN ( 'CLOSE', 'REJECT', 'CANCEL', 'NOSHOW' )
       AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  user_type = @USERROLE
                               OR empno = @EMPNO
                               OR deptcode = @IVRCODE)
ORDER  BY updatetime DESC 
";

            switch (dto.USERROLE)
            {
                case "VENDOR":
                    originSQL = @"
SELECT DISTINCT form_no                                      AS form_no,
                tt_category                                  AS tt_category,
                l2_desc                                      AS l2_desc,
                ciname                                       AS ciname,
                To_char(createtime, 'yyyy/mm/dd hh24:mi:ss') AS createtime,
                shop_name                                    AS shop_name,
                statusname                                   AS statusname,
                To_char(updatetime, 'yyyy/mm/dd hh24:mi:ss') AS updatetime,
                Find_action_name(form_no)                    AS CurrentInchargeName,
                @EMPNO,
                statusid
FROM   v_ftt_form2
WHERE  statusid IN ( 'AGREE', 'OFFER', 'COMPLETE' )
       AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  user_type = @USERROLE
                              AND deptcode = @IVRCODE
                              AND @EMPNO IS NOT NULL)
ORDER  BY updatetime DESC 
";
                    break;
                case "MANAGER":
                    originSQL = @"
SELECT DISTINCT form_no                                      AS form_no,
                tt_category                                  AS tt_category,
                l2_desc                                      AS l2_desc,
                ciname                                       AS ciname,
                To_char(createtime, 'yyyy/mm/dd hh24:mi:ss') AS createtime,
                shop_name                                    AS shop_name,
                statusname                                   AS statusname,
                To_char(updatetime, 'yyyy/mm/dd hh24:mi:ss') AS updatetime,
                Find_action_name(form_no)                    AS CurrentInchargeName,
                @EMPNO,
                statusid
FROM   v_ftt_form2
WHERE  statusid NOT IN ( 'CLOSE', 'REJECT', 'CANCEL', 'NOSHOW' )
       AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  user_type = @USERROLE
                              AND empno = @EMPNO
                              AND @IVRCODE IS NOT NULL)
ORDER  BY updatetime DESC 
";
                    break;
                default:
                    break;
            }

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

        internal string GetFORM_TYPE(string form_no)
        {
            return GetFieldData("FORM_TYPE", "APPROVE_FORM", new Dictionary<string, object>() { { "FORM_NO", form_no } });
        }

        internal string GetKPITime(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                {"form_no", form_no },
            };

            string sql = " select category.kpitime from FTT_FORM form, CI_RELATIONS_CATEGORY category where category.CISID=form.CATEGORY_ID AND form.FORM_NO=@form_no ";

            return GetDBHelper().FindScalar<string>(sql, paras);
        }

        internal void InsertFTT_FORM_LOG(string formNo, string empName, string formType)
        {
            Dictionary<string, object> paras = new()
            {
                {"formNo", formNo },
                {"empName", empName },
                {"formType", formType },
            };


            string sql = "INSERT INTO FTT_FORM_LOG (FORM_NO,UPDATE_EMPNO,UPDATETIME,FIELDNAME,ACTION,FORM_TYPE,ROOT_NO) VALUES (@formNo,@empName,SYSDATE,'催單','FORM',@formType,@formNo)";
            GetDBHelper().Execute(sql, paras);
            GetDBHelper().Commit();
        }
    }
}
