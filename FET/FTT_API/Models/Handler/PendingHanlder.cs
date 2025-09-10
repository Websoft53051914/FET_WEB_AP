using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System.ServiceModel;
using System.Text;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class PendingHanlder : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public PendingHanlder(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal bool CheckDataExist_CI_EXCEPTION_CONFIG(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                { "FORM_NO", form_no },
            };

            string tableName = " CI_EXCEPTION_CONFIG ";
            string strWhere = " ENABLE='Y' AND CISID IN (SELECT CATEGORY_ID FROM FTT_FORM WHERE FORM_NO=@FORM_NO ) AND IVRCODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=@FORM_NO ) AND SYSDATE-APPROVAL_DATE<=365 ";

            return CheckDataExist(tableName, strWhere, paras);
        }

        internal bool CheckDataExist_Ftt_form_amount(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                { "FORM_NO", form_no },
            };

            string tableName = " Ftt_form_amount ";
            string strWhere = " FORM_NO=@FORM_NO AND ENABLE='Y' ";

            return CheckDataExist(tableName, strWhere, paras);

        }

        internal bool CheckDataExist_FTT_FORM_LOG(string form_no, string fieldName, string oldValue, string newValue)
        {
            Dictionary<string, object> paras = new()
            {
                { "FORM_NO", form_no },
                { "FIELDNAME", fieldName },
                { "OLDVALUE", oldValue },
                { "NEWVALUE", newValue },
            };

            string tableName = " FTT_FORM_LOG ";
            string strWhere = " FORM_NO=@FORM_NO AND FIELDNAME=@FIELDNAME AND OLDVALUE=@OLDVALUE AND NEWVALUE=@NEWVALUE ";

            return CheckDataExist(tableName, strWhere, paras);
        }

        internal bool CheckDataExist_STORE_PROFILE(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                { "FORM_NO", form_no },
            };

            string tableName = " STORE_PROFILE ";
            string strWhere = " IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=@FORM_NO) AND SYSDATE BETWEEN APPROVAL_DATE AND APPROVAL_DATE+365 ";

            return CheckDataExist(tableName, strWhere, paras);
        }

        internal bool CheckDataExist_STORE_PROFILENotIn1278And1260(string form_no)
        {
            //IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=" + formNo + " and ci_sid_l1 (category_id) not in (1278,1260))

            Dictionary<string, object> paras = new()
            {
                { "FORM_NO", form_no },
            };

            string tableName = " STORE_PROFILE ";
            string strWhere = " IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=@FORM_NO and ci_sid_l1 (category_id) not in (1278,1260)) ";

            return CheckDataExist(tableName, strWhere, paras);
        }

        internal approve_formDTO GetApproveFormInfo(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                {"FORM_NO", form_no },
            };

            string sql = @" 
            SELECT 
            STATUS,PRIOR_STATUS,STATUS_ORDERID,FORM_TYPE,(SELECT STATUS_NAME FROM FORM_ACCESS_STATUS WHERE FORM_TYPE='FTT_FORM' AND FORM_ACCESS_STATUS.STATUS=APPROVE_FORM.STATUS) as STATUS_NAME 
            FROM APPROVE_FORM WHERE FORM_NO=@FORM_NO
                ";

            return GetDBHelper().Find<approve_formDTO>(sql, paras);
        }

        internal string GetCreateTime(string form_no)
        {
            return GetFieldData("to_char(CREATETIME,'yyyy/mm/dd hh24:mi:ss')", "FTT_FORM", new Dictionary<string, object>() { { "FORM_NO", form_no } });
        }

        internal string GetFormTypeByFormNo(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                {"FORM_NO", form_no },
            };

            string sql = @" 
SELECT 
FORM_TYPE
FROM APPROVE_FORM 
where FORM_NO=@FORM_NO
GROUP BY FORM_TYPE

                ";

            return GetDBHelper().FindScalar<string>(sql, paras);
        }

        internal bool CheckDataExist_FTT_FORM_AMOUNT(string form_no)
        {
            //FTT_FORM_AMOUNT", "FORM_NO='" + Request.QueryString["FORM_NO"] + "' AND ENABLE='Y'
            Dictionary<string, object> paras = new()
            {
                { "FORM_NO", form_no },
            };

            string tableName = " FTT_FORM_AMOUNT ";
            string strWhere = " FORM_NO=@FORM_NO AND ENABLE='Y' ";

            return CheckDataExist(tableName, strWhere, paras);
        }

        internal Ftt_formDTO GetFttFormInfo(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                {"FORM_NO", form_no },
            };

            string sql = @" 
