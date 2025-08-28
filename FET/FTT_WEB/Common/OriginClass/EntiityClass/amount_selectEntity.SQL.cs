using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class amount_selectSQL
    {
        public List<amount_selectDTO> GetListByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT 
*
FROM amount_select 
where form_no=@form_no

";

            return baseHandler.GetDBHelper().FindList<amount_selectDTO>(qrySQL, paras);

        }

        public amount_selectDTO GetInfoByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT *
FROM amount_select 
where form_no=@form_no

";

            return baseHandler.GetDBHelper().Find<amount_selectDTO>(qrySQL, paras);

        }

        internal List<amount_selectDTO> GetExpenseTypeListByCategoryId(string category_id)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("category_id", category_id);

            string sqlWhere = "";

            string qrySQL = @"
SELECT ''as ExpenseType FROM DUAL 
UNION 
SELECT DISTINCT EXPENSE_TYPE as ExpenseType 
FROM amount_select 
WHERE ENABLE='Y' AND CHK_CI_LIST(@CATEGORY_ID,category_id::text)='Y'";

            return baseHandler.GetDBHelper().FindList<amount_selectDTO>(qrySQL, paras);
        }

        internal amount_selectDTO GetInfoByCategory_Id(string category_id)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("category_id", category_id);

            string sqlWhere = "";

            string qrySQL = $@"

select * from AMOUNT_SELECT where ENABLE='Y' AND CHK_CI_LIST(@category_id::text,category_id::text)='Y'

";

            return baseHandler.GetDBHelper().Find<amount_selectDTO>(qrySQL, paras);
        }
    }
}
