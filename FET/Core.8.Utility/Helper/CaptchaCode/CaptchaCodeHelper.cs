using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Core.Utility.Helper.CaptchaCode
{
    /// <summary>
    /// 製作驗證碼
    /// 【驗證碼、CAPTCHA】
    /// </summary>
    public class CaptchaCodeHelper
    {
        public static readonly string CAPTCHA_CODE = "CAPTCHA_CODE";
        public CaptchaCodeHelper()
        {
            this.LineCount = 5;
            this.CodeSize = 4;
            this.Width = 120;
            this.Height = 38;
        }

        /// <summary>
        /// 字元長度
        /// </summary>
        public int CodeSize { set; get; }

        /// <summary>
        /// 干擾線的長度
        /// </summary>
        public int LineCount { set; get; }

        /// <summary>
        /// 寬度
        /// </summary>
        public int Width { set; get; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { set; get; }

        /// <summary>
        /// 畫出 圖形驗證碼
        /// </summary>
        /// <returns>圖形驗證碼類別</returns>
        public CaptchaResult Result()
        {
            string code = RandomCode();


            //定義一個畫板
            MemoryStream ms = new();
            using (Bitmap map = new(this.Width, this.Height))
            {
                //畫筆,在指定畫板畫板上畫圖
                //g.Dispose();
                using (Graphics g = Graphics.FromImage(map))
                {
                    g.Clear(RendomBgColor());
                    int i = 0;
                    foreach (char c in code.ToCharArray())
                    {
                        g.DrawString(c.ToString(), new Font("黑體", WordSize()), Brushes.Blue, new Point(7 + (i * 19), 8));
                        i++;
                    }
                    //繪製干擾線(數字代表幾條)
                    PaintInterLine(g, this.LineCount, map.Width, map.Height);
                }
                map.Save(ms, ImageFormat.Jpeg);
            }


            return new CaptchaResult()
            {
                CaptchaImage = ms.GetBuffer(),
                ResultCode = code
            };
        }

        Random rand = new();

        /// <summary>
        /// 字體大小
        /// </summary>
        /// <returns>回傳字體大小</returns>
        private float WordSize()
        {
            float[] ary = new float[]
            {
                16F,
                17F,
                18F,
                19F,
                20F
            };


            int randomIdx = rand.Next(0, ary.Length);
            return ary[randomIdx];
        }

        /// <summary>
        /// 取得背景顏色
        /// </summary>
        /// <returns>顏色</returns>
        private Color RendomBgColor()
        {
            Color[] ary = new Color[]
            {
                Color.Yellow,
                Color.LightCyan,
                Color.LightPink,
                Color.LightGreen,
                Color.White
            };

            int randomIdx = rand.Next(0, ary.Length);
            return ary[randomIdx];
        }

        /// <summary>
        /// 產生驗證碼
        /// </summary>
        /// <param name="length">長度</param>
        /// <returns>回傳驗證碼</returns>
        private string RandomCode()
        {
            string s = "123456789ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            StringBuilder sb = new();
            int index;
            for (int i = 0; i < this.CodeSize; i++)
            {
                index = rand.Next(0, s.Length);
                sb.Append(s[index]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 產生刪除線
        /// </summary>
        /// <param name="g">圖形</param>
        /// <param name="num">代表幾條刪除線</param>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param>
        private void PaintInterLine(Graphics g, int num, int width, int height)
        {
            Brush[] colors = new Brush[]
            {
                Brushes.Green,
                Brushes.Blue,
                Brushes.Gray,
                Brushes.Red,
            };

            int startX, startY, endX, endY;
            for (int i = 0; i < num; i++)
            {
                int colorIdx = i;
                if (i >= colors.Length - 1)
                {
                    colorIdx = i % (colors.Length - 1);
                }

                startX = rand.Next(0, width);
                startY = rand.Next(0, height);
                endX = rand.Next(0, width);
                endY = rand.Next(0, height);
                g.DrawLine(new Pen(colors[colorIdx]), startX, startY, endX, endY);
            }
        }
    }

}
