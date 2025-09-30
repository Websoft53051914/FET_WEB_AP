using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Security;
using System.Text;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class SupplierMgtHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public SupplierMgtHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal void Create(store_vender_profileDTO vm)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("merchant_name", vm.merchant_name);
            paras.Add("cp_name", vm.cp_name);
            paras.Add("cp_tel", vm.cp_tel);
            paras.Add("email", vm.email);
            paras.Add("merchant_login", vm.merchant_login);
            paras.Add("merchant_password", vm.merchant_password);

            string sql = @"
INSERT INTO store_vender_profile
(
    merchant_name,
    cp_name,
    cp_tel,
    email,
    merchant_login,
    merchant_password
)
VALUES
(
    @merchant_name,
    @cp_name,
    @cp_tel,
    @email,
    @merchant_login,
    @merchant_password
)";

            dbHelper.Execute(sql, paras);
            dbHelper.Commit();
        }

        internal void Delete(string order_id)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("order_id", order_id);

            string originSQL = @"
delete from store_vender_profile where order_id = @order_id
";

            dbHelper.Execute(originSQL, paras);
            dbHelper.Commit();
        }

        internal string? Edit(store_vender_profileDTO vm)
        {

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("order_id", vm.order_id);

            paras.Add("merchant_name", vm.merchant_name);
            paras.Add("cp_name", vm.cp_name);
            paras.Add("cp_tel", vm.cp_tel);
            paras.Add("email", vm.email);
            paras.Add("merchant_login", vm.merchant_login);
            paras.Add("merchant_password", vm.merchant_password);

            string sql = @"
UPDATE store_vender_profile
SET
    merchant_name     = @merchant_name,
    cp_name           = @cp_name,
    cp_tel            = @cp_tel,
    email             = @email,
    merchant_login    = @merchant_login,
    merchant_password = @merchant_password
WHERE order_id = @order_id";

            dbHelper.Execute(sql, paras);
            dbHelper.Commit();

            return "";
        }

        internal PageResult<store_vender_profileDTO> FindPageList(PageEntity pageEntity, store_vender_profileDTO dto)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("merchant_name", "%" + dto.merchant_name + "%");

            string strWhere = "";

            if (!string.IsNullOrEmpty(dto.merchant_name))
            {
                strWhere += " and merchant_name like @merchant_name ";
            }


            string originSQL = $@"
select * from store_vender_profile
where 1=1
{strWhere}
";

            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = dbHelper.FindPageList<store_vender_profileDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal store_vender_profileDTO GetDetail(string order_id)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("order_id", order_id);

            string strWhere = "";

            string originSQL = $@"
select * from
store_vender_profile
where order_id=@order_id
";

            var result = dbHelper.Find<store_vender_profileDTO>(originSQL, paras);
            return result;
        }

        internal string SendPWD(string order_id)
        {
            string msg = "";
            string eMail = GetFieldData("EMAIL", "STORE_VENDER_PROFILE", new Dictionary<string, object>() { { "order_id", order_id } });

            if (eMail != "")
            {
                SecureString sec = getPwdSecurity(GetFieldData("MERCHANT_PASSWORD", "STORE_VENDER_PROFILE", new Dictionary<string, object>() { { "order_id", order_id } }));

                Dictionary<string, object> paras = new Dictionary<string, object>();
                paras.Add("order_id", int.Parse(order_id));
                paras.Add("eMail", eMail);
                paras.Add("sec", sec.ToString());

                string sql = @"
INSERT INTO notify_profile
            (recordid,
             notifytype,
             deptcode,
             subject,
             alerttype,
             description,
             status,
             nexttime,
             opid)
VALUES      (@order_id,
             'SENDPWD',
             @eMail,
             '您的密碼',
             '2',
             '請使用密碼[@sec]登入FTT系統！',
             'P',
             To_char(sysdate, 'yyyy/mm/dd hh24:mi:ss'),
             'system') 
";
                dbHelper.Execute(sql, paras);
                dbHelper.Commit();
            }
            else
            {
                msg = "查無eMail：" + order_id;
            }

            return msg;
        }

        private static SecureString getPwdSecurity(string value)
        {
            SecureString result = new SecureString();
            foreach (char c in value)
            {
                result.AppendChar(c);
            }

            return result;
        }
    }
}
