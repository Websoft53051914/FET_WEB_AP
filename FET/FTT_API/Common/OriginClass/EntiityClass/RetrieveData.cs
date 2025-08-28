using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{

    public abstract class RetrieveData
    {
        public abstract DataTable RetrieveDBData(string sCondition);

        public abstract DataTable RetrieveDBData(string acc, string region, bool leave);
    }
}
