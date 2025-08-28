using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class RetrieveEmpData : RetrieveData
    {
        public override DataTable RetrieveDBData(string sCondition)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            string sql = "SELECT emp.*,dept.DEPTCHINAME,dept.SDEPTNAME,GET_DEPT_DESC(emp.deptcode) as deptnamelist,  (SELECT DEPTCHINAME FROM FET_DEPT_PROFILE tmpDept WHERE tmpDept.DEPTCODE=emp.COSTCENTER) as COSTCENTER_DEPTCHINAME,  (SELECT SDEPTNAME FROM FET_DEPT_PROFILE tmpDept WHERE tmpDept.DEPTCODE=emp.COSTCENTER) as COSTCENTER_SDEPTNAME,  GET_DEPT_DESC(emp.COSTCENTER) as COSTCENTER_NAMELIST FROM fet_user_profile emp, fet_dept_profile dept WHERE emp." + sCondition + " and emp.deptcode = dept.deptcode(+)";
            DataTable result = baseHandler.GetDBHelper().FindDataTable(sql, null);
            return result;
        }

        public override DataTable RetrieveDBData(string acc, string region, bool leave)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("aliasname", acc);
            parameters.Add("region", region.ToUpper());

            string whereClause = " AND emp.aliasname=@aliasname ";
            whereClause = ((region == null || !(region == "FET")) ? (whereClause + " AND emp.REGION= @region") : (whereClause + " AND nvl(trim(emp.REGION),'NULL')<>'FETI'"));
            if (!leave)
            {
                whereClause += " AND emp.offdate is null ";
            }

            string sql = "SELECT emp.*,dept.DEPTCHINAME,dept.SDEPTNAME,GET_DEPT_DESC(emp.deptcode) as deptnamelist,  (SELECT DEPTCHINAME FROM FET_DEPT_PROFILE tmpDept WHERE tmpDept.DEPTCODE=emp.COSTCENTER) as COSTCENTER_DEPTCHINAME,  (SELECT SDEPTNAME FROM FET_DEPT_PROFILE tmpDept WHERE tmpDept.DEPTCODE=emp.COSTCENTER) as COSTCENTER_SDEPTNAME,  GET_DEPT_DESC(emp.COSTCENTER) as COSTCENTER_NAMELIST FROM fet_user_profile emp, fet_dept_profile dept  WHERE emp.deptcode = dept.deptcode(+) " + whereClause;
            DataTable result = baseHandler.GetDBHelper().FindDataTable(sql, parameters);
            return result;
        }
    }
}
