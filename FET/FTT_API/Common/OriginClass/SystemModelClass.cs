using Core.Utility.Extensions;
using FTT_API.Models.Handler;

namespace FTT_API.Common.OriginClass
{
    public class SystemModelClass
    {

        /// <summary>
        /// 取得特定人員的角色權限
        /// </summary>
        /// <param name="EmpNo">員工編號或識別帳號</param>
        /// <param name="sessionVO"></param>
        /// <returns>角色</returns>
        public static string GetUserRole(string EmpNo, SessionVO sessionVO)
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
                    else if (sessionVO.usertype == "VASS")
                    {
                        // 20260407 修正：VASS（加盟店）以 IVR Code 做為 empno，不會出現在 FTT_GROUP，
                        // 必須明確回傳 "VASS" 以確保後續 OnActionExecuting 能正確判斷角色分支。
                        // 修正前此處走 else 回傳 "STORE"，導致 JWT userrole 寫入錯誤值，
                        // OnActionExecuting 找不到對應分支，_sessionVO.ivrcode 為空，新開單初始化失敗。
                        return "VASS";
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
