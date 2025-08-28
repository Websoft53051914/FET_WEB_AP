using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.CaptchaCode
{
    /// <summary>
    /// 製作驗回傳類別
    /// </summary>
    public class CaptchaResult
    {
        /// <summary>
        /// 驗證碼圖片
        /// </summary>
        public byte[] CaptchaImage { set; get; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string ResultCode { set; get; }
    }
}
