using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class Ftt_form_amountSQL
    {
        public List<Ftt_form_amountDTO> GetListByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT FORM_NO, EXPENSE_TYPE, EXPENSE_DESC, QTY, PRICE, SUBTOTAL, ORDERID, UNIT, FAULT_REASON, REPAIR_ACTION, ENABLE 
FROM Ftt_form_amount 
where form_no=@form_no

";

            return baseHandler.GetDBHelper().FindList<Ftt_form_amountDTO>(qrySQL, paras);

        }

        public Ftt_form_amountDTO GetInfoByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT b.cp_name,b.cp_tel,b.merchant_name,a.*,(SELECT CINAME FROM CI_RELATIONS WHERE CI_RELATIONS.CISID=a.CATEGORY_ID AND ROWNUM=1) as CIDesc 
FROM Ftt_form_amount a
left outer join store_vender_profile b on a.vender_id=b.order_id

where form_no=@form_no

";

            return baseHandler.GetDBHelper().Find<Ftt_form_amountDTO>(qrySQL, paras);

        }

        internal string GetTotalPrice(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select sum(qty*price) from Ftt_form_amount WHERE FORM_NO=@form_no AND ENABLE='Y'

";

            return baseHandler.GetDBHelper().FindScalar<string>(qrySQL, paras);
        }

        internal Ftt_form_amountDTO GetInfoByFormNo_ENABLE_Y(string form_no)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select * from Ftt_form_amount where FORM_NO=@form_no AND ENABLE='Y'

";

            return baseHandler.GetDBHelper().Find<Ftt_form_amountDTO>(qrySQL, paras);
        }
    }
}
