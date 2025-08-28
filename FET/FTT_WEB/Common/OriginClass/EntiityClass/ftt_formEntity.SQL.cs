using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class ftt_formSQL
    {
        public ftt_formDTO GetInfoByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT b.cp_name,b.cp_tel,b.merchant_name,a.*,(SELECT CINAME FROM CI_RELATIONS WHERE CI_RELATIONS.CISID=a.CATEGORY_ID AND ROWNUM=1) as CIDesc 
FROM FTT_FORM a
left outer join store_vender_profile b on a.vender_id=b.order_id

where form_no=@form_no

";

            return baseHandler.GetDBHelper().Find<ftt_formDTO>(qrySQL, paras);

        }

        internal ftt_formDTO GetTT_COUNTByCATEGORY_ID(string form_no, string CATEGORY_ID)
        {
            //select decode(count(FORM_NO),0,'NO','YES') from FTT_FORM where CATEGORY_ID=" + mCIID + " AND IVRCODE='" + mFormNo + "' AND CREATETIME > to_date(to_char(sysdate,'yyyy/mm/dd')||' 00:00:00','yyyy/mm/dd hh24:mi:ss')

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);
            paras.Add("CATEGORY_ID", CATEGORY_ID);

            string sqlWhere = "";

            string qrySQL = $@"
select decode(count(FORM_NO),0,'NO','YES')  as TT_COUNT
from FTT_FORM 
where CATEGORY_ID=@CATEGORY_ID
AND IVRCODE=@form_no
AND CREATETIME > to_date(to_char(sysdate,'yyyy/mm/dd')||' 00:00:00','yyyy/mm/dd hh24:mi:ss')

";

            return baseHandler.GetDBHelper().Find<ftt_formDTO>(qrySQL, paras);

        }
    }
}
