using DocumentFormat.OpenXml.Vml.Spreadsheet;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.ViewModel.ChangePw;
using FTT_VENDER_API.Models.ViewModel.StoreVenderProfile;
using Microsoft.AspNetCore.SignalR;
using System.Transactions;
using static Const.Enums;

namespace FTT_VENDER_API.Models.Handler
{
    public class ChangePwHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public ChangePwHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        public void UpdatePw(ChangePwVM vm)
        {
            var dtTime = DateTime.Now;
            var iv = Common.Method.GetAppSettingsDataByName("iv");
            var ke = Common.Method.GetAppSettingsDataByName("ke");

            var newPw = new Common.PasswordService().HashPassword(vm.NewPD);

            Dictionary<string, object> paras_Insert = new Dictionary<string, object>()
            {
                { "newPw",newPw},
                { "account",vm.AC},
                { "createtime",dtTime},
                { "locked_reason",4},
            };

            GetDBHelper().Execute(@"
            INSERT INTO tb_vender_password_history(
	            account, pw, createtime, locked_reason)
	            VALUES
            (@account, @newPw, @createtime, @locked_reason);

            ", paras_Insert);

            string update_SQL = "";
            Dictionary<string, object> paras_Update = new();
            update_SQL = @$"UPDATE store_vender_profile SET 
                merchant_password = @pw, 
                locked = @Locked, 
                locked_reason = @LockedReason, 
                is_pwchange_remind = NULL, 
                pw_chgtime = @Now ,
                lasturltime=null, 
                lasturlkey=null,
locked_time=null,
geturltime=null
                WHERE merchant_login = @AC;";

            paras_Update = new Dictionary<string, object>
                        {
                            { "pw", newPw },
                            { "Locked", "N" },
                            { "LockedReason", (int)LockReasonEnum.ChangePwByVender },
                            { "Now", dtTime},
                            { "AC",vm.AC }
                        };

            GetDBHelper().Execute(update_SQL, paras_Update);
            GetDBHelper().Commit();
        }
        public string CheckVenderInfoCorrect(ChangePwVM vm)
        {
            string ErrorMsg = "";
            LoginHanlder LoginHanlder = new LoginHanlder(_configHelper, _httpContext);
            StoreVenderProfileVM StoreVenderProfileVM = new();

            StoreVenderProfileVM = GetStoreVenderProfileByLastUrlKey(vm.tempGuid);
            if (StoreVenderProfileVM == null)
            {
                ErrorMsg = "連結此已失效，請重新進行變更密碼";
                return ErrorMsg;
            }

            var validityperiod = 2;
            var _LastURLValidityperiod = Common.Method.GetAppSettingsDataByName("LastURLValidityperiod");
            if (int.TryParse(_LastURLValidityperiod, out validityperiod) == false)
            {
                validityperiod = 2;
            }
            if (StoreVenderProfileVM.LastUrlKey != vm.tempGuid || StoreVenderProfileVM.LastUrlTime.AddDays(validityperiod) <= DateTime.Now)
            {
                ErrorMsg = "連結此已失效，請重新進行變更密碼";
                return ErrorMsg;
            }

            List<vender_pw_historyDTO> VenderPwHistoryDTOs = GetTheNewestVenderPwHistory(StoreVenderProfileVM.merchant_login);

            var iv = Common.Method.GetAppSettingsDataByName("iv");
            var ke = Common.Method.GetAppSettingsDataByName("ke");
            var tempCommon = new Common.PasswordService();
            var newPw = tempCommon.HashPassword(vm.NewPD);

            if (VenderPwHistoryDTOs.Any(x => tempCommon.VerifyPassword(x.pw, vm.NewPD)))
            {
                ErrorMsg = "密碼與前3次相同，請重新建立";
                return ErrorMsg;
            }

            vm.AC = StoreVenderProfileVM.merchant_login;

            return ErrorMsg;
        }

        private StoreVenderProfileVM GetStoreVenderProfileByLastUrlKey(string lastUrlKey)
        {
            string sql = @"SELECT * FROM STORE_VENDER_PROFILE WHERE LastUrlKey= @LastUrlKey ";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "LastUrlKey", lastUrlKey },
            };
            StoreVenderProfileVM? result = base.dbHelper.Find<StoreVenderProfileVM>(sql, parameters);
            return result;
        }

        public StoreVenderProfileVM GetStoreVenderProfileNoPWD(string AC)
        {
            string sql = @"SELECT * FROM STORE_VENDER_PROFILE WHERE MERCHANT_LOGIN= @AC ";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "AC", AC },
            };
            StoreVenderProfileVM? result = base.dbHelper.Find<StoreVenderProfileVM>(sql, parameters);
            return result;
        }

        private StoreVenderProfileVM GetStoreVenderProfileWithACAndcp_tel(string AC, string cp_tel)
        {
            string sql = @"SELECT * FROM STORE_VENDER_PROFILE WHERE MERCHANT_LOGIN= @AC AND regexp_replace(cp_tel, '[^0-9]', '', 'g') = @cp_tel";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "AC", AC },
                { "cp_tel", cp_tel },
            };
            StoreVenderProfileVM? result = base.dbHelper.Find<StoreVenderProfileVM>(sql, parameters);
            return result;
        }

        private List<vender_pw_historyDTO> GetTheNewestVenderPwHistory(string AC)
        {
            string sql = @"SELECT * FROM tb_vender_password_history WHERE account = @AC and locked_reason=4 ORDER BY createtime DESC LIMIT 3";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "AC", AC },

            };
            List<vender_pw_historyDTO> VenderPwHistoryDTOs = new List<vender_pw_historyDTO>();
            VenderPwHistoryDTOs = base.dbHelper.FindList<vender_pw_historyDTO>(sql, parameters);
            return VenderPwHistoryDTOs;
        }
    }
}
