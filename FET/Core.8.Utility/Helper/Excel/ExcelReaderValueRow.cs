namespace Core.Utility.Helper.Excel
{
    public class ExcelReaderValueRow
    {
        ExcelReaderHelper readHelper;
        public ExcelReaderValueRow(ExcelReaderHelper readHelper)
        {
            this.readHelper = readHelper;
        }


        /// <summary>
        /// 取得cell的共用方法
        /// </summary>
        /// <typeparam name="T">指定的類型</typeparam>
        /// <param name="getValue">轉型方法</param>
        /// <returns>指定的類型</returns>
        private T RowValue<T>(Func<T> getValue)
        {
            T value = getValue();
            int index = this.readHelper.GetRowIndex();
            this.readHelper.SetRowIndex(index + 1);
            return value;
        }

        /// <summary>
        /// 取得cell值-String，自動換下一個cell
        /// </summary>
        /// <returns>當下cell值</returns>
        public string String()
        {
            return this.RowValue(() => this.readHelper.GetStringValue());
        }


        /// <summary>
        /// 取得cell值-DateTime，自動換下一個cell
        /// </summary>
        /// <returns>當下cell值</returns>
        public DateTime DateTime()
        {
            return this.RowValue(() => this.readHelper.GetDateTimeValue());
        }


        /// <summary>
        /// 取得cell值-Long，自動換下一個cell
        /// </summary>
        /// <returns>當下cell值</returns>
        public long Long()
        {
            return this.RowValue(() => this.readHelper.GetLongValue());
        }

        /// <summary>
        /// 取得cell值-Int，自動換下一個cell
        /// </summary>
        /// <returns>當下cell值</returns>
        public int Int()
        {
            return this.RowValue(() => this.readHelper.GetIntValue());
        }

        /// <summary>
        /// 取得cell值-Double，自動換下一個cell
        /// </summary>
        /// <returns>當下cell值</returns>
        public double Double()
        {
            return this.RowValue(() => this.readHelper.GetDoubleValue());
        }
    }
}
