using log4net;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "./Helper/Log/log4net.config", Watch = false)]
namespace Helper.Log
{
    /// <summary>
    /// 【Log、紀錄】
    /// </summary>
    public class LogHelper
    {
        static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LogHelper()
        {
            SetColumn1(string.Empty);
            SetColumn2(string.Empty);
        }

        /// <summary>
        /// 設定用來記錄到table內的帳號
        /// </summary>
        /// <param name="accounId">帳號</param>
        public void SetAccountId(int accounId)
        {
            log4net.ThreadContext.Properties["Creator"] = accounId;//log4net.config 嵌入 creator 名稱
            log4net.ThreadContext.Properties["Updator"] = accounId;// log4net.config 嵌入 updator 名稱
        }

        /// <summary>
        /// 紀錄為 一般 Log
        /// </summary>
        /// <param name="msg">訊息</param>
        public void Info(string msg)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // 篩選掉 IPv6 的 IP，只留IPv4 的 IP

                {
                    log4net.ThreadContext.Properties["Ip"] = ip.ToString();
                }
            }

            var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            string callerName = methodInfo.Name;
            string className = methodInfo.ReflectedType.Name;
            log4net.ThreadContext.Properties["ActionName"] = callerName;//log4net.config 嵌入 caller class 名稱
            log4net.ThreadContext.Properties["ControllerName"] = className;//log4net.config 嵌入 caller method 名稱
                                                                           //
            _logger.Info(msg);
        }

        /// <summary>
        /// 紀錄為 Debug Log
        /// </summary>
        /// <param name="msg">訊息</param>
        public void Debug(string msg)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // 篩選掉 IPv6 的 IP，只留IPv4 的 IP

                {
                    log4net.ThreadContext.Properties["Ip"] = ip.ToString();
                }
            }

            var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            string callerName = methodInfo.Name;
            string className = methodInfo.ReflectedType.Name;
            log4net.ThreadContext.Properties["ActionName"] = callerName;//log4net.config 嵌入 caller class 名稱
            log4net.ThreadContext.Properties["ControllerName"] = className;//log4net.config 嵌入 caller method 名稱

            _logger.Debug(msg);
        }

        /// <summary>
        /// 紀錄為 警告 Log
        /// </summary>
        /// <param name="msg">訊息</param>
        public void Warn(string msg)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // 篩選掉 IPv6 的 IP，只留IPv4 的 IP

                {
                    log4net.ThreadContext.Properties["Ip"] = ip.ToString();
                }
            }

            var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            string callerName = methodInfo.Name;
            string className = methodInfo.ReflectedType.Name;
            log4net.ThreadContext.Properties["ActionName"] = callerName;//log4net.config 嵌入 caller class 名稱
            log4net.ThreadContext.Properties["ControllerName"] = className;//log4net.config 嵌入 caller method 名稱

            _logger.Warn(msg);
        }

        /// <summary>
        /// 紀錄為 錯誤 Log
        /// </summary>
        /// <param name="msg">訊息</param>
        public void Error(string msg)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // 篩選掉 IPv6 的 IP，只留IPv4 的 IP

                {
                    log4net.ThreadContext.Properties["Ip"] = ip.ToString();
                }
            }

            var methodInfo = new StackTrace().GetFrame(1).GetMethod();
            string callerName = methodInfo.Name;
            string className = methodInfo.ReflectedType.Name;
            log4net.ThreadContext.Properties["ActionName"] = callerName;//log4net.config 嵌入 caller class 名稱
            log4net.ThreadContext.Properties["ControllerName"] = className;//log4net.config 嵌入 caller method 名稱

            _logger.Error(msg);
        }

        // TODO
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetColumn1(string value)
        {
            ThreadContext.Properties["Column1"] = value;
        }

        // TODO
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetColumn2(string value)
        {
            ThreadContext.Properties["Column2"] = value;
        }
    }
}
