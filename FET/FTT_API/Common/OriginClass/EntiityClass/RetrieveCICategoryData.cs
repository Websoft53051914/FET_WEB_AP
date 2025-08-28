using FTT_API.Models.Handler;
using System.Data;
using System.Diagnostics;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class RetrieveCICategoryData : RetrieveData
    {
        public override DataTable RetrieveDBData(string sCondition)
        {
            BaseDBHandler handler = new BaseDBHandler();
            string text = "SELECT * FROM ci_category WHERE cicategory = " + sCondition + "";
            Trace.WriteLine("SQL Query = " + text);

            return handler.GetDBHelper().FindDataTable(text, []);
        }

        public override DataTable RetrieveDBData(string acc, string region, bool leave)
        {
            return null;
        }
    }
}
