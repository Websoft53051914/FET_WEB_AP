using Const.DTO;
using Core.Utility.Helper.DB.Entity;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.ConfigurationHelper;
using System.Text;

namespace FTT_VENDER_API.Models.Handler
{
    /// <summary>
    /// 已派工
    /// </summary>
    public class DispatchedHandler : BaseDBHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatchedHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }
        private readonly ConfigurationHelper _configHelper;
        /// <summary>
        /// 登入資訊
        /// </summary>
        public SessionVO? SessionVO { get; set; } = null;

        /// <summary>
        /// 取得分頁資料
        /// </summary>
        /// <returns></returns>
        public PageResult<VFttForm2DTO> GetPageList(PageEntity pageEntity)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                { "ivr_code", SessionVO?.ivrcode ?? string.Empty },
            };
            if (string.IsNullOrWhiteSpace(pageEntity.Sort))
            {
                pageEntity.Sort = nameof(VFttForm2DTO.updatetime);
                pageEntity.Asc = "DESC";
            }

            string sql = $@"
SELECT DISTINCT form_no
                , GET_USER_NAME('SUBMITTER', ivrcode)         AS shop_name
                , tt_category
                , l2_desc
                , ciname
                , TO_CHAR(createtime, 'yyyy/mm/dd')           AS createtime_text
                , vender
                , statusname
                , updatetime
                , TO_CHAR(vendor_arrive_date, 'yyyy/mm/dd')   AS vendor_arrive_date_text
                , TO_CHAR(SYSDATE - 5, 'yyyy/mm/dd')          AS limit_date_text
                , (SELECT TO_CHAR(MIN(updatetime), 'yyyy/mm/dd')
                   FROM   ftt_form_log
                   WHERE  form_no = v_ftt_form2.form_no
                          AND newvalue = 'ASSIGN')            AS assign_date_text
                , EXISTS(SELECT 1
                         FROM   ftt_form_log ffl
                         WHERE  ffl.form_no = v_ftt_form2.form_no
                                AND ffl.oldvalue = 'CONFIRM'
                                AND ffl.newvalue = 'PRWP')    AS flag1
FROM   v_ftt_form2
WHERE  form_no IN (SELECT form_no
                   FROM   access_role
                   WHERE  deptcode = @ivr_code)
       AND statusname IN( '已派工', '待料中' )
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

            return GetDBHelper().FindPageList<VFttForm2DTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras, $"{pageEntity.Sort} {pageEntity.Asc}");
        }
    }
}
