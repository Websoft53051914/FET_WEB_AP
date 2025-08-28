using System.IO;

namespace Core.Utility.Utility
{
    /// <summary>
    /// 【File、檔案】
    /// </summary>
    public class FileUtility
    {
        /// <summary>
        /// memorystream寫至實體檔案
        /// </summary>
        /// <param name="memoryStream">資訊流</param>
        /// <param name="path">儲存位置</param>
        public static void MemoryStreamToFile(MemoryStream memoryStream, string path)
        {
            using FileStream fileStream = new(path, FileMode.Create, System.IO.FileAccess.Write);
            memoryStream.WriteTo(fileStream);
        }

        /// <summary>
        /// 二進位資料寫至實體檔案
        /// </summary>
        /// <param name="byteArray">byte array</param>
        /// <param name="path">儲存位置</param>
        public static void MemoryStreamToFile(byte[] byteArray, string path)
        {
            MemoryStreamToFile(new MemoryStream(byteArray), path);
        }

        /// <summary>
        /// 實體檔案寫至memorystream
        /// </summary>
        /// <param name="path">儲存位置</param>
        public static MemoryStream FileToMemoryStream(string path)
        {
            string path2 = Path.Combine(path);
            MemoryStream memoryStream = new();
            using (FileStream fileStream = File.OpenRead(path2))
            {
                fileStream.CopyTo(memoryStream);
            }
            return memoryStream;
        }

        /// <summary>
        /// 二進位資料轉為MemoryStream
        /// </summary>
        /// <param name="byteArray">檔案byte array</param>
        /// <returns>檔案資訊流</returns>
        public static MemoryStream ByteArrayToMemoryStream(byte[] byteArray)
        {
            return new MemoryStream(byteArray);

        }

        /// <summary>
        /// MemoryStream轉為二進位資料
        /// </summary>
        /// <param name="memoryStream">檔案資訊流</param>
        /// <returns>檔案 byte array</returns>
        public static byte[] ByteArrayToMemoryStream(MemoryStream memoryStream)
        {
            return memoryStream.ToArray();
        }
    }
}
