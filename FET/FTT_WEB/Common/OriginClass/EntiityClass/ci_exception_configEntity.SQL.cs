using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class ci_exception_configSQL
    {
        public List<ci_exception_configDTO> GetListByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT 
*
FROM ci_exception_config 
where form_no=@form_no

";

            return baseHandler.GetDBHelper().FindList<ci_exception_configDTO>(qrySQL, paras);

        }

        public ci_exception_configDTO GetInfoByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
select * from 
CI_EXCEPTION_CONFIG 
where ENABLE='Y' 
AND CISID IN (SELECT CATEGORY_ID FROM FTT_FORM WHERE FORM_NO=@form_no) 
AND IVRCODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=@form_no) 
AND SYSDATE-APPROVAL_DATE<=365

";

            return baseHandler.GetDBHelper().Find<ci_exception_configDTO>(qrySQL, paras);

        }

        internal List<ci_exception_configDTO> GetExpenseTypeListByCategoryId(string category_id)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("category_id", category_id);

            string sqlWhere = "";

            string qrySQL = @"
SELECT ''as ExpenseType FROM DUAL 
UNION 
SELECT DISTINCT EXPENSE_TYPE as ExpenseType 
FROM ci_exception_config 
WHERE ENABLE='Y' AND CHK_CI_LIST(@CATEGORY_ID,category_id::text)='Y'";

            return baseHandler.GetDBHelper().FindList<ci_exception_configDTO>(qrySQL, paras);
        }

        internal ci_exception_configDTO GetInfoByCategory_Id(string category_id)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("category_id", category_id);

            string sqlWhere = "";

            string qrySQL = $@"

select '1' from ci_exception_config where ENABLE='Y' AND CHK_CI_LIST(@category_id::text,category_id::text)='Y'

";

            return baseHandler.GetDBHelper().Find<ci_exception_configDTO>(qrySQL, paras);
        }
    }
}
