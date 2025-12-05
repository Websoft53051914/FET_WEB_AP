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
using System.Runtime.Intrinsics.X86;

namespace FTT_API.Controllers.Pending
{
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
    [EnableCors("AllowLocalhost7234")]
    public partial class PendingController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PendingController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

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


        [HttpPost("[action]")]
        public IActionResult FileUpload(IFormFile file, string formNo)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return JsonValidFail("未選擇檔案");
                }

                //紀錄舊檔名
                var originFileName = file.FileName;
                //新檔案名稱
                var newFileName = _sessionVO.empno + "_" + DateTime.Now.ToString("HHmmss") + "_" + file.FileName;
                //檔案路徑
                string destFilePath = _config.Config["OutputPath"] + System.IO.Path.GetFileName(_config.Config["OutputPath"] + newFileName);

                // 檢查資料夾是否存在
                if (!Directory.Exists(_config.Config["OutputPath"]))
                {
                    Directory.CreateDirectory(_config.Config["OutputPath"]);
                }

                // 儲存檔案
                using (var stream = new FileStream(destFilePath, FileMode.Create))
                {
                    //file.CopyToAsync(stream); 非同步導致檔案是 0 kb 時就被使用
                    file.CopyTo(stream);
                }

                DateTime dtNow = DateTime.Now;

                var fileOriginName = Path.GetFileName(file.FileName);
                var fileExt = Path.GetExtension(fileOriginName);

                //存至TABLE
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
                    fileext = fileExt,

                },
                formNo,
                out fileId
                );


                if (string.IsNullOrEmpty(msg) == false)
                {
                    this.LogError(msg);
                    return JsonValidFail("新增檔案時發生異常");
                }
                else
                {
                    this.LogSuccess("新增檔案成功");
                    return JsonSuccess(new SelectListItem() { Text = originFileName, Value = fileId.ToString() }); ;
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }


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
