using Const;
using Const.VO;
using Core.Utility.Helper.CaptchaCode;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Presentation;
using FTT_API.Common;
using FTT_API.Common.Attribute;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Common.OriginClass.ModelClass;
using FTT_API.Models.Handler;
using FTT_API.Models.ViewModel.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using System.Data;
using static Const.Enums;

namespace FTT_API.Controllers.Login
{
    [Route("[controller]")]
    public partial class LoginController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        public LoginController(IWebHostEnvironment hostingEnvironment, ConfigurationHelper configHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _configHelper = configHelper;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login(LoginVM vm)
        {
            try
            {
                this.LogSuccess("login/login---開始");

                var loginHanlder = new LoginHandler(_configHelper, HttpContext);

                this.LogSuccess("login/login---開始登入驗證");
                (LoginResultVO resultVO, SessionVO? sessionVO) = loginHanlder.Login(vm);
                this.LogSuccess("login/login---結束登入驗證");

                if (!string.IsNullOrEmpty(resultVO.ErrorMsg))
                {
                    this.LogSuccess();
                    return JsonValidFail(resultVO.ErrorMsg);
                }

                if (!string.IsNullOrEmpty(resultVO.Token.TokenId))
                {
                    Response.Cookies.Append(FTT_API.Common.Const.TOKEN_NAME, resultVO.Token.TokenId, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = false, // HTTP測試用false https用true
                        SameSite = SameSiteMode.Lax, // http測試用Lax https用none
                    });
                }

                string userLoginName = string.Empty;
                if (sessionVO != null)
                {
                    string userType = sessionVO.usertype;

                    switch (userType)
                    {
                        case "RETAIL":
                        case "EMPLOYEE":
                            CEDS ceds = new CEDS();
                            userLoginName = ceds.GetEmpName(Employee.RefType.EmpNo, sessionVO.empno);
                            if (userType == "RETAIL")
                                userLoginName += $"IVR Code：{sessionVO.ivrcode} ";
                            ceds.Dispose();
                            break;
                        case "VASS":
                            userLoginName = $"IVR Code - {sessionVO.ivrcode} ";
                            break;
                        case "VENDOR":
                            userLoginName = sessionVO.engname;
                            break;
                        default:
                            break;
                    }
                }
                Response.Cookies.Append(FTT_API.Common.Const.USER_LOGIN_NAME, userLoginName ?? string.Empty, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // HTTP測試用false https用true
                    SameSite = SameSiteMode.Lax, // http測試用Lax https用none
                });
                Response.Cookies.Append(FTT_API.Common.Const.USER_ROLE, sessionVO?.userrole ?? string.Empty, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // HTTP測試用false https用true
                    SameSite = SameSiteMode.Lax, // http測試用Lax https用none
                });
                _sessionVO = sessionVO ?? new();

