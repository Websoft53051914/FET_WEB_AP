using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using Helper.Log;


namespace Core.Utility.Helper.Zip
{
    /// <summary>
    /// 【ZIP、解壓縮、壓縮檔案】
    /// </summary>
    public class ZipHelper
    {
        private LogHelper _logger = new LogHelper();

        public void ZipCompression()
        {
            _logger.Info("Program started.");

            try
            {
                string ReceiveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "接收檔案路徑.txt");  //接收文檔「接收檔案路徑.txt」

                if (File.Exists(ReceiveFilePath))
                {
                    string ActionFilePath = File.ReadAllText(ReceiveFilePath).Trim(); //讀取並接收要被壓縮的檔案的路徑 (來自「接收檔案路徑.txt」)             
                    {
                        if (Directory.Exists(ActionFilePath))
                        {
                            string desktopFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "desktopBoca.txt");

                            string zipFilePath = File.ReadAllText(desktopFilePath).Trim() + DateTime.Now.ToString(" yyyyMMdd HHmmss") + ".zip";

                            string zipDirectory = Path.GetDirectoryName(zipFilePath);

                            if (Directory.Exists(zipDirectory))
                            {
                                using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipFilePath)))

                                {
                                    zipStream.SetLevel(9); // 0 - store only to 9 - means best compression

                                    zipStream.Password = "123"; // Set your password here

                                    CompressFolder(ActionFilePath, zipStream, ActionFilePath);

                                    Console.WriteLine("Compression is finished");

                                    _logger.Info("Compression is finished");

                                    zipStream.Finish();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Output path of zip does not exist.");
                                _logger.Error("Output path of zip does not exist.");// 輸出檔案的路徑不存在
                            }
                        }
                        else
                        {
                            Console.WriteLine("The path provided in '接收檔案路徑.txt' does not exist.");
                            _logger.Error("The path provided in '接收檔案路徑.txt' does not exist."); //被壓縮的檔案路徑不存在
                        }
                    }
                }

            }
            catch (OutOfMemoryException) // 記憶體不足的情況
            {
                _logger.Error("Out of memory");
            }
            catch (Exception) //所有錯誤
            {
                _logger.Error("Something went wrong");
            }

            static void CompressFolder(string path, ZipOutputStream zipStream, string rootPath)
            {
                string[] files = Directory.GetFiles(path);
                string[] folders = Directory.GetDirectories(path);

                foreach (string file in files)
                {
                    //FileInfo fileInfo = new FileInfo(file);

                    string entryName = file.Substring(rootPath.Length + 1); // Removes the root path and adds the relative path
                    ZipEntry entry = new ZipEntry(entryName)
                    {
                        DateTime = File.GetLastWriteTime(file),
                        //Size = fileInfo.Length
                    };

                    zipStream.PutNextEntry(entry);

                    using (var fs = File.OpenRead(file))
                    {
                        fs.CopyTo(zipStream);
                    }
                    zipStream.CloseEntry();
                }

                foreach (string folder in folders)
                {
                    CompressFolder(folder, zipStream, rootPath);
                }
            }
        }

        /// <summary>
        /// 製作成zip
        /// </summary>
        /// <param name="streams">檔案stream清單</param>
        /// <returns>回傳zip FileStream</returns>
        static public FileStream ZipByZipOutputStreamByStream(List<FileStreamInfo> streams)
        {
            var tempFile = Path.GetTempFileName();
            using (var zipFile = System.IO.File.Create(tempFile))
            using (ZipOutputStream s = new ZipOutputStream(zipFile))
            {
                s.SetLevel(9); // 0 - store only to 9 - means best compression
                byte[] buffer = new byte[4096];
                foreach (var streamFile in streams)
                {
                    //ZipEntry entry = new ZipEntry(Path.GetFileName(streamFile.FileName));
                    ZipEntry entry = new ZipEntry(streamFile.FileName);
                    entry.DateTime = DateTime.Now;
                    //if (streamFile.IsFile)
                    {
                        s.PutNextEntry(entry);
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = streamFile._FileStream.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                }
                s.Finish();
                s.Close();
            }

            var stream = new FileStream(tempFile, FileMode.Open);
            return stream;
        }

        /// <summary>
        /// 檔案資訊
        /// </summary>
        public class FileStreamInfo
        {
            public MemoryStream _MemoryStream { get; set; }
            public FileStream _FileStream { get; set; }
            public string TempFilePath { get; set; }
            public string FileName { get; set; }
            public string FileFormat { get; set; }
            //public string TempFolderPath { get; set; }
            public bool IsFile { get; set; }

        }

        /// <summary>
        /// 製作壓縮檔案
        /// </summary>
        /// <param name="data">預計壓縮檔案</param>
        /// <returns>回傳zip byte array</returns>
        public static byte[] ZipData(Dictionary<string, byte[]> data)
        {
            using (var zipStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    foreach (var fileName in data.Keys)
                    {
                        var entry = zipArchive.CreateEntry(fileName);
                        using (var entryStream = entry.Open())
                        {
                            var buff = data[fileName];
                            entryStream.Write(buff, 0, buff.Length);
                        }
                    }
                }
                return zipStream.ToArray();
            }
        }

        /// <summary>
        /// 解壓縮
        /// </summary>
        /// <param name="zip">壓縮擋</param>
        /// <returns>字典檔案byte array</returns>
        public static Dictionary<string, byte[]> UnzipData(byte[] zip)
        {
            var dict = new Dictionary<string, byte[]>();
            using (var msZip = new MemoryStream(zip))
            {
                using (var archive = new ZipArchive(msZip, ZipArchiveMode.Read))
                {
                    archive.Entries.ToList().ForEach(entry =>
                    {
                        //e.FullName可取得完整路徑
                        if (string.IsNullOrEmpty(entry.Name))
                            return;

                        using (var entryStream = entry.Open())
                        {
                            using (var msEntry = new MemoryStream())
                            {
                                entryStream.CopyTo(msEntry);
                                dict.Add(entry.Name, msEntry.ToArray());
                            }
                        }
                    });
                }
            }
            return dict;
        }
    }
}
