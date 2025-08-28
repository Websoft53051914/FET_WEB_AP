using NPOI.SS.UserModel;
using System.Text;

namespace Core.Utility.Helper.Excel
{
    /// <summary>
    /// 驗証excel使用
    /// </summary>
    public class ExcelVaildHelper
    {
        ExcelReaderHelper readerHelper = null;
        //int cellSize = 0;

        /// <summary>
        /// 空值row記錄
        /// </summary>
        List<string> errorRow_Null = new();

        /// <summary>
        /// 未重覆資料記錄
        /// </summary>
        HashSet<string> noExistList = new();
        /// <summary>
        /// 重覆資料記錄
        /// </summary>
        HashSet<string> existList = new();

        //自訂的錯誤訊息
        Dictionary<string, StringBuilder> customMsg = new();

        public ExcelVaildHelper(ExcelReaderHelper readerHelper)
        {
            this.readerHelper = readerHelper;
        }

        /// <summary>
        /// 判斷整個row有沒有空值，有空值則記錄錯誤log，不會自動換下個row，全空值則不記錄log
        /// </summary>
        /// <param name="cellIndex">第幾欄</param>
        /// <returns>是/否</returns>
        public bool IsRowHasNull(int cellIndex)
        {
            //目前cell座標
            int nowCellIdx = readerHelper.GetCellIndex();
            bool isRowHasNull = false;
            int nullSum = 0;
            for (int cellIdx = 0; cellIdx < cellIndex; cellIdx++)
            {
                readerHelper.SetCellIndex(cellIdx);
                ICell cell = readerHelper.GetCell();
                cell.SetCellType(CellType.String);
                string value = cell.StringCellValue;

                if (cell == null || string.IsNullOrEmpty(value))
                {
                    nullSum++;
                }
            }

            //有一個cell沒有值則記錄該row的index
            if (nullSum > 0 && nullSum < cellIndex)
            {
                errorRow_Null.Add(readerHelper.GetRowIndex() + 1 + "");//記錄空值的row
                isRowHasNull = true;
            }

            //全空值不記錄
            if (nullSum > 0 && nullSum == cellIndex)
            {
                isRowHasNull = true;
            }

            readerHelper.SetCellIndex(nowCellIdx);
            return isRowHasNull;
        }

        /// <summary>
        /// 取得有空值的row的清單
        /// </summary>
        /// <returns>空值清單</returns>
        public List<string> GetNullRowList()
        {
            return this.errorRow_Null;
        }

        /// <summary>
        /// 確認是否重覆
        /// </summary>
        /// <param name="data">指定的值</param>
        /// <returns>是/否</returns>
        public bool Exist(string data)
        {
            if (this.noExistList.Contains(data))
            {
                this.existList.Add(data);
                return true;
            }
            this.noExistList.Add(data);

            return false;
        }

        /// <summary>
        /// 取得重覆的清單
        /// </summary>
        /// <returns>重複清單</returns>
        public IEnumerable<string> GetExistList()
        {
            return this.existList;
        }

        /// <summary>
        /// 設定自訂的錯誤訊息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="message">訊息</param>
        public void AddCustomMsg(string key, string message)
        {
            if (!this.customMsg.ContainsKey(key))
            {
                this.customMsg.Add(key, new StringBuilder());
            }
            this.customMsg[key].AppendLine(message);
        }

        /// <summary>
        /// 取得指定key的訊息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetCustomMsg(string key)
        {
            if (!this.customMsg.ContainsKey(key))
            {
                return "";
            }
            return this.customMsg[key].ToString();
        }
    }
}
