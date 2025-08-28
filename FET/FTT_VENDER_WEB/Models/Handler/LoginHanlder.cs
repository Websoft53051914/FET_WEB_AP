using Const;
using Core.Utility.Extensions;
using FTT_VENDER_WEB.Common;
using FTT_VENDER_WEB.Common.ConfigurationHelper;
using FTT_VENDER_WEB.Common.OriginClass;
using FTT_VENDER_WEB.Common.OriginClass.EntiityClass;
using FTT_VENDER_WEB.Models.ViewModel;
using FTT_VENDER_WEB.Models.ViewModel.Login;
using FTT_VENDER_WEB.Models.ViewModel.StoreVenderProfile;
using System.Data;
using System.Data.Common;
using System.Web;
using static Const.Enums;

namespace FTT_VENDER_WEB.Models.Handler
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

            if (vm.Role == "VENDER")
            {
                if (checkUserAuthenticated == true)
                {
                    try
                    {
                        bool isLocked = true;

                        Dictionary<string, object> paras = new Dictionary<string, object>
                        {
                            { "MERCHANT_LOGIN", vm.AC },
                        };
                        string Locked = base.dbHelper.Find<string>("SELECT LOCKED FROM STORE_VENDER_PROFILE WHERE MERCHANT_LOGIN = @MERCHANT_LOGIN", paras);
                        if( Locked == "N")
                        {
                            isLocked = false;
                        }

                        if (isLocked == false)
                        {
                            StoreVenderProfileVM storeVenderProfileVM  = GetStoreVenderProfile(vm.AC, vm.PD);
                            if(storeVenderProfileVM != null)
                            {
                                boolIsAuthenticated = true;
                                logLoginStatus = true;
                                base.dbHelper.Execute("UPDATE STORE_VENDER_PROFILE SET LOGIN_COUNT=1 WHERE MERCHANT_LOGIN = @MERCHANT_LOGIN", paras);
                                base.dbHelper.Commit();

                                sessionVO = new SessionVO
                                {
                                    empno = vm.AC,
                                    empname = storeVenderProfileVM.merchant_name,
                                    engname = storeVenderProfileVM.merchant_name,
                                    ext = storeVenderProfileVM.cp_tel,
                                    username = storeVenderProfileVM.merchant_login,
                                    deptcode = storeVenderProfileVM.merchant_name,
                                    usertype = vm.Role,
                                    ivrcode = storeVenderProfileVM.order_id?.ToString(),
                                };
                            }
                            else
                            {
                                errorMsg = "帳號或密碼輸入錯誤，請重新輸入！";
                                base.dbHelper.Execute("UPDATE STORE_VENDER_PROFILE SET LOGIN_COUNT=LOGIN_COUNT+1 WHERE MERCHANT_LOGIN = @MERCHANT_LOGIN", paras);
                                base.dbHelper.Commit();
                            }
                        }
                        else
                        {
                            errorMsg = "該帳號因密碼輸入錯誤次數太多已遭鎖定，請通知相關單位處理！";
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg = "Error authenticating. [" + HttpUtility.HtmlEncode(vm.AC) + "] : " + ex.ToString();
                    }
                }
                else
                {
                    boolIsAuthenticated = true;
                }
            }

            if (sessionVO != null)
            {
                sessionVO.Functions.AddRange(RoleFunc.Vender);
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
       
        public StoreVenderProfileVM GetStoreVenderProfile(string AC , string PD)
        {
            string sql = @"SELECT * FROM STORE_VENDER_PROFILE WHERE MERCHANT_LOGIN= @AC AND MERCHANT_PASSWORD= @PD";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "AC", AC },
                { "PD", PD }
            };
            StoreVenderProfileVM? result = base.dbHelper.Find<StoreVenderProfileVM>(sql, parameters);
            return result;
        }
    }
}
