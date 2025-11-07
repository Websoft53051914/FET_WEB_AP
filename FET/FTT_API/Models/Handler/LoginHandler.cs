using Const;
using Const.VO;
using Core.Utility.Extensions;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using FTT_API.Models.ViewModel.Login;
using Microsoft.Extensions.Options;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class LoginHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public LoginHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        /// <summary>
        /// 紀錄成功訊息於資料庫
        /// </summary>
        /// <param name="description"></param>
        protected void LogSuccess(string description = null)
        {
            var entity = new TB_Control_LogEntity()
            {
                IP = Method.GetClientIPAddress(),
                Status = ((int)LogStatusEnum.Success).ToString(),
                ControllerName = "login",
                ActionName = "login",
                Exception = description,
                Account = "system",
                Name = "system",
                LogTime = DateTime.Now,
                Token = string.Empty,
            };

            InsertLog(entity);
        }

        protected void InsertLog(TB_Control_LogEntity entity)
        {
            TB_Control_LogHandler _BaseDBHandler = new TB_Control_LogHandler();
            _BaseDBHandler.Insert(entity);
        }

        /// <summary>
        /// 登入檢查
        /// </summary>
        /// <param name="vm">登入資訊</param>
        /// <returns>錯誤訊息</returns>
        public (LoginResultVO, SessionVO?) Login(LoginVM vm)
        {
            LogSuccess("login/login/login---開始");
            bool logLoginStatus = false;
            bool boolIsAuthenticated = false;
            string logAccount = vm.AC;
            string logFromIP = _httpContext.Connection.RemoteIpAddress?.ToString();
            string logUserType = vm.Role;
            bool checkUserAuthenticated = _configHelper.Config.GetValue<bool>("CheckUserAuthenticated", true);
            TokenInfoVO token = new();

            string adDomain = _configHelper.Config.GetValue<string>("FETADServer", "");
            LdapAuthentication adAuth = new LdapAuthentication(adDomain);

            string errorMsg = string.Empty;
            SessionVO? sessionVO = null;

            JwtConfigVO jwtConfigVO = new();

            LogSuccess("login/login/login/---vm.Role=" + vm.Role);
            if (vm.Role == "RETAIL" || vm.Role == "EMPLOYEE")
            {
                if (checkUserAuthenticated == true)
                {
                    try
                    {
                        LogSuccess("login/login/login/IsAuthenticated---開始");
                        if (true == adAuth.IsAuthenticated(adDomain, vm.AC, vm.PD))
                        {
                            boolIsAuthenticated = true;
                        }
                        else
                        {
                            errorMsg = "帳號或密碼輸入錯誤，請重新輸入！";
                        }
                        LogSuccess("login/login/login/IsAuthenticated---結束");


                        LogSuccess("login/login/login/---vm.IVR_Code=" + vm.IVR_Code);
                        if (!vm.IVR_Code.IsNullOrEmpty())
                        {
                            Dictionary<string, object> condition = new Dictionary<string, object>()
                            {
                                { "IVR_Code", vm.IVR_Code },
                            };
                            LogSuccess("login/login/login/CheckDataExist(STORE_PROFILE)---開始");
                            if (!base.CheckDataExist("STORE_PROFILE", condition))
                            {
                                errorMsg = "IVRCode輸入錯誤，請重新輸入！";
                                boolIsAuthenticated = false;
                            }
                            LogSuccess("login/login/login/CheckDataExist(STORE_PROFILE)---結束");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogSuccess("login/login/login/---row 114 =" + ex.Message);
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
                    LogSuccess("login/login/login/Employee(checkUserAuthenticated)---開始 ac=" + vm.AC);
                    Employee emp = new Employee(checkUserAuthenticated, vm.AC, vm.PD, "FET", false, "FTT");
                    LogSuccess("login/login/login/Employee(checkUserAuthenticated)---結束 emp=" + Newtonsoft.Json.JsonConvert.SerializeObject(emp));

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
                        };
                        LogSuccess("login/login/login/GetUserRole---開始");
                        sessionVO.userrole = SystemModelClass.GetUserRole(sessionVO.empno, sessionVO);
                        LogSuccess("login/login/login/GetUserRole---結束 sessionVO.userrole=" + sessionVO.userrole);

                        LogSuccess("login/login/login/GenerateJwtToken---開始");
                        token = Method.GenerateJwtToken(sessionVO, jwtConfigVO);
                        LogSuccess("login/login/login/GenerateJwtToken---結束");
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
                        };
                        sessionVO.userrole = SystemModelClass.GetUserRole(sessionVO.empno ?? string.Empty, sessionVO);
                        token = Method.GenerateJwtToken(sessionVO, jwtConfigVO);
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

            if (sessionVO != null)
            {

                sessionVO.Functions.Append(FuncID.Home_View);
                //Method.SetToSession(sessionVO);
            }


            LogSuccess("login/login/login/將登入資訊寫入Log Table---開始");
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
                if (logLoginStatus == true)
                {
                    paras = new Dictionary<string, object>()
                        {
                            { "TokenId", token.TokenId },
                            { "RegisterDate", token.Iat },
                            { "VoidStartTime", token.Nbf },
                            { "VoidEndTime", token.Exp },
                            { "Account",logAccount.Replace("'", "''") },
                            { "NOW", DateTime.Now },
                            { "Status", StatusEnum.Enabled.ToInt() }
                        };
                    insertSQL = @"INSERT INTO TB_Token(
	TokenId, RegisterDate, VoidStartTime, VoidEndTime, Account, CreateTime, UpdateTime, Status)
	VALUES (@TokenId, @RegisterDate,@VoidStartTime,@VoidEndTime, @Account, @NOW, @NOW, @Status);";
                    base.dbHelper.Execute(insertSQL, paras);
                }

                base.dbHelper.Commit();
                LogSuccess("login/login/login/將登入資訊寫入Log Table---結束");

            }
            catch (Exception err)
            {

            }
            LogSuccess("login/login/login---結束");
            return (new LoginResultVO()
            {
                ErrorMsg = errorMsg,
                Token = token
            }, sessionVO);
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

        public void Logout(string token)
        {
            string sql = $"UPDATE tb_token SET Status = {StatusEnum.Cancel.ToInt()},UpdateTime = @NOW WHERE TokenId = @TokenId";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "TokenId", token },
                { "NOW", DateTime.Now }
            };
            base.dbHelper.Execute(sql, parameters);
            base.dbHelper.Commit();
        }


    }
}
