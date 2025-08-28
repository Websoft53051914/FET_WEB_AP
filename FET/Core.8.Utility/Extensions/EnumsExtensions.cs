using System;
using System.ComponentModel;
using System.Reflection;

namespace Core.Utility.Extensions
{
    public static class EnumsExtensions
    {
        /// <summary>
        /// 取得Enum的Description內容
        /// </summary>
        /// <param name="self">要取得的Enum值</param>
        /// <returns>Description內容</returns>
        public static string GetDescription(this Enum self)
        {
            FieldInfo field = self.GetType().GetField(self.ToString());
            DescriptionAttribute[] array = null;
            if (field != null)
            {
                array = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
                if (array != null && array.Length != 0)
                {
                    return array[0].Description;
                }
            }

            return self.ToString();
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }
    }
}
