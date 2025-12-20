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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using static Const.Enums;

namespace FTT_API.Controllers.Login
{
    [Route("[controller]")]
    public partial class LoginController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        // 使用 IMemoryCache 替代 static ConcurrentDictionary，避免重啟後資料遺失
        private readonly IMemoryCache _memoryCache;

        public LoginController(IWebHostEnvironment hostingEnvironment, ConfigurationHelper configHelper, IMemoryCache memoryCache)
        {
            _hostingEnvironment = hostingEnvironment;
            _configHelper = configHelper;
            _memoryCache = memoryCache;
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
                var safeToken = "";
                if (!string.IsNullOrEmpty(resultVO.Token.TokenId))
                {
                    safeToken = CookieSafeEncode(resultVO.Token.TokenId);
                    //Response.Cookies.Append(FTT_API.Common.Const.TOKEN_NAME, safeToken, new CookieOptions
                    //{
                    //    HttpOnly = true,   // 防止 JS 讀取
                    //    Secure = true,     // 只允許 HTTPS
                    //    SameSite = SameSiteMode.None, // 防止 CSRF
                    //});
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

                userLoginName = SanitizeCookieValue(userLoginName);
                var userrole = SanitizeCookieValue(sessionVO?.userrole);

                // 假設 userLoginName 是從 vm 取得的使用者輸入
                string safeUserLoginName = string.IsNullOrEmpty(userLoginName) ? string.Empty : Uri.EscapeDataString(userLoginName); // 將特殊字符編碼

                //Response.Cookies.Append(FTT_API.Common.Const.USER_LOGIN_NAME, safeUserLoginName ?? string.Empty, new CookieOptions
                //{
                //    HttpOnly = true,   // 防止 JS 讀取
                //    Secure = true,     // 只允許 HTTPS
                //    SameSite = SameSiteMode.None, // 防止 CSRF
                //});
                Response.Cookies.Append(FTT_API.Common.Const.USER_ROLE, userrole ?? string.Empty, new CookieOptions
                {
                    HttpOnly = true,   // 防止 JS 讀取
                    Secure = true,     // 只允許 HTTPS
                    SameSite = SameSiteMode.None, // 防止 CSRF
                });
                _sessionVO = sessionVO ?? new();

                this.LogSuccess("login/login---結束");
                this.LogSuccess("登入成功");
                return JsonSuccess(new { FTT_Token = safeToken, FTT_userLoginName = userLoginName });
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                //20251208 mark return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));

                //20251208 Add begin
                // 為了快速除錯，臨時將例外訊息回傳給前端 (注意安全性，除錯完畢後移除)
#if DEBUG // 假設您在 Production 環境沒有使用 DEBUG
                return JsonValidFail(ex.Message + " | Trace: " + ex.StackTrace);
#else
                    return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg")); // 恢復為通用錯誤
#endif
                //20251208 Add end

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
            TempData[CaptchaCodeHelper_ImageSharp.CAPTCHA_CODE] = result.ResultCode;

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
                // 安全清理
                string logUserType = "";
                string logAccount = "";
                string RETAILID = Sanitize(vm.RETAILID);
                string IVRCODE = Sanitize(vm.IVRCODE);
                string EMPNO = Sanitize(vm.EMPNO);
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
                // 1. 完整安全化：消毒 + URL 編碼（Checkmarx 可辨識）
                //var safeToken = CookieSafeEncode(token.TokenId);
                //Response.Cookies.Append(FTT_API.Common.Const.TOKEN_NAME, safeToken, new CookieOptions
                //{
                //    HttpOnly = true,   // 防止 JS 讀取
                //    Secure = true,     // 只允許 HTTPS
                //    SameSite = SameSiteMode.None, // 防止 CSRF
                //});
            }

            // --- User Name Cookie ---
            string userLoginName = GetSafeUserLoginName(sessionVO);

            // --- User Role Cookie ---
            var userRoleSafe = CookieSafeEncode(sessionVO?.userrole);

            //Response.Cookies.Append(FTT_API.Common.Const.USER_LOGIN_NAME, userLoginName ?? string.Empty, new CookieOptions
            //{
            //    HttpOnly = true,   // 防止 JS 讀取
            //    Secure = true,     // 只允許 HTTPS
            //    SameSite = SameSiteMode.None, // 防止 CSRF
            //});
            Response.Cookies.Append(FTT_API.Common.Const.USER_ROLE, userRoleSafe ?? string.Empty, new CookieOptions
            {
                HttpOnly = true,   // 防止 JS 讀取
                Secure = true,     // 只允許 HTTPS
                SameSite = SameSiteMode.None, // 防止 CSRF
            });

        }

