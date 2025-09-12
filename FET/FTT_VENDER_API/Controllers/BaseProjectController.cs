using Core.Utility.Helper.Message;
using Core.Utility.Web.Base;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Models;
using FTT_VENDER_API.Models.Handler;
using FTT_VENDER_API.Models.ViewModel.StoreVenderProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace FTT_VENDER_API.Controllers
{
    public class BaseProjectController : BaseController
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            if (headers.TryGetValue("X-Custom-Header_acc", out var headerValue_acc))
            {
                headers.TryGetValue("X-Custom-Header_userrole", out var headerValue_userrole);
                headers.TryGetValue("X-Custom-Header_ivrCode", out var headerValue_ivrCode);
                headers.TryGetValue("X-Custom-Header_usertype", out var headerValue_usertype);

                bool logLoginStatus = false;
                bool boolIsAuthenticated = false;
                string logAccount = headerValue_acc;
                //string logFromIP = _httpContext.Connection.RemoteIpAddress?.ToString();
                string logUserType = headerValue_usertype;
                bool checkUserAuthenticated = false;

                string adDomain = Method.GetAppSettingsDataByName("FETADServer");
                //LdapAuthentication adAuth = new LdapAuthentication(adDomain);

                string errorMsg = string.Empty;
                SessionVO? sessionVO = null;

                if (headerValue_usertype == "VENDER")
                {
                    BaseDBHandler _BaseDBHandler = new BaseDBHandler();

                    if (checkUserAuthenticated == true)
                    {
                        try
                        {
                            bool isLocked = true;

                            Dictionary<string, object> paras = new Dictionary<string, object>
                        {
                            { "MERCHANT_LOGIN", headerValue_acc },
                        };
                            string Locked = _BaseDBHandler.GetDBHelper().Find<string>("SELECT LOCKED FROM STORE_VENDER_PROFILE WHERE MERCHANT_LOGIN = @MERCHANT_LOGIN", paras);
                            if (Locked == "N")
                            {
                                isLocked = false;
                            }

                            if (isLocked == false)
                            {
                                StoreVenderProfileVM storeVenderProfileVM = GetStoreVenderProfile(headerValue_acc, "");
                                if (storeVenderProfileVM != null)
                                {
                                    boolIsAuthenticated = true;
                                    logLoginStatus = true;
                                    _BaseDBHandler.GetDBHelper().Execute("UPDATE STORE_VENDER_PROFILE SET LOGIN_COUNT=1 WHERE MERCHANT_LOGIN = @MERCHANT_LOGIN", paras);
                                    _BaseDBHandler.GetDBHelper().Commit();

                                    sessionVO = new SessionVO
                                    {
                                        empno = headerValue_acc,
                                        empname = storeVenderProfileVM.merchant_name,
                                        engname = storeVenderProfileVM.merchant_name,
                                        ext = storeVenderProfileVM.cp_tel,
                                        username = storeVenderProfileVM.merchant_login,
                                        deptcode = storeVenderProfileVM.merchant_name,
                                        usertype = headerValue_usertype,
                                        ivrcode = storeVenderProfileVM.order_id?.ToString(),
                                    };
                                }
                                else
                                {
                                    //errorMsg = "帳號或密碼輸入錯誤，請重新輸入！";
                                    _BaseDBHandler.GetDBHelper().Execute("UPDATE STORE_VENDER_PROFILE SET LOGIN_COUNT=LOGIN_COUNT+1 WHERE MERCHANT_LOGIN = @MERCHANT_LOGIN", paras);
                                    _BaseDBHandler.GetDBHelper().Commit();
                                }
                            }
                            else
                            {
                                //errorMsg = "該帳號因密碼輸入錯誤次數太多已遭鎖定，請通知相關單位處理！";
                            }
                        }
                        catch (Exception ex)
                        {
                            //errorMsg = "Error authenticating. [" + HttpUtility.HtmlEncode(headerValue_acc) + "] : " + ex.ToString();
                        }
                    }
                    else
                    {
                        boolIsAuthenticated = true;
                    }
                }

                if (boolIsAuthenticated)
                {
                    StoreVenderProfileVM storeVenderProfileVM = GetStoreVenderProfile(logAccount, "TEST", true);
                    if (storeVenderProfileVM != null)
                    {
                        logLoginStatus = true;

                        sessionVO = new SessionVO
                        {
                            empno = logAccount,
                            empname = storeVenderProfileVM.merchant_name,
                            engname = storeVenderProfileVM.merchant_name,
                            ext = storeVenderProfileVM.cp_tel,
                            username = storeVenderProfileVM.merchant_login,
                            deptcode = storeVenderProfileVM.merchant_name,
                            usertype = headerValue_usertype,
                            ivrcode = storeVenderProfileVM.order_id?.ToString(),
                        };
                    }
                }

                if (sessionVO != null)
                {
                    //sessionVO.Functions.AddRange(RoleFunc.Vender);
                    Method.SetToSession(sessionVO);
                }
            }

            base.OnActionExecuting(context);
        }
        public StoreVenderProfileVM GetStoreVenderProfile(string AC, string PD, bool isPassPWD = false)
        {
            BaseDBHandler _BaseDBHandler = new BaseDBHandler();
            string sql = @"SELECT * FROM STORE_VENDER_PROFILE WHERE MERCHANT_LOGIN= @AC AND MERCHANT_PASSWORD= @PD";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "AC", AC },
                { "PD", PD }
            };

            if (isPassPWD == true)
            {
                sql = @"SELECT * FROM STORE_VENDER_PROFILE WHERE MERCHANT_LOGIN= @AC ";
            }

            StoreVenderProfileVM? result = _BaseDBHandler.GetDBHelper().Find<StoreVenderProfileVM>(sql, parameters);
            return result;
        }

        /// <summary>
        /// 登入資訊
        /// </summary>
        public SessionVO _sessionVO = new();

        #region -- Instance --

        private MessageHelper? _msgHelper = null;
        /// <summary>
        /// 錯誤訊息資訊
        /// </summary>
        /// <returns></returns>
        public MessageHelper GetMessage()
        {
            _msgHelper ??= new MessageHelper();
            return _msgHelper;
        }

        private SelectListHandler? _selectListHandler = null;
        /// <summary>
        /// SelectListHandler
        /// </summary>
        /// <returns></returns>
        public SelectListHandler GetSelectListHandler()
        {
            _selectListHandler ??= new SelectListHandler();
            return _selectListHandler;
        }

        #endregion  -- Instance --

        /// <summary>
        /// 轉址至指定位置並顯示訊息
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="message"></param>
        /// <param name="alertType"></param>
        /// <returns></returns>
        protected IActionResult RedirectToAlertMsg(string actionName, string controllerName, string message, string alertType = "success")
        {
            var paras = new AlertMsgRedirection()
            {
                ActionName = actionName,
                ControllerName = controllerName,
                Msgs = new List<string>() { message },
                AlertType = alertType
            };

            if (HttpContext.Request != null && HttpContext.Request.Query.ContainsKey("className"))
            {
                paras.ClassName = HttpContext.Request.Query["className"].FirstOrDefault() ?? string.Empty;
            }

            return RedirectToAction("Redirection", "AlertMsg", paras);
        }

        /// <summary>
        /// 紀錄例外
        /// </summary>
        /// <param name="ex"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected void LogError(Exception ex)
        {
            Trace.Write("<font color=red>Source:" + ex.Source + "</font>");
            Trace.Write("<font color=red>Msg:" + ex.Message + "</font>");
            Trace.Write(ex.ToString());
        }


        /// <summary>
        /// 紀錄例外於資料庫
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>ControlLog.Id</returns>
        //protected long LogError(Exception ex)
        //{
        //    var blLog = BLFactory.GetInstance<LogBL>();

        //    var logDM = new ControlLogDM()
        //    {
        //        IP = LoginSession.Current.IP ?? Method.GetClientIPAddress(),
        //        Status = ((int)LogStatusEnum.Failed).ToString(),
        //        ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
        //        ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
        //        Exception = ex.ToString()
        //    };

        //    return blLog.InsertLog(logDM);
        //}

        /// <summary>
        /// 紀錄失敗訊息於資料庫
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>ControlLog.Id</returns>
        //protected long LogError(string exception)
        //{
        //    var blLog = BLFactory.GetInstance<LogBL>();

        //    var logDM = new ControlLogDM()
        //    {
        //        IP = LoginSession.Current.IP ?? Method.GetClientIPAddress(),
        //        Status = ((int)LogStatusEnum.Failed).ToString(),
        //        ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
        //        ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
        //        Exception = exception
        //    };

        //    return blLog.InsertLog(logDM);
        //}

        /// <summary>
        /// 紀錄成功訊息於資料庫
        /// </summary>
        /// <param name="description"></param>
        //protected void LogSuccess(string description = null)
        //{
        //    var blLog = BLFactory.GetInstance<LogBL>();

        //    var logDM = new ControlLogDM()
        //    {
        //        IP = LoginSession.Current.IP ?? Method.GetClientIPAddress(),
        //        Status = ((int)LogStatusEnum.Success).ToString(),
        //        ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
        //        ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
        //        Exception = description,
        //    };

        //    blLog.InsertLog(logDM);
        //}

    }


}
