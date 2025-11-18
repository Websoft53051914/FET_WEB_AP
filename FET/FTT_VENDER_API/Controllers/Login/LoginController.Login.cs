using Const.VO;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Models.Handler;
using FTT_VENDER_API.Models.ViewModel.Login;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.Text.RegularExpressions;

namespace FTT_VENDER_API.Controllers.Login
{
    /// <summary>
    /// 登入 API
    /// </summary>
    [Route("[controller]")]
    public partial class LoginController : BaseProjectController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ConfigurationHelper _configHelper;
        /// <summary>
        /// Constructor
        /// </summary>
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
                var loginHanlder = new LoginHanlder(_configHelper, HttpContext);
                (LoginResultVO resultVO, SessionVO? sessionVO) = loginHanlder.Login(vm);

                if (!string.IsNullOrEmpty(resultVO.ErrorMsg))
                {
                    this.LogSuccess();
                    return JsonValidFail(resultVO.ErrorMsg);
                }

                if (!string.IsNullOrEmpty(resultVO.Token.TokenId))
                {
                    resultVO.Token.TokenId = InputSanitizer.SanitizeForCookie(resultVO.Token.TokenId);
                    Response.Cookies.Append("Token", resultVO.Token.TokenId, new CookieOptions
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

                    userLoginName = sessionVO.engname;
                }

                userLoginName = SanitizeCookieValue(userLoginName);
                var userrole = SanitizeCookieValue(sessionVO?.userrole);

                Response.Cookies.Append("userLoginName", userLoginName ?? string.Empty, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // HTTP測試用false https用true
                    SameSite = SameSiteMode.Lax, // http測試用Lax https用none
                });
                Response.Cookies.Append("userrole", userrole ?? string.Empty, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false, // HTTP測試用false https用true
                    SameSite = SameSiteMode.Lax, // http測試用Lax https用none
                });
                _sessionVO = sessionVO ?? new();

                this.LogSuccess("登入成功");
                return JsonOK();
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
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


        private static ConcurrentDictionary<string, string> _captchaStore = new ConcurrentDictionary<string, string>();

        [HttpGet("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult GetCaptcha()
        {
            var code = GenerateCode(4); // 4位隨機碼
            var id = Guid.NewGuid().ToString();
            _captchaStore[id] = code;

            var imageBytes = GenerateCaptchaImage(code);

            return JsonSuccess(new
            {
                captchaId = id,
                imageBase64 = "data:image/png;base64," + Convert.ToBase64String(imageBytes)
            });
        }

        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult VerifyCaptcha(CaptchaVerifyRequest request)
        {
            if (_captchaStore.TryGetValue(request.CaptchaId, out var code))
            {
                if (string.Equals(code, request.CaptchaCode, StringComparison.OrdinalIgnoreCase))
                {
                    _captchaStore.TryRemove(request.CaptchaId, out _);
                    return JsonSuccess("");
                }
            }

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
                var font = SystemFonts.CreateFont("Arial", 30, FontStyle.Bold);

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
