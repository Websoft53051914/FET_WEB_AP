using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Data;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FTT_API.Controllers.StoreMgt
{
    [Route("[controller]")]
    [IgnoreAntiforgeryToken]
    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class StoreMgtController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public StoreMgtController(ConfigurationHelper configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPageList(DataSourceRequest request, Store_profileDTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);

                vm.USERROLE = _sessionVO.userrole;
                vm.IVRCODE = _sessionVO.ivrcode;
                vm.EMPNO = _sessionVO.empno;

                var list = _StoreMgtHandler.FindPageList(pageEntity, vm);

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

        
        [HttpGet("[action]")]
        public async Task<IActionResult> GetSelectContent(Store_profileDTO vm)
        {
            try
            {
                var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);
                Store_profile_selectVM result = new Store_profile_selectVM();

                result.company_leaves = new List<SelectListItem>();
                result.store_types = new List<SelectListItem>();
                result.channels = new List<SelectListItem>();
                result.area = new List<SelectListItem>();

                var company_leaves = _StoreMgtHandler.GetSTORE_TYPE_TYPE_VALUE("COMPANY_LEAVES");
                if (company_leaves != null && company_leaves.Count > 0) result.company_leaves = company_leaves.OrderBy(s => s.type_value).Select(s => new SelectListItem() { Text = s.type_value, Value = s.type_value }).ToList();

                var storeTypes = _StoreMgtHandler.GetSTORE_TYPE_TYPE_VALUE("STORE_TYPE");
                if (storeTypes != null && storeTypes.Count > 0) result.store_types = storeTypes.OrderBy(s => s.type_value).Select(s => new SelectListItem() { Text = s.type_value, Value = s.type_value }).ToList();

                var channels = _StoreMgtHandler.GetSTORE_TYPE_TYPE_VALUE("CHANNEL");
                if (channels != null && channels.Count > 0) result.channels = channels.OrderBy(s => s.type_value).Select(s => new SelectListItem() { Text = s.type_value, Value = s.type_value }).ToList();

                var area = _StoreMgtHandler.GetSTORE_PROFILE_AREA();
                if (area != null && area.Count > 0) result.area = area.Select(s => new SelectListItem() { Text = s.area, Value = s.area }).ToList();
                List<string> tempArr = new List<string>() { "N1", "N2", "N3", "C1", "C2", "S1", "S2", "N", "C", "S" };
                foreach (var item in tempArr)
                {
                    if (result.area.Where(s => s.Text == item).Count() == 0)
                        result.area.Add(new SelectListItem() { Text = item, Value = item });
                }
                if (result.area != null && result.area.Count > 0)
                    result.area = result.area.OrderBy(s => s.Text).ToList();

                var decoration_conditions = _StoreMgtHandler.GetSTORE_TYPE_TYPE_VALUE("DECORATION_CONDITION");
                if (decoration_conditions != null && decoration_conditions.Count > 0) result.decoration_conditions = decoration_conditions.OrderBy(s => s.type_value).Select(s => new SelectListItem() { Text = s.type_value, Value = s.type_value }).ToList();

                this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

        
        [HttpPost("[action]")]
        public async Task<IActionResult> GetEmpList(DataSourceRequest request, fet_user_profileDTO vm)
        {
            try
            {
                var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);
                PageEntity pageEntity = base.GetPageEntity(request);
                var list = _StoreMgtHandler.GetEmpPageList(pageEntity, vm);

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

        public class Store_profile_selectVM
        {
            public List<SelectListItem> company_leaves { get; set; }
            public List<SelectListItem> store_types { get; set; }
            public List<SelectListItem> channels { get; set; }
            public List<SelectListItem> area { get; set; }
            public List<SelectListItem> decoration_conditions { get; set; }

        }


        
        [HttpGet("[action]")]
        public async Task<IActionResult> GetDetail(string ivrcode)
        {
            try
            {
                var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);
                Store_profileDTO result = _StoreMgtHandler.GetDetail(ivrcode);
                this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

        
        [HttpPost("[action]")]
        public IActionResult ImportRetail(IFormFile file)
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

                StoreMgtHandler _StoreMgtHanlder = new StoreMgtHandler(_config, HttpContext);
                var msg = _StoreMgtHanlder.ImportRetail(destFilePath);

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
        public IActionResult ExportRetail()
        {
            try
            {
                string fileName = "retail_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

                IWorkbook wb = new HSSFWorkbook();
                ISheet ws;
                ws = wb.CreateSheet("直營門市");

                DataTable dt = GetRetailData();
                if (dt.Rows.Count > 0)
                {
                    //ws = wb.CreateSheet(dt.TableName);

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
                    ws.GetRow(0).CreateCell(1).SetCellValue("店名");
                    ws.GetRow(0).CreateCell(2).SetCellValue("區域");
                    ws.GetRow(0).CreateCell(3).SetCellValue("店長員編");
                    ws.GetRow(0).CreateCell(4).SetCellValue("店長名字");
                    ws.GetRow(0).CreateCell(5).SetCellValue("區主管員編");
                    ws.GetRow(0).CreateCell(6).SetCellValue("區主管名字");
                    ws.GetRow(0).CreateCell(7).SetCellValue("週一~五");
                    ws.GetRow(0).CreateCell(8).SetCellValue("星期六");
                    ws.GetRow(0).CreateCell(9).SetCellValue("星期日");
                    ws.GetRow(0).CreateCell(10).SetCellValue("國定假日");
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

        private DataTable GetRetailData()
        {
            var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);
            DataTable dtTable = _StoreMgtHandler.GetRetailData();


            return dtTable;
        }


        
        [HttpPost("[action]")]
        public IActionResult ImportVass(IFormFile file)
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

                StoreMgtHandler _StoreMgtHanlder = new StoreMgtHandler(_config, HttpContext);
                var msg = _StoreMgtHanlder.ImportVass(destFilePath);

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
        public IActionResult ExportVass()
        {
            try
            {
                string fileName = "vass_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

                IWorkbook wb = new HSSFWorkbook();
                ISheet ws;
                ws = wb.CreateSheet("加盟門市");

                DataTable dt = GetVassData();
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
                    ws.GetRow(0).CreateCell(1).SetCellValue("店名");
                    ws.GetRow(0).CreateCell(2).SetCellValue("區域");
                    ws.GetRow(0).CreateCell(3).SetCellValue("業務員編");
                    ws.GetRow(0).CreateCell(4).SetCellValue("業務名字");
                    ws.GetRow(0).CreateCell(5).SetCellValue("週一~五");
                    ws.GetRow(0).CreateCell(6).SetCellValue("星期六");
                    ws.GetRow(0).CreateCell(7).SetCellValue("星期日");
                    ws.GetRow(0).CreateCell(8).SetCellValue("國定假日");
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

        private DataTable GetVassData()
        {
            var _StoreMgtHandler = new StoreMgtHandler(_config, HttpContext);
            DataTable dtTable = _StoreMgtHandler.GetVassData();
            return dtTable;
        }
    }
}
