using Core.Utility.Extensions;
using FTT_VENDER_API.Models.Handler;

namespace FTT_VENDER_API.Common.OriginClass
{
    public class SystemModelClass
    {

        /// <summary>
        /// 取得特定人員的角色權限
        /// </summary>
        /// <param name="EmpNo">員工編號或識別帳號</param>
        /// <returns>角色</returns>
        public static string GetUserRole(string EmpNo, SessionVO? sessionVO)
        {
            string m_Result;

            // 重新取得資料
            BaseDBHandler handler = new BaseDBHandler();
            string sql = "SELECT DISTINCT FTT_GROUP FROM FTT_GROUP WHERE EMPNO = @EMPNO";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "EMPNO", EmpNo }
                };
            List<string> results = handler.GetDBHelper().FindList<string>(sql, parameters);
            m_Result = string.Join(",", results);

            if (results.IsNullOrEmpty())
            {
                string sqlFindManager = "SELECT AS_EMPNO FROM STORE_PROFILE WHERE AS_EMPNO = @EMPNO";
                List<string> retFindManager = handler.GetDBHelper().FindList<string>(sqlFindManager, parameters);

                if (!retFindManager.IsNullOrEmpty())
                {
                    return "MANAGER";
                }

                if (!string.IsNullOrEmpty(sessionVO?.usertype))
                {
                    if (sessionVO.usertype == "VENDOR")
                    {
                        return "VENDOR";
                    }
                    else if (sessionVO.usertype == "EMPLOYEE")
                    {
                        return "EMP";
                    }
                    else
                    {
                        return "STORE";
                    }
                }
            }

            return m_Result;
        }
    }
}
