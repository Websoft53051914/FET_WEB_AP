using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{

    public class RetrieveEmpWithoutFETIData : RetrieveData
    {
        public override DataTable RetrieveDBData(string sCondition)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("sCondition", sCondition);

            string sqlWhere = "";

            string qrySQL = $"SELECT emp.*,dept.DEPTCHINAME,dept.SDEPTNAME,GET_DEPT_DESC(emp.deptcode) as deptnamelist FROM fet_user_profile emp, fet_dept_profile dept WHERE emp.{sCondition} and nvl(trim(emp.region),'NULL')<>'FETI' and emp.deptcode = dept.deptcode(+)";

            return baseHandler.GetDBHelper().FindDataTable(qrySQL, paras);
        }

        public override DataTable RetrieveDBData(string acc, string region, bool leave)
        {
            return null;
        }
    }
}
