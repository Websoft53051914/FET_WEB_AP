using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.TypeConverters
{
    /// <summary>
    /// 【資料庫型別轉換、轉型】
    /// </summary>
    public interface ITypeConverter
    {
        /// <summary>
        /// 轉型
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <returns>回傳對應的型別</returns>
        object Convert(object ValueToConvert);

        /// <summary>
        /// 自訂為null情況下的預設值
        /// </summary>
        /// <param name="ValueToConvert">object</param>
        /// <param name="nullDefaultValue"></param>
        /// <returns>回傳對應的型別</returns>
        object Convert(object ValueToConvert, object nullDefaultValue);
    }
}
