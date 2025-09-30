using Const;
using Core.Utility.Helper.Message;
using Core.Utility.Web.Base;
using FEE_VENDER_API.Common;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.OriginClass;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models;
using FTT_VENDER_API.Models.Handler;
using FTT_VENDER_API.Models.ViewModel.StoreVenderProfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static Const.Enums;

namespace FTT_VENDER_API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseProjectController : BaseController
    {
        private readonly object _lock = new object();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            context.HttpContext.Request.Cookies.TryGetValue("Token", out string? token);
            context.HttpContext.Request.Headers.TryGetValue("Content-From", out var from);
            if (!string.IsNullOrEmpty(token) && from != "Logout")
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
                        Response.Cookies.Append("Token", resultVO.TokenInfoVO.TokenId, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.None,
                        });
                    }
                    session = sessionRes;
                }

                bool logLoginStatus = false;
                string headerValue_acc = session.username.ToString();
                string role = session.userrole.ToString();
                string ivrCode = session.ivrcode.ToString();
                string headerValue_usertype = session.usertype.ToString();
                bool boolIsAuthenticated = false;
                string logAccount = headerValue_acc;
                //string logFromIP = _httpContext.Connection.RemoteIpAddress?.ToString();
                string logUserType = headerValue_usertype;
                bool checkUserAuthenticated = false;

                string adDomain = Method.GetAppSettingsDataByName("FETADServer");
                //LdapAuthentication adAuth = new LdapAuthentication(adDomain);

                string errorMsg = string.Empty;
                SessionVO? sessionVO = null;

                if (headerValue_usertype == "VENDOR")
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
                                    sessionVO.userrole = SystemModelClass.GetUserRole(sessionVO.empno ?? string.Empty, sessionVO);
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
                        sessionVO.userrole = SystemModelClass.GetUserRole(sessionVO.empno ?? string.Empty, sessionVO);
                    }
                }

                if (sessionVO != null)
                {
                    //sessionVO.Functions.AddRange(RoleFunc.Vender);
                    //Method.SetToSession(sessionVO);
                    _sessionVO = sessionVO;
                }
            }
            else
            {

                if (from != "Login" && from != "Logout")
                {
                    context.Result = new UnauthorizedObjectResult(JsonValiFail("無權限進入"));
                    return;
                }

            }

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
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
        /// 
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public void RefreshToken(TokenInfoVO tokenInfoVO, string oldToken)
        {
            BaseDBHandler _BaseDBHandler = new BaseDBHandler();
            string sql = $"UPDATE tb_token SET Status = {(int)StatusEnum.Cancel},UpdateTime = @NOW WHERE tokenid = @TokenId";
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
                            { "Status",(int)StatusEnum.Enabled }
                        };
            sql = @"INSERT INTO TB_Token(
	TokenId, RegisterDate, VoidStartTime, VoidEndTime, Account, CreateTime, UpdateTime, Status)
	VALUES (@TokenId, @RegisterDate,@VoidStartTime,@VoidEndTime, @Account, @NOW, @NOW, @Status);";
            _BaseDBHandler.GetDBHelper().Execute(sql, parameters);
            _BaseDBHandler.GetDBHelper().Commit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public TokenInfoVO? GetTokenInfo(string token)
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
        /// <returns>TB_Control_Log.Id</returns>
        //protected long LogError(Exception ex)
        //{
        //    var blLog = BLFactory.GetInstance<LogBL>();

        //    var logDM = new TB_Control_LogDM()
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
        /// <returns>TB_Control_Log.Id</returns>
        protected void LogError(string exception)
        {
            var entity = new TB_Control_LogEntity()
            {
                IP = Method.GetClientIPAddress(),
                Status = ((int)LogStatusEnum.Failed).ToString(),
                ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
                ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
                Exception = exception,
                Token = Request.Cookies["Token"] ?? string.Empty,
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
                Token = Request.Cookies["Token"] ?? string.Empty,
            };

            InsertLog(entity);
        }

        protected void InsertLog(TB_Control_LogEntity entity)
        {
            TB_Control_LogHandler _BaseDBHandler = new TB_Control_LogHandler();
            _BaseDBHandler.Insert(entity);
        }
    }
}
