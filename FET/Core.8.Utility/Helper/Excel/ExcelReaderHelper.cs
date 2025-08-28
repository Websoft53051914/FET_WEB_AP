using NPOI.SS.UserModel;

namespace Core.Utility.Helper.Excel
{

    public class ExcelReaderHelper:BaseExcel
    {
        /// <summary>
        /// 轉型錯誤的座標(實際座標 1A,1B...)
        /// </summary>
        List<string> castErrorCellRow = new();

        public ExcelReaderHelper()
        {
            cellValue = new ExcelReaderValueCell(this);
        }

        /// <summary>
        /// 取得欄位值的物件
        /// </summary>
        ExcelReaderValueCell cellValue = null;

        /// <summary>
        /// 取得欄位值的物件
        /// </summary>
        public ExcelReaderValueCell CellValue
        {
            get
            {
                return cellValue;
            }
        }
        /// <summary>
        /// 取得轉型錯誤的座標(實際座標 1A,1B...)
        /// </summary>
        /// <returns>座標清單</returns>
        public List<string> GetCastErrorCellRow()
        {
            return this.castErrorCellRow;
        }

        /// <summary>
        /// 取得字串值
        /// </summary>
        /// <returns>CELL的值</returns>
        public string GetStringValue()
        {
            ICell cell = GetCell();

            if (cell == null)
            {
                return null;
            }

            cell.SetCellType(CellType.String);

            string value = cell.StringCellValue;
            return value;
        }

        /// <summary>
        /// 依型態與預設值轉型
        /// </summary>
        /// <typeparam name="T">指定的類別</typeparam>
        /// <param name="convertFun">轉型方法</param>
        /// <param name="defaultValue">轉型失敗時使用的預設值</param>
        /// <returns></returns>
        private T GetValue<T>(Func<string, T> convertFun, T defaultValue)
        {
            string valueStr = GetStringValue();

            if (string.IsNullOrEmpty(valueStr))
            {
                return defaultValue;
            }
            T value = defaultValue;
            try
            {
                value = convertFun(valueStr);
            }
            catch (Exception)
            {
                //記錄轉型錯誤座標
                string pos = GetCellIdxStr(this.cellIndex) + "" + (this.rowIndex + 1);
                castErrorCellRow.Add(pos);
            }
            return value;
        }

        /// <summary>
        /// 取得數字值 long
        /// </summary>
        /// <returns>long 型態數值</returns>
        public long GetLongValue()
        {
            return GetValue(v => Convert.ToInt64(v), 0);
        }

        /// <summary>
        /// 取得數字值 int
        /// </summary>
        /// <returns>Int 型態數值</returns>
        public int GetIntValue()
        {
            return GetValue(v => Convert.ToInt32(v), 0);
        }

        /// <summary>
        /// 取得數字值 double
        /// </summary>
        /// <returns>double 型態數值</returns>
        public double GetDoubleValue()
        {
            return GetValue(v => Convert.ToDouble(v), 0);
        }

        /// <summary>
        /// 取得日期值
        /// </summary>
        /// <returns>日期 型態數值</returns>
        public DateTime GetDateTimeValue()
        {
            return GetValue(v => {
                DateTime dt = DateTime.FromOADate(Convert.ToInt32(v));
                return dt;
            }, DateTime.MinValue);
        }

        /// <summary>
        /// cell轉成實際座標
        /// </summary>
        /// <param name="cellIndex">指定的欄位</param>
        /// <returns>實際座標</returns>
        public static string GetCellIdxStr(int cellIndex)
        {
            string cellStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int char1Idx = (int)Math.Floor((double)(cellIndex / cellStr.Length));
            int char2Idx = (int)cellIndex % cellStr.Length;

            string char1 = char1Idx == 0 ? "" : cellStr[char1Idx - 1] + "";

            return char1 + cellStr[char2Idx];
        }
    }
}
