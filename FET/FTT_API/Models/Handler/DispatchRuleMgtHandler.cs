using Const.DTO;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common.ConfigurationHelper;
using System.Text;

namespace FTT_API.Models.Handler
{
    /// <summary>
    /// 派工規則維護
    /// </summary>
    public class DispatchRuleMgtHandler : BaseDBHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DispatchRuleMgtHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }
        private readonly ConfigurationHelper _configHelper;

        /// <summary>
        /// 取得分頁資料
        /// </summary>
        public PageResult<DispatchProfileDTO> GetPageList(PageEntity pageEntity)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];
            // id == 0 為原專案產生建立列使用
            string sql = $@"
SELECT GET_DISPATCH_IDNAME('ID', 'VENDER', id)     AS tvender
       , GET_DISPATCH_IDNAME('NAME', 'VENDER', id) AS vender
       , GET_DISPATCH_IDNAME('ID', 'IVRCODE', id)  AS tivr_code
       , 'IVRCODE'                                 AS store
       , GET_DISPATCH_IDNAME('ID', 'CISID', id)    AS tcisid
       , GET_DISPATCH_IDNAME('NAME', 'CISID', id)  AS ci_name
       , ifwarrant                                 
       , id                                      
FROM   dispatch_profile
WHERE 1 = 1
AND id <> 0
{condition}
ORDER  BY id 
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

            return GetDBHelper().FindPageList<DispatchProfileDTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <returns></returns>
        public DispatchProfileDTO GetOneDispatchProfile(int id)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
               { "id", id }
            };

            // id == 0 為原專案產生建立列使用
            string sql = $@"
SELECT GET_DISPATCH_IDNAME('ID', 'VENDER', id)     AS tvender
       , GET_DISPATCH_IDNAME('NAME', 'VENDER', id) AS vender
       , GET_DISPATCH_IDNAME('ID', 'IVRCODE', id)  AS tivr_code
       , 'IVRCODE'                                 AS store
       , GET_DISPATCH_IDNAME('ID', 'CISID', id)    AS tcisid
       , GET_DISPATCH_IDNAME('NAME', 'CISID', id)  AS ci_name
       , ifwarrant                                 
       , id                                      
FROM   dispatch_profile
WHERE 1 = 1
AND id <> 0
AND id = @id
{condition}
ORDER  BY id 
LIMIT 1
";

            return GetDBHelper().FindList<DispatchProfileDTO>(sql, paras).FirstOrDefault();
        }

        /// <summary>
        /// 取得分頁資料
        /// </summary>
        public PageResult<VDispatchQueryDTO> GetPageListQuery(PageEntity pageEntity, VDispatchQueryDTO searchVO)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];
            if (string.IsNullOrWhiteSpace(pageEntity.Sort))
            {
                pageEntity.Sort = nameof(VDispatchQueryDTO.ivr_code);
            }

            if (!string.IsNullOrWhiteSpace(searchVO.CategoryIdFilter))
            {
                condition.Append($@"AND cisid IN (SELECT cisid FROM ci_data
WHERE instr('-' || acisid || '-', '-' || @{nameof(searchVO.CategoryIdFilter)} || '-') > 0) ");
                paras.Add(nameof(searchVO.CategoryIdFilter), searchVO.CategoryIdFilter);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.VenderIdEq))
            {
                condition.Append($"AND {nameof(VDispatchQueryDTO.vender_id)} = @{nameof(searchVO.VenderIdEq)} ");
                paras.Add(nameof(searchVO.VenderIdEq), searchVO.VenderIdEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.IvrCodeEq))
            {
                condition.Append($"AND {nameof(VDispatchQueryDTO.ivr_code)} = @{nameof(searchVO.IvrCodeEq)} ");
                paras.Add(nameof(searchVO.IvrCodeEq), searchVO.IvrCodeEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.CompanyEq))
            {
                condition.Append($"AND {nameof(VDispatchQueryDTO.company)} = @{nameof(searchVO.CompanyEq)} ");
                paras.Add(nameof(searchVO.CompanyEq), searchVO.CompanyEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.StoreTypeEq))
            {
                condition.Append($"AND {nameof(VDispatchQueryDTO.store_type)} = @{nameof(searchVO.StoreTypeEq)} ");
                paras.Add(nameof(searchVO.StoreTypeEq), searchVO.StoreTypeEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.ChannelEq))
            {
                condition.Append($"AND {nameof(VDispatchQueryDTO.channel)} = @{nameof(searchVO.ChannelEq)} ");
                paras.Add(nameof(searchVO.ChannelEq), searchVO.ChannelEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.AreaEq))
            {
                condition.Append($"AND {nameof(VDispatchQueryDTO.area)} = @{nameof(searchVO.AreaEq)} ");
                paras.Add(nameof(searchVO.AreaEq), searchVO.AreaEq);
            }
            if (!string.IsNullOrWhiteSpace(searchVO.IfWarrantEq))
            {
                condition.Append($"AND {nameof(VDispatchQueryDTO.ifwarrant)} = @{nameof(searchVO.IfWarrantEq)} OR {nameof(VDispatchQueryDTO.ifwarrant)} IS NULL  ");
                paras.Add(nameof(searchVO.IfWarrantEq), searchVO.IfWarrantEq);
            }

            string sql = $@"
SELECT ivr_code
       , shop_name
       , l1_desc
       , l2_desc
       , ciname
       , warrant
       , nonwarrant
FROM   v_dispatch_query
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

            return GetDBHelper().FindPageList<VDispatchQueryDTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras, $"{pageEntity.Sort} {pageEntity.Asc}");
        }

        /// <summary>
        /// 執行 dispatch_delete
        /// </summary>
        public void ExecDispatchDelete(int id)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"tid", id },
            };

            string sql = $@"
SELECT dispatch_delete(@tid)
";

            GetDBHelper().Execute(sql, paras);
            GetDBHelper().Commit();
        }

        /// <summary>
        /// 執行 dispatch_insert
        /// </summary>
        public void ExecDispatchInsert(string ivrCode, string vender, string ifWarrant, string cisId)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"tivr_code", ivrCode },
                {"tvender", vender },
                {"tifwarrant", ifWarrant },
                {"tcisid", cisId },
            };

            string sql = $@"
SELECT dispatch_insert(@tivr_code, @tvender, @tifwarrant, @tcisid)
";

            GetDBHelper().Execute(sql, paras);
            GetDBHelper().Commit();
        }

        /// <summary>
        /// 執行 dispatch_update
        /// </summary>
        public void ExecDispatchUpdate(int id, string ivrCode, string vender, string ifWarrant, string cisId)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"tivr_code", ivrCode },
                {"tvender", vender },
                {"tifwarrant", ifWarrant },
                {"tcisid", cisId },
                {"tid", id },
            };

            string sql = $@"
SELECT dispatch_update(@tivr_code, @tvender, @tifwarrant, @tcisid, @tid)
";

            GetDBHelper().Execute(sql, paras);
            GetDBHelper().Commit();
        }
    }
}
