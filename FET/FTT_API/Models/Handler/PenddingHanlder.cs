using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using System.Text;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class PenddingHanlder : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public PenddingHanlder(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal string GetCreateTime(string form_no)
        {
            return GetFieldData("to_char(CREATETIME,'yyyy/mm/dd hh24:mi:ss')", "FTT_FORM", new Dictionary<string, object>() { { "FORM_NO", form_no } });
        }

        internal ftt_formDTO GetFttFormInfo(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                {"form_no", form_no },
            };

            string sql = " SELECT FTT_FORM.*,(SELECT CINAME FROM CI_RELATIONS WHERE CI_RELATIONS.CISID=FTT_FORM.CATEGORY_ID AND ROWNUM=1) as CIDesc FROM FTT_FORM WHERE FORM_NO=@form_no ";

            return GetDBHelper().Find<ftt_formDTO>(sql, paras);
        }

        internal string GetIVRCode(string form_no)
        {
            return GetFieldData("IVRCODE", "FTT_FORM", new Dictionary<string, object>() { { "FORM_NO", form_no } });
        }

        internal string GetShopName(string mIVRCode)
        {

            return GetFieldData("SHOP_NAME", "STORE_PROFILE", new Dictionary<string, object>() { { "IVR_CODE", mIVRCode } });
        }
    }
}
