using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FTT_API.Controllers.QuoteMgt
{
    [Route("[controller]")]
    public class QuoteMgtController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public QuoteMgtController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("[action]")]
        public IActionResult Import(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "未選擇檔案" });

                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".xls" && ext != ".xlsx")
                    return Json(new { success = false, message = "檔案格式錯誤，只能上傳 Excel (.xls / .xlsx)" });

                string AttachFileName = LoginSession.Current.empno + "_" + DateTime.Now.ToString("HHmmss") + "_" + file.FileName;
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
                    file.CopyToAsync(stream);
                }

                QuoteMgtHanlder _QuoteMgtHanlder = new QuoteMgtHanlder(_config, HttpContext);
                var msg = _QuoteMgtHanlder.Import(destFilePath);

                if (string.IsNullOrEmpty(msg) == false)
                {
                    return JsonValidFail(msg);
                }
                else
                    return JsonSuccess("匯入成功");
            }
            catch (Exception ex)
            {
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
                if (dt != null && dt.Rows.Count > 0)
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
                    ws.CreateRow(0);

                    ws.GetRow(0).CreateCell(0).SetCellValue("更新註記");
                    ws.GetRow(0).CreateCell(1).SetCellValue("ID");
                    ws.GetRow(0).CreateCell(2).SetCellValue("CISID");
                    ws.GetRow(0).CreateCell(3).SetCellValue("報修類別名稱");
                    ws.GetRow(0).CreateCell(4).SetCellValue("費用種類");
                    ws.GetRow(0).CreateCell(5).SetCellValue("維修細項_L1");
                    ws.GetRow(0).CreateCell(6).SetCellValue("維修細項_L2");
                    ws.GetRow(0).CreateCell(7).SetCellValue("維修細項_L3");
                    ws.GetRow(0).CreateCell(8).SetCellValue("單位");
                    ws.GetRow(0).CreateCell(9).SetCellValue("數量");
                    ws.GetRow(0).CreateCell(10).SetCellValue("單價");
                    ws.GetRow(0).CreateCell(11).SetCellValue("備註");
                }

                dt = GetCategoryData();
                ws = wb.CreateSheet("報修類別");

                if (dt != null && dt.Rows.Count > 0)
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

                return File(fileContents,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }

        private DataTable? GetCategoryData()
        {
            QuoteMgtHanlder _QuoteMgtHanlder = new QuoteMgtHanlder(_config, HttpContext);
            DataTable dtTable = _QuoteMgtHanlder.GetCategoryData();
            return dtTable;
        }

        private DataTable GetQueryData()
        {
            QuoteMgtHanlder _QuoteMgtHanlder = new QuoteMgtHanlder(_config, HttpContext);
            DataTable dtTable = _QuoteMgtHanlder.GetQueryData();
            return dtTable;
        }

        [HttpPost("[action]")]
        public IActionResult ImportStore(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "未選擇檔案" });

                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".xls" && ext != ".xlsx")
                    return Json(new { success = false, message = "檔案格式錯誤，只能上傳 Excel (.xls / .xlsx)" });

                string AttachFileName = LoginSession.Current.empno + "_" + DateTime.Now.ToString("HHmmss") + "_" + file.FileName;
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
                    file.CopyToAsync(stream);
                }

                QuoteMgtHanlder _QuoteMgtHanlder = new QuoteMgtHanlder(_config, HttpContext);
                var msg = _QuoteMgtHanlder.ImportStore(destFilePath);

                if (string.IsNullOrEmpty(msg) == false)
                {
                    return JsonValidFail(msg);
                }
                else
                    return JsonSuccess("匯入成功");
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }

        [HttpGet("[action]")]
        public IActionResult ExportStore()
        {
            try
            {
                string fileName = "export_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

                IWorkbook wb = new HSSFWorkbook();
                ISheet ws;
                ws = wb.CreateSheet("報修類別設定");

                DataTable dt = GetCategoryConfig();
                if (dt != null && dt.Rows.Count > 0)
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
                            if (string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                                continue;
                            ws.GetRow(i + 1).CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                }

                byte[] fileContents;
                // 輸出到 MemoryStream
                using (var stream = new MemoryStream())
                {
                    wb.Write(stream, true);
                    fileContents = stream.ToArray();
                }

                return File(fileContents,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }

        private DataTable GetCategoryConfig()
        {
            QuoteMgtHanlder _QuoteMgtHanlder = new QuoteMgtHanlder(_config, HttpContext);
            DataTable dtTable = _QuoteMgtHanlder.GetCategoryConfig();
            return dtTable;
        }

        [HttpPost("[action]")]
        public IActionResult SaveMarquee(string content)
        {
            try
            {
                QuoteMgtHanlder _QuoteMgtHanlder = new QuoteMgtHanlder(_config, HttpContext);
                _QuoteMgtHanlder.SaveMarquee(content);
                return JsonSuccess("儲存成功");
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }

        [HttpGet("[action]")]
        public IActionResult GetMarquee()
        {
            try
            {
                //txtPromt.Text = db.GetFieldData("CONFIG_VALUE", "MAINTAIN_CONFIG", "CONFIG_NAME='MARQUEE'");
                QuoteMgtHanlder _QuoteMgtHanlder = new QuoteMgtHanlder(_config, HttpContext);
                var result = _QuoteMgtHanlder.GetFieldData("CONFIG_VALUE", "MAINTAIN_CONFIG", new Dictionary<string, object>() { { "CONFIG_NAME", "MARQUEE" } });
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }
    }
}
