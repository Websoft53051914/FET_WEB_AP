using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Helper.Mail
{
    /// <summary>
    /// 【Mail、信件】
    /// </summary>
    public class MailHelper
    {
        #region "設定相關"
        /// <summary>
        /// smtp Server
        /// </summary>
        string smtpServer;
        /// <summary>
        /// smtp Port
        /// </summary>
        int smtpPort;
        /// <summary>
        /// 發信帳號
        /// </summary>
        string mailAccount;
        /// <summary>
        /// 發信帳號密碼
        /// </summary>
        string mailPwd;

        #endregion
        /// <summary>
        /// MailHelper 建構子
        /// </summary>
        /// <param name="smtpServer">smtp server位置</param>
        /// <param name="smtpPort">smtp Port</param>
        /// <param name="mailAccount">發信帳號</param>
        /// <param name="mailPwd">發信密碼</param>
        public MailHelper(string smtpServer, int smtpPort, string mailAccount, string mailPwd)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.mailAccount = mailAccount;
            this.mailPwd = mailPwd;
        }

        #region "參數相關"
        //寄信人Email Address
        public string MailFrom { set; get; }
        //收信人Email Address
        public string[] MailTos { set; get; }
        //副本Email
        public string[] Ccs { set; get; }
        //要夾帶的附檔
        public Dictionary<string, Stream> Files { set; get; }
        public string Subject { set; get; }
        public string Body { set; get; }
        public bool EnableSsl { set; get; }
        #endregion

        /// <summary>
        /// 完整的寄信函數
        /// 【寄信、發信、發mail】
        /// </summary>
        /// <returns>回傳寄信是否成功(true:成功,false:失敗)</returns>
        public void Send()
        {

            //沒給寄信人mail address
            if (string.IsNullOrEmpty(MailFrom))
            {
                MailFrom = "";
            }

            //命名空間： System.Web.Mail已過時，http://msdn.microsoft.com/zh-tw/library/system.web.mail.mailmessage(v=vs.80).aspx
            //建立MailMessage物件
            using MailMessage msg = new();
            foreach (string mailTo in MailTos)
            {
                //加入信件的收信人(們)address
                if (!string.IsNullOrEmpty(mailTo.Trim()))
                {
                    msg.To.Add(new MailAddress(mailTo.Trim()));
                }

            }


            //End if (MailTos !=null)//防呆

            if (Ccs != null) //防呆
            {
                for (int i = 0; i < Ccs.Length; i++)
                {
                    if (!string.IsNullOrEmpty(Ccs[i].Trim()))
                    {
                        //加入信件的副本(們)address
                        msg.CC.Add(new MailAddress(Ccs[i].Trim()));
                    }

                }
            }//End if (Ccs!=null) //防呆


            //附件處理
            if (Files != null && Files.Count > 0)//寄信時有夾帶附檔
            {
                foreach (string fileName in Files.Keys)
                {
                    Attachment attfile = new(Files[fileName], fileName);
                    msg.Attachments.Add(attfile);
                }//end foreach
            }//end if 

            msg.From = new MailAddress(MailFrom, "Mail", System.Text.Encoding.UTF8);
            msg.IsBodyHtml = true;
            //郵件標題   
            msg.Subject = Subject;
            //郵件標題編碼    
            msg.SubjectEncoding = Encoding.UTF8;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            //郵件內容  
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.BodyEncoding = Encoding.UTF8;//郵件內容編碼   
            msg.Priority = MailPriority.Normal;//郵件優先級  

            using (SmtpClient client = new(smtpServer, smtpPort))//或公司、客戶的smtp_server
            {
                if (!string.IsNullOrEmpty(mailAccount) && !string.IsNullOrEmpty(mailPwd))
                {
                    client.Credentials = new NetworkCredential(mailAccount, mailPwd);
                    client.EnableSsl = this.EnableSsl;
                }
                client.Send(msg);
            }//end using 

            if (msg.Attachments != null && msg.Attachments.Count > 0)
            {
                for (int i = 0; i < msg.Attachments.Count; i++)
                {
                    msg.Attachments[i].Dispose();
                    msg.Attachments[i] = null;
                }
            }

        }//End 寄信
    }
}
