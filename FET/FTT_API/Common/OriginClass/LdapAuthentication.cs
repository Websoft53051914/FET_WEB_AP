using System.DirectoryServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FTT_API.Common.OriginClass
{
    public class LdapAuthentication
    {
        private string _path;

        private string _domain;

        private string _filterAttribute;

        private DateTime _ticketExpireDate = DateTime.Now.AddYears(100);

        public DateTime TicketExpireDate
        {
            get
            {
                return _ticketExpireDate;
            }
            set
            {
                _ticketExpireDate = value;
            }
        }

        public LdapAuthentication()
        {
        }

        public LdapAuthentication(string domain)
        {
            _domain = domain;
            _path = "LDAP://" + _domain;
        }

        public bool IsAuthenticated(string username, string pwd)
        {
            if (_domain == "" || _domain == null)
            {
                throw new Exception("未指定LDAP 目錄伺服器！");
            }

            return ValidateUser(_domain, username, pwd);
        }

        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            return ValidateUser(domain, username, pwd);
        }
        public static string EscapeLdapSearchFilter(string filter)
        {
            if (filter == null) return string.Empty;

            return filter
                .Replace(@"\", @"\5c")
                .Replace("*", @"\2a")
                .Replace("(", @"\28")
                .Replace(")", @"\29")
                .Replace("\u0000", @"\00");
        }

        private bool ValidateUser(string domain, string username, string pwd)
        {
            // 1) 必要的基本檢查（避免空或過長）
            if (string.IsNullOrWhiteSpace(username)) return false;
            if (username.Length > 256) return false; // 或其他合理上限

            // 2) 白名單（只允許常見的帳號字元）
            //    根據你們 AD 的 sAMAccountName 規則調整此 regex（此處為安全且常見的範例）
            var whitelist = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9._\-@]+$");
            if (!whitelist.IsMatch(username))
            {
                // 不符白名單的帳號直接拒絕（避免注入）
                return false;
            }

            // 3) Bind 用戶（domain\username） — 只用於 bind，不放入 LDAP filter
            string bindUser = string.Concat(domain, "\\", username);
            DirectoryEntry directoryEntry = new DirectoryEntry(_path, bindUser, pwd);

            try
            {
                // 驗證 bind（帳密）
                var nativeObject = directoryEntry.NativeObject;

                DirectorySearcher searcher = new DirectorySearcher(directoryEntry);

                // 4) Escape 特殊字元（防止 LDAP 特殊字元被誤解）
                string safeUsername = EscapeLdapSearchFilter(username);

                // 5) 使用 string.Format 參數化 Filter（避免字串插值 / 直接串接）
                //    僅使用等於比對（=），不使用通配或其他運算子
                searcher.Filter = string.Format("(&(objectClass=user)(sAMAccountName={0}))", safeUsername);

                searcher.SearchScope = SearchScope.Subtree;

                searcher.PropertiesToLoad.Clear();
                searcher.PropertiesToLoad.Add("cn");

                SearchResult result = searcher.FindOne();
                if (result == null) return false;

                _path = result.Path;
                _filterAttribute = result.Properties["cn"][0]?.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("驗證使用者錯誤: " + ex.Message);
            }

            return true;
        }

        private string ExtractUserName(string path)
        {
            string[] array = path.Split('\\');
            return array[array.Length - 1];
        }

        public bool IsExistInAD(string loginName)
        {
            string arg = ExtractUserName(loginName);
            DirectorySearcher directorySearcher = new DirectorySearcher();
            directorySearcher.Filter = $"(SAMAccountName={arg})";
            directorySearcher.PropertiesToLoad.Add("cn");
            SearchResult searchResult = directorySearcher.FindOne();
            if (searchResult == null)
            {
                return false;
            }

            return true;
        }

        public string GetGroups()
        {
            DirectorySearcher directorySearcher = new DirectorySearcher(_path);
            directorySearcher.Filter = "(cn=" + _filterAttribute + ")";
            directorySearcher.PropertiesToLoad.Add("memberOf");
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                SearchResult searchResult = directorySearcher.FindOne();
                int count = searchResult.Properties["memberOf"].Count;
                for (int i = 0; i < count; i++)
                {
                    string text = (string)searchResult.Properties["memberOf"][i];
                    int num = text.IndexOf("=", 1);
                    int num2 = text.IndexOf(",", 1);
                    if (-1 == num)
                    {
                        return null;
                    }

                    stringBuilder.Append(text.Substring(num + 1, num2 - num - 1));
                    stringBuilder.Append("|");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }

            return stringBuilder.ToString();
        }

        //public void setFormsAuthTicket(string UserName, string UserData, bool SetExpires)
        //{
        //    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, UserName, DateTime.Now, DateTime.Now.AddYears(100), isPersistent: false, UserData);
        //    string value = FormsAuthentication.Encrypt(ticket);
        //    HttpCookie httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, value);
        //    httpCookie.Path = FormsAuthentication.FormsCookiePath;
        //    if (SetExpires)
        //    {
        //        httpCookie.Expires = _ticketExpireDate;
        //    }

        //    HttpContext.Current.Response.Cookies.Add(httpCookie);
        //}

        //public string getFormsAuthTicket()
        //{
        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        return ((FormsIdentity)HttpContext.Current.User.Identity).Ticket.UserData;
        //    }

        //    return "";
        //}
    }
}
