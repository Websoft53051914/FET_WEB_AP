using System;

namespace Core.Utility.Utility
{
    /// <summary>
    /// 【數字、國字、轉換】
    /// </summary>
    public static class NumberConvertCHNUtility
    {
        /// <summary>
        /// 數字中文
        /// </summary>
        private static readonly string[] TEXT_NUMS = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        /// <summary>
        /// 位數中文
        /// </summary>
        private static readonly string[] TEXT_DIGITS = { "", "十", "百", "千" };
        /// <summary>
        /// 單位中文
        /// </summary>
        private static readonly string[] TEXT_UNITS = { "", "萬", "億", "兆" };

        /// <summary>
        /// 數字轉中文
        /// </summary>
        /// <param name="num">數值string</param>
        /// <returns>國字數字</returns>
        public static string ToChineseText(string num)
        {
            var rslt = "";
            var finger = 0; //字符位置指针
            var mod = num.Length % 4;
            //	四位一組(個位開始)
            var groupNum = mod > 0 ? num.Length / 4 + 1 : num.Length / 4;

            //	由高位組開始
            for (var i = groupNum; i > 0; i--)
            {
                //	位組長度(最高位組可能不足4位)
                var lenGroup = (i == groupNum && mod != 0) ? mod : 4;
                var four = num.Substring(finger, lenGroup);
                var lenFour = four.Length;

                for (int j = 0; j < lenFour; j++)
                {
                    var n = Convert.ToInt32(four[j].ToString());
                    var isEndZero = rslt.EndsWith(TEXT_NUMS[0]);

                    if (n == 0)
                    {//	數字為0
                        if (j < lenFour - 1 && Convert.ToInt32(four[j + 1].ToString()) > 0 && !isEndZero)
                            rslt += TEXT_NUMS[n];
                    }
                    else
                    {
                        if (!(n == 1 && (isEndZero | rslt.Length == 0) && j == lenFour - 2))
                            rslt += TEXT_NUMS[n];
                        rslt += TEXT_DIGITS[lenFour - j - 1];
                    }
                }

                //	加上單位(四位皆為0不加)
                if (Convert.ToInt32(four) != 0)
                {
                    rslt += TEXT_UNITS[i - 1];
                }

                finger += lenGroup;
            }

            if (string.IsNullOrEmpty(rslt))
            {
                rslt = TEXT_NUMS[0];
            }

            return rslt;
        }
    }
}
