using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Core.Utility.Helper.Excel
{
    /// <summary>
    /// 【excel】
    /// </summary>
    public class BaseExcel
    {
        /// <summary>
        /// 允許附檔名格式
        /// </summary>
        public static readonly HashSet<string> ALLOW_EXT_FROMAT = new(StringComparer.OrdinalIgnoreCase) { ".xls", ".xlsx" };
        protected IWorkbook wk = null;
        protected ISheet sheet = null;

        protected int rowIndex = 0;
        protected int cellIndex = 0;

        protected ExcelType excelType = ExcelType.XSSF;

        protected ICellStyle hlinkStyle = null;

        /// <summary>
        /// 建立work book
        /// </summary>
        /// <param name="type">Excel類型</param>
        /// <returns>work book</returns>
        public IWorkbook CreateWorkBook(ExcelType type)
        {
            if (type == ExcelType.HSSF)
            {
                this.wk = new HSSFWorkbook();
                excelType = ExcelType.HSSF;
            }
            else
            {
                this.wk = new XSSFWorkbook();
            }

            hlinkStyle = this.GetHLinkStyle();
            return this.wk;
        }

        /// <summary>
        /// 設定WorkBook
        /// </summary>
        /// <param name="filePath">檔案位置</param>
        /// <returns>work book</returns>
        public IWorkbook SetWorkBook(string filePath)
        {
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.ReadWrite);
            return SetWorkBook(fileStream);

        }

        /// <summary>
        /// 設定WorkBook
        /// </summary>
        /// <param name="fileByte">EXCEL檔案 byte array</param>
        /// <returns>work book</returns>
        public IWorkbook SetWorkBook(byte[] fileByte)
        {
            MemoryStream stream = new(fileByte);
            return SetWorkBook(stream);
        }

        /// <summary>
        /// 設定WorkBook
        /// </summary>
        /// <param name="stream">EXCEL檔案 數據流</param>
        /// <returns>work book</returns>
        public IWorkbook SetWorkBook(Stream stream)
        {
            this.wk = WorkbookFactory.Create(stream);
            hlinkStyle = this.GetHLinkStyle();
            return this.wk;
        }

        // TODO
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ICellStyle GetHLinkStyle()
        {
            ICellStyle hlink_style = wk.CreateCellStyle();
            IFont hlink_font = wk.CreateFont();
            hlink_font.Underline = FontUnderlineType.Single;
            hlink_font.Color = HSSFColor.Blue.Index;
            hlink_style.SetFont(hlink_font);

            return hlink_style;
        }

        /// <summary>
        /// 取得work book
        /// </summary>
        /// <returns>work book</returns>
        public IWorkbook GetWorkBook()
        {
            return this.wk;
        }

        /// <summary>
        /// 建立sheet
        /// </summary>
        /// <param name="name">sheet名稱</param>
        /// <returns>sheet</returns>
        public ISheet CreateSheet(string name)
        {
            this.sheet = this.wk.CreateSheet(name);
            return this.sheet;
        }

        /// <summary>
        /// 設定sheet
        /// </summary>
        /// <param name="sheet">sheet</param>
        public void SetSheet(ISheet sheet)
        {
            this.sheet = sheet;
        }

        /// <summary>
        /// 取得sheet
        /// </summary>
        /// <returns>當下使用sheet</returns>
        public ISheet GetSheet()
        {
            return this.sheet;
        }

        /// <summary>
        /// 設定Row位置
        /// </summary>
        /// <param name="rowIndex">指定的列數</param>
        public void SetRowIndex(int rowIndex)
        {
            this.rowIndex = rowIndex;
        }

        /// <summary>
        /// 設定cell位置
        /// </summary>
        /// <param name="cellIndex">指定的欄數</param>
        public void SetCellIndex(int cellIndex)
        {
            this.cellIndex = cellIndex;
        }

        /// <summary>
        /// 設定row和cell位置
        /// </summary>
        /// <param name="rowIndex">指定的列數</param>
        /// <param name="cellIndex">指定的欄數</param>
        public void SetRowCellIndex(int rowIndex, int cellIndex)
        {
            this.rowIndex = rowIndex;
            this.cellIndex = cellIndex;
        }

        /// <summary>
        /// 取得目前row index
        /// </summary>
        /// <returns>當下第幾列</returns>
        public int GetRowIndex()
        {
            return this.rowIndex;
        }

        /// <summary>
        /// 取得目前cell index
        /// </summary>
        /// <returns>當下第幾欄</returns>
        public int GetCellIndex()
        {
            return this.cellIndex;
        }

        /// <summary>
        /// 下一列
        /// </summary>
        public void NextRow()
        {
            this.rowIndex++;
            this.cellIndex = 0;
        }
        /// <summary>
        /// 下一個cell
        /// </summary>
        public void NextCell()
        {
            this.cellIndex++;
            // TODO: 20250210-切換至下一個cell時，通常會在同一列，是否 rowIndex 應該不需要歸零？
            //this.rowIndex = 0;
        }

        /// <summary>
        /// 取得cell物件
        /// </summary>
        /// <returns>cell</returns>
        public ICell GetCell()
        {
            ICell cell = GetRow().GetCell(this.cellIndex);
            cell ??= GetRow().CreateCell(this.cellIndex);

            return cell;
        }

        /// <summary>
        /// 取得row物件
        /// </summary>
        /// <returns>列</returns>
        public IRow GetRow()
        {
            IRow row = this.sheet.GetRow(this.rowIndex);
            row ??= this.sheet.CreateRow(this.rowIndex);

            return row;
        }
    }

    /// <summary>
    /// excel檔案型態
    /// </summary>
    public enum ExcelType
    {
        XSSF,
        HSSF
    }
}
