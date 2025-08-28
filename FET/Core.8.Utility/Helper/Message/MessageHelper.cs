using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.Message
{
    /// <summary>
    /// 邏輯層訊息元件
    /// </summary>
    public class MessageHelper
    {
        /// <summary>
        /// 錯誤訊息清單
        /// </summary>
        Dictionary<string, string> errorMsgs;

        public MessageHelper()
        {
            errorMsgs = new Dictionary<string, string>();
        }

        /// <summary>
        /// 新增訊息
        /// </summary>
        /// <param name="key">ket</param>
        /// <param name="msg">訊息</param>
        public void Add(string key, string msg)
        {
            errorMsgs[key] = errorMsgs.ContainsKey(key) ? errorMsgs[key] + "\n" + msg : msg;
        }

        /// <summary>
        /// 取得訊息
        /// </summary>
        /// <param name="key">ket</param>
        /// <param name="msg">訊息</param>
        /// <returns>訊息內容</returns>
        public string Get(string key)
        {
            if (!errorMsgs.ContainsKey(key))
            {
                return null;
            }
            return errorMsgs[key];
        }
        /// <summary>
        /// 取得所有訊息
        /// </summary>
        /// <returns>字典型態所有訊息</returns>
        public Dictionary<string, string> GetAll()
        {
            return errorMsgs;
        }

        /// <summary>
        /// 取得所有異常訊息
        /// </summary>
        /// <returns>字典串成字串</returns>
        public string GetErrMsg()
        {
            StringBuilder StringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, string> item in errorMsgs)
            {
                StringBuilder.AppendLine(item.Value);
            }

            return StringBuilder.ToString();
        }

        /// <summary>
        /// 是否錯誤
        /// </summary>
        /// <returns>是/否</returns>
        public bool IsError()
        {
            return errorMsgs.Count > 0;
        }


        /// <summary>
        /// 清空訊息
        /// </summary>
        public void Clear()
        {
            errorMsgs.Clear();
        }


        private readonly string ALERT_KEY = "_||_Alert_||_";

        /// <summary>
        /// 新增 alert 訊息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="msg">訊息</param>
        public void SetAlert(string msg)
        {
            Add(ALERT_KEY, msg);
        }
        /// <summary>
        /// 取得 alert 訊息
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="msg">訊息</param>
        /// <returns>alert訊息</returns>
        public string GetAlert()
        {
            return this.Get(ALERT_KEY);
        }
    }
}
