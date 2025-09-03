using Const.DTO;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common.ConfigurationHelper;
using System.Text;

namespace FTT_API.Models.Handler
{
    /// <summary>
    /// 門市報修管理-查詢
    /// </summary>
    public class QueryHandler : BaseDBHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QueryHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }
        private readonly ConfigurationHelper _configHelper;

        /// <summary>
        /// 取得分頁資料
        /// </summary>
        public PageResult<VFttForm2DTO> GetPageList(PageEntity pageEntity, VFttForm2DTO searchVO)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
SELECT DISTINCT form_no
                , tt_category
                , ciname
                , TO_CHAR(createtime, 'YYYY/MM/DD HH24:MI:SS')   AS createtime_text
                , shop_name
                , statusname
                , TO_CHAR(dispatchtime, 'YYYY/MM/DD HH24:MI:SS') AS dispatchtime_text
                , vender
                , descr
                , FIND_ACTION_NAME(form_no)                      AS processer
FROM v_ftt_form2
WHERE  1 = 1
{condition}
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
    }
}
