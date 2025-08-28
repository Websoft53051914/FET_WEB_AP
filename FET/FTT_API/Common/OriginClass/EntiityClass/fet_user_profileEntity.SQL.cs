using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class fet_user_profileSQL
    {

        internal fet_user_profileDTO GetInfoByEmpno(string empno)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("empno", empno);

            string sqlWhere = "";

            string qrySQL = $@"
select * from fet_user_profile 
where empno=@empno
";

            return baseHandler.GetDBHelper().Find<fet_user_profileDTO>(qrySQL, paras);
        }
    }
}
