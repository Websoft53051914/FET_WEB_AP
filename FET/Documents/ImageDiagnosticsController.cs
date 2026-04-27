using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FTT_API.Controllers
{
    /// <summary>
    /// 🔧 圖片檔案診斷工具 (Linux 跨平台問題排查專用)
    /// </summary>
    [Route("[controller]")]
    public class ImageDiagnosticsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImageDiagnosticsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// 🔍 診斷 Item 目錄內的圖片檔案狀況
        /// </summary>
        [HttpGet("check-item-images")]
        public IActionResult CheckItemImages()
        {
            try
            {
                var result = new
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Environment = new
                    {
                        OS = Environment.OSVersion.ToString(),
                        IsCaseSensitive = Environment.OSVersion.Platform != PlatformID.Win32NT,
                        WebRootPath = _env.WebRootPath,
                        ContentRootPath = _env.ContentRootPath,
                        EnvironmentName = _env.EnvironmentName
                    },
                    ItemDirectory = CheckItemDirectory(),
                    SampleFiles = GetSampleFiles(),
                    PathTests = TestPathOperations()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// 🔍 檢查特定檔案名稱的搜尋結果
        /// </summary>
        [HttpGet("search-file/{fileName}")]
        public IActionResult SearchFile(string fileName)
        {
            try
            {
                var webRootPath = !string.IsNullOrEmpty(_env.WebRootPath) 
                    ? _env.WebRootPath 
                    : Path.Combine(_env.ContentRootPath, "wwwroot");

                var itemDir = Path.Combine(webRootPath, "Item");
                var results = new List<object>();

                if (!Directory.Exists(itemDir))
                {
                    return Ok(new { 
                        Message = "Item directory does not exist", 
                        DirectoryPath = itemDir 
                    });
                }

                // 嘗試不同的搜尋方式
                string[] extensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                
                foreach (var ext in extensions)
                {
                    // 原始大小寫
                    var testFile = fileName + ext;
                    var fullPath = Path.Combine(itemDir, testFile);
                    results.Add(new
                    {
                        TestFile = testFile,
                        Extension = ext,
                        Case = "Original",
                        FullPath = fullPath,
                        Exists = System.IO.File.Exists(fullPath)
                    });

                    // 小寫副檔名
                    testFile = fileName + ext.ToLower();
                    fullPath = Path.Combine(itemDir, testFile);
                    results.Add(new
                    {
                        TestFile = testFile,
                        Extension = ext.ToLower(),
                        Case = "Lower",
                        FullPath = fullPath,
                        Exists = System.IO.File.Exists(fullPath)
                    });

                    // 大寫副檔名
                    testFile = fileName + ext.ToUpper();
                    fullPath = Path.Combine(itemDir, testFile);
                    results.Add(new
                    {
                        TestFile = testFile,
                        Extension = ext.ToUpper(),
                        Case = "Upper",
                        FullPath = fullPath,
                        Exists = System.IO.File.Exists(fullPath)
                    });
                }

                // 目錄掃描
                var directoryFiles = new List<object>();
                try
                {
                    var files = Directory.GetFiles(itemDir, $"{fileName}.*", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        directoryFiles.Add(new
                        {
                            FileName = fileInfo.Name,
                            FullPath = file,
                            Size = fileInfo.Length,
                            LastModified = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            Extension = fileInfo.Extension
                        });
                    }
                }
                catch (Exception ex)
                {
                    directoryFiles.Add(new { Error = ex.Message });
                }

                return Ok(new
                {
                    SearchFileName = fileName,
                    ItemDirectoryPath = itemDir,
                    TestResults = results.Where(r => ((dynamic)r).Exists).ToList(),
                    AllTests = results,
                    DirectoryScanResults = directoryFiles
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        private object CheckItemDirectory()
        {
            try
            {
                var webRootPath = !string.IsNullOrEmpty(_env.WebRootPath) 
                    ? _env.WebRootPath 
                    : Path.Combine(_env.ContentRootPath, "wwwroot");

                var itemDir = Path.Combine(webRootPath, "Item");

                return new
                {
                    WebRootPath = webRootPath,
                    ItemDirectoryPath = itemDir,
                    WebRootExists = Directory.Exists(webRootPath),
                    ItemDirectoryExists = Directory.Exists(itemDir),
                    ItemDirectoryFileCount = Directory.Exists(itemDir) ? Directory.GetFiles(itemDir).Length : 0,
                    Permissions = GetDirectoryPermissions(itemDir)
                };
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }
        }

        private List<object> GetSampleFiles()
        {
            try
            {
                var webRootPath = !string.IsNullOrEmpty(_env.WebRootPath) 
                    ? _env.WebRootPath 
                    : Path.Combine(_env.ContentRootPath, "wwwroot");

                var itemDir = Path.Combine(webRootPath, "Item");
                var files = new List<object>();

                if (!Directory.Exists(itemDir))
                    return files;

                var allFiles = Directory.GetFiles(itemDir).Take(20); // 只取前 20 個檔案避免過多
                
                foreach (var file in allFiles)
                {
                    var fileInfo = new FileInfo(file);
                    files.Add(new
                    {
                        FileName = fileInfo.Name,
                        NameWithoutExtension = Path.GetFileNameWithoutExtension(file),
                        Extension = fileInfo.Extension,
                        Size = fileInfo.Length,
                        LastModified = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }

                return files;
            }
            catch (Exception ex)
            {
                return new List<object> { new { Error = ex.Message } };
            }
        }

        private object TestPathOperations()
        {
            try
            {
                var testPath1 = Path.Combine("Item", "test.jpg");
                var testPath2 = Path.Combine("Item", "test.JPG");
                
                return new
                {
                    PathSeparator = Path.DirectorySeparatorChar,
                    AltPathSeparator = Path.AltDirectorySeparatorChar,
                    TestPath1 = testPath1,
                    TestPath2 = testPath2,
                    WebPath1 = testPath1.Replace('\\', '/'),
                    WebPath2 = testPath2.Replace('\\', '/'),
                    PathsEqual = testPath1.Equals(testPath2),
                    PathsEqualIgnoreCase = testPath1.Equals(testPath2, StringComparison.OrdinalIgnoreCase)
                };
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }
        }

        private object GetDirectoryPermissions(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                    return new { Message = "Directory does not exist" };

                // 基本的權限檢查
                var canRead = true;
                var canWrite = true;

                try
                {
                    Directory.GetFiles(dirPath);
                }
                catch
                {
                    canRead = false;
                }

                try
                {
                    var testFile = Path.Combine(dirPath, $"test_{Guid.NewGuid()}.tmp");
                    System.IO.File.WriteAllText(testFile, "test");
                    System.IO.File.Delete(testFile);
                }
                catch
                {
                    canWrite = false;
                }

                return new
                {
                    CanRead = canRead,
                    CanWrite = canWrite,
                    DirectoryExists = true
                };
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }
        }
    }
}
