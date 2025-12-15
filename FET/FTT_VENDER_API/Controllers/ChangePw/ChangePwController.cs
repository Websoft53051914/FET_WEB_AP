using Core.Utility.Extensions;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Models.Handler;
using FTT_VENDER_API.Models.ViewModel.ChangePw;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using static Const.Enums;
using static FTT_VENDER_API.Controllers.Login.LoginController;
using static FTT_VENDER_API.Models.AlertMsgRedirection;


namespace FTT_VENDER_API.Controllers.ChangePw
{
    [Route("[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChangePwController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        /// <summary>
        /// Constructor
        /// </summary>
        public ChangePwController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
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
        [AllowAnonymous] // 允許匿名訪問，登入前需要使用
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
            int width = 100, height = 40;
            using var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            var font = new Font("Arial", 20, FontStyle.Bold);
            var brush = new SolidBrush(Color.Black);
            g.DrawString(code, font, brush, 10, 5);
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
