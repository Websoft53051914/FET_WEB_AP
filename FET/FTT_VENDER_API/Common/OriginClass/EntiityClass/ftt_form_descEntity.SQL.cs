using FTT_VENDER_API.Models.Handler;

namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class ftt_form_descSQL
    {
        internal ftt_form_descDTO GetInfoByFormNo(string form_no)
        {
            //select '<img src=\"/images/icon/date.gif\" align=\"absmiddle\" />' || to_char(CREATE_DATE,'yyyy/mm/dd hh24:mi') || '&nbsp;&nbsp;&nbsp;<img src=\"/images/icon/emp.gif\" align=\"absmiddle\" />' || ACTION_NAME || '&nbsp;&nbsp;&nbsp;<img src=\"/images/icon/edit.gif\" align=\"absmiddle\" />' || DESCRIPTION from FTT_FORM_DESC where FORM_NO=" + formNo + " ORDER BY CREATE_DATE DESC

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select 
*
from FTT_FORM_DESC 
where FORM_NO=@form_no
ORDER BY CREATE_DATE DESC

";

            return baseHandler.GetDBHelper().Find<ftt_form_descDTO>(qrySQL, paras);
        }

        internal ftt_form_descDTO GetTT_LAST_DESC(string form_no)
        {
            //select '<img src=\"/images/icon/date.gif\" align=\"absmiddle\" />' || to_char(CREATE_DATE,'yyyy/mm/dd hh24:mi') || '&nbsp;&nbsp;&nbsp;<img src=\"/images/icon/emp.gif\" align=\"absmiddle\" />' || ACTION_NAME || '&nbsp;&nbsp;&nbsp;<img src=\"/images/icon/edit.gif\" align=\"absmiddle\" />' || DESCRIPTION from FTT_FORM_DESC where FORM_NO=" + formNo + " ORDER BY CREATE_DATE DESC

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select 

'<img src=\""/images/icon/date.gif\"" align=\""absmiddle\"" />' 
|| to_char(CREATE_DATE,'yyyy/mm/dd hh24:mi') 
|| '<img src=\""/images/icon/emp.gif\"" align=\""absmiddle\"" />' 
|| ACTION_NAME 
|| '<img src=\""/images/icon/edit.gif\"" align=\""absmiddle\"" />' 
|| DESCRIPTION as TT_LAST_DESC

from FTT_FORM_DESC 
where FORM_NO=@form_no
ORDER BY CREATE_DATE DESC

";

            return baseHandler.GetDBHelper().Find<ftt_form_descDTO>(qrySQL, paras);
        }
    }
}