                this.LogSuccess("login/login---結束");
                this.LogSuccess("登入成功");
                return JsonOK();
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }



        /// <summary>
        /// 畫出 圖形驗證碼
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public ActionResult CaptchaCode()
        {
            //自製的土炮驗證碼
            CaptchaCodeHelper_ImageSharp captchaCode = new()
            {
                Width = 100
            };

            CaptchaResult result = captchaCode.Result();
            TempData[CaptchaCodeHelper.CAPTCHA_CODE] = result.ResultCode;

            this.LogSuccess();
            return File(result.CaptchaImage, "image/jpeg");
        }

        //[CustomAuthorization(FuncID.Home_View)]
        [HttpGet("[action]")]
        public ActionResult CheckLogin()
        {
            if (LoginSession.Current.empno != null)
            {
                this.LogSuccess();
                return JsonOK();
            }
            this.LogSuccess();
            return JsonValidFail("逾時");
        }

        public class SSOVM
        {
            public string IVRCODE { get; set; }
            public string EMPNO { get; set; }
            public string RETAILID { get; set; }
        }

        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult CheckSSO(SSOVM vm)
        {
            try
            {
                string logUserType = "";
                string logAccount = "";
                string RETAILID = vm.RETAILID;
                string IVRCODE = vm.IVRCODE;
                string EMPNO = vm.EMPNO;
                bool logLoginStatus = false;

                var role = SystemModelClass.GetUserRole(EMPNO, new SessionVO());
                SessionVO sessionVO;
                TokenInfoVO token;
                JwtConfigVO jwtConfigVO = new();

                if (RETAILID != "" && EMPNO != "" && RETAILID != EMPNO)
                {
                    logAccount = EMPNO;
                    logUserType = "SP-RETAIL";
                    Employee emp = new Employee(Employee.RefType.EmpNo, EMPNO, new RetrieveEmpWithoutFETIData());
                    if (emp.hasData() == true)
                    {
                        //JWT
                        sessionVO = new SessionVO
                        {
                            empno = EMPNO,
                            empname = emp.EmployeeName,
                            engname = emp.EnglishName,
                            ext = emp.Mobile + "(" + emp.Ext + ")",
                            username = emp.AliasName,
                            deptcode = emp.DeptCode,
                            usertype = "RETAIL",
                            ivrcode = RETAILID,
                        };

                        SetJWT(sessionVO);
                        //Session["ISLOGIN"] = "true";
                        //logLoginStatus = true;
                        //Session["empno"] = emp.EmpNO;
                        //Session["empname"] = emp.EmployeeName;
                        //Session["engname"] = emp.EnglishName;
                        //Session["ext"] = emp.Mobile + "(" + emp.Ext + ")";
                        //Session["username"] = emp.AliasName;
                        //Session["deptcode"] = emp.DeptCode;
                        //Session["usertype"] = "RETAIL";
                        //Session["ivrcode"] = RETAILID;

                        this.LogSuccess();
                        return JsonSuccess("");
                    }
                    else
                    {
                        this.LogError("該帳號[" + EMPNO + "]不存在、人員已離職，或無權限使用。");
                        return JsonValidFail("該帳號[" + EMPNO + "]不存在、人員已離職，或無權限使用。");
                    }
                }
                else if (IVRCODE != "" || EMPNO == "")
                {
                    BaseDBHandler baseHandler = new BaseDBHandler();
                    Dictionary<string, object> paras = new Dictionary<string, object>();
                    paras.Add("IVRCODE", IVRCODE);

                    logAccount = IVRCODE;
                    logUserType = "SP-VASS";
                    DataTable vassData = baseHandler.GetDBHelper().FindDataTable("SELECT * FROM STORE_PROFILE WHERE IVR_CODE=@IVRCODE ", paras);
                    if (vassData.Rows.Count > 0)
                    {
                        //JWT
                        sessionVO = new SessionVO
                        {
                            empno = IVRCODE,
                            empname = vassData.Rows[0]["SHOP_NAME"].ToString(),
                            engname = vassData.Rows[0]["SHOP_NAME"].ToString(),
                            ext = vassData.Rows[0]["URGENT_TEL"].ToString() + "(" + vassData.Rows[0]["OWNER_TEL"].ToString() + ")",
                            username = IVRCODE,
                            deptcode = vassData.Rows[0]["AREA"].ToString(),
                            usertype = "VASS",
                            ivrcode = IVRCODE,
                        };
                        SetJWT(sessionVO);
                        //Session["ISLOGIN"] = "true";
                        //logLoginStatus = true;
                        //Session["empno"] = IVRCODE;
                        //Session["empname"] = vassData.Rows[0]["SHOP_NAME"].ToString();
                        //Session["engname"] = vassData.Rows[0]["SHOP_NAME"].ToString();
                        //Session["ext"] = vassData.Rows[0]["URGENT_TEL"].ToString() + "(" + vassData.Rows[0]["OWNER_TEL"].ToString() + ")";
                        //Session["username"] = IVRCODE;
                        //Session["deptcode"] = vassData.Rows[0]["AREA"].ToString();
                        //Session["usertype"] = "VASS";
                        //Session["ivrcode"] = IVRCODE;

                        try
                        {
                            this.LogSuccess();
                            return JsonSuccess("");
                        }
                        catch (Exception ex)
                        {
                            this.LogError(ex.ToString());
                            return JsonValidFail("請由首頁連結!");
                        }
                    }
                    else
                    {
                        this.LogError("門市[" + IVRCODE + "]尚未完成工程收驗無法報修!<br />加盟門市報修請聯絡 Tsai, Eric 蔡東良 (15793)。");
                        return JsonValidFail("門市[" + IVRCODE + "]尚未完成工程收驗無法報修!<br />加盟門市報修請聯絡 Tsai, Eric 蔡東良 (15793)。");
                    }
                }
                else if (IVRCODE == "" && RETAILID == "" && EMPNO != "")
                {
                    BaseDBHandler baseHandler = new BaseDBHandler();
                    Dictionary<string, object> paras = new Dictionary<string, object>();
                    paras.Add("EMPNO", EMPNO);

                    logAccount = EMPNO;
                    logUserType = "SP-EMP";
                    DataTable vassData = baseHandler.GetDBHelper().FindDataTable("select * from fet_user_profile where empno in (select distinct as_empno as empno from store_profile union select distinct empno from ftt_group) and EMPNO=@EMPNO ", paras);
                    if (vassData.Rows.Count > 0)
                    {
                        //JWT
                        sessionVO = new SessionVO
                        {
                            empno = EMPNO,
                            empname = vassData.Rows[0]["EMPNAME"].ToString(),
                            engname = vassData.Rows[0]["ENGNAME"].ToString(),
                            ext = vassData.Rows[0]["EXT"].ToString(),
                            username = vassData.Rows[0]["EXT"].ToString(),
                            deptcode = vassData.Rows[0]["ALIASNAME"].ToString(),
                            usertype = "EMPLOYEE",
                            ivrcode = "NULL",
                        };
                        SetJWT(sessionVO);
                        //Session["ISLOGIN"] = "true";
                        //logLoginStatus = true;
                        //Session["empno"] = EMPNO;
                        //Session["empname"] = vassData.Rows[0]["EMPNAME"].ToString();
                        //Session["engname"] = vassData.Rows[0]["ENGNAME"].ToString();
                        //Session["ext"] = vassData.Rows[0]["EXT"].ToString();
                        //Session["username"] = vassData.Rows[0]["EXT"].ToString();
                        //Session["deptcode"] = vassData.Rows[0]["ALIASNAME"].ToString();
                        //Session["usertype"] = "EMPLOYEE";
                        //Session["ivrcode"] = "NULL";

                        try
                        {
                            this.LogSuccess();
                            return JsonSuccess("");
                        }
                        catch (Exception ex)
                        {
                            this.LogError(ex.ToString());
                            return JsonValidFail("請由首頁連結!");
                        }
                    }
                    else
                    {
                        this.LogError("員工編號[" + EMPNO + "]無權限使用。");
                        return JsonValidFail("員工編號[" + EMPNO + "]無權限使用。");
                    }
                }
            }
            catch (Exception exxx)
            {
                this.LogError(exxx.ToString());
            }

            this.LogError("無法預期的登入情境");
            return JsonValidFail("無法預期的登入情境");
        }

        private void SetJWT(SessionVO sessionVO)
        {
            JwtConfigVO jwtConfigVO = new();
            TokenInfoVO token;

            sessionVO.userrole = SystemModelClass.GetUserRole(sessionVO.empno, sessionVO);
            token = Method.GenerateJwtToken(sessionVO, jwtConfigVO);

            if (!string.IsNullOrEmpty(token.TokenId))
            {
                Response.Cookies.Append(FTT_API.Common.Const.TOKEN_NAME, token.TokenId, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // HTTP測試用false https用true
                    SameSite = SameSiteMode.Lax, // http測試用Lax https用none
                });
            }

            string userLoginName = string.Empty;
            if (sessionVO != null)
            {
                string userType = sessionVO.usertype;

                switch (userType)
                {
                    case "RETAIL":
                    case "EMPLOYEE":
                        CEDS ceds = new CEDS();
                        userLoginName = ceds.GetEmpName(Employee.RefType.EmpNo, sessionVO.empno);
                        if (userType == "RETAIL")
                            userLoginName += $"IVR Code：{sessionVO.ivrcode} ";
                        ceds.Dispose();
                        break;
                    case "VASS":
                        userLoginName = $"IVR Code - {sessionVO.ivrcode} ";
                        break;
                    case "VENDOR":
                        userLoginName = sessionVO.engname;
                        break;
                    default:
                        break;
                }
            }
            Response.Cookies.Append(FTT_API.Common.Const.USER_LOGIN_NAME, userLoginName ?? string.Empty, new CookieOptions
            {
                HttpOnly = false,
                Secure = false, // HTTP測試用false https用true
                SameSite = SameSiteMode.Lax, // http測試用Lax https用none
            });
            Response.Cookies.Append(FTT_API.Common.Const.USER_ROLE, sessionVO?.userrole ?? string.Empty, new CookieOptions
            {
                HttpOnly = false,
                Secure = false, // HTTP測試用false https用true
                SameSite = SameSiteMode.Lax, // http測試用Lax https用none
            });

        }
    }
}
