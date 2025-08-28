using AutoMapper;
using Core.Utility.Extensions;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using FTT_API.Models.ViewModel.Login;
using FTT_API.Models.ViewModel.MailServerSetting;
using Microsoft.Graph.Models;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class MailServerHandler : BaseDBHandler
    {

        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;

        IMapper mapper;

        public MailServerHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.CreateMap<MailServerSetting, MailServerSettingVM>();
                cfg.CreateMap<MailServerSettingVM, MailServerSetting>();

                

            });

            mapper = configuration.CreateMapper();
        }

        public MailServerSettingVM GetEdit()
        {
            string sql = $@"Select * from tb_mailserver";
            var MailServer = dbHelper.Find<MailServerSetting>(sql);

            MailServerSettingVM vm = new MailServerSettingVM();

            if (MailServer != null)
            {
                vm = mapper.Map<MailServerSettingVM>(MailServer);
            }
            vm.IsEnabled = MailServer != null ? (MailServer.Status == StatusEnum.Enabled.ToInt() ? true : false) : false;          
            return vm;
        }

        public void Update(MailServerSettingVM vm)
        {
            string insertCommand = @"
INSERT INTO tb_mailserver
(
    server,
    port,
    sender,
    senderaddress,
    username,
    password,
    creator,
    createtime,
    updater,
    updatetime,
   
    status,
    enablessl
)
VALUES
(
    @server,
    @port,
    @sender,
    @senderaddress,
    @username,
    @password,
    @creator,
    @createtime,
    @updater,
    @updatetime,
    @status,
    @enablessl
);";

            string updateCommand = @"
UPDATE tb_mailserver
SET
    server = @server,
    port = @port,
    sender = @sender,
    senderaddress = @senderaddress,
    username = @username,
    password = @password,  
    updater = @updater,
    updatetime = @updatetime,
    status = @status,
    enablessl = @enablessl
WHERE id = @id;";

            MailServerSetting mailServerEntity = mapper.Map<MailServerSetting>(vm);

            mailServerEntity.Status = vm.IsEnabled ? StatusEnum.Enabled.ToInt() : StatusEnum.Disabled.ToInt();

            if (mailServerEntity.Id == 0)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@server", mailServerEntity.Server },
                    { "@port", mailServerEntity.Port },
                    { "@sender", mailServerEntity.Sender},
                    { "@senderaddress", mailServerEntity.SenderAddress },
                    { "@username", mailServerEntity.UserName },
                    { "@password",mailServerEntity.Password },
                    { "@creator", 0},
                    { "@createtime", DateTime.Now },
                    { "@updater", 0},
                    { "@updatetime", DateTime.Now },                    
                    { "@enablessl", mailServerEntity.EnableSSL},
                    { "@status", mailServerEntity.Status },                           
                };
                dbHelper.Execute(insertCommand, parameters);
            }
            else
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@server", mailServerEntity.Server },
                    { "@port", mailServerEntity.Port },
                    { "@sender", mailServerEntity.Sender},
                    { "@senderaddress", mailServerEntity.SenderAddress },
                    { "@username", mailServerEntity.UserName },
                    { "@password",mailServerEntity.Password },
                    { "@updater", 0},
                    { "@updatetime", DateTime.Now },                    
                    { "@enablessl", mailServerEntity.EnableSSL},
                    { "@status", mailServerEntity.Status },
                };
                dbHelper.Execute(updateCommand, parameters);
            }
            dbHelper.Commit();
        }
    }
}
