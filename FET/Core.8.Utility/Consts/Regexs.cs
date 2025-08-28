using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Consts
{
    /// <summary>
    /// 正則表示式攻勢
    /// </summary>
    public class Regexs
    {
        /// <summary>
        /// 手機電話驗證
        /// </summary>
        public const string CELLPHONE = @"^09[0-9]{8}$";
        public const string PHONE = @"^[0-9*+#\-\s]*$";

        public const string CODE = @"^[0-9A-Za-z]+$";

        /// <summary>
        /// 郵遞區號
        /// </summary>
        public const string ZIPCODE = @"^[1-9]{1}[0-9]{2}$";

        /// <summary>
        /// EMAIL
        /// </summary>
        public const string EMAIL = "^[\\w-]+(\\.[\\w-]+)*@[\\w-]+(\\.[\\w-]+)+$";
        public const string EMAILSUFFIX = "^[\\w-]+(\\.[\\w-]+)*@[\\w-]+(\\.[\\w-]+)+$";

        /// <summary>
        /// IP
        /// </summary>
        public const string IP = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";

        /// <summary>
        /// 手機載具驗證
        /// </summary>
        public const string CARRIER = @"^/[0-9A-Z.+1]{7}$";

        /// <summary>
        /// 家用電話驗證
        /// </summary>
        public const string TELEPHONE = @"^(\d{2,3}-)?\d{6,8}$";
    }
}
