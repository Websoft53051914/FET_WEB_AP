using Core.Utility.Helper.Message;
using Core.Utility.Web.Base;
using FTT_API.Common;
using FTT_API.Common.ExtensionMethod;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models;
using FTT_API.Models.Handler;
using FTT_API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Graph.Models;
using System.Diagnostics;
using static Const.Enums;
using static FTT_API.Models.Handler.ControlLogHandler;

namespace FTT_API.Controllers
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

                var logAccount = "";
                string acc = headerValue_acc.ToString();
                string role = headerValue_userrole.ToString();
                string ivrCode = headerValue_ivrCode.ToString();
                string usertype = headerValue_usertype.ToString();
                // 可以存入 context.HttpContext.Items 給後續使用
                //context.HttpContext.Items["CustomHeader"] = value;

                bool logLoginStatus = false;
                bool boolIsAuthenticated = false;
                string errorMsg = string.Empty;
                SessionVO? sessionVO = null;
                bool checkUserAuthenticated = false;

                if (usertype == "RETAIL" || usertype == "EMPLOYEE")
                {
                    if (checkUserAuthenticated == true)
                    {
                        try
                        {
                            //if (true == adAuth.IsAuthenticated(adDomain, vm.AC, vm.PD))
                            //{
                            //    boolIsAuthenticated = true;
                            //}
                            //else
                            //{
                            //    errorMsg = "帳號或密碼輸入錯誤，請重新輸入！";
                            //}

                            //if (!IVR_Code.IsNullOrEmpty())
                            //{
                            //    Dictionary<string, object> condition = new Dictionary<string, object>()
                            //{
                            //    { "IVR_Code", IVR_Code },
                            //};
                            //    if (!base.CheckDataExist("STORE_PROFILE", condition))
                            //    {
                            //        errorMsg = "IVRCode輸入錯誤，請重新輸入！";
                            //        boolIsAuthenticated = false;
                            //    }
                            //}
                        }
                        catch (Exception ex)
                        {
                            //errorMsg = "Error authenticating. [" + adDomain + "]  : " + ex.Message;
                        }
                    }
                    else
                    {
                        boolIsAuthenticated = true;
                    }
                    //logAccount = vm.AC;

                    if (boolIsAuthenticated)
                    {
                        Employee emp = new Employee(checkUserAuthenticated, acc, "TEST", "FET", false, "FTT");
                        if (emp.hasData())
                        {
                            logLoginStatus = true;
                            sessionVO = new SessionVO
                            {
                                empno = emp.EmpNO,
                                empname = emp.EmployeeName,
                                engname = emp.EnglishName,
                                ext = emp.Mobile + "(" + emp.Ext + ")",
                                username = acc,
                                deptcode = emp.DeptCode,
                                usertype = role,
                                ivrcode = ivrCode.IsNullOrEmpty() ? "NULL" : ivrCode,
                                userrole = SystemModelClass.GetUserRole(emp.EmpNO)
                            };
                        }
                        else
                        {
                            errorMsg = $"該帳號[{acc}]不存在、人員已離職，或無權限使用";
                        }
                    }
                }
                else if (role == "VASS")
                {
                    BaseDBHandler _BaseDBHandler = new BaseDBHandler();
                    if (checkUserAuthenticated == true)
                    {
                        try
                        {
                            Dictionary<string, object> condition = new Dictionary<string, object>()
                            {
                                { "IVR_CODE", ivrCode },
                                { "SHOP_PASSWORD", "TEST" }
                            };

                            if (_BaseDBHandler.CheckDataExist("STORE_PROFILE", condition))
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
                            errorMsg = $"Error authenticating. [{ivrCode}] : " + ex.Message;
                        }
                    }
                    else
                    {
                        boolIsAuthenticated = true;
                    }

                    logAccount = ivrCode;
                    if (true == boolIsAuthenticated)
                    {
                        var info = GetStoreInfo(ivrCode, "TEST");
                        if (info != null)
                        {
                            logLoginStatus = true;
                            sessionVO = new SessionVO
                            {
                                empno = info.ivr_code,
                                empname = info.shop_name,
                                engname = info.shop_name,
                                ext = info.urgent_tel + "(" + info.owner_tel + ")",
                                username = ivrCode,
                                deptcode = info.area,
                                usertype = role,
                                ivrcode = ivrCode,
                                userrole = SystemModelClass.GetUserRole(info.ivr_code)
                            };
                            Method.SetToSession(sessionVO);
                        }
                        else
                        {
                            //errorMsg = $"該IVRCode[{IVR_Code}]不存在或密碼錯誤";
                        }
                    }
                }
                else if (role == "VENDOR")
                {

                }

                if (sessionVO != null)
                {
                    //sessionVO.Functions.Append(FuncID.Home_View);
                    Method.SetToSession(sessionVO);
                }
            }

            base.OnActionExecuting(context);
        }

        public StoreProfileVM GetStoreInfo(string ivr_code, string pd)
        {
            BaseDBHandler _BaseDBHandler = new BaseDBHandler();
            string sql = @"SELECT*
                           FROM STORE_PROFILE
                           WHERE IVR_CODE = @IVR_CODE AND SHOP_PASSWORD = @SHOP_PASSWORD";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "IVR_CODE", ivr_code },
                { "SHOP_PASSWORD", pd }
            };
            StoreProfileVM? result = _BaseDBHandler.GetDBHelper().Find<StoreProfileVM>(sql, parameters);
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
        [ApiExplorerSettings(IgnoreApi = true)]
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
        [ApiExplorerSettings(IgnoreApi = true)]
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
        [ApiExplorerSettings(IgnoreApi = true)]
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
        protected void LogError(string exception)
        {
            var entity = new controllogEntity()
            {
                IP = Method.GetClientIPAddress(),
                Status = ((int)LogStatusEnum.Failed).ToString(),
                ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
                ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
                Exception = exception
            };

            InsertLog(entity);
        }

        /// <summary>
        /// 紀錄成功訊息於資料庫
        /// </summary>
        /// <param name="description"></param>
        protected void LogSuccess(string description = null)
        {
            var entity = new controllogEntity()
            {
                IP = Method.GetClientIPAddress(),
                Status = ((int)LogStatusEnum.Success).ToString(),
                ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
                ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
                Exception = description,
                Account = LoginSession.Current?.username?? "",
                Name = LoginSession.Current?.empname ?? "",
                LogTime = DateTime.Now
            };

            InsertLog(entity);
        }

        protected void InsertLog(controllogEntity entity)
        {
            ControlLogHandler _BaseDBHandler = new ControlLogHandler();
            _BaseDBHandler.Insert(entity);
        }



    }


}
