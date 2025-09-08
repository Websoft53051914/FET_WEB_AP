using FTT_VENDER_API.Models.Handler;

namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class store_profileSQL
    {
        public List<Store_profileDTO> GetListByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT 
*
FROM store_profile 
where form_no=@form_no

";

            return baseHandler.GetDBHelper().FindList<Store_profileDTO>(qrySQL, paras);

        }

        public Store_profileDTO GetInfoByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select * from STORE_PROFILE 
where IVR_CODE IN 
(
SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=@form_no
)

";

            return baseHandler.GetDBHelper().Find<Store_profileDTO>(qrySQL, paras);

        }

        internal Store_profileDTO GetListByFormNoDate365(string form_no)
        {
            //select '1' from STORE_PROFILE where IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=" + formNo + ") AND SYSDATE BETWEEN APPROVAL_DATE AND APPROVAL_DATE+365


            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select * from STORE_PROFILE 
where IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=@form_no) 
AND SYSDATE BETWEEN APPROVAL_DATE 
AND APPROVAL_DATE+365


";

            return baseHandler.GetDBHelper().Find<Store_profileDTO>(qrySQL, paras);
        }

        internal Store_profileDTO GetListByFormNoNotIn1278And1260(string form_no)
        {
            //select '1' from STORE_PROFILE where IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=" + formNo + " and ci_sid_l1 (category_id) not in (1278,1260))

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"

select '1' from STORE_PROFILE 
where IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=@form_no
and ci_sid_l1 (category_id) not in (1278,1260))


";

            return baseHandler.GetDBHelper().Find<Store_profileDTO>(qrySQL, paras);
        }
    }
}
