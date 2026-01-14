using Core.Utility.Extensions;
using Core.Utility.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FTT_VENDER_API.Models.ViewModel;
using static Const.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Const.VO;
using FEE_VENDER_API.Common;
using Const;
using FTT_VENDER_API.Models.Handler;
using static FTT_VENDER_API.Models.Handler.MailPoolHandler;

namespace FTT_VENDER_API.Common
{
    public partial class Method
    {

        public static string SendMailByGmail(string mailSubject, string mailContent, string userEmail)
        {
            try
            {

                // Google 發信帳號密碼
                string mailUserID = Method.GetAppSettingsDataByName("MailUserID");
                string mailUserPwd = Method.GetAppSettingsDataByName("MailUserPwd");
                string smtpServer = Method.GetAppSettingsDataByName("SmtpServer");
                string smtpPort = Method.GetAppSettingsDataByName("SmtpPort");
                string enableSsl = Method.GetAppSettingsDataByName("EnableSsl");
                int intSmtpPort = int.Parse(smtpPort);
                if (string.IsNullOrEmpty(mailUserID)
                    || string.IsNullOrEmpty(mailUserPwd)
                    || string.IsNullOrEmpty(smtpServer)
                    || string.IsNullOrEmpty(smtpPort)
                    )
                {
                    return "MAIL SERVER相關帳號密碼未設定，請洽詢管理員";
                }

                // 使用 Google Mail Server 發信
                //string SmtpServer = "smtp.gmail.com";
                //int SmtpPort = 587;
                MailMessage mms = new();
                mms.From = new MailAddress(mailUserID);
                mms.Subject = mailSubject;
                mms.Body = mailContent;
                mms.IsBodyHtml = true;
                mms.SubjectEncoding = Encoding.UTF8;
                mms.To.Add(new MailAddress(userEmail));
                using (SmtpClient client = new(smtpServer, intSmtpPort))
                {
                    client.EnableSsl = bool.Parse(enableSsl);
                    client.Credentials = new NetworkCredential(mailUserID, mailUserPwd);//寄信帳密 
                    client.Send(mms); //寄出信件
                }

                return string.Empty;

            }
            catch (Exception ex)
            {
                return "信件發送失敗" + ex.Message;
            }
        }

     
        public static List<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var objectProperties = typeof(T).GetProperties(flags);
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var list = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();
                var propertiesList = objectProperties.Where(properties => columnNames.Contains(properties.Name)
                && properties.CanWrite
                && dataRow[properties.Name] != null
                && dataRow[properties.Name] != DBNull.Value);
                foreach (var properties in propertiesList)
                {
                    var type = properties.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }
                    var value = Convert.ChangeType(dataRow[properties.Name], type);
                    //properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                    properties.SetValue(instanceOfT, value, null);
                }
                return instanceOfT;
            }).ToList();
            return list;
        }

        /// <summary>
        /// 設定Session資訊
        /// </summary>
        /// <param name="loginDM"></param>
        /// <param name="roleDMs"></param>
        //public static void SetToSession(Business.DomainModel.LoginDM loginDM)
        //{
        //    DateTime dtTime;
        //    var _Current = LoginSession.Current;
        //    _Current.AccountId = loginDM.Id;
        //    _Current.AccountName = loginDM.AccountName;
        //    _Current.Account = loginDM.MemberAccount;
        //    _Current.Functions = loginDM.Functions;

        //    _Current.TempAccount = loginDM.TempAccount;

        //    LoginSession.Current = _Current;
        //}

        public static void SetToSession(SessionVO vo)
        {
            LoginSession.Current = vo;

        }


        /// <summary>
        /// 檢核中華民國外僑及大陸人士在台居留證(舊式+新式)
        /// </summary>
        /// <param name="idNo">身分證</param>
        /// <returns></returns>
        public static bool CheckResidentID(string idNo)
        {
            if (idNo == null)
            {
                return false;
            }
            idNo = idNo.ToUpper();
            Regex regex = new Regex(@"^([A-Z])(A|B|C|D|8|9)(\d{8})$");
            Match match = regex.Match(idNo);
            if (!match.Success)
            {
                return false;
            }

            if ("ABCD".IndexOf(match.Groups[2].Value) >= 0)
            {
                //舊式
                return CheckOldResidentID(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
            }
            else
            {
                //新式(2021/01/02)正式生效
                return CheckNewResidentID(match.Groups[1].Value, match.Groups[2].Value + match.Groups[3].Value);
            }
        }
        /// <summary>
        /// 舊式檢核
        /// </summary>
        /// <param name="firstLetter">第1碼英文字母(區域碼)</param>
        /// <param name="secondLetter">第2碼英文字母(性別碼)</param>
        /// <param name="num">第3~9流水號 + 第10碼檢查碼</param>
        /// <returns></returns>
        private static bool CheckOldResidentID(string firstLetter, string secondLetter, string num)
        {
            ///建立字母對應表(A~Z)
            ///A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
            ///P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35 
            string alphabet = "ABCDEFGHJKLMNPQRSTUVXYWZIO";
            string transferIdNo =
                $"{alphabet.IndexOf(firstLetter) + 10}" +
                $"{(alphabet.IndexOf(secondLetter) + 10) % 10}" +
                $"{num}";
            int[] idNoArray = transferIdNo.ToCharArray()
                                          .Select(c => Convert.ToInt32(c.ToString()))
                                          .ToArray();

            int sum = idNoArray[0];
            int[] weight = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
            for (int i = 0; i < weight.Length; i++)
            {
                sum += weight[i] * idNoArray[i + 1];
            }
            return (sum % 10 == 0);
        }
        /// <summary>
        /// 新式檢核
        /// </summary>
        /// <param name="firstLetter">第1碼英文字母(區域碼)</param>
        /// <param name="num">第2碼(性別碼) + 第3~9流水號 + 第10碼檢查碼</param>
        /// <returns></returns>
        private static bool CheckNewResidentID(string firstLetter, string num)
        {
            ///建立字母對應表(A~Z)
            ///A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
            ///P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35 
            string alphabet = "ABCDEFGHJKLMNPQRSTUVXYWZIO";
            string transferIdNo = $"{(alphabet.IndexOf(firstLetter) + 10)}" +
                                  $"{num}";
            int[] idNoArray = transferIdNo.ToCharArray()
                                          .Select(c => Convert.ToInt32(c.ToString()))
                                          .ToArray();

            int sum = idNoArray[0];
            int[] weight = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 1 };
            for (int i = 0; i < weight.Length; i++)
            {
                sum += (weight[i] * idNoArray[i + 1]) % 10;
            }
            return (sum % 10 == 0);
        }


        /// <summary>
        /// 檢查身分證格式
        /// </summary>
        /// <param name="idnumber"></param>
        /// <returns></returns>
        public static bool IsIdentificationId(string idnumber)
        {
            var result = false;
            if (idnumber.Length == 10)
            {
                idnumber = idnumber.ToUpper();
                if (idnumber[0] >= 0x41 && idnumber[0] <= 0x5A)
                {
                    var a = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
                    var b = new int[11];
                    b[1] = a[(idnumber[0]) - 65] % 10;
                    var c = b[0] = a[(idnumber[0]) - 65] / 10;
                    for (var i = 1; i <= 9; i++)
                    {
                        b[i + 1] = idnumber[i] - 48;
                        c += b[i] * (10 - i);
                    }
                    if (((c % 10) + b[10]) % 10 == 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 檢查統一編號
        /// </summary>
        /// <param name="cTax"></param>
        /// <returns></returns>
        public static (bool IsTrue, string cMessage) CheckTaxID(string cTax)
        { //回傳結果集
            var oResult = (IsTrue: true, cMessage: "統一編號格式合法");
            //邏輯乘數（財政部制定）
            var cMagic = "12121241";
            try
            {
                if (string.IsNullOrEmpty(cTax) || cTax.Length != 8 || !int.TryParse(cTax, out int iUnused))
                { throw new System.Exception("統一編號請輸入八位數純數字"); }
                //轉成數值陣列
                var aryTax = cTax.ToCharArray().Select(x => (int)(x - '0')).ToArray();
                var aryMagic = cMagic.ToCharArray().Select(x => (int)(x - '0')).ToArray();
                //運算乘積
                var aryResult = new int[8];
                for (int i = 0; i < aryTax.Length; i++)
                { aryResult[i] = aryTax[i] * aryMagic[i]; }
                //運算整理：大於10就進行位數相加
                aryResult = aryResult.Select(x => x < 10 ? x : x.ToString().ToCharArray().Select(y => (int)(y - '0')).Sum()).ToArray();
                //運算整理：第七位數大於10之分拆
                var oList = new System.Collections.Generic.List<int[]>();
                foreach (var cItem in aryResult[6].ToString().ToCharArray())
                {
                    var aryTemp = aryResult.ToArray();
                    aryTemp[6] = (int)(cItem - '0');
                    oList.Add(aryTemp);
                }
                //運算整理：乘積和與除5判斷
                if (!oList.Select(x => x.Sum()).Select(x => x % 5 == 0).Any(x => x))
                { throw new System.Exception("格式錯誤"); }
            }
            catch (System.Exception oEx)
            {
                oResult.IsTrue = false;
                oResult.cMessage = oEx.Message;
            }
            return oResult;
        }
        public static string GetClientIPAddress()
        {
            var context = HttpContext.Current;
            string ClientIP = context.GetServerVariable("HTTP_X_FORWARDED_FOR");
            if (String.IsNullOrEmpty(ClientIP))
            {
                ClientIP = context.GetServerVariable("REMOTE_ADDR")?.ToString() ?? "::1";
            }
            ClientIP = ClientIP.Replace("::1", "127.0.0.1");
            return ClientIP;
        }

        public static bool IsUploadFileExtensionValid(string extension)
        {
            return extension == ".jpg" || extension == ".jpeg"
                || extension == ".pdf"
                || extension == ".odt" || extension == ".ods"
                || extension == ".xls" || extension == ".xlsx"
                || extension == ".doc" || extension == ".docx"
                || extension == ".ppt" || extension == ".pptx";
        }



        public static string MixUnicodeToString(string mixUnicode)
        {
            byte[] textBytes = Encoding.Unicode.GetBytes(mixUnicode);
            return Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, textBytes));
        }

        internal static List<SelectListItem> GetClassTypeList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "1", Text = "移工一站式" });
            list.Add(new SelectListItem() { Value = "2", Text = "關懷服務" });
            return list;
        }

        /// <summary>
        /// 判斷是否為正確網址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsValidUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        /// <summary>
        /// 判斷是否為合法的郵件地址
        /// </summary>
        /// <param name="MailAddress">郵件地址</param>
        public static bool IsValidMailAddress(string MailAddress)
        {
            string RegPattern = @"[a-z0-9._%+\-]+@[a-z0-9.\-]+\.[a-z]{2,}$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(MailAddress);
        }

        /// <summary>
        /// 判斷是否為合法的台灣手機號碼
        /// </summary>
        /// <param name="CellPhoneNumber">手機號碼</param>
        public static bool IsValidCellPhoneNummberTW(string CellPhoneNumber)
        {
            string RegPattern = @"^(09)([0-9]{2})([-]?)([0-9]{6})$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(CellPhoneNumber);
        }

        /// <summary>
        /// 判斷是否為合法的台灣市話號碼
        /// </summary>
        /// <param name="PhoneNumber">手機號碼</param>
        public static bool IsValidPhoneNummberTW(string PhoneNumber)
        {
            string RegPattern = @"^(0)([0-9]{1})([-]?)([0-9]{6,8})$";
            Regex _tmpRegex = new Regex(RegPattern, RegexOptions.IgnoreCase);
            return _tmpRegex.IsMatch(PhoneNumber);
        }

        public static string SaveFile(IFormFile file)
        {
            Guid fileId = Guid.NewGuid();
            var lo = System.IO.Path.GetFullPath(System.Configuration.ConfigurationManager.AppSettings["FileLocation"]);

            //if (file.Length > 0)
            {
                var loc = $@"{lo}\{fileId.ToString()}{System.IO.Path.GetExtension(file.FileName)}";
                loc = loc.Replace("..", "");

                using (var stream = System.IO.File.Create(loc))
                {
                    file.CopyTo(stream);
                }

                return loc;
            }

            //return null;
        }


        //public static string SaveFile(IFormFile file, out string readLocation)
        //{
        //    Guid fileId = Guid.NewGuid();
        //    readLocation = @"../" + System.Configuration.ConfigurationManager.AppSettings["FileLocation"] + $"/{fileId.ToString()}{System.IO.Path.GetExtension(file.FileName)}";
        //    var lo = System.IO.Path.GetFullPath(System.Configuration.ConfigurationManager.AppSettings["FileLocation"]);

        //    //if (file.Length > 0)
        //    {
        //        var loc = $@"{lo}\{fileId.ToString()}{System.IO.Path.GetExtension(file.FileName)}";
        //        loc = loc.Replace("..", "");

        //        using (var stream = System.IO.File.Create(loc))
        //        {
        //            file.CopyTo(stream);
        //        }

        //        return loc;
        //    }

        //    //return null;
        //}

        /// <summary>
        /// 上傳檔案至指定目錄
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="DirName">目錄名稱</param>
        /// <param name="_env"></param>
        /// <returns>guid 檔案名稱</returns>
        public static async Task<string> SaveFileAsync(IFormFile file, string DirName, IWebHostEnvironment _env)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }
            string DirPath = Path.Combine(_env.ContentRootPath, DirName);
            // 確保目錄存在
            if (!string.IsNullOrEmpty(DirPath) && !Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }
            string FileName = Guid.NewGuid().ToString();

            // 使用 FileStream 以非同步方式寫入
            string targetPath = Path.Combine(DirPath, FileName + Path.GetExtension(file.FileName));
            using (var stream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await file.CopyToAsync(stream);
                return FileName + Path.GetExtension(file.FileName);
            }
        }

        /// <summary>
        ///  刪除檔案
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private static bool IsPrivateServer()
        {
            string fileServer = System.Configuration.ConfigurationManager.AppSettings["FileServer"];
            bool isPrivateServer = fileServer != null && "PRIVATE".Equals(fileServer.ToUpper());

            return isPrivateServer;
        }

        //20251218為了解決部署在linux後，一段時間會出現異常訊息，系統就掛掉
            //IOException: The configured user limit (512) on the number of inotify instances has been reached,
            //or the per-process limit on the number of open file descriptors has been reached.
        
        //20251218 1.在 Method 類別中新增一個私有的靜態變數
        private static IConfiguration _cachedConfig;

        public static string GetAppSettingsDataByName(string columnName)
        {
            //20251218  2. 檢查是否已經 Build 過，如果沒有才 Build
            if (_cachedConfig == null)
            {
                _cachedConfig = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    // 將 reloadOnChange 設為 false (最安全) 
                    // 或者保留 true，但因為只執行一次，所以只會佔用 1 個 inotify
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            }

            //20251218 3. 永遠從快取中讀取資料
            if (_cachedConfig[columnName] != null)
            {
                return _cachedConfig[columnName];
            }
            //20251218
            //IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            //if (config[columnName] != null)
            //{
            //    return config[columnName];
            //}

            return string.Empty;
        }

        public static string CalculateGrowthRate(int current, int previous)
        {
            if (current == previous)
            {
                return "0%";
            }
            if (current != 0 && previous == 0)
            {
                return "";
            }
            if (current == 0 && previous != 0)
            {
                return "-100%";
            }
            var growthRate = ((decimal)current - previous) / previous * 100;
            return $"{Math.Round(growthRate, 2)}%";
        }

        public static string CalculateGrowthRate(int current, int previous, out bool? Positive)
        {
            if (current == previous)
            {
                Positive = null;
                return "0%";
            }
            if (current != 0 && previous == 0)
            {
                Positive = null;
                return "";
            }
            if (current == 0 && previous != 0)
            {
                Positive = false;
                return "-100%";
            }
            var growthRate = ((decimal)current - previous) / previous * 100;
            Positive = growthRate > 0;
            return $"{Math.Round(growthRate, 2)}%";
        }

        /// <summary>
        /// 產生JWT Token
        /// </summary>
        /// <param name="vo">Session資料</param>
        /// <param name="jwtConfigVO">JWT設定</param>
        /// <returns></returns>
        public static TokenInfoVO GenerateJwtToken(SessionVO vo, JwtConfigVO jwtConfigVO)
        {
            TokenInfoVO tokenInoVO = new();
            #region 建立JWT Token
            //宣告JwtSecurityTokenHandler，用來建立token
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();

            //appsettings中JwtConfig的Secret值
            byte[] key = Encoding.ASCII.GetBytes(jwtConfigVO.Secret);
            var jti = Guid.NewGuid().ToString();
            //定義token描述
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                //設定要加入到 JWT Token 中的聲明資訊(Claims)
                Subject = new ClaimsIdentity(new[]
                {
        new Claim(nameof(SessionVO.userrole),vo.userrole),
        new Claim(nameof(SessionVO.usertype),vo.usertype),
                new Claim(nameof(SessionVO.ivrcode),vo.ivrcode),
                new Claim(nameof(SessionVO.username),vo.username),
                new Claim(nameof(SessionVO.empname),vo.empname ?? ""), // 加入廠商名稱
                new Claim(JwtRegisteredClaimNames.Jti, jti), // 設定 JTI
        }),

                //設定Token的時效
                Expires = DateTime.Now.AddSeconds(int.Parse(jwtConfigVO.ExpireTimeDuration)),

                //設定加密方式，key(appsettings中JwtConfig的Secret值)與HMAC SHA512演算法
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),

                Issuer = jwtConfigVO.Issuer, //簽發者
            };

            //使用SecurityTokenDescriptor建立JWT securityToken
            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);

            //token序列化為字串
            string jwtToken = jwtTokenHandler.WriteToken(token);
            #endregion

            var readToken = jwtTokenHandler.ReadJwtToken(jwtToken);
            var nbf = readToken.Claims.FirstOrDefault(c => c.Type == "nbf")?.Value;
            var iat = readToken.Claims.FirstOrDefault(c => c.Type == "iat")?.Value;
            var exp = readToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            DateTimeOffset nbfTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(nbf));
            DateTimeOffset nbfTimeLocal = nbfTime.ToLocalTime(); // 自動轉成本地時區
            DateTimeOffset iatTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(iat));
            DateTimeOffset iatTimeLocal = iatTime.ToLocalTime(); // 自動轉成本地時區
            DateTimeOffset expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
            DateTimeOffset expTimeLocal = expTime.ToLocalTime(); // 自動轉成本地時區
            tokenInoVO.TokenId = jwtToken;
            tokenInoVO.Iat = iatTimeLocal.DateTime;
            tokenInoVO.Nbf = nbfTimeLocal.DateTime;
            tokenInoVO.Exp = expTimeLocal.DateTime;
            tokenInoVO.LogAccount = vo.username;
            return tokenInoVO;
        }

        /// <summary>
        /// 驗證JWT Token和過期後產生新Token
        /// </summary>
        /// <param name="token">JWT Token</param>
        /// <param name="jwtConfigVO">JWT設定</param>
        /// <returns></returns>
        public static (VerifyTokenResultVO, SessionVO?) VerifyAndGenerateJwtToken(string token, JwtConfigVO jwtConfigVO)
        {
            VerifyTokenResultVO vo = new();
            SessionVO? sessionVO = null;
            //建立JwtSecurityTokenHandler
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {


                //驗證參數的Token，回傳SecurityToken
                var key = Encoding.ASCII.GetBytes(jwtConfigVO.Secret);
                TokenValidationParameters tokenValidation = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidIssuer = jwtConfigVO.Issuer,

                    //驗證IssuerSigningKey
                    ValidateIssuerSigningKey = true,
                    //以JwtConfig:Secret為Key，做為Jwt加密
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    //驗證時效
                    ValidateLifetime = true,

                    //設定token的過期時間可以以秒來計算，當token的過期時間低於五分鐘時使用。
                    ClockSkew = TimeSpan.Zero
                };
                ClaimsPrincipal tokenInVerification = jwtTokenHandler.ValidateToken(token, tokenValidation, out SecurityToken validatedToken);



                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    //檢核Token的演算法
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        vo.IsAvailable = false;
                    }
                    else
                    {
                        sessionVO = new SessionVO
                        {
                            userrole = tokenInVerification.Claims.First(x => x.Type == nameof(SessionVO.userrole)).Value,
                            usertype = tokenInVerification.Claims.First(x => x.Type == nameof(SessionVO.usertype)).Value,
                            ivrcode = tokenInVerification.Claims.First(x => x.Type == nameof(SessionVO.ivrcode)).Value,
                            username = tokenInVerification.Claims.First(x => x.Type == nameof(SessionVO.username)).Value,
                        };
                    }
                }
                else
                {
                    vo.IsAvailable = false;
                }
                return (vo, sessionVO);
            }
            catch (Exception ex)
            {
                if (ex is SecurityTokenExpiredException)
                {
                    vo.IsExpired = true;
                    vo.IsAvailable = true;
                    var jwtToken = jwtTokenHandler.ReadJwtToken(token);
                    sessionVO = new SessionVO
                    {
                        userrole = jwtToken.Claims.First(x => x.Type == nameof(SessionVO.userrole)).Value,
                        usertype = jwtToken.Claims.First(x => x.Type == nameof(SessionVO.usertype)).Value,
                        ivrcode = jwtToken.Claims.First(x => x.Type == nameof(SessionVO.ivrcode)).Value,
                        username = jwtToken.Claims.First(x => x.Type == nameof(SessionVO.username)).Value,
                    };
                    var tokenVO = GenerateJwtToken(sessionVO, jwtConfigVO);
                    vo.TokenInfoVO = tokenVO;
                    return (vo, sessionVO);
                }
                vo.IsAvailable = false;
                return (vo, null);
            }
        }

        public static string CreateMailPool(string form_no, string oldStatus, string newStatus, MailPoolHandler _MailPoolHandler)
        {
            var dtTime = DateTime.Now;

            try
            {
                if (!string.IsNullOrEmpty(newStatus) && newStatus != oldStatus)
                {
                    var mail_reciver = "";
                    var mail_reciver_cc = "";
                    var reviverName = "";
                    var list = _MailPoolHandler.FindMailPoolRuleList(oldStatus + "," + newStatus);
                    foreach (var item in list)
                    {
                        var _AccessRole = _MailPoolHandler.FindAccessRole(form_no, item.mail_reciver);
                        if (_AccessRole != null)
                        {
                            //取得收件人
                            mail_reciver = GetReciverMail(_MailPoolHandler, _AccessRole, item.mail_reciver, out reviverName);

                            List<string> mails = new();

                            //取得CC
                            if (!string.IsNullOrEmpty(item.mail_reciver_cc))
                            {
                                var ccs = item.mail_reciver_cc.Split(',');
                                foreach (var cc in ccs)
                                {
                                    mails.Add(GetReciverMail(_MailPoolHandler, _AccessRole, item.mail_reciver, out reviverName));
                                }
                            }

                            //取得appsettings.json CC
                            var other_cc = Method.GetAppSettingsDataByName(oldStatus + "," + newStatus);
                            if (!string.IsNullOrEmpty(other_cc))
                            {
                                mails.AddRange(other_cc.Split(','));
                            }

                            mail_reciver_cc = string.Join(",", mails);
                        }

                        if (!string.IsNullOrEmpty(mail_reciver))
                        {
                            var fttForm = _MailPoolHandler.GetFttForm(form_no);

                            _MailPoolHandler.Insert(new MailPoolEntity()
                            {
                                CreateTime = dtTime,
                                Creator = 0,
                                Updater = 0,
                                UpdateTime = dtTime,
                                SendStatus = 0,
                                EstimateSendTime = dtTime,

                                DestinationEmail = mail_reciver,
                                DestinationEmail_CC = mail_reciver_cc,
                                Subject = item.mailsubject
                                .Replace("([FORM_NO])", form_no)
                                .Replace("([STORE])", reviverName)
                                .Replace("([VENDOR])", reviverName)
                                ,
                                Status = 1,
                                Content =
                                "<html>" +
                                item.mailhead
                                .Replace("([FORM_NO])", form_no)
                                .Replace("([STORE])", reviverName)
                                .Replace("([VENDOR])", reviverName)
                                .Replace("([MailURL])", Method.GetAppSettingsDataByName("MailURL"))
                                .Replace("([MailURL_VENDOR])", Method.GetAppSettingsDataByName("MailURL_VENDOR"))
                                + "<br>"
                                + "<br>"
                                + item.mailcontent
                                .Replace("([FORM_NO])", form_no)
                                .Replace("([STORE])", reviverName)
                                .Replace("([VENDOR])", reviverName)

                                .Replace("([EMPNAME])", fttForm.empname)
                                .Replace("([CREATETIME])", DateTime.Parse(fttForm.createtime).ToString("yyyy/MM/dd HH:mm:ss"))
                                .Replace("([CATEGORY_NAME])", fttForm.category_name)

                                + "</html>"
                                ,
                            });

                            _MailPoolHandler.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }

            return "";
        }

        private static string GetReciverMail(MailPoolHandler _MailPoolHandler, access_roleDTO _AccessRole, string mail_reciver, out string reviverName)
        {
            var revier = "";
            reviverName = "";
            switch (mail_reciver)
            {
                case "MANAGER"://訂單的管理者
                               //取得管理者mail
                    var userProfile = _MailPoolHandler.GetFetUserProfile(_AccessRole.empno);
                    if (userProfile != null)
                    {
                        revier = userProfile.email;
                    }
                    break;
                case "SUBMITTER"://creater
                                 //取得申請者mail
                    var storeProfile = _MailPoolHandler.GetStoreProfile(_AccessRole.deptcode);
                    if (storeProfile != null)
                    {
                        revier = storeProfile.email;
                    }

                    if (string.IsNullOrEmpty(reviverName))
                    {
                        reviverName = storeProfile.shop_name;
                    }
                    break;
                case "VENDOR"://訂單的廠商
                    var store_vender_profile = _MailPoolHandler.GetStoreVenderProfile(_AccessRole.deptcode);
                    if (store_vender_profile != null)
                    {
                        revier = store_vender_profile.email;
                    }
                    if (string.IsNullOrEmpty(reviverName))
                    {
                        reviverName = store_vender_profile.merchant_name;
                    }
                    break;

                case "SECURITY":
                case "ASSETER":
                case "ADMIN":
                    var emails = _MailPoolHandler.GetEmailListByRole(mail_reciver);

                    if (emails.Count > 0)
                    {
                        var temps = emails.Where(w => !string.IsNullOrEmpty(w.email)).Select(s => s.email).ToList();
                        if (temps != null && temps.Count > 0)
                        {
                            revier = string.Join(",", temps);
                        }
                    }
                    break;
                default:
                    break;
            }

            return revier;
        }
    }
}
