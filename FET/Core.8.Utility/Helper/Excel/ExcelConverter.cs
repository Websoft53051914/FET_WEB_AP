namespace Core.Utility.Helper.Excel
{
    /// <summary>
    /// 【EXCEL座標轉換】
    /// </summary>
    public class ExcelConverter
    {
        /// <summary>
        /// 欄位號碼轉成大寫英文字母字串(Ex:1->A,2->B,3->C,...,27->AA)
        /// </summary>
        /// <param name="columnNumber">欄位號碼</param>
        /// <returns>大寫英文字母字串</returns>
        public string NumberToColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        /// <summary>
        /// 英文字母字串轉成欄位號碼(Ex:A->1,B->2,C->3,...,AA->27)
        /// </summary>
        /// <param name="columnName">英文字母字串</param>
        /// <returns>欄位號碼</returns>
        public int ColumnNameToNumber(string columnName)
        {
            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }
    }
}
