using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Core.Utility.Helper.Excel
{
    public class ExcelWriterHelper : ExcelReaderHelper
    {
        /// <summary>
        /// 寫入cell值-字串，自動換下一個cell
        /// </summary>
        /// <param name="value">指定值</param>
        /// <returns>下一欄cell</returns>
        public ICell SetCellValue(string value)
        {
            ICell cell = GetCell();
            cell.SetCellValue(value);
            this.cellIndex++;
            return cell;
        }


        /// <summary>
        /// 寫入cell值-日期，自動換下一個cell
        /// </summary>
        /// <returns>下一欄cell</returns>

        public ICell SetCellValue(DateTime value)
        {
            ICell cell = GetCell();
            cell.SetCellValue(value);
            this.cellIndex++;
            return cell;
        }


        /// <summary>
        /// 寫入cell值-數值，自動換下一個cell
        /// </summary>
        /// <returns>下一欄cell</returns>
        public ICell SetCellValue(double value)
        {
            ICell cell = GetCell();
            cell.SetCellValue(value);
            this.cellIndex++;
            return cell;
        }

        /// <summary>
        /// 寫入cell 公式，自動換下一個cell
        /// </summary>
        /// <returns>下一欄cell</returns>
        public ICell SetCellCellFormula(string value)
        {
            ICell cell = GetCell();
            cell.CellFormula = value;
            this.cellIndex++;
            return cell;
        }

        /// <summary>
        /// 寫入cell 超連結，自動換下一個cell
        /// </summary>
        /// <returns>下一欄cell</returns>
        public ICell SetCellHyperlink(string value, string sheetName)
        {
            ICell cell = GetCell();

            IHyperlink hyperLinkObj = this.GetHyperlinkObj();
            hyperLinkObj.Address = "" + sheetName + "!A1";
            hyperLinkObj.Label = value;
            cell.Hyperlink = hyperLinkObj;

            cell.SetCellValue(value);
            cell.CellStyle = hlinkStyle;

            this.cellIndex++;
            return cell;
        }


        /// <summary>
        /// 寫入row值-字串，自動換下一個row的cell
        /// </summary>
        /// <returns>下一欄cell</returns>
        public ICell SetRowValue(string value)
        {
            ICell cell = GetCell();
            cell.SetCellValue(value);
            this.rowIndex++;
            return cell;
        }


        /// <summary>
        /// 寫入row值-日期，自動換下一個row的cell
        /// </summary>
        /// <returns>下一欄cell</returns>
        public ICell SetRowValue(DateTime value)
        {
            ICell cell = GetCell();
            cell.SetCellValue(value);
            this.rowIndex++;
            return cell;
        }


        /// <summary>
        /// 寫入row值-數值，自動換下一個row的cell
        /// </summary>
        /// <returns>下一欄cell</returns>
        public ICell SetRowValue(double value)
        {
            ICell cell = GetCell();
            cell.SetCellValue(value);
            this.rowIndex++;
            return cell;
        }

        /// <summary>
        /// 寫入row值-數值，自動換下一個row的cell
        /// </summary>
        /// <returns>下一欄cell</returns>
        public ICell SetRowCellFormula(string value)
        {
            ICell cell = GetCell();
            cell.SetCellFormula(value);
            this.rowIndex++;
            return cell;
        }



        /// <summary>
        /// 寫入cell 超連結，自動換下一個cell
        /// </summary>
        /// <returns>下一欄cell</returns>
        public ICell SetRowHyperlink(string value, string sheetName)
        {
            IHyperlink hyperLinkObj = this.GetHyperlinkObj();
            hyperLinkObj.Address = sheetName + "!A1";
            hyperLinkObj.Label = value;
            ICell cell = GetCell();
            cell.Hyperlink = hyperLinkObj;
            cell.CellStyle = base.hlinkStyle;

            this.cellIndex++;
            return cell;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void SaveTo(string path)
        {
            using FileStream file = new(path, FileMode.Create);
            base.wk.Write(file, false);
        }

        /// <summary>
        /// 寫到memory stream
        /// </summary>
        /// <returns></returns>
        public MemoryStream SaveToStream()
        {
            MemoryStream memoryStream = null;

            using (MemoryStream stream = new())
            {
                base.wk.Write(stream, false);
                memoryStream = new MemoryStream(stream.ToArray());
            }

            return memoryStream;
        }

        /// <summary>
        /// 取得超連結物件
        /// </summary>
        /// <returns></returns>
        private IHyperlink GetHyperlinkObj()
        {
            IHyperlink obj;
            if (this.excelType == ExcelType.HSSF)
            {
                obj = new HSSFHyperlink(HyperlinkType.Document);
            }
            else
            {
                obj = new XSSFHyperlink(HyperlinkType.Document);
            }
            return obj;
        }
    }
}
