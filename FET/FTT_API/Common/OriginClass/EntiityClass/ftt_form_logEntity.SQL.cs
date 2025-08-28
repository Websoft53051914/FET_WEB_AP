using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class ftt_form_logSQL
    {
        public ftt_form_logDTO GetInfoByFormNo(string form_no, string FIELDNAME, string OLDVALUE, string NEWVALUE)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            paras.Add("FIELDNAME", FIELDNAME);
            paras.Add("OLDVALUE", OLDVALUE);
            paras.Add("NEWVALUE", NEWVALUE);

            string sqlWhere = "";

            string qrySQL = $@"
select * from FTT_FORM_LOG 
where FORM_NO=@form_no
AND FIELDNAME=@FIELDNAME
AND OLDVALUE=@OLDVALUE
AND NEWVALUE=@NEWVALUE

";

            return baseHandler.GetDBHelper().Find<ftt_form_logDTO>(qrySQL, paras);

        }
         
    }
}
