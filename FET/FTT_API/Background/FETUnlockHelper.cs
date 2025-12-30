using System;
using System.Data;
//using Npgsql;   // dotnet add package Npgsql
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using FTT_API.Common;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.Graph.Models;
using static Const.Enums;

namespace FTT_API.Background
{
    public class FETUnlockHelper
    {
        public FETUnlockHelper()
        {
        }

        // ========= 主流程 =========
        public void Unlock(string groupid)
        {
            DateTime dtTime = DateTime.Now;
            BaseDBHandler baseHandler = new BaseDBHandler();

            string sql = @"
SELECT *
FROM store_vender_profile
WHERE locked = 'Y'
  AND locked_reason = 5
  AND locked_time + INTERVAL '15 minutes' < now();
";

            try
            {
                var dtos = baseHandler.GetDBHelper().FindList<store_vender_profileDTO>(sql);
                Dictionary<string, object> para ;

                foreach (var dto in dtos)
                {
                    para = new() { { "acc", dto.merchant_login },{ "dtTime", dtTime } };
                    baseHandler.GetDBHelper().Execute(@"
update store_vender_profile 
set 

login_count=0 
,LOCKED='N'
,locked_reason=3
,unlocked_time=@dtTime

where merchant_login=@acc
", para);

                    Dictionary<string, object> paras2 = new Dictionary<string, object>()
                                            {
                                                { "account",dto.merchant_login}, 
                                                { "createtime",dtTime},
                                                { "locked_reason",3},
                                            };
                    baseHandler.GetDBHelper().Execute(@"
INSERT INTO tb_vender_password_history(
	account, pw, createtime, locked_reason)
	VALUES
(@account, null, @createtime, @locked_reason);

", paras2);
                }

                baseHandler.GetDBHelper().Commit();
            }
            catch (Exception ex)
            {
                var entity = new TB_Control_LogEntity()
                {
                    IP = Method.GetClientIPAddress(),
                    Status = ((int)LogStatusEnum.Success).ToString(),
                    ControllerName = "Background FETTaskHelper",
                    ActionName = "Send_TT_No_RootCause",
                    Exception = ex.ToString(),
                    Account = "service",
                    Name = "service",
                    LogTime = DateTime.Now,
                    Token = "",
                };
                TB_Control_LogHandler _BaseDBHandler = new TB_Control_LogHandler();
                _BaseDBHandler.Insert(entity);
            }
        }
    }
}
