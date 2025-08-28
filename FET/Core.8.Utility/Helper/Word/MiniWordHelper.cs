namespace Core.Utility.Helper.Word
{
    /// <summary>
    /// 使用Miniword 進行套表的Helper
    /// 【Miniword、word、套表】
    /// </summary>
    public class MiniWordHelper
    {
        /// <summary>
        /// 產生 Word byte array
        /// </summary>
        /// <param name="templatePath">word 檔案位置</param>
        /// <param name="paras">套版參數與值</param>
        /// <param name="miniWordPictures">圖片清單</param>
        /// <returns>回傳Word byte array</returns>
        public byte[] Print(string templatePath , Dictionary<string, object> paras , List<MiniWordPictureVo> miniWordPictures = null)
        {
            
            if(miniWordPictures != null)
            {
                foreach (var miniWordPicture in miniWordPictures)
                {
                    if(paras.ContainsKey(miniWordPicture.TagName))
                    {
                        paras[miniWordPicture.TagName] = new MiniSoftware.MiniWordPicture()
                        {
                            Path = miniWordPicture.Path,
                            Width = miniWordPicture.Width,
                            Height = miniWordPicture.Height
                        };
                    }
                    else
                    {
                        paras.TryAdd(miniWordPicture.TagName, new MiniSoftware.MiniWordPicture()
                        {
                            Path = miniWordPicture.Path,
                            Width = miniWordPicture.Width,
                            Height = miniWordPicture.Height
                        });
                    }

                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                MiniSoftware.MiniWord.SaveAsByTemplate(stream, templatePath, paras);
                return ((MemoryStream)stream).ToArray();
            }
        }

        /// <summary>
        /// 產出CehckBox
        /// </summary>
        /// <param name="para">true或false</param>
        /// <returns>回傳check是否勾選字樣</returns>
        public string GetCheckbox(bool para)
        {
            return para ? "☑" : "□";
        }

    }

    /// <summary>
    /// 圖片大小位置
    /// </summary>
    public class MiniWordPictureVo
    {
        public string TagName { get; set; }
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
