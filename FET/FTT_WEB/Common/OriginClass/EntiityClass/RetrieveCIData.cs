using FTT_WEB.Models.Handler;
using System.Data;
using System.Diagnostics;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class RetrieveCIData : RetrieveData
    {
        public override DataTable RetrieveDBData(string sCondition)
        {
            BaseDBHandler handler = new();
            string text = "SELECT ci_relations.*,get_ci_desc(cisid) as fulldesc FROM ci_relations WHERE cisid = " + sCondition + "";
            Trace.WriteLine("SQL Query = " + text);
            DataTable result = handler.GetDBHelper().FindDataTable(text, []);
            return result;
        }

        public override DataTable RetrieveDBData(string acc, string region, bool leave)
        {
            return null;
        }
    }
}
