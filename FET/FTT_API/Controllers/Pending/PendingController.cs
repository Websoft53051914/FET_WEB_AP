using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using DocumentFormat.OpenXml.Office2010.Excel;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace FTT_API.Controllers.Pending
{
    [Route("[controller]")]




    public partial class PendingController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PendingController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                v_ftt_form2SQL _v_ftt_form2SQL = new v_ftt_form2SQL();

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _v_ftt_form2SQL.FindPageList(pageEntity, vm);

                for (int i = 0; i < list.Results.Count; i++)
                {
                    var item = list.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;
                }

                this.LogSuccess();
                return Json(new DataSourceResult
                {
                    Data = list.Results,
                    Total = list.DataCount
                });
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }


        
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public async Task<IActionResult> FileUpload(IFormFile file, string formNo)
        {
            try
            {
                // 基本檔案檢查
                if (file == null || file.Length == 0)
                {
                    return JsonValidFail("未選擇檔案");
                }

                // 從配置取得允許的副檔名和檔案大小限制
                var allowedExtensions = _config.Config.GetSection("FileUpload:AllowedExtensions").Get<string[]>() 
                    ?? new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".xlsx" };
                var maxSizeMB = _config.Config.GetValue<int>("FileUpload:MaxFileSizeMB", 10);

                // 副檔名檢查
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return JsonValidFail($"不允許的檔案格式。允許的格式：{string.Join(", ", allowedExtensions)}");
                }

                // 檔案大小檢查
                if (file.Length > maxSizeMB * 1024 * 1024)
                {
                    return JsonValidFail($"檔案過大，最大允許 {maxSizeMB}MB");
                }

                var originFileName = file.FileName;
                string destFilePath = Path.Combine(_config.Config["OutputPath"], originFileName);

                // 檢查資料夾是否存在
                if (!Directory.Exists(_config.Config["OutputPath"]))
                {
                    Directory.CreateDirectory(_config.Config["OutputPath"]);
                }

                // 如果檔案已存在，產生新檔名
                int counter = 1;
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originFileName);
                string fileExt = Path.GetExtension(originFileName);
                while (System.IO.File.Exists(destFilePath))
                {
                    string newFileName = $"{fileNameWithoutExt}_{counter}{fileExt}";
                    destFilePath = Path.Combine(_config.Config["OutputPath"], newFileName);
                    counter++;
                }

                // 儲存檔案
                using (var stream = new FileStream(destFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                DateTime dtNow = DateTime.Now;

                // 存至TABLE
                FileHandler _FileHandler = new FileHandler();
                int fileId = 0;

                var msg = _FileHandler.Insert(new FileEntity()
                {
                    destfilepath = destFilePath,
                    Status = "1",
                    createtime = dtNow,
                    creator = _sessionVO.empno,
                    filename = originFileName,
                    filesize = file.Length,
                    updater = _sessionVO.empno,
                    updatetime = dtNow,
                    fileext = Path.GetExtension(originFileName),
                },
                formNo,
                out fileId
                );

                if (string.IsNullOrEmpty(msg) == false)
                {
                    // 如果資料庫儲存失敗，刪除已上傳的檔案
                    try { System.IO.File.Delete(destFilePath); } catch { }
                    this.LogError(msg);
                    return JsonValidFail("新增檔案時發生異常");
                }
                else
                {
                    return JsonSuccess(new SelectListItem() { Text = originFileName, Value = fileId.ToString() });
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }


        
        [ValidateAntiForgeryToken]
        [HttpGet("[action]")]
        public IActionResult DownloadFile(string id)
        {
            try
            {
                FileHandler _FileHandler = new FileHandler();
                var dto = _FileHandler.FindById(id);
                if (dto != null)
                {
                    var filePath = dto.destfilepath;

                    if (!System.IO.File.Exists(filePath))
                        return NotFound("檔案不存在");

                    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(filePath, out var contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    return File(stream, contentType, dto.filename);
                }
                else
                {
                    return JsonValidFail("查無檔案");
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

    }
}
