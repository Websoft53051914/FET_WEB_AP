using Const.DTO;
using Core.Utility.Helper.DB.Entity;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.ConfigurationHelper;
using System.Text;

namespace FTT_VENDER_API.Models.Handler
{
    /// <summary>
    /// 派工中
    /// </summary>
    public class DispatchingHandler : BaseDBHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatchingHandler(ConfigurationHelper confighelper)
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
                { "user_type", SessionVO?.usertype ?? string.Empty },
                { "empno", SessionVO?.empno ?? string.Empty },
            };
            if (string.IsNullOrWhiteSpace(pageEntity.Sort))
            {
                pageEntity.Sort = nameof(VFttForm2DTO.updatetime);
                pageEntity.Asc = "DESC";
            }

            string sql = $@"
SELECT DISTINCT form_no                                        
                , tt_category                                  
                , l2_desc                                      
                , ciname                                       
                , TO_CHAR(createtime, 'yyyy/mm/dd hh24:mi:ss') AS createtime_text
                , shop_name                                    
                , statusname 
                , updatetime
                , TO_CHAR(updatetime, 'yyyy/mm/dd hh24:mi:ss') AS updatetime_text
FROM   v_ftt_form2
WHERE  form_no IN (SELECT form_no
                   FROM   access_role
                   WHERE  action = 'Y'
                          AND deptcode = @ivr_code
                          AND user_type = @user_type
                          AND @empno IS NOT NULL)
       AND statusname = '派工中'
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
