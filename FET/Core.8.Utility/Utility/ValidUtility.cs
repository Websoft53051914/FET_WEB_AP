using Core.Utility.Consts;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Utility.Utility
{
    public class ValidUtility
    {

        /// <summary>
        /// 判斷是否為email地址
        /// 【驗證、判斷、email】
        /// </summary>
        /// <param name="value">要判斷的字串</param>
        /// <returns>是/否</returns>
        public static bool IsValidEmail(string value)
        {
            Regex pattern = new("^[\\w-]+(\\.[\\w-]+)*@[\\w-]+(\\.[\\w-]+)+$");
            return pattern.IsMatch(value);
        }

        /// <summary>
        /// 判斷是否為email後綴地址
        /// 【驗證、判斷、email】
        /// </summary>
        /// <param name="value">要判斷的字串</param>
        /// <returns>是/否</returns>
        public static bool IsValidEmailSuffix(string value)
        {
            Regex pattern = new("^@[\\w-]+(\\.[\\w-]+)+$");
            return pattern.IsMatch(value);
        }

        /// <summary>
        /// 判斷是否為 url
        /// 【驗證、判斷、url】
        /// </summary>
        /// <param name="value">要判斷的字串</param>
        /// <returns>是/否</returns>
        public static bool IsValidUrl(string value)
        {
            return Uri.IsWellFormedUriString(value, UriKind.Absolute);
        }

        /// <summary>
        /// 檢查ip正確性
        /// 範圍從1.0.0.0 to 255.255.255.255
        /// 【驗證、判斷、IP】
        /// </summary>
        /// <param name="addr">欲驗證的ip位址</param>
        /// <returns>是/否</returns>
        public static bool IsValidIP(string addr)
        {
            //create our match pattern
            string pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            //create our Regular Expression object
            Regex check = new(pattern);
            //boolean variable to hold the status
            bool valid;
            //check to make sure an ip address was provided
            if (addr == "")
            {
                //no address provided so return false
                valid = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                valid = check.IsMatch(addr, 0);
            }
            //return the results
            return valid;
        }

        /// <summary>
        /// 驗證手機電話
        /// 【驗證、判斷、手機電話】
        /// </summary>
        /// <param name="idNumber"> 正確範例 0912456789 </param>
        /// <returns>是/否</returns>
        public static bool IsValidCellphone(string idNumber)
        {
            if (!Regex.IsMatch(idNumber, Regexs.CELLPHONE))
                return false;

            return true;
        }

        /// <summary>
        /// 驗證手機載具
        /// 【驗證、判斷、手機載具】
        /// </summary>
        /// <param name="carrier"> 正確範例 /1234567 </param>
        /// <returns>是/否</returns>
        public static bool IsValidCarrier(string carrier)
        {
            if (!Regex.IsMatch(carrier, Regexs.CARRIER))
                return false;

            return true;
        }

        /// <summary>
        /// 驗證家庭電話
        /// 【驗證、判斷、家庭電話】
        /// </summary>
        /// <param name="telephone"> 正確範例 0912456789 </param>
        /// <returns>是/否</returns>
        public static bool IsValidTelephone(string telephone)
        {
            if (!Regex.IsMatch(telephone, @"^(\d{2,3}-)?\d{6,8}$"))
                return false;

            return true;
        }

        /// <summary>
        /// 驗證身分證
        /// 【驗證、判斷、身分證】
        /// </summary>
        /// <param name="idNumber"> 正確範例 A123456789 </param>
        /// <returns>是/否</returns>
        public static bool IsValidIDNumber(string idNumber)
        {
            var result = false;
            if (idNumber.Length == 10)
            {
                idNumber = idNumber.ToUpper();
                if (idNumber[0] >= 0x41 && idNumber[0] <= 0x5A)
                {
                    var a = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
                    var b = new int[11];
                    b[1] = a[(idNumber[0]) - 65] % 10;
                    var c = b[0] = a[(idNumber[0]) - 65] / 10;
                    for (var i = 1; i <= 9; i++)
                    {
                        b[i + 1] = idNumber[i] - 48;
                        c += b[i] * (10 - i);
                    }
                    if (((c % 10) + b[10]) % 10 == 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 是否字串字元皆非 Unicode 字元
        /// 【驗證、判斷、Unicode】
        /// </summary>
        /// <param name="str">要判斷的字串</param>
        /// <returns>是/否</returns>
        public static bool IsValidVarchar(string str)
        {
            return string.IsNullOrEmpty(str)
                || Encoding.UTF8.GetByteCount(str) == str.Length;
        }

        /// <summary>
        /// 驗證電話號碼(簡易)
        /// 【驗證、判斷、電話號碼】
        /// </summary>
        /// <param name="str">要判斷的字串</param>
        /// <returns>是/否</returns>
        public static bool IsValidPhoneNumber(string str)
        {
            // "-"：間隔數字；"#"：代表分機；"("、")"：間隔區碼
            if (string.IsNullOrEmpty(str) || !Regex.IsMatch(str, @"^[0-9-()#]+$"))
                return false;

            return true;
        }

        /// <summary>
        /// 驗證統一編號
        /// 【驗證、判斷、統一編號】
        /// </summary>
        /// <param name="cTax">統一編號</param>
        /// <returns>為tuple，(是否有error,錯誤原因)</returns>
        /// 
        public static (bool bIsError, string cMessage) CheckTaxID(string cTax)
        { //回傳結果集
            var oResult = (bIsError: false, cMessage: "統一編號格式合法");
            //邏輯乘數（財政部制定）
            var cMagic = "12121241";
            try
            {
                if (string.IsNullOrEmpty(cTax) || cTax.Length != 8 || !int.TryParse(cTax, out int iUnused))
                { throw new System.Exception("TaxID Error"); }
                //轉成數值陣列
                var aryTax = cTax.ToCharArray().Select(x => (int)(x - '0')).ToArray();
                var aryMagic = cMagic.ToCharArray().Select(x => (int)(x - '0')).ToArray();
                //運算乘積
                var aryResult = new int[8];
                for (int i = 0; i < aryTax.Length; i++)
                { aryResult[i] = aryTax[i] * aryMagic[i]; }
                //運算整理：大於10就進行位數相加
                aryResult = aryResult.Select(x => x < 10 ? x : x.ToString().ToCharArray().Select(y => (int)(y - '0')).Sum()).ToArray();
                //運算整理：第七位數大於10之分拆
                var oList = new System.Collections.Generic.List<int[]>();
                foreach (var cItem in aryResult[6].ToString().ToCharArray())
                {
                    var aryTemp = aryResult.ToArray();
                    aryTemp[6] = (int)(cItem - '0');
                    oList.Add(aryTemp);
                }
                //運算整理：乘積和與除5判斷
                if (!oList.Select(x => x.Sum()).Select(x => x % 5 == 0).Any(x => x))
                { throw new System.Exception("TaxId Rule Error"); }
            }
            catch (System.Exception oEx)
            {
                oResult.bIsError = true;
                oResult.cMessage = oEx.Message;
            }
            return oResult;
        }

        /// <summary>
        /// 判斷是否為數字
        /// 【驗證、判斷、數字】
        /// </summary>
        /// <param name="value">要判斷的字串</param>
        /// <returns>是/否</returns>
        public static bool IsValidNumber(string value)
        {
            Regex pattern = new("^\\d+$");
            return pattern.IsMatch(value);
        }

        /// <summary>
        /// 起訖日期檢查
        /// 【驗證、判斷、日期】
        /// </summary>
        /// <param name="start">開始日期</param>
        /// <param name="end">結束日期</param>
        /// <returns>是/否</returns>
        public static bool IsValidStartEndDate(DateTime start, DateTime end)
        {
            return DateTime.Compare(start, end) <= 0;
        }


        /// <summary>
        /// 驗證AD帳號登入
        /// 【驗證、判斷、AD】
        /// </summary>
        /// <param name="adIP">AD帳號登入的IP位址</param>
        /// <param name="pwd">AD帳號登入的密碼</param>
        /// <param name="username">AD帳號使用者名稱</param>
        /// <returns>是/否</returns>
        public static bool IsValidationForAD(string adIP, string pwd, string username)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, adIP, username, pwd))
                {
                    UserPrincipal user = new UserPrincipal(context)
                    {
                        SamAccountName = username,
                        Enabled = true
                    };

                    PrincipalSearcher pS = new PrincipalSearcher(user);
                    Principal result = pS.FindOne();

                    if (result == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true; //verify password is succeed!
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