        // --- 安全 API（Checkmarx/ Fortify 能辨識） ---
        private string CookieSafeEncode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.Trim();

            // 第一層：清除注入字元（Checkmarx 看得見）
            value = Regex.Replace(value, @"[<>""'`;\\]", "");

            // 第二層：UrlEncode → 防止 cookie injection
            return HttpUtility.UrlEncode(value);
        }

        private string GetSafeUserLoginName(SessionVO sessionVO)
        {
            if (sessionVO == null) return string.Empty;

            string userType = sessionVO.usertype;
            string empno = sessionVO.empno;
            string name = string.Empty;

            switch (userType)
            {
                case "RETAIL":
                case "EMPLOYEE":
                    using (CEDS ceds = new CEDS())
                    {
                        name = ceds.GetEmpName(Employee.RefType.EmpNo, empno);
                        if (userType == "RETAIL")
                            name += $"IVR Code：{sessionVO.ivrcode} ";
                    }
                    break;
                case "VASS":
                    name = $"IVR Code - {sessionVO.ivrcode} ";
                    break;
                case "VENDOR":
                    name = sessionVO.engname;
                    break;
            }

            return name ?? string.Empty;
        }
        private string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            return input
                .Replace("<", "")
                .Replace(">", "")
                .Replace("\"", "")
                .Replace("'", "")
                .Trim();
        }

        private string SanitizeCookieValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            // 移除 CR, LF, 空字元
            return Regex.Replace(value, @"[\r\n]", string.Empty);
        }

        public class CaptchaVerifyRequest
        {
            public string CaptchaId { get; set; }
            public string CaptchaCode { get; set; }
        }

        // 測試用 API - 檢查快取狀態

        [HttpGet("[action]")]
        [AllowAnonymous] // 允許匿名訪問，用於除錯
        public IActionResult TestCache()
        {
            try
            {
                var testKey = "test_key";
                var testValue = "test_value_" + DateTime.Now.ToString("HHmmss");

                // 測試寫入
                _memoryCache.Set(testKey, testValue, TimeSpan.FromMinutes(1));
                Console.WriteLine($"[TestCache] Set: {testKey} = {testValue}");

                // 測試讀取
                var retrieved = _memoryCache.TryGetValue(testKey, out var value);
                Console.WriteLine($"[TestCache] Get: Found={retrieved}, Value={value}");

                // 清理
                _memoryCache.Remove(testKey);

                return JsonSuccess(new
                {
                    platform = Environment.OSVersion.Platform.ToString(),
                    machine = Environment.MachineName,
                    set = testValue,
                    retrieved = retrieved,
                    value = value?.ToString()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TestCache] Exception: {ex}");
                return JsonValidFail($"Cache test failed: {ex.Message}");
            }
        }

        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous] // 允許匿名訪問，登入前需要使用
        public IActionResult GetCaptcha()
        {
            var code = GenerateCode(4); // 4位隨機碼
            var id = Guid.NewGuid().ToString();

            // 使用 MemoryCache 存儲驗證碼，設定 5 分鐘過期時間
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                Priority = CacheItemPriority.Normal
            };

            var cacheKey = $"captcha_{id}";
            _memoryCache.Set(cacheKey, code, cacheOptions);

            // 詳細日誌記錄
            Console.WriteLine($"[GetCaptcha] Generated: ID={id}, Code={code}, Key={cacheKey}");
            Console.WriteLine($"[GetCaptcha] Platform: {Environment.OSVersion.Platform}");
            Console.WriteLine($"[GetCaptcha] Machine: {Environment.MachineName}");

            // 立即驗證快取是否存在
            var testResult = _memoryCache.TryGetValue(cacheKey, out var testCode);
            Console.WriteLine($"[GetCaptcha] Cache test: Found={testResult}, Value={testCode}");

            var imageBytes = GenerateCaptchaImage(code);

            return JsonSuccess(new
            {
                captchaId = id,
                imageBase64 = "data:image/png;base64," + Convert.ToBase64String(imageBytes)
            });
        }

        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous] // 允許匿名訪問，登入前需要使用
        public IActionResult VerifyCaptcha(CaptchaVerifyRequest request)
        {
            // 詳細日誌記錄
            Console.WriteLine($"[VerifyCaptcha] === 開始驗證 ===");
            Console.WriteLine($"[VerifyCaptcha] Platform: {Environment.OSVersion.Platform}");
            Console.WriteLine($"[VerifyCaptcha] Request: ID={request?.CaptchaId ?? "NULL"}, Code={request?.CaptchaCode ?? "NULL"}");

            if (string.IsNullOrEmpty(request?.CaptchaId) || string.IsNullOrEmpty(request?.CaptchaCode))
            {
                Console.WriteLine("[VerifyCaptcha] Error: Empty captcha ID or code");
                return JsonValidFail("驗證碼不能為空");
            }

            var cacheKey = $"captcha_{request.CaptchaId}";
            Console.WriteLine($"[VerifyCaptcha] Looking for key: {cacheKey}");

            // 檢查快取是否存在（不移除）
            if (_memoryCache.TryGetValue(cacheKey, out var code))
            {
                Console.WriteLine($"[VerifyCaptcha] Found in cache: '{code}'");

                var expectedCode = code.ToString();
                var inputCode = request.CaptchaCode;

                Console.WriteLine($"[VerifyCaptcha] Comparing (case-insensitive):");
                Console.WriteLine($"  Expected: '{expectedCode}' (length={expectedCode.Length})");
                Console.WriteLine($"  Input:    '{inputCode}' (length={inputCode.Length})");

                // 移除已使用的驗證碼
                _memoryCache.Remove(cacheKey);
                Console.WriteLine($"[VerifyCaptcha] Removed cache key: {cacheKey}");

                if (string.Equals(expectedCode, inputCode, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("[VerifyCaptcha] SUCCESS: Captcha verified!");
                    return JsonSuccess("");
                }
                else
                {
                    Console.WriteLine("[VerifyCaptcha] FAIL: Code mismatch");
                    // 檢查每個字符
                    for (int i = 0; i < Math.Max(expectedCode.Length, inputCode.Length); i++)
                    {
                        var expectedChar = i < expectedCode.Length ? expectedCode[i] : '?';
                        var inputChar = i < inputCode.Length ? inputCode[i] : '?';
                        Console.WriteLine($"  [{i}] Expected='{expectedChar}'({(int)expectedChar}) Input='{inputChar}'({(int)inputChar})");
                    }
                }
            }
            else
            {
                Console.WriteLine($"[VerifyCaptcha] NOT FOUND in cache for key: {cacheKey}");

                // 嘗試列出所有快取項目
                Console.WriteLine("[VerifyCaptcha] Attempting to list cache contents...");
                try
                {
                    var field = _memoryCache.GetType().GetField("_store", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null)
                    {
                        Console.WriteLine("[VerifyCaptcha] Cache store exists");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VerifyCaptcha] Cache inspection failed: {ex.Message}");
                }
            }

            Console.WriteLine("[VerifyCaptcha] === 驗證失敗 ===");
            return JsonValidFail("驗證碼錯誤或過期");
        }

        private string GenerateCode(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789abcdefghjklmnpqrstuvwxyz";
            var random = new Random();
            var code = new char[length];
            for (int i = 0; i < length; i++)
                code[i] = chars[random.Next(chars.Length)];
            return new string(code);
        }

        private byte[] GenerateCaptchaImage(string code)
        {
            int width = 100;
            int height = 40;

            using var image = new Image<Rgba32>(width, height);
            image.Mutate(ctx =>
            {
                ctx.Fill(Color.White);

                // 嘗試使用多種字型作為備選方案
                SixLabors.Fonts.Font font;
                var fontNames = new[] { "Arial", "Liberation Sans", "DejaVu Sans", "sans-serif" };

                font = null;
                foreach (var fontName in fontNames)
                {
                    try
                    {
                        font = SystemFonts.CreateFont(fontName, 30, FontStyle.Bold);
                        break;
                    }
                    catch (FontFamilyNotFoundException)
                    {
                        if (fontName == fontNames.Last())
                        {
                            // 如果都找不到，使用系統預設字型
                            var families = SystemFonts.Families;
                            if (!families.Any())
                                throw new InvalidOperationException("系統無可用字型");

                            var defaultFamily = families.First();
                            font = SystemFonts.CreateFont(defaultFamily.Name, 30, FontStyle.Bold);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                // 在圖片上畫文字
                ctx.DrawText(code, font, Color.Black, new PointF(10, 5));

                // 加入簡單干擾線
                var random = new Random();
                for (int i = 0; i < 3; i++)
                {
                    ctx.DrawLine(Color.Gray, 1,
                        new PointF[]
                        {
                    new PointF(random.Next(width), random.Next(height)),
                    new PointF(random.Next(width), random.Next(height))
                        });
                }
            });

            using var ms = new MemoryStream();
            image.Save(ms, new PngEncoder());
            return ms.ToArray();
        }
    }
}
