using FTT_VENDER_API.Models.Handler;

namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class ci_relations_categorySQL
    {
        internal ci_relations_categoryDTO GetInfoByCISID(string CISID)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("CISID", CISID);

            string sqlWhere = "";

            string qrySQL = $@"

select NOTES from CI_RELATIONS_CATEGORY
where CISID=@CISID

";

            return baseHandler.GetDBHelper().Find<ci_relations_categoryDTO>(qrySQL, paras);
        }
    }
}
