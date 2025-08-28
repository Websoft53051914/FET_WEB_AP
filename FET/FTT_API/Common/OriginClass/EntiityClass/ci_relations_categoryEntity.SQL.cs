using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
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
