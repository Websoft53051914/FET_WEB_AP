using Const;
using Const.DTO;
using Const.VO;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Helper.Excel;
using Core.Utility.Web.EX;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace FTT_API.Controllers.CIConfig
{
    /// <summary>
    /// 例外派工維護 API
    /// </summary>
    [Route("[controller]")]
    public partial class CIConfigController : BaseProjectController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CIConfigController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        private ConfigurationHelper _configHelper;
    }

    public partial class CIConfigController
    {
        /// <summary>
        /// 取得分頁資料<para/>
        /// [/Storemgt/CIConfig.aspx.cs]gridView_DataBind()
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        
        public IActionResult GetPageList(DataSourceRequest request, CIConfigIndexVO vm)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(vm);
                CIConfigHandler cIConfigHandler = new(_configHelper);
                // 取得資料
                PageResult<CIExceptionConfigDTO> pageList = cIConfigHandler.GetPageList(GetPageEntity(request), new CIExceptionConfigDTO
                {
                    ShopNameLike = vm.ShopNameLike,
                });
                // 轉成 ViewModel
                List<CIConfigGridVO> dataList = [];
                for (int i = 0; i < pageList.Results.Count; i++)
                {
                    CIExceptionConfigDTO data = pageList.Results[i];

                    CIConfigGridVO item = new()
                    {
                        Cisid = data.cisid,
                        VendorId = data.vendor_id,
                        Ivrcode = data.ivrcode,
                        Aciname = data.aciname,
                        MerchantName = data.merchant_name,
                        ShopName = data.shop_name,
                        ApprovalDateText = data.approval_date_text,
                    };

                    dataList.Add(item);
                }

                this.LogSuccess();

                return JsonPage(new DataSourceResult
                {
                    Data = dataList,
                    Total = pageList.DataCount,
                });
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 匯出Excel<para/>
        /// [/Storemgt/CIConfig.aspx.cs]btnExport_Click()<para/>
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        
        public IActionResult ExportExcel(string jsonData)
        {
            try
            {
                ArgumentNullException.ThrowIfNullOrWhiteSpace(jsonData);
                CIConfigIndexVO? vm = JsonConvert.DeserializeObject<CIConfigIndexVO>(jsonData);
                DataSourceRequest? request = JsonConvert.DeserializeObject<DataSourceRequest>(jsonData);
                ArgumentNullException.ThrowIfNull(vm);
                ArgumentNullException.ThrowIfNull(request);
                CIConfigHandler cIConfigHandler = new(_configHelper);
                CommonHandler commonHandler = new(_configHelper, cIConfigHandler.GetDBHelper());
                CIExceptionConfigDTO searchVO = new()
                {
                    ShopNameLike = vm.ShopNameLike,
                };

                ExcelWriterHelper writer = new();
                IWorkbook wb = writer.CreateWorkBook(ExcelType.XSSF);

                BuildSheet1(writer, request, cIConfigHandler, searchVO);
                BuildSheetCiData(writer, cIConfigHandler);
                BuildSheetStore(writer, cIConfigHandler);
                BuildSheetVender(writer, commonHandler);

                MemoryStream? memoryStream = null;
                using (MemoryStream stream = new())
                {
                    writer.GetWorkBook().Write(stream, false);
                    memoryStream = new MemoryStream(stream.ToArray());
                }

                writer.GetWorkBook().Close();

                this.LogSuccess();

                return File(memoryStream, "application/vnd.ms-excel", $"export_{DateTime.Now.ToString(DbConst.FORMAT_SHORTDATETIME)}.xlsx");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        #region -- 匯出Excel 子方法 --
        /// <summary>
        /// 產生 Sheet1
        /// </summary>
        protected void BuildSheet1(ExcelWriterHelper writer, DataSourceRequest request, CIConfigHandler handler, CIExceptionConfigDTO searchVO)
        {
            ISheet sheet = writer.CreateSheet("Sheet1");
            writer.SetRowCellIndex(0, 0);
            #region -- 設定標題 --
            writer.SetCellValue("CISID");
            writer.SetCellValue("報修類別");
            writer.SetCellValue("廠商代碼");
            writer.SetCellValue("廠商名稱");
            writer.SetCellValue("IVRCODE");
            writer.SetCellValue("門市名稱");
            writer.SetCellValue("驗收日期");
            writer.SetCellValue("執行註記");
            #endregion -- 設定標題 --

            // 調整欄寬
            for (int i = 0; i < writer.GetRow().LastCellNum; i++)
            {
                writer.SetRowCellIndex(0, i);
                int lenTitle = Encoding.UTF8.GetByteCount(writer.GetCell().StringCellValue);
                writer.GetSheet().SetColumnWidth(writer.GetCellIndex(), (lenTitle + 6) * 256);
            }

            int totalPage = 1;
            int currentPage = 1;
            int pageDataSize = 100;
            int posRow = 1;
            request.pageSize = pageDataSize;
            do
            {
                request.pageIndex = currentPage;

                PageEntity pageEntity = GetPageEntity<DispatchRuleMgtQueryVO>(request);
                // 取得資料
                PageResult<CIExceptionConfigDTO> pageList = handler.GetPageList(pageEntity, searchVO);

                if (currentPage == 1)
                {
                    totalPage = (int)Math.Ceiling((double)pageList.DataCount / pageDataSize);
                }

                if (pageList.Results.Count == 0)
                {
                    break;
                }

                foreach (CIExceptionConfigDTO item in pageList.Results)
                {
                    writer.SetRowCellIndex(posRow, 0);
                    writer.SetCellValue(item.cisid ?? string.Empty);
                    writer.SetCellValue(item.aciname ?? string.Empty);
                    writer.SetCellValue(item.vendor_id ?? string.Empty);
                    writer.SetCellValue(item.merchant_name ?? string.Empty);
                    writer.SetCellValue(item.ivrcode ?? string.Empty);
                    writer.SetCellValue(item.shop_name ?? string.Empty);
                    writer.SetCellValue(item.approval_date_text ?? string.Empty);
                    writer.SetCellValue("U");

                    posRow++;
                }

                currentPage++;
            } while (currentPage <= totalPage);
        }

        /// <summary>
        /// 產生 Sheet 報修品項類別
        /// </summary>
        protected void BuildSheetCiData(ExcelWriterHelper writer, CIConfigHandler handler)
        {
            ISheet sheet = writer.CreateSheet("報修品項類別");
            writer.SetRowCellIndex(0, 0);
            #region -- 設定標題 --
            writer.SetCellValue("CISID");
            writer.SetCellValue("報修類別");
            writer.SetCellValue("L1NAME");
            writer.SetCellValue("L2NAME");
            writer.SetCellValue("L3NAME");
            writer.SetCellValue("L4NAME");
            #endregion -- 設定標題 --

            // 調整欄寬
            for (int i = 0; i < writer.GetRow().LastCellNum; i++)
            {
                writer.SetRowCellIndex(0, i);
                int lenTitle = Encoding.UTF8.GetByteCount(writer.GetCell().StringCellValue);
                writer.GetSheet().SetColumnWidth(writer.GetCellIndex(), (lenTitle + 6) * 256);
            }

            List<CIDataDTO> dataList = handler.GetListCIData();
            int posRow = 1;
            foreach (CIDataDTO item in dataList)
            {
                writer.SetRowCellIndex(posRow, 0);
                writer.SetCellValue(item.cisid ?? string.Empty);
                writer.SetCellValue(item.aciname ?? string.Empty);
                writer.SetCellValue(item.l1Name ?? string.Empty);
                writer.SetCellValue(item.l2Name ?? string.Empty);
                writer.SetCellValue(item.l3Name ?? string.Empty);
                writer.SetCellValue(item.l4Name ?? string.Empty);

                posRow++;
            }
        }

        /// <summary>
        /// 產生 Sheet 門市列表
        /// </summary>
        protected void BuildSheetStore(ExcelWriterHelper writer, CIConfigHandler handler)
        {
            ISheet sheet = writer.CreateSheet("門市列表");
            writer.SetRowCellIndex(0, 0);
            #region -- 設定標題 --
            writer.SetCellValue("IVRCODE");
            writer.SetCellValue("店名");
            writer.SetCellValue("店格");
            writer.SetCellValue("通路");
            writer.SetCellValue("區域");
            writer.SetCellValue("EMAIL ADDRESS");
            writer.SetCellValue("地址");
            #endregion -- 設定標題 --

            // 調整欄寬
            for (int i = 0; i < writer.GetRow().LastCellNum; i++)
            {
                writer.SetRowCellIndex(0, i);
                int lenTitle = Encoding.UTF8.GetByteCount(writer.GetCell().StringCellValue);
                writer.GetSheet().SetColumnWidth(writer.GetCellIndex(), (lenTitle + 6) * 256);
            }

            List<StoreProfileDTO> dataList = handler.GetListStoreProfile();
            int posRow = 1;
            foreach (StoreProfileDTO item in dataList)
            {
                writer.SetRowCellIndex(posRow, 0);
                writer.SetCellValue(item.ivr_code ?? string.Empty);
                writer.SetCellValue(item.shop_name ?? string.Empty);
                writer.SetCellValue(item.store_type ?? string.Empty);
                writer.SetCellValue(item.channel ?? string.Empty);
                writer.SetCellValue(item.area ?? string.Empty);
                writer.SetCellValue(item.email ?? string.Empty);
                writer.SetCellValue(item.address ?? string.Empty);

                posRow++;
            }
        }

        /// <summary>
        /// 產生 Sheet 廠商代碼
        /// </summary>
        protected void BuildSheetVender(ExcelWriterHelper writer, CommonHandler handler)
        {
            ISheet sheet = writer.CreateSheet("廠商代碼");
            writer.SetRowCellIndex(0, 0);
            #region -- 設定標題 --
            writer.SetCellValue("廠商代碼");
            writer.SetCellValue("廠商名稱");
            writer.SetCellValue("聯絡人");
            writer.SetCellValue("聯絡人電話");
            writer.SetCellValue("聯絡人EMAIL");
            #endregion -- 設定標題 --

            // 調整欄寬
            for (int i = 0; i < writer.GetRow().LastCellNum; i++)
            {
                writer.SetRowCellIndex(0, i);
                int lenTitle = Encoding.UTF8.GetByteCount(writer.GetCell().StringCellValue);
                writer.GetSheet().SetColumnWidth(writer.GetCellIndex(), (lenTitle + 6) * 256);
            }

            List<StoreVenderProfileDTO> dataList = handler.GetListCommonVender();
            int posRow = 1;
            foreach (StoreVenderProfileDTO item in dataList)
            {
                writer.SetRowCellIndex(posRow, 0);
                writer.SetCellValue(item.order_id.ToString());
                writer.SetCellValue(item.merchant_name ?? string.Empty);
                writer.SetCellValue(item.cp_name ?? string.Empty);
                writer.SetCellValue(item.cp_tel ?? string.Empty);
                writer.SetCellValue(item.email ?? string.Empty);

                posRow++;
            }
        }
        #endregion -- 匯出Excel 子方法 --

        /// <summary>
        /// 匯入Excel<para/>
        /// [/Storemgt/CIConfig.aspx.cs]btnImport_Click()<para/>
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        
        public IActionResult ImportExcel(IFormFile? file)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(file);
                if (file.Length == 0)
                {
                    throw new ArgumentException(nameof(file) + "_size");
                }
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".xls" && ext != ".xlsx")
                {
                    throw new ArgumentException(nameof(file) + "_ext");
                }

                string pathOutputPath = _configHelper.GetOutputPath();
                if (string.IsNullOrEmpty(pathOutputPath))
                {
                    this.LogSuccess();

                    return JsonValidFail("未設定 OutputPath 參數");
                }
                else if (!Directory.Exists(pathOutputPath))
                {
                    Directory.CreateDirectory(pathOutputPath);
                }

                // 只取檔名，不包含任何路徑
                var safeFileName = Path.GetFileName(file.FileName);

                // 加上前綴，生成最終檔名
                string attachFileName = _sessionVO.empno + "_" + DateTime.Now.ToString("HHmmss") + "_" + safeFileName;

                // 組合安全的資料夾路徑
                string pathFile = Path.Combine(pathOutputPath, attachFileName);

                // 儲存檔案
                using (var stream = new FileStream(pathFile, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                CIConfigHandler cIConfigHandler = new(_configHelper);
                CommonHandler commonHandler = new(_configHelper, cIConfigHandler.GetDBHelper());

                ExcelReaderHelper reader = new();
                IWorkbook wb = reader.SetWorkBook(pathFile);

                int idxSheet1 = wb.GetSheetIndex("Sheet1");
                if (idxSheet1 == -1)
                {
                    this.LogSuccess();
                    return JsonValidFail("Excel 中無「Sheet1」");
                }

                List<CIExceptionConfigDTO> dtoList = [];
                reader.SetSheet(wb.GetSheetAt(idxSheet1));
                for (int i = 1; i <= reader.GetSheet().LastRowNum; i++)
                {
                    try
                    {
                        CIExceptionConfigDTO dto = new();
                        reader.SetRowCellIndex(i, 0);
                        dto.cisid = reader.GetStringValue()?.Trim() ?? "0";
                        reader.SetRowCellIndex(i, 2);
                        dto.vendor_id = reader.GetStringValue()?.Trim() ?? "0";
                        reader.SetRowCellIndex(i, 4);
                        dto.ivrcode = reader.GetStringValue()?.Trim() ?? "0";
                        reader.SetRowCellIndex(i, 6);
                        dto.approval_date_text = reader.GetStringValue()?.Trim() ?? "0";
                        reader.SetRowCellIndex(i, 7);
                        dto.flag = reader.GetStringValue()?.Trim() ?? "0";

                        if (DateTime.TryParse(dto.approval_date_text, out DateTime odAppro))
                        {
                            dto.approval_date = odAppro;
                            //dto.approval_date_text = odAppro.ToString(DbConst.FORMAT_DATE2);
                        }
                        else if (double.TryParse(dto.approval_date_text, out double odoApprovalDate))
                        {
                            dto.approval_date = DateTime.FromOADate(odoApprovalDate);
                            //dto.approval_date_text = DateTime.FromOADate(odoApprovalDate).ToString(DbConst.FORMAT_DATE2);
                        }
                        else
                        {
                            GetMessage().SetAlert($"第{i + 1}列 日期格式錯誤");
                        }
                        if (string.IsNullOrEmpty(dto.flag)
                            || (dto.flag != "A" && dto.flag != "U" && dto.flag != "D"))
                        {
                            GetMessage().SetAlert($"第{i + 1}列 需輸入執行註記(A：新增 U：更新 D：刪除)!");
                        }
                        else if (dto.flag == "A")
                        {
                            if (cIConfigHandler.CheckDataExist("CI_EXCEPTION_CONFIG", new Dictionary<string, object>
                            {
                                { "CISID",dto.cisid },
                                { "IVRCODE",dto.ivrcode },
                                { "ENABLE", 'Y' },
                            }) || dtoList.Any(x => x.cisid == dto.cisid && x.ivrcode == dto.ivrcode && x.flag == "A"))
                            {
                                GetMessage().SetAlert($"第{i + 1}列 已存在此筆相同IVRCODE+CISID!");
                            }
                            else if (!cIConfigHandler.CheckExistCisId(dto.cisid))
                            {
                                GetMessage().SetAlert($"第{i + 1}列 無此({dto.cisid})報修品項!");
                            }
                            else if (!cIConfigHandler.CheckDataExist("STORE_PROFILE", new Dictionary<string, object>
                            {
                                { "IVR_CODE",dto.ivrcode },
                            }))
                            {
                                GetMessage().SetAlert($"第{i + 1}列 無此({dto.ivrcode})門市!");
                            }
                            else if (!cIConfigHandler.CheckDataExist("STORE_VENDER_PROFILE", new Dictionary<string, object>
                            {
                                { "ORDER_ID",dto.vendor_id },
                            }))
                            {
                                GetMessage().SetAlert($"第{i + 1}列 無此({dto.vendor_id})廠商代碼!");
                            }
                            else
                            {
                                dtoList.Add(dto);
                            }
                        }
                        else if (dto.flag == "D")
                        {
                            if (cIConfigHandler.CheckDataExist("CI_EXCEPTION_CONFIG", new Dictionary<string, object>
                            {
                                { "CISID",dto.cisid },
                                { "IVRCODE",dto.ivrcode },
                                { "ENABLE", 'Y' },
                            }))
                            {
                                dtoList.Add(dto);
                            }
                        }
                        else if (dto.flag == "U")
                        {
                            if (!cIConfigHandler.CheckDataExist("CI_EXCEPTION_CONFIG", new Dictionary<string, object>
                            {
                                { "CISID",dto.cisid },
                                { "IVRCODE",dto.ivrcode },
                                { "ENABLE", 'Y' },
                            }))
                            {
                                GetMessage().SetAlert($"第{i + 1}列 無此筆存在!");
                            }
                            else if (!cIConfigHandler.CheckDataExist("STORE_VENDER_PROFILE", new Dictionary<string, object>
                            {
                                { "ORDER_ID",dto.vendor_id },
                            }))
                            {
                                GetMessage().SetAlert($"第{i + 1}列 無此({dto.vendor_id})廠商代碼!");
                            }
                            else
                            {
                                dtoList.Add(dto);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogError($"{pathFile}；{i + 1}；{ex}");
                        GetMessage().SetAlert($"第{i + 1}列 處理時發生錯誤!");
                    }
                }

                reader.GetWorkBook().Close();
                if (GetMessage().IsError())
                {
                    this.LogSuccess();
                    return JsonValidFail(GetMessage().GetAlert());
                }

                cIConfigHandler.BatchUpdate(dtoList);

                this.LogSuccess();
                return JsonOK();
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
