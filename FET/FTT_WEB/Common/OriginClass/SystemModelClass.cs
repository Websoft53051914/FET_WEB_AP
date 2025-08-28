using Core.Utility.Extensions;
using FTT_WEB.Models.Handler;

namespace FTT_WEB.Common.OriginClass
{
    public class SystemModelClass
    {

        /// <summary>
        /// 取得特定人員的角色權限
        /// </summary>
        /// <param name="EmpNo">員工編號或識別帳號</param>
        /// <returns>角色</returns>
        public static string GetUserRole(string EmpNo)
        {
            string m_Result;

            if (LoginSession.Current != null && !LoginSession.Current.userrole.IsNullOrEmpty())
            {   // 直接套用存在 Session 中的資料
                m_Result = LoginSession.Current.userrole;
            }
            else
            {   // 重新取得資料

                BaseDBHandler handler = new BaseDBHandler();
                string sql = "SELECT DISTINCT FTT_GROUP FROM FTT_GROUP WHERE EMPNO = @EMPNO";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "EMPNO", EmpNo }
                };
                List<string> results = handler.GetDBHelper().FindList<string>(sql, parameters);
                m_Result = string.Join(",", results);
            }

            return m_Result;
        }
    }
}
