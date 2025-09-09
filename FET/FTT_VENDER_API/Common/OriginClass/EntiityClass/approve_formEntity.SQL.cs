using FTT_VENDER_API.Models.Handler;

namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class approve_formSQL
    {
        public approve_formDTO GetInfoByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT *,
       (SELECT status_name
        FROM   form_access_status
        WHERE  form_type = 'FTT_FORM'
               AND form_access_status.status = approve_form.status) AS STATUS_NAME
FROM   approve_form
WHERE  form_no =@form_no 
";

            return baseHandler.GetDBHelper().Find<approve_formDTO>(qrySQL, paras);

        }

        internal approve_formDTO GetInfoByFormNoGroupByFORM_TYPE(string form_no)
        {
            //select FORM_TYPE from APPROVE_FORM where FORM_NO=" + formNo + " GROUP BY FORM_TYPE


            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
select * from APPROVE_FORM 
where FORM_NO=@form_no
GROUP BY FORM_TYPE
";

            return baseHandler.GetDBHelper().Find<approve_formDTO>(qrySQL, paras);
        }
    }
}
