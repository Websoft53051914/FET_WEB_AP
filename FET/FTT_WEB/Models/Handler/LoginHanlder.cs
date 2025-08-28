using Core.Utility.Extensions;
using FTT_WEB.Common;
using FTT_WEB.Common.ConfigurationHelper;
using FTT_WEB.Common.OriginClass;
using FTT_WEB.Common.OriginClass.EntiityClass;
using FTT_WEB.Models.ViewModel;
using FTT_WEB.Models.ViewModel.Login;
using static Const.Enums;

namespace FTT_WEB.Models.Handler
{
    public class LoginHanlder : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public LoginHanlder(ConfigurationHelper confighelper  , Microsoft.AspNetCore.Http.HttpContext httpContext) 
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        /// <summary>
        /// 登入檢查
        /// </summary>
        /// <param name="vm">登入資訊</param>
        /// <returns>錯誤訊息</returns>
        public string Login(LoginVM vm)
        {
            bool logLoginStatus = false;
            bool boolIsAuthenticated = false;
            string logAccount = vm.AC;
            string logFromIP = _httpContext.Connection.RemoteIpAddress?.ToString();
            string logUserType = vm.Role;
            bool checkUserAuthenticated = _configHelper.Config.GetValue<bool>("CheckUserAuthenticated", true);

            string adDomain = _configHelper.Config.GetValue<string>("FETADServer", "");
            LdapAuthentication adAuth = new LdapAuthentication(adDomain);

            string errorMsg = string.Empty;
            SessionVO? sessionVO = null;

            if (vm.Role == "RETAIL" || vm.Role == "EMPLOYEE")
            {
                if (checkUserAuthenticated == true)
                {
                    try
                    {
                        if (true == adAuth.IsAuthenticated(adDomain, vm.AC, vm.PD))
                        {
                            boolIsAuthenticated = true;
                        }
                        else
                        {
                            errorMsg = "帳號或密碼輸入錯誤，請重新輸入！";
                        }

                        if (!vm.IVR_Code.IsNullOrEmpty())
                        {
                            Dictionary<string, object> condition = new Dictionary<string, object>()
                            {
                                { "IVR_Code", vm.IVR_Code },
                            };
                            if (!base.CheckDataExist("STORE_PROFILE", condition))
                            {
                                errorMsg = "IVRCode輸入錯誤，請重新輸入！";
                                boolIsAuthenticated = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = "Error authenticating. [" + adDomain + "]  : " + ex.Message;
                    }
                }
                else
                {
                    boolIsAuthenticated = true;
                }
                //logAccount = vm.AC;

                if (boolIsAuthenticated)
                {
                    Employee emp = new Employee(checkUserAuthenticated, vm.AC, vm.PD, "FET", false, "FTT");
                    if (emp.hasData())
                    {
                        logLoginStatus = true;
                        sessionVO = new SessionVO
                        {
                            empno = emp.EmpNO,
                            empname = emp.EmployeeName,
                            engname = emp.EnglishName,
                            ext = emp.Mobile + "(" + emp.Ext + ")",
                            username = vm.AC,
                            deptcode = emp.DeptCode,
                            usertype = vm.Role,
                            ivrcode = vm.IVR_Code.IsNullOrEmpty() ? "NULL" : vm.IVR_Code,
                            userrole = SystemModelClass.GetUserRole(emp.EmpNO)
                        };
                    }
                    else
                    {
                        errorMsg = $"該帳號[{vm.AC}]不存在、人員已離職，或無權限使用";
                    }
                }
            }
            else if (vm.Role == "VASS")
            {
                if (checkUserAuthenticated == true)
                {
                    try
                    {
                        Dictionary<string, object> condition = new Dictionary<string, object>()
                            {
                                { "IVR_CODE", vm.IVR_Code },
                                { "SHOP_PASSWORD", vm.PD }
                            };
                        if (base.CheckDataExist("STORE_PROFILE", condition))
                        {
                            boolIsAuthenticated = true;
                        }
                        else
                        {
                            errorMsg = "帳號或密碼輸入錯誤，請重新輸入！";
                        }

                    }
                    catch (Exception ex)
                    {
                        errorMsg = $"Error authenticating. [{vm.IVR_Code}] : " + ex.Message;
                    }
                }
                else
                {
                    boolIsAuthenticated = true;
                }

                logAccount = vm.IVR_Code;
                if (true == boolIsAuthenticated)
                {
                    var info = GetStoreInfo(vm.IVR_Code, vm.PD);
                    if (info != null)
                    {
                        logLoginStatus = true;
                        sessionVO = new SessionVO
                        {
                            empno = info.ivr_code,
                            empname = info.shop_name,
                            engname = info.shop_name,
                            ext = info.urgent_tel + "(" + info.owner_tel + ")",
                            username = vm.IVR_Code,
                            deptcode = info.area,
                            usertype = vm.Role,
                            ivrcode = vm.IVR_Code,
                            userrole = SystemModelClass.GetUserRole(info.ivr_code)
                        };
                        Method.SetToSession(sessionVO);
                    }
                    else
                    {
                        errorMsg = $"該IVRCode[{vm.IVR_Code}]不存在或密碼錯誤";
                    }
                }
            }
            else if (vm.Role == "VENDOR")
            {

            }

            if(sessionVO != null)
            {

                sessionVO.Functions.Append(FuncID.Home_View);
                Method.SetToSession(sessionVO);
            }
            

            // 將登入資訊寫入Log Table，以利事後分析是否有不正常登入
            try
            {
                string insertSQL = "";
                Dictionary<string, object> paras = new();
                if (logLoginStatus == true)
                {
                    paras = new Dictionary<string, object>()
                        {
                            { "FROMIPADDRESS", logFromIP },
                            { "USERTYPE", logUserType },
                            { "ACCOUNT", logAccount.Replace("'", "''") },
                            { "LOGINSTATUS", logLoginStatus.ToString() }
                        };
                    insertSQL = "INSERT INTO USER_LOGIN_LOG (FROMIPADDRESS,USERTYPE,ACCOUNT,LOGINSTATUS) VALUES(@FROMIPADDRESS,@USERTYPE,@ACCOUNT,@LOGINSTATUS)";
                }
                else
                {
                    insertSQL = @$"INSERT INTO USER_LOGIN_LOG (FROMIPADDRESS,USERTYPE,ACCOUNT,PASSWORD,LOGINSTATUS) VALUES
                         (@FROMIPADDRESS,@USERTYPE,@ACCOUNT,@PASSWORD,@LOGINSTATUS) ";
                    paras = new Dictionary<string, object>
                        {
                            { "FROMIPADDRESS", logFromIP },
                            { "USERTYPE", logUserType },
                            { "ACCOUNT", logAccount.Replace("'", "''") },
                            { "PASSWORD", vm.PD.Replace("'", "''") },
                            { "LOGINSTATUS", logLoginStatus.ToString() }
                        };
                }

                base.dbHelper.Execute(insertSQL, paras);
                base.dbHelper.Commit();

            }
            catch (Exception err)
            {

            }
            return errorMsg;
        }
       
        public StoreProfileVM GetStoreInfo(string ivr_code, string pd)
        {
            string sql = @"SELECT*
                           FROM STORE_PROFILE
                           WHERE IVR_CODE = @IVR_CODE AND SHOP_PASSWORD = @SHOP_PASSWORD";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "IVR_CODE", ivr_code },
                { "SHOP_PASSWORD", pd }
            };
            StoreProfileVM? result = base.dbHelper.Find<StoreProfileVM>(sql, parameters);
            return result;
        }
    }
}
