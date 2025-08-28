using NPOI.SS.UserModel;

namespace Core.Utility.Helper.Excel
{
    public class ExcelReaderValueCell
    {
        ExcelReaderHelper readHelper;
        public ExcelReaderValueCell(ExcelReaderHelper readHelper)
        {
            this.readHelper = readHelper;
        }

        /// <summary>
        /// 取得cell的共用方法
        /// </summary>
        /// <typeparam name="T">指定的類別</typeparam>
        /// <param name="getValue"></param>
        /// <returns>指定的類別</returns>
        private T CellValue<T>(Func<T> getValue)
        {
            T value = getValue();
            int cellIdx = this.readHelper.GetCellIndex();
            this.readHelper.SetCellIndex(cellIdx + 1);
            return value;
        }

        /// <summary>
        /// 取得cell值-String，自動換下一個cell
        /// </summary>
        /// <returns>cell值</returns>
        public string String()
        {
            return this.CellValue(() => this.readHelper.GetStringValue());
        }


        /// <summary>
        /// 取得cell值-DateTime，自動換下一個cell
        /// </summary>
        /// <returns>cell值</returns>
        public DateTime DateTime()
        {
            IDataFormat dataFormatCustom = this.readHelper.GetWorkBook().CreateDataFormat();
            this.readHelper.GetCell().CellStyle.DataFormat = dataFormatCustom.GetFormat("yyyy/MM/dd");
            return this.CellValue(() => this.readHelper.GetDateTimeValue());
        }


        /// <summary>
        /// 取得cell值-Long，自動換下一個cell
        /// </summary>
        /// <returns>cell值</returns>
        public long Long()
        {
            return this.CellValue(() => this.readHelper.GetLongValue());
        }

        /// <summary>
        /// 取得cell值-Int，自動換下一個cell
        /// </summary>
        /// <returns>cell值</returns>
        public int Int()
        {
            return this.CellValue(() => this.readHelper.GetIntValue());
        }

        /// <summary>
        /// 取得cell值-Double，自動換下一個cell
        /// </summary>
        /// <returns>cell值</returns>
        public double Double()
        {
            return this.CellValue(() => this.readHelper.GetDoubleValue());
        }
    }
}
