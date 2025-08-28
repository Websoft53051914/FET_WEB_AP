using FTT_WEB.Common.ConfigurationHelper;
using FTT_WEB.Common.OriginClass.EntiityClass;
using System.Text;
using System.Transactions;

namespace FTT_WEB.Models.Handler
{
    public class NewOrderHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        public NewOrderHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }

        public string RetrieveTTCount(int cisid, string ivrCode)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"CATEGORY_ID", cisid },
                {"IVRCODE", ivrCode },
            };

            string sql = $@"
SELECT decode(count(FORM_NO),0,'NO','YES')
FROM FTT_FORM 
WHERE CATEGORY_ID=@CATEGORY_ID
AND CREATETIME > to_date(to_char(sysdate,'yyyy/mm/dd')||' 00:00:00','yyyy/mm/dd hh24:mi:ss')
LIMIT 1
";

            return GetDBHelper().FindScalar<string>(sql, paras);
        }

        public List<FormDispatchGetDTO> GetListFormDispatchGet(int cisid, int ivrCode, string ifWarrant)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"CISID", cisid },
                {"IVRCODE", ivrCode },
                {"IFWARRANT", ifWarrant },
            };

            string sql = $@"
SELECT *
FROM TABLE(form_dispatch_get(@IVRCODE, @CISID, @IFWARRANT));
";
#if DEBUG
            sql = $@"
SELECT *
FROM form_dispatch_get(@IVRCODE, @CISID, @IFWARRANT);
";
#endif

            return GetDBHelper().FindList<FormDispatchGetDTO>(sql, paras);
        }

        public int GetCountVDispatchList(int cisid, string ivrCode)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"CISID", cisid },
                {"IVRCODE", ivrCode },
            };

            string sql = $@"
SELECT case when sysdate>approval_date then 1 else 0 end
FROM v_dispatch_list 
WHERE IVR_CODE=@IVRCODE AND CISID = @CISID
";

            return GetDBHelper().FindScalar<int>(sql, paras);
        }

        public int GetNextTTNo()
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
SELECT TT_NO_SEQ.NEXTVAL
FROM DUAL
";

            return GetDBHelper().FindScalar<int>(sql, paras);
        }

        public string GetValCIDescL1(int categoryId)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                { "CATEGORY_ID", categoryId },
            };

            string sql = $@"
SELECT CI_DESC_L1(@CATEGORY_ID)
FROM DUAL
";

            return GetDBHelper().FindScalar<string>(sql, paras);
        }

        public void DoCreateFttForm(List<Dictionary<string, object>> dataList)
        {
            string sqlInsert = @"
INSERT INTO public.ftt_form(
	form_no, ivrcode, category_id, category_name, createtime, empname, emptel, descr, checkitem, tt_category, order_id, tt_no, remark, vender_id, tt_type, repair, resupply, selfconfig)
	VALUES (@form_no, @ivrcode, @category_id, @category_name, @createtime, @empname, @emptel, @descr, @checkitem, @tt_category, @order_id, @tt_no, @remark, @vender_id, @tt_type, @repair, @resupply, @selfconfig)
";
            string sqlApprovalGenerate = @"
SELECT APPROVAL_GENERATE(@formtype, @form_no, @tt_no, @ivrcode, null, null, null, null)
";

            //using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            foreach (Dictionary<string, object> data in dataList)
            {
                GetDBHelper().Execute(sqlInsert, data);
                GetDBHelper().Execute(sqlApprovalGenerate, data);
            }

            GetDBHelper().Commit();
            //scope.Complete();
        }
        /// <summary>
        /// [Form/checkdata.asp]
        /// </summary>
        public int GetCountRepairReported(int categoryId, string ivrCode)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"CATEGORY_ID", categoryId },
                {"IVRCODE", ivrCode },
            };

            string sql = $@"
SELECT COUNT(FORM_NO) 
FROM V_FTT_FORM 
WHERE statusid not in ('CANCEL','CLOSE','REJECT','REVIEW2','COMPLETE','TICKET','NOSHOW') 
AND IVRCODE = @IVRCODE
AND CATEGORY_ID = @CATEGORY_ID
";

            return GetDBHelper().FindScalar<int>(sql, paras);
        }
    }
}
