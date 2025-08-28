using System.DirectoryServices;
using System.Text;

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

        private bool ValidateUser(string domain, string username, string pwd)
        {
            string username2 = domain + "\\" + username;
            DirectoryEntry directoryEntry = new DirectoryEntry(_path, username2, pwd);
            try
            {
                object nativeObject = directoryEntry.NativeObject;
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = "(SAMAccountName=" + username + ")";
                directorySearcher.PropertiesToLoad.Add("cn");
                SearchResult searchResult = directorySearcher.FindOne();
                if (null == searchResult)
                {
                    return false;
                }

                _path = searchResult.Path;
                _filterAttribute = (string)searchResult.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                throw new Exception("驗證使用者錯誤： " + ex.Message);
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
