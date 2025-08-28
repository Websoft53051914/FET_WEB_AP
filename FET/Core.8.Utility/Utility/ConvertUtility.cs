using Core.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Utility
{
    public class ConvertUtility
    {
        /// <summary>
        /// 西元日期轉民國日期
        /// </summary>
        /// <param name="adDate">西元日期時間</param>
        /// <returns>民國日期時間</returns>
        public static string AD2MinguoDate(DateTime adDate)
        {
            TaiwanCalendar tc = new();
            string minguoDate = string.Format("{0}/{1}/{2}", tc.GetYear(adDate), tc.GetMonth(adDate).ToString().PadLeft(2, '0'), tc.GetDayOfMonth(adDate).ToString().PadLeft(2, '0'));
            return minguoDate;
        }

        /// <summary>
        /// 西元日期時間轉民國日期時間
        /// </summary>
        /// <param name="adDateTime">西元日期時間</param>
        /// <returns>民國日期時間</returns>
        public static string AD2MinguoDateTime(DateTime adDateTime)
        {
            TaiwanCalendar tc = new();
            string minguoDateTime = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
                tc.GetYear(adDateTime), tc.GetMonth(adDateTime).ToString().PadLeft(2, '0'), tc.GetDayOfMonth(adDateTime).ToString().PadLeft(2, '0'),
                tc.GetHour(adDateTime).ToString().PadLeft(2, '0'), tc.GetMinute(adDateTime).ToString().PadLeft(2, '0'), tc.GetSecond(adDateTime).ToString().PadLeft(2, '0'));
            return minguoDateTime;
        }

        /// <summary>
        /// Enum轉Dictionary
        /// </summary>
        /// <typeparam name="T">要轉換的Enum類型</typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> Enum2Dictionary<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(x => Convert.ToInt32(x), x => x.GetDescription());
        }

        /// <summary>
        /// 副檔名轉MIME TYPE
        /// </summary>
        /// <param name="type">副檔名轉</param>
        /// <returns>MIME TYPE</returns>
        public static string FileExtension2MIMEType(string type)
        {
            string mimetype = "";
            switch (type)
            {
                case ".xls":
                    mimetype = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    mimetype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".ods":
                    mimetype = "application/vnd.oasis.opendocument.spreadsheet";
                    break;
                case ".pdf":
                    mimetype = "application/pdf";
                    break;
                case ".csv":
                    mimetype = "text/csv";
                    break;
                case ".jpeg":
                case ".jpg":
                    mimetype = "image/jpeg";
                    break;
                case ".png":
                    mimetype = "image/png";
                    break;
                case ".zip":
                    mimetype = "application/zip";
                    break;
                case ".doc":
                    mimetype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".docx":
                    mimetype = "application/msword";
                    break;
                case ".json":
                    mimetype = "application/json";
                    break;
                case ".ppt":
                    mimetype = "application/vnd.ms-powerpoint";
                    break;
                case ".pptx":
                    mimetype = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case ".gif":
                    mimetype = "image/gif";
                    break;
            }

            return mimetype;
        }

        /// <summary>
        /// 解析日期字串變Date物件
        /// </summary>
        /// <param name="text">日期字串</param>
        /// <returns>Nullable Date物件</returns>
        public static DateTime? DateTimeTryParse(string text)
        {

            DateTime date;
            if (DateTime.TryParse(text, out date))
            {
                return date;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// string 安全轉換成 int ，若轉換失敗則回傳預設值<para/>
        /// 範例：<para/>
        /// CommonUtility.ConvertToInt32("123", 0) => 123<para/>
        /// </summary>
        /// <param name="str">轉換值</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns></returns>
        public static int ConvertToInt32(string str, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out int result))
            {
                return defaultValue;
            }

            return result;
        }


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
            /// <param name="num"></param>
            /// <returns></returns>
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
                    var lenGroup = i == groupNum && mod != 0 ? mod : 4;
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
                            if (!(n == 1 && isEndZero | rslt.Length == 0 && j == lenFour - 2))
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
}
