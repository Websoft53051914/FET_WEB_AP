using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;

namespace FTT_API.Controllers.SecurityMgt
{
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
    [EnableCors("AllowLocalhost7234")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public partial class SecurityMgtController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public SecurityMgtController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList(DataSourceRequest request, store_sec_vendor_listDTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                var _SecurityMgtHandler = new SecurityMgtHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.ivrcode = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _SecurityMgtHandler.FindPageList(pageEntity, vm);

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
        public IActionResult Import(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    this.LogSuccess();
                    return Json(new { success = false, message = "未選擇檔案" });
                }

                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".xls" && ext != ".xlsx")
                {
                    this.LogSuccess();
                    return Json(new { success = false, message = "檔案格式錯誤，只能上傳 Excel (.xls / .xlsx)" });
                }

                string AttachFileName = _sessionVO.empno + "_" + DateTime.Now.ToString("HHmmss") + "_" + file.FileName;
                AttachFileName = System.IO.Path.GetFileName(_config.Config["OutputPath"] + AttachFileName);
                string destFilePath = _config.Config["OutputPath"] + AttachFileName;

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

                SecurityMgtHandler _SecurityMgtHanlder = new SecurityMgtHandler(_config, HttpContext);
                var msg = _SecurityMgtHanlder.Import(destFilePath, _sessionVO.empno);

                if (string.IsNullOrEmpty(msg) == false)
                {
                    this.LogSuccess();
                    return JsonValidFail(msg);
                }
                else
                {
                    this.LogSuccess("匯入成功");
                    return JsonSuccess("匯入成功");
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

        [HttpGet("[action]")]
        public IActionResult Export()
        {
            try
            {
                string fileName = "export_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

                IWorkbook wb = new HSSFWorkbook();
                ISheet ws;
                ws = wb.CreateSheet("Sheet1");

                DataTable dt = GetQueryData();
                if (dt.Rows.Count > 0)
                {
                    ws.CreateRow(0);
                    //第一行為欄位名稱
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ws.GetRow(0).CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ws.CreateRow(i + 1);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ws.GetRow(i + 1).CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                }
                else
                {
                    ws = wb.CreateSheet(dt.TableName);

                    ws.CreateRow(0);

                    ws.GetRow(0).CreateCell(0).SetCellValue("IVRCODE");
                    ws.GetRow(0).CreateCell(1).SetCellValue("門市名稱");
                    ws.GetRow(0).CreateCell(2).SetCellValue("廠商代碼");
                    ws.GetRow(0).CreateCell(3).SetCellValue("廠商名稱");
                    ws.GetRow(0).CreateCell(4).SetCellValue("聯絡人");
                    ws.GetRow(0).CreateCell(5).SetCellValue("聯絡人電話");
                    ws.GetRow(0).CreateCell(6).SetCellValue("刪除註記");
                }


                dt = GetStoreData();
                ws = wb.CreateSheet("門市列表");

                if (dt.Rows.Count > 0)
                {

                    ws.CreateRow(0);
                    //第一行為欄位名稱
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ws.GetRow(0).CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ws.CreateRow(i + 1);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ws.GetRow(i + 1).CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                }
                else
                {
                }

                ws = wb.CreateSheet("廠商代碼");
                dt = GetVendorData();

                if (dt.Rows.Count > 0)
                {
                    ws.CreateRow(0);
                    //第一行為欄位名稱
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ws.GetRow(0).CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ws.CreateRow(i + 1);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ws.GetRow(i + 1).CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                }
                else
                {
                }

                byte[] fileContents;
                // 輸出到 MemoryStream
                using (var stream = new MemoryStream())
                {
                    wb.Write(stream, true);
                    fileContents = stream.ToArray();
                }

                this.LogSuccess();
                return File(fileContents,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

        private DataTable GetStoreData()
        {
            SecurityMgtHandler _SecurityMgtHanlder = new SecurityMgtHandler(_config, HttpContext);
            DataTable dtTable = _SecurityMgtHanlder.GetStoreData();
            return dtTable;
        }

        private DataTable GetVendorData()
        {
            SecurityMgtHandler _SecurityMgtHanlder = new SecurityMgtHandler(_config, HttpContext);
            DataTable dtTable = _SecurityMgtHanlder.GetVendorData();
            return dtTable;
        }

        private DataTable GetQueryData()
        {
            SecurityMgtHandler _SecurityMgtHanlder = new SecurityMgtHandler(_config, HttpContext);
            DataTable dtTable = _SecurityMgtHanlder.GetQueryData();

            return dtTable;
        }
    }
}
