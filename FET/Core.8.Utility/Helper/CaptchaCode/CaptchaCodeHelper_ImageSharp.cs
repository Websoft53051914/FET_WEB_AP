using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using System.Text;
using System.Numerics;


namespace Core.Utility.Helper.CaptchaCode
{
    public class CaptchaCodeHelper_ImageSharp
    {

        public static readonly string CAPTCHA_CODE = "CAPTCHA_CODE";
        public CaptchaCodeHelper_ImageSharp()
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
        /// <returns></returns>
        public CaptchaResult Result()
        {
            string code = RandomCode();

            MemoryStream ms = new();

            using (var image = new SixLabors.ImageSharp.Image<Rgba32>(120, 38))
            {
                image.Mutate(ctx => ctx.BackgroundColor(Color.White));

                //var collection = new FontCollection();
                //var font = SixLabors.Fonts.SystemFonts.CreateFont("Times New Roman", 30, SixLabors.Fonts.FontStyle.Bold);

                FontCollection collection = new();

                FontFamily family;
#if DEBUG
                family = collection.Add(@"font/arial.ttf");
#else
                family = SystemFonts.Get("Arial");
#endif

                Font font = family.CreateFont(30, FontStyle.Italic);

                var textOptions = new RichTextOptions(font)
                {
                    Origin = new SixLabors.ImageSharp.PointF(0, 0),
                    WrappingLength = 3840f * 0.25f,
                    HorizontalAlignment = SixLabors.Fonts.HorizontalAlignment.Left,
                };

                var brush = SixLabors.ImageSharp.Drawing.Processing.Brushes.Solid(Color.Black);

                string text = code;

                using var newImage = image.Clone(ctx =>
                {
                    ctx.DrawText(textOptions, text, brush);

                    ctx.DrawLine(SixLabors.ImageSharp.Drawing.Processing.Pens.Dash(Color.Red, 1), new SixLabors.ImageSharp.PointF[] { new Vector2(rand.Next(0, 10), rand.Next(5, 40)), new Vector2(rand.Next(90, 100), rand.Next(5, 40)), });
                    ctx.DrawLine(SixLabors.ImageSharp.Drawing.Processing.Pens.Dash(Color.Red, 1), new SixLabors.ImageSharp.PointF[] { new Vector2(rand.Next(0, 10), rand.Next(5, 40)), new Vector2(rand.Next(90, 100), rand.Next(5, 40)), });
                    ctx.DrawLine(SixLabors.ImageSharp.Drawing.Processing.Pens.Dash(Color.Red, 1), new SixLabors.ImageSharp.PointF[] { new Vector2(rand.Next(0, 10), rand.Next(5, 40)), new Vector2(rand.Next(90, 100), rand.Next(5, 40)), });
                    ctx.DrawLine(SixLabors.ImageSharp.Drawing.Processing.Pens.Dash(Color.Red, 1), new SixLabors.ImageSharp.PointF[] { new Vector2(rand.Next(0, 10), rand.Next(5, 40)), new Vector2(rand.Next(90, 100), rand.Next(5, 40)), });
                    ctx.DrawLine(SixLabors.ImageSharp.Drawing.Processing.Pens.Dash(Color.Red, 1), new SixLabors.ImageSharp.PointF[] { new Vector2(rand.Next(0, 10), rand.Next(5, 40)), new Vector2(rand.Next(90, 100), rand.Next(5, 40)), });


                });

                newImage.Save(ms, PngFormat.Instance);
            }

            return new CaptchaResult()
            {
                CaptchaImage = ms.GetBuffer(),
                ResultCode = code
            };
        }

        /// <summary>
        /// 取得背景顏色
        /// </summary>
        /// <returns></returns>
        /// 
        Random rand = new();
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
        /// 產生驗證碼
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
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
    }
}
