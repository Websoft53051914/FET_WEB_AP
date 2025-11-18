using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.ViewModel.ChangePw;
using FTT_VENDER_API.Models.ViewModel.StoreVenderProfile;
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

            string InsertVenderPwHistorySQL = "";
            Dictionary<string, object> ParasForInsertVenderPwHistory = new();
            InsertVenderPwHistorySQL = @$"INSERT INTO tb_vender_pw_history (account,pw) VALUES
                         (@account,@pw) ";
            ParasForInsertVenderPwHistory = new Dictionary<string, object>
                        {
                            { "account", vm.AC },
                            { "pw", vm.NewPD },
                        };

            GetDBHelper().Execute(InsertVenderPwHistorySQL, ParasForInsertVenderPwHistory);

            string UpdateStoreVenderProfileSQL = "";
            Dictionary<string, object> ParasForUpdateStoreVenderProfile = new();
            UpdateStoreVenderProfileSQL = @$"UPDATE store_vender_profile SET 
merchant_password = @pw, 
locked = @Locked, 
locked_reason = @LockedReason, 
is_pwchange_remind = NULL, 
pw_chgtime = @Now 
WHERE merchant_login = @AC;";

            ParasForUpdateStoreVenderProfile = new Dictionary<string, object>
                        {
                            { "pw", vm.NewPD },
                            { "Locked", "N" },
                            { "LockedReason", (int)LockReasonEnum.ChangePwByVender },
                            { "Now", DateTime.Now },
                            { "AC",vm.AC }
                        };

            GetDBHelper().Execute(UpdateStoreVenderProfileSQL, ParasForUpdateStoreVenderProfile);
            GetDBHelper().Commit();            

        }
        public string CheckVenderInfoCorrect(ChangePwVM vm)
        {
            string ErrorMsg = "";
            LoginHanlder LoginHanlder = new LoginHanlder(_configHelper, _httpContext);
            StoreVenderProfileVM StoreVenderProfileVM = new();

            StoreVenderProfileVM = GetStoreVenderProfileNoPWD(vm.AC);
            if (StoreVenderProfileVM == null)
            {
                ErrorMsg = "帳號不正確請確認";
                return ErrorMsg;
            }

            StoreVenderProfileVM = GetStoreVenderProfileWithACAndcp_tel(vm.AC, vm.cp_tel);
            if (StoreVenderProfileVM == null)
            {
                ErrorMsg = "聯絡電話不正確請確認";
                return ErrorMsg;
            }

            List<vender_pw_historyDTO> VenderPwHistoryDTOs = GetTheNewestVenderPwHistory(vm.AC);
            if (VenderPwHistoryDTOs.Any(x => x.pw == vm.NewPD))
            {
                ErrorMsg = "密碼與前3次相同，請重新建立";
                return ErrorMsg;
            }
            return ErrorMsg;
        }



        private StoreVenderProfileVM GetStoreVenderProfileNoPWD(string AC)
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
            string sql = @"SELECT * FROM tb_vender_pw_history WHERE account = @AC ORDER BY createtime DESC LIMIT 3";
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
