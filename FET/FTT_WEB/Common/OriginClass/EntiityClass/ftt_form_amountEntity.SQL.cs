using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class ftt_form_amountSQL
    {
        public List<ftt_form_amountDTO> GetListByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT FORM_NO, EXPENSE_TYPE, EXPENSE_DESC, QTY, PRICE, SUBTOTAL, ORDERID, UNIT, FAULT_REASON, REPAIR_ACTION, ENABLE 
FROM FTT_FORM_AMOUNT 
where form_no=@form_no

";

            return baseHandler.GetDBHelper().FindList<ftt_form_amountDTO>(qrySQL, paras);

        }

        public ftt_form_amountDTO GetInfoByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT b.cp_name,b.cp_tel,b.merchant_name,a.*,(SELECT CINAME FROM CI_RELATIONS WHERE CI_RELATIONS.CISID=a.CATEGORY_ID AND ROWNUM=1) as CIDesc 
FROM ftt_form_amount a
left outer join store_vender_profile b on a.vender_id=b.order_id

where form_no=@form_no

";

            return baseHandler.GetDBHelper().Find<ftt_form_amountDTO>(qrySQL, paras);

        }

        internal string GetTotalPrice(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select sum(qty*price) from FTT_FORM_AMOUNT WHERE FORM_NO=@form_no AND ENABLE='Y'

";

            return baseHandler.GetDBHelper().FindScalar<string>(qrySQL, paras);
        }

        internal ftt_form_amountDTO GetInfoByFormNo_ENABLE_Y(string form_no)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select * from FTT_FORM_AMOUNT where FORM_NO=@form_no AND ENABLE='Y'

";

            return baseHandler.GetDBHelper().Find<ftt_form_amountDTO>(qrySQL, paras);
        }
    }
}
