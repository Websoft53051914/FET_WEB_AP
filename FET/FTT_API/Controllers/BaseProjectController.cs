using Const;
using Core.Utility.Extensions;
using Core.Utility.Helper.Message;
using Core.Utility.Web.Base;
using FTT_API.Common;
using FTT_API.Common.Attribute;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models;
using FTT_API.Models.Handler;
using FTT_API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text.RegularExpressions;
using static Const.Enums;

namespace FTT_API.Controllers
{
    [AntiforgeryTokenCookieAttribute]
    public class BaseProjectController : BaseController
    {
        private readonly object _lock = new object();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            context.HttpContext.Request.Cookies.TryGetValue(FTT_API.Common.Const.TOKEN_NAME, out string? token);

            // 1. 嚴格過濾非法字元與 CRLF，防止注入
            // 僅允許 JWT 常用的 Base64Url 字元
            if (!string.IsNullOrEmpty(token)&&!Regex.IsMatch(token, @"^[A-Za-z0-9\-_\.]+$"))
                    token = "";

            context.HttpContext.Request.Headers.TryGetValue("Content-From", out var from);

            // 預防 Header 注入：過濾從 Header 拿到的 'from' 變數
            string safeFrom = from.ToString().Replace("\r", "").Replace("\n", "");

            if (!string.IsNullOrEmpty(token) && safeFrom != "Logout")
            {
                SessionVO? session = null;
                lock (_lock)
                {
                    var tokenInfoEntity = GetTokenInfo(token);
                    JwtConfigVO jwtConfig = new JwtConfigVO();
                    var (resultVO, sessionRes) = Method.VerifyAndGenerateJwtToken(token, jwtConfig);
                    if (resultVO.IsExpired && tokenInfoEntity != null && tokenInfoEntity.Status != (int)StatusEnum.Cancel)
                    {
                        RefreshToken(resultVO.TokenInfoVO, token);

                        // 2. 在寫入 Cookie 前，對 TokenId 再次進行消毒 (Sanitize)
                        string safeTokenId = resultVO.TokenInfoVO.TokenId.Replace("\r", "").Replace("\n", "");

                        // 確保 TokenId 符合預期格式才寫入
                        if (!string.IsNullOrEmpty(safeTokenId) && Regex.IsMatch(safeTokenId, @"^[A-Za-z0-9\-_\.]+$"))
                        {
                            Response.Cookies.Append(FTT_API.Common.Const.TOKEN_NAME, safeTokenId, new CookieOptions
                            {
                                HttpOnly = false,
                                Secure = true,     // 只允許 HTTPS
                                SameSite = SameSiteMode.None,
                                Path = "/"         // 建議明確設定 Path
                            });
                        }
                    }

                    session = sessionRes;
                }

                var logAccount = "";
                string acc = session.username.ToString();
                string role = session.userrole.ToString();
                string ivrCode = session.ivrcode.ToString();
                string usertype = session.usertype.ToString();
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
                                ivrcode = string.IsNullOrEmpty(ivrCode) ? "NULL" : ivrCode,
                            };
                            sessionVO.userrole = SystemModelClass.GetUserRole(sessionVO.empno, sessionVO);
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
                            };
                            sessionVO.userrole = SystemModelClass.GetUserRole(sessionVO.empno ?? string.Empty, sessionVO);
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
                    //Method.SetToSession(sessionVO);
                    _sessionVO = sessionVO;
                }
            }
            else
            {

                if (context.RouteData.Values["action"] != null && (context.RouteData.Values["action"].ToString() == "GetCaptcha" || context.RouteData.Values["action"].ToString() == "VerifyCaptcha"))
                {

                }
                else if (from != "Login" && from != "Logout" && from != "CheckSSO")
                {
                    context.Result = new UnauthorizedObjectResult(JsonValiFail("無權限進入"));
                    return;
                }

            }

            base.OnActionExecuting(context);
        }

        protected StoreProfileVM GetStoreInfo(string ivr_code, string pd)
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

        protected void RefreshToken(TokenInfoVO tokenInfoVO, string oldToken)
        {
            BaseDBHandler _BaseDBHandler = new BaseDBHandler();
            string sql = $"UPDATE tb_token SET Status = {StatusEnum.Cancel.ToInt()},UpdateTime = @NOW WHERE tokenid = @TokenId";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "TokenId", oldToken },
                { "NOW", DateTime.Now }
            };
            _BaseDBHandler.GetDBHelper().Execute(sql, parameters);
            parameters = new Dictionary<string, object>()
                        {
                            { "TokenId", tokenInfoVO.TokenId },
                            { "RegisterDate", tokenInfoVO.Iat },
                            { "VoidStartTime", tokenInfoVO.Nbf },
                            { "VoidEndTime", tokenInfoVO.Exp },
                            { "Account",tokenInfoVO.LogAccount },
                            { "NOW", DateTime.Now },
                            { "Status", StatusEnum.Enabled.ToInt() }
                        };
            sql = @"INSERT INTO TB_Token(
	TokenId, RegisterDate, VoidStartTime, VoidEndTime, Account, CreateTime, UpdateTime, Status)
	VALUES (@TokenId, @RegisterDate,@VoidStartTime,@VoidEndTime, @Account, @NOW, @NOW, @Status);";
            _BaseDBHandler.GetDBHelper().Execute(sql, parameters);
            _BaseDBHandler.GetDBHelper().Commit();
        }

        protected TokenInfoVO? GetTokenInfo(string token)
        {
            string sql = "SELECT * FROM tb_token WHERE tokenid = @tokenid";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "tokenid", token }
            };
            BaseDBHandler _BaseDBHandler = new BaseDBHandler();
            TokenInfoVO? result = _BaseDBHandler.GetDBHelper().Find<TokenInfoVO>(sql, parameters);
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

        /// <summary>
        /// 紀錄例外於資料庫
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Control_Log.Id</returns>
        //protected long LogError(Exception ex)
        //{
        //    var blLog = BLFactory.GetInstance<LogBL>();

        //    var logDM = new Control_LogDM()
        //    {
        //        IP = _sessionVO.IP ?? Method.GetClientIPAddress(),
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
        /// <returns>Control_Log.Id</returns>
        protected void LogError(string exception)
        {
            var entity = new TB_Control_LogEntity()
            {
                IP = Method.GetClientIPAddress(),
                Status = ((int)LogStatusEnum.Failed).ToString(),
                ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
                ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
                Exception = exception,
                Token = Request.Cookies[FTT_API.Common.Const.TOKEN_NAME] ?? string.Empty,

                Account = _sessionVO?.username ?? "",
                Name = _sessionVO?.empname ?? "",
                LogTime = DateTime.Now,
            };

            InsertLog(entity);
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
                ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
                ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
                Exception = description,
                Account = _sessionVO?.username ?? "",
                Name = _sessionVO?.empname ?? "",
                LogTime = DateTime.Now,
                Token = Request.Cookies[FTT_API.Common.Const.TOKEN_NAME] ?? string.Empty,
            };

            InsertLog(entity);
        }

        protected void InsertLog(TB_Control_LogEntity entity)
        {
            TB_Control_LogHandler _BaseDBHandler = new TB_Control_LogHandler();
            _BaseDBHandler.Insert(entity);
        }

        private bool IsSafeToken(string token)
        {
            return !string.IsNullOrEmpty(token)
                   && token.Length <= 256
                   && Regex.IsMatch(token, @"^[A-Za-z0-9\-_\.]+$");
        }
    }
}
