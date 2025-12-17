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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using FTT_VENDER_API.Models.ViewModel.ChangePw;
using Core.Utility.Extensions;
using static FTT_VENDER_API.Controllers.Login.LoginController;


namespace FTT_VENDER_API.Controllers.ChangePw
{
    [Route("[controller]")]
    public class ChangePwController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMemoryCache _memoryCache;
        /// <summary>
        /// Constructor
        /// </summary>
        public ChangePwController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
            _memoryCache = memoryCache;
        }

        [HttpPost("[action]")]
        [AllowAnonymous] // 允許匿名訪問，登入前需要使用
        public IActionResult Submit(ChangePwVM vm)
        {
            try
            {
                ChangePwHandler changePwHandler = new(_config, HttpContext);
                string ErrorMsg = changePwHandler.CheckVenderInfoCorrect(vm);
                if (!ErrorMsg.IsNullOrEmpty())
                {
                    return JsonValidFail(ErrorMsg);
                }

                changePwHandler.UpdatePw(vm);
                this.LogSuccess("變更密碼成功");
                return JsonOK();
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_config.GetMessage("SystemErrorMsg"));
            }
           
        }

        private static ConcurrentDictionary<string, string> _captchaStore = new ConcurrentDictionary<string, string>();

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