SELECT b.cp_name,b.cp_tel,b.merchant_name,a.*,(SELECT CINAME FROM CI_RELATIONS WHERE CI_RELATIONS.CISID=a.CATEGORY_ID AND ROWNUM=1) as CIDesc 
FROM FTT_FORM a
left outer join store_vender_profile b on a.vender_id=b.order_id
WHERE FORM_NO=@FORM_NO 
";

            return GetDBHelper().Find<Ftt_formDTO>(sql, paras);
        }

        internal string GetIVRCode(string form_no)
        {
            return GetFieldData("IVRCODE", "FTT_FORM", new Dictionary<string, object>() { { "FORM_NO", form_no } });
        }

        internal List<Ftt_form_amountDTO> GetList_FttFormAmount(string form_no)
        {
            Dictionary<string, object> paras = new()
            {
                {"FORM_NO", form_no },
            };

            string sql = @" 
                
                ";

            return GetDBHelper().FindList<Ftt_form_amountDTO>(sql, paras);
        }

        internal string GetShopName(string mIVRCode)
        {

            return GetFieldData("SHOP_NAME", "STORE_PROFILE", new Dictionary<string, object>() { { "IVR_CODE", mIVRCode } });
        }

        internal Store_profileDTO GetStoreProfileInfoByFormNo(string form_no)
        {

            Dictionary<string, object> paras = new()
            {
                {"FORM_NO", form_no },
            };

            string sql = " select * from STORE_PROFILE where IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=@FORM_NO) ";

            return GetDBHelper().Find<Store_profileDTO>(sql, paras);
        }

        internal string GetTotalPrice(string form_no)
        {
            //select sum(qty*price) from FTT_FORM_AMOUNT WHERE FORM_NO=" + Server.HtmlEncode(FORM_NO.Value) + "  AND ENABLE='Y'
            Dictionary<string, object> paras = new()
            {
                {"FORM_NO", form_no },
            };

            string sql = " select sum(qty*price) from FTT_FORM_AMOUNT WHERE FORM_NO=@FORM_NO  AND ENABLE='Y' ";
            return GetDBHelper().FindScalar<string>(sql, paras);
        }

        internal bool CheckDataExist_AMOUNT_SELECT(string category_id)
        {
            //if (db.CheckDataExist("AMOUNT_SELECT", "ENABLE='Y' AND CHK_CI_LIST('" + CATEGORY_ID.Value + "'::text,category_id::text)='Y'") == true)
            Dictionary<string, object> paras = new()
            {
                { "category_id", category_id },
            };

            string tableName = " AMOUNT_SELECT ";
            string strWhere = " ENABLE='Y' AND CHK_CI_LIST(@category_id::text,category_id::text)='Y' ";

            return CheckDataExist(tableName, strWhere, paras);
        }

        internal List<amount_selectDTO> GetListAMOUNT_SELECT(string category_id)
        {
            Dictionary<string, object> paras = new()
            {
                {"category_id", category_id },
            };

            string sql = @" 
SELECT '' as EXPENSE_TYPE
FROM DUAL
UNION
SELECT DISTINCT EXPENSE_TYPE as EXPENSE_TYPE
FROM AMOUNT_SELECT
WHERE ENABLE = 'Y' AND CHK_CI_LIST(@category_id,category_id::text)= 'Y'"";
                ";

            return GetDBHelper().FindList<amount_selectDTO>(sql, paras);
        }

        internal void InsertFttFormAmount(string form_no, string expense_type, string expense_desc, string qty, string price, string subtotal, decimal? orderid, string unit, string fault_reason, string repair_action)
        {
            Dictionary<string, object> paras = new()
            {
                {"form_no", form_no },
                {"expense_type", expense_type },
                {"expense_desc", expense_desc },
                {"qty", qty },
                {"price", price },
                {"subtotal", subtotal },
                {"orderid", orderid },
                {"unit", unit },
                {"fault_reason", fault_reason },
                {"repair_action", repair_action },
            };

            var inserSQL = "";
            inserSQL = @"
INSERT INTO Ftt_form_amount 
(   FORM_NO,    EXPENSE_TYPE,   EXPENSE_DESC,   QTY,    PRICE,  SUBTOTAL,   ORDERID,    UNIT,   FAULT_REASON,   REPAIR_ACTION
) 
VALUES 
(   @form_no,   @expense_type,  @expense_desc,  @qty,   @price, @subtotal,  @orderid,   @unit,  @fault_reason,  @repair_action
)";

            GetDBHelper().Execute(inserSQL, paras);
        }

        internal void DeleteFttFormAmount(string form_no)
        {
            //          baseHandler.GetDBHelper().Execute("DELETE FROM Ftt_form_amount WHERE FORM_NO = '" + vm.form_no + "'", null);

            Dictionary<string, object> paras = new()
            {
                {"FORM_NO", form_no },
            };

            var strSQL = "";
            strSQL = @"
DELETE FROM Ftt_form_amount WHERE FORM_NO = @FORM_NO
";

            GetDBHelper().Execute(strSQL, paras);

            throw new NotImplementedException();
        }

        internal PageResult<ftt_form_descDTO> GetPageList_Desc(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", dto.form_no);

            string originSQL = @"
SELECT ROWNUM                                          AS ROWCOUNT,
       create_date,
       Substr(User_Type, 1, Instr(User_Type, ',') - 1) AS User_Type,
       action_name,
       description,
       Get_status_real_name('FTT_FORM', prior_status)  AS PRIOR_STATUS,
       Get_status_real_name('FTT_FORM', status)        AS STATUS
FROM   ftt_form_desc
WHERE  ( ROWNUM = 0 ) 
";

            if (!string.IsNullOrEmpty(dto.form_no))
            {
                switch (dto.USERROLE.ToUpper())
                {
                    case "VENDOR":
                        originSQL = @"
SELECT ROWNUM                                          AS ROWCOUNT,
       create_date,
       Substr(user_type, 1, Instr(user_type, ',') - 1) AS USER_TYPE,
       action_name,
       description,
       Get_status_real_name('FTT_FORM', prior_status)  AS PRIOR_STATUS,
       Get_status_real_name('FTT_FORM', status)        AS STATUS
FROM   ftt_form_desc
WHERE  user_type NOT LIKE '%MANAGER%'
       AND form_no =@form_no
ORDER  BY create_date 
";
                        break;
                    default:
                        originSQL = @"
SELECT ROWNUM                                          AS ROWCOUNT,
       create_date,
       Substr(user_type, 1, Instr(user_type, ',') - 1) AS USER_TYPE,
       action_name,
       description,
       Get_status_real_name('FTT_FORM', prior_status)  AS PRIOR_STATUS,
       Get_status_real_name('FTT_FORM', status)        AS STATUS
FROM   ftt_form_desc
WHERE  form_no =@form_no
ORDER  BY create_date 
";
                        break;
                }
            }

            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = GetDBHelper().FindPageList<ftt_form_descDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal PageResult<ftt_form_logDTO> GetPageList_Log(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", dto.form_no);

            string originSQL = @"
SELECT ROWNUM                                     AS ROWCOUNT,
       updatetime,
       fieldname,
       Get_status_real_name('FTT_FORM', oldvalue) AS OLDVALUE,
       Get_status_real_name('FTT_FORM', newvalue) AS NEWVALUE,
       Get_employee_name('empno', update_empno)   AS UPDATE_EMPNO,
       update_engname
FROM   ftt_form_log
WHERE  ( ROWNUM = 0 ) 
";

            if (!string.IsNullOrEmpty(dto.form_no))
            {
                switch (dto.USERROLE.ToUpper())
                {
                    case "VENDOR":
                        originSQL = @"
SELECT ROWNUM                                     AS ROWCOUNT,
       updatetime,
       fieldname,
       Get_status_real_name('FTT_FORM', oldvalue) AS OLDVALUE,
       Get_status_real_name('FTT_FORM', newvalue) AS NEWVALUE,
       Get_employee_name('empno', update_empno)   AS UPDATE_EMPNO,
       update_engname
FROM   ftt_form_log
WHERE  oldvalue NOT IN ( 'REVIEW', 'REVIEW2', 'AGREE' )
       AND form_no = @form_no
       AND fieldname <> 'STATUS'
ORDER  BY updatetime 
";
                        break;
                    default:
                        originSQL = @"
SELECT ROWNUM                                     AS ROWCOUNT,
       updatetime,
       fieldname,
       Get_status_real_name('FTT_FORM', oldvalue) AS OLDVALUE,
       Get_status_real_name('FTT_FORM', newvalue) AS NEWVALUE,
       Get_action_name(update_empno)              AS UPDATE_EMPNO,
       update_engname
FROM   ftt_form_log
WHERE  form_no = @form_no
ORDER  BY updatetime 
";
                        break;
                }
            }

            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = GetDBHelper().FindPageList<ftt_form_logDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal PageResult<v_access_roleView> GetPageList_V_ACCESS_ROLE(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", dto.form_no);

            string originSQL = @"
SELECT form_type,
       user_type,
       empno,
       deptcode,
       engname,
       action,
       Get_form_status(form_type, form_no) AS STATUSID
FROM   v_access_role
WHERE  form_no = @form_no
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

            var result = GetDBHelper().FindPageList<v_access_roleView>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal PageResult<form_access_statusDTO> GetPageList_Access(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", dto.form_no);

            string originSQL = @"
SELECT DISTINCT status,
                status_name
FROM   form_access_status 
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

            var result = GetDBHelper().FindPageList<form_access_statusDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal PageResult<store_vender_profileDTO> GetPageList_Vender(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("form_no", dto.form_no);

            string originSQL = @"

SELECT order_id,
       merchant_name
FROM   store_vender_profile
ORDER  BY order_id 

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

            var result = GetDBHelper().FindPageList<store_vender_profileDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal void UpdateAccessRole(decimal? form_no, string user_type, string empno, string deptcode, string updateEmpno)
        {
            Dictionary<string, object> paras = new()
            {
                {"form_no", form_no },
                {"user_type", user_type },
                {"empno", empno },
                {"deptcode", deptcode },
                {"updateEmpno", updateEmpno },
            };

            var inserSQL = "";
            inserSQL = @"
Update Access_Role 

Set 
Empno =@empno, 
Deptcode = @deptcode, 
Update_Empno = @updateEmpno, 
UpdateTime = SysDate 

Where 
Form_No =@form_no 
And User_Type = @user_type
";

            GetDBHelper().Execute(inserSQL, paras);
        }

        internal void UpdateFttForm_VENDOR(decimal? form_no, string deptcode)
        {
            Dictionary<string, object> paras = new()
            {
                {"form_no", form_no },
                {"deptcode", decimal.Parse(deptcode) },
            };

            var inserSQL = "";
            inserSQL = @"
UPDATE FTT_FORM 
SET VENDER_ID=@deptcode
WHERE FORM_NO=@form_no
";

            GetDBHelper().Execute(inserSQL, paras);
        }

        internal void UpdateApproveForm(string form_no, string statusId, string empno)
        {
            Dictionary<string, object> paras = new()
            {
                {"form_no", form_no },
                {"statusId", statusId },
                {"empno", empno },
            };

            var inserSQL = "";
            inserSQL = @"
Update Approve_Form 

Set 
Status = @statusId , 
Update_Empno = @empno, 
UpdateTime = SysDate  

Where 
Form_No = @form_no

";

            GetDBHelper().Execute(inserSQL, paras);
        }

        internal List<amount_selectDTO> GetAmountSelectInfo(string categoryID, string expenseType)
        {
            //AMOUNT_SELECT
            Dictionary<string, object> paras = new()
            {
                {"categoryID", categoryID },
                {"expenseType", expenseType },
            };

            string sql = @" 
SELECT 
DISTINCT ID, 
DECODE(L2_DESC,NULL,L1_DESC,L1_DESC || '-' || L2_DESC) as dataValue 
FROM AMOUNT_SELECT 
WHERE 
ENABLE='Y' 
AND CHK_CI_LIST(@categoryID::text,category_id::text)='Y' AND EXPENSE_TYPE=@expenseType
ORDER BY ID
                ";

            return GetDBHelper().FindList<amount_selectDTO>(sql, paras);
        }

        internal IEnumerable<amount_selectDTO> GetAmountSelectInfoById(string id)
        {
            Dictionary<string, object> paras = new()
            {
                {"id", id },
            };

            string sql = @" 
SELECT 

ID, 
UNIT, 
QTY, 
PRICE, 
REMARK 

FROM 
AMOUNT_SELECT 

WHERE ENABLE='Y' 
AND ID=@id
                ";

            return GetDBHelper().FindList<amount_selectDTO>(sql, paras);
        }
    }
}
