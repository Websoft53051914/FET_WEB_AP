using Core.Utility.Extensions;
using Core.Utility.Helper.Mail;
using FTT_WEB.Common;
using FTT_WEB.Common.ConfigurationHelper;
using FTT_WEB.Common.OriginClass.EntiityClass;
using Microsoft.Graph.DeviceManagement.WindowsInformationProtectionAppLearningSummaries.Item;
using Org.BouncyCastle.Utilities.Encoders;
using static Const.Enums;


namespace FTT_WEB.Models.Handler
{
    public class SendMailHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly IHttpContextAccessor _httpContext;


        public SendMailHandler(ConfigurationHelper configHelper, IHttpContextAccessor httpContextAccessor)
        {
            _configHelper = configHelper;
            _httpContext = httpContextAccessor;
        }
        //public SendMailHandler sendMailHandler;

        private MailHelper _MailHelper = null;
        public MailHelper GetMailHelper()
        {
            if (_MailHelper == null)
            {
                string sql = $"Select * from tb_mailserver where status = @status";
                Dictionary<string, object> paras = new Dictionary<string, object> { { "Status", StatusEnum.Enabled.ToInt() } };
                MailServerSetting mailServerSettingEntity = dbHelper.Find<MailServerSetting>(sql, paras);
                if (mailServerSettingEntity != null)
                {
                    if (int.TryParse(mailServerSettingEntity.Port, out int SmtpPort))
                    {
                        //_MailHelper = new MailHelper(_configHelper.Config["GmailConfig:SmtpServer"], SmtpPort, _configHelper.Config["GmailConfig:MailUserID"], _configHelper.Config["GmailConfig:MailUserPwd"]);
                        _MailHelper = new MailHelper(mailServerSettingEntity.Server, SmtpPort, mailServerSettingEntity.SenderAddress, mailServerSettingEntity.Password);
                        _MailHelper.MailFrom = mailServerSettingEntity.SenderAddress;
                    }
                }               
            }
            return _MailHelper;
        }
        public void Send()
        {
            if (!_configHelper.Config.GetValue<bool>("EnableSendMailSchedule"))
            {
                return;
            }
            var mailHelper = GetMailHelper();
            if (mailHelper == null)
            {
                return;
            }
            List<MailPool> UnSentMails = GetUnSentMails();
            string updateCommand = @"UPDATE tb_mailpool SET Status = @Status, SendStatus = @SendStatus, RealSendTime = @RealSendTime
, ErrorMsg = @ErrorMsg, updatetime = @UpdateTime, updater = @Updater
                                         WHERE Id = @Id";
            for (int i = 0; i < UnSentMails.Count; i++)
            {
                try
                {
                    //todo send
                    
                    mailHelper.MailTos = new string[] { UnSentMails[i].DestinationEmail };
                    mailHelper.EnableSsl = true;
                    mailHelper.Subject = UnSentMails[i].Subject;
                    mailHelper.Body = UnSentMails[i].Content;
                    mailHelper.Send();

                    var parameters = new Dictionary<string, object>
                        {
                            { "@Status", StatusEnum.Cancel.ToInt() },
                            { "@SendStatus", (int)MailSendStatusEnum.Sent },
                            { "@RealSendTime", DateTime.Now },
                            { "@ErrorMsg", string.Empty},
                            { "@Id", UnSentMails[i].Id },
                            { "@UpdateTime", DateTime.Now },
                            { "@updater", 0}
                        };


                    dbHelper.Execute(updateCommand, parameters);
                }
                catch (Exception iex)
                {
                    var parameters = new Dictionary<string, object>
                        {
                            { "@Status", StatusEnum.Cancel.ToInt() },
                            { "@SendStatus", (int)MailSendStatusEnum.Error },
                            { "@RealSendTime", DateTime.Now },
                            { "@ErrorMsg", iex.Message },
                            { "@Id", UnSentMails[i].Id },
                            { "@UpdateTime", DateTime.Now },
                            { "@updater", 0}
                        };

                    dbHelper.Execute(updateCommand, parameters);
                }
            }
            dbHelper.Commit();

        }

        public List<MailPool> GetUnSentMails()
        {
            List<MailPool> list = new();

            var parameters = new Dictionary<string, object>
                        {
                            { "@Status", StatusEnum.Enabled.ToInt() },
                            { "@SendStatus", (int)MailSendStatusEnum.UnSent },
                            { "@estimatesendtime", DateTime.Now },
                           
                        };

            string sql = $@"Select * from tb_mailpool where 1=1 AND status = @Status AND estimatesendtime < @estimatesendtime  AND sendstatus = @SendStatus";

            list = this.dbHelper.FindList<MailPool>(sql, parameters);
            return list;
        }

        

    }
}
