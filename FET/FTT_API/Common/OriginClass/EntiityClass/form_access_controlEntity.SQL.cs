using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class form_access_controlSQL
    {
        public form_access_controlDTO GetInfoByFormNo(string form_no)
        {

            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", form_no);

            string sqlWhere = "";

            string qrySQL = $@"
SELECT *,
       (SELECT status_name
        FROM   form_access_status
        WHERE  form_type = 'FTT_FORM'
               AND form_access_status.status = form_access_control.status) AS STATUS_NAME
FROM   form_access_control
WHERE  form_no =@form_no 
";

            return baseHandler.GetDBHelper().Find<form_access_controlDTO>(qrySQL, paras);

        }

        internal form_access_controlDTO GetInfo(string IVRCode, string FormType, string TSTATUS, string FormNo, string EmpNo)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("IVRCode", IVRCode);
            paras.Add("FormType", FormType);
            paras.Add("TSTATUS", TSTATUS);
            paras.Add("FormNo", FormNo);
            paras.Add("EmpNo", EmpNo);

            string qrySQL = "";
            if (IVRCode == "" || IVRCode == "NULL")
                qrySQL = "select * from form_access_control where form_type=@FormType and (status=@TSTATUS) and User_Type IN (SELECT User_Type FROM V_ACCESS_ROLE WHERE FORM_TYPE=@FormType AND FORM_NO=@FormNo AND (EMPNO=@EmpNo or instr(AGENT,@EmpNo)>0 OR USER_GROUP IN (SELECT FTT_GROUP AS USER_GROUP FROM FTT_GROUP WHERE EMPNO=@EmpNo))) order by orderid";
            else
                qrySQL = "select * from form_access_control where form_type=@FormType and (status=@TSTATUS) and User_Type IN (SELECT User_Type FROM V_ACCESS_ROLE WHERE FORM_TYPE=@FormType AND FORM_NO=@FormNo AND (EMPNO=@EmpNo or DEPTCODE=@IVRCode or instr(AGENT,@EmpNo)>0 OR USER_GROUP IN (SELECT FTT_GROUP AS USER_GROUP FROM FTT_GROUP WHERE EMPNO=@EmpNo))) order by orderid";

            return baseHandler.GetDBHelper().Find<form_access_controlDTO>(qrySQL, paras);
        }
    }
}
