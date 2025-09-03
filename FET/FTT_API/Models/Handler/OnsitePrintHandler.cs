using Const.DTO;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common.ConfigurationHelper;
using System.Data;
using System.Text;

namespace FTT_API.Models.Handler
{
    /// <summary>
    /// 列印到場單
    /// </summary>
    public class OnsitePrintHandler : BaseDBHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OnsitePrintHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }
        private readonly ConfigurationHelper _configHelper;

        /// <summary>
        /// 取得狀態為 PRWP 的分頁資料
        /// </summary>
        /// <param name="pageEntity"></param>
        /// <param name="ivrCode"></param>
        /// <returns></returns>
        public PageResult<VFttForm2DTO> GetPageListPrwp(PageEntity pageEntity, string ivrCode)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"ivr_code", ivrCode }
            };

            string sql = $@"
SELECT DISTINCT form_no
                , ivrcode
                , tt_category
                , l2_desc
                , ciname
                , TO_CHAR(createtime, 'yyyy/mm/dd hh24:mi:ss') AS createtime_text
                , vender
                , statusname
                , vendor_arrive_date
                , TO_CHAR(vendor_arrive_date, 'yyyy/mm/dd')    AS vendor_arrive_date_text
                , TO_CHAR(SYSDATE - 5, 'yyyy/mm/dd')           AS limit_date_text
                , (SELECT TO_CHAR(MIN(updatetime), 'yyyy/mm/dd')
                   FROM   ftt_form_log
                   WHERE  form_no = v_ftt_form2.form_no
                          AND newvalue = 'ASSIGN')             AS assign_date_text
FROM   v_ftt_form2
WHERE  statusid = 'PRWP'
       AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  User_Type = 'VENDOR'
                              AND deptcode = @ivr_code)
ORDER  BY vender 
";
            string sqlCount = $@"
SELECT
    COUNT(*)
FROM(
{sql}
) AS pageData
WHERE
    1 = 1
";

            return GetDBHelper().FindPageList<VFttForm2DTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
        }

        /// <summary>
        /// 取得狀態為 CONFIRM 的分頁資料
        /// </summary>
        /// <param name="pageEntity"></param>
        /// <param name="ivrCode"></param>
        /// <returns></returns>
        public PageResult<VFttForm2DTO> GetPageListConfirm(PageEntity pageEntity, string ivrCode)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"ivr_code", ivrCode }
            };

            string sql = $@"
SELECT DISTINCT form_no
                , ivrcode
                , tt_category            
                , l2_desc
                , ciname
                , TO_CHAR(createtime, 'yyyy/mm/dd hh24:mi:ss') AS createtime_text
                , vender
                , statusname
                , TO_CHAR(vendor_arrive_date, 'yyyy/mm/dd')    AS vendor_arrive_date_text            
FROM   v_ftt_form2
WHERE  statusid = 'CONFIRM'
       AND form_no IN (SELECT form_no
                       FROM   access_role
                       WHERE  deptcode = @ivr_code)
ORDER  BY vender 
";
            string sqlCount = $@"
SELECT
    COUNT(*)
FROM(
{sql}
) AS pageData
WHERE
    1 = 1
";

            return GetDBHelper().FindPageList<VFttForm2DTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formNoList"></param>
        /// <returns></returns>
        public DataTable GetDataTablePrintWP(List<int> formNoList)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"form_no", formNoList },
            };

            string sql = $@"
SELECT * 
from v_ftt_form 
WHERE form_no IN @form_no
";

            return GetDBHelper().FindDataTable(sql, paras);
        }

        /// <summary>
        /// 更新 ftt_form.vendor_arrive_date
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="vendorArriveDate"></param>
        public void UpdateVendorArriveDate(int formNo, DateTime vendorArriveDate)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"form_no", formNo },
                {"vendor_arrive_date", vendorArriveDate },
            };

            string sql = $@"
UPDATE ftt_form
SET    vendor_arrive_date = @vendor_arrive_date
WHERE  form_no = @form_no 
";

            GetDBHelper().Execute(sql, paras);
        }
    }
}
