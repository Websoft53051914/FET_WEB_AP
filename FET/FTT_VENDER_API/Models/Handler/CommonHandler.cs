using Const.DTO;
using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Common.ConfigurationHelper;
using System.Text;

namespace FTT_VENDER_API.Models.Handler
{
    /// <summary>
    /// 共用
    /// </summary>
    public partial class CommonHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="confighelper"></param>
        public CommonHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CommonHandler(ConfigurationHelper confighelper, IDBHelper dBHelper) : base(dBHelper)
        {
            _configHelper = confighelper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CIRelationsDTO> GetListCIRelations(int parentSid, string reqSrc, string acType = "")
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"parentsid", parentSid },
                {"reqsrc", reqSrc },
            };

            condition.Append(@"AND parentsid = @parentsid ");

            condition.Append(@"AND ( DECODE(disable, '', NULL, disable) IS NULL
OR disable = 'N' ) ");

            if (reqSrc == "ALL")
            {
                condition.Append(@"AND ( INSTR(',' || reqsrc || ',', @reqsrc) > 0
OR INSTR(reqsrc, 'ALL,') > 0 ) ");
            }
            else
            {
                condition.Append("AND ( INSTR(',' || reqsrc || ',', @reqsrc) > 0 ) ");
            }

            if (!string.IsNullOrWhiteSpace(acType))
            {
                condition.Append(@"AND ci.cisid IN (SELECT cisid
FROM   ci_relations_category
WHERE  INSTR(actype, @actype) > 0) ");
                paras.Add("@actype", acType);
            }

            string sql = $@"
SELECT ci.*
       , circ.notes
       , circ.descr
       , (SELECT ciname
          FROM   ci_relations ci3
          WHERE  ci3.cisid = ci.parentsid
                 AND rownum = 1) || '-' || ci.ciname AS fullname
       , EXISTS(SELECT 1
                FROM   ci_relations ci2
                WHERE  ci2.parentsid = ci.cisid
            ) AS HasChildren
FROM   ci_relations ci
       LEFT JOIN ci_relations_category circ
              ON circ.cisid = ci.cisid
WHERE  1 = 1
{condition}
ORDER  BY ciname 
";

            return GetDBHelper().FindList<CIRelationsDTO>(sql, paras);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CIRelationsDTO> GetListCIRelations(List<int> idList, string reqSrc, string acType = "")
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"IdIn", idList },
                {"reqsrc", reqSrc },
            };

            condition.Append(@"AND ci.cisid IN @IdIn ");
            condition.Append(@"AND ( DECODE(disable, '', NULL, disable) IS NULL
OR disable = 'N' ) ");

            if (reqSrc == "ALL")
            {
                condition.Append(@"AND ( INSTR(',' || reqsrc || ',', @reqsrc) > 0
OR INSTR(reqsrc, 'ALL,') > 0 ) ");
            }
            else
            {
                condition.Append("AND ( INSTR(',' || reqsrc || ',', @reqsrc) > 0 ");
            }

            if (!string.IsNullOrWhiteSpace(acType))
            {
                condition.Append(@"AND cisid IN (SELECT cisid
FROM   ci_relations_category
WHERE  INSTR(actype, @actype) > 0) ");
                paras.Add("@actype", acType);
            }

            string sql = $@"
WITH recursive path_cte AS
(
       SELECT cisid,
              parentsid,
              cisid                   AS leaf_id,
              ARRAY[cisid]::NUMERIC[] AS path_ids
       FROM   ci_relations
       WHERE  cisid IN @IdIn
       UNION ALL
       SELECT tn.cisid,
              tn.parentsid,
              cte.leaf_id,
              ARRAY[tn.cisid]::NUMERIC[]
                     || cte.path_ids
       FROM   ci_relations tn
       join   path_cte cte
       ON     tn.cisid = cte.parentsid )
SELECT ci.*
       , circ.notes
       , circ.descr
       , t_path.path_csv
       , (SELECT ciname
          FROM   ci_relations ci3
          WHERE  ci3.cisid = ci.parentsid
                 AND rownum = 1) || '-' || ci.ciname AS fullname
       , EXISTS(SELECT 1
                FROM   ci_relations ci2
                WHERE  ci2.parentsid = ci.cisid
            ) AS HasChildren
FROM   ci_relations ci
LEFT JOIN ci_relations_category circ
        ON circ.cisid = ci.cisid
LEFT JOIN (
SELECT leaf_id,
       array_to_string(path_ids, ',') AS path_csv
FROM   path_cte
WHERE  parentsid = 1006
) t_path 
        ON t_path.leaf_id = ci.cisid
WHERE  1 = 1
{condition}
ORDER  BY ciname 
";

            return GetDBHelper().FindList<CIRelationsDTO>(sql, paras);
        }

        /// <summary>
        /// [Not Commit]執行 set_status
        /// </summary>
        public void ExecSetStatus(string tFormType, int tFormNo, string ttNewStatus, string tEmpNo, string tType1 = "", string tType2 = "")
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"tformtype", tFormType },
                {"tformno", tFormNo },
                {"ttnewstatus", ttNewStatus },
                {"tempno", tEmpNo },
                {"ttype1", tType1 },
                {"ttype2", tType2 },
            };

            string sql = $@"
SELECT set_status(@tformtype, @tformno, @ttnewstatus, @tempno, @ttype1, @ttype2)
";

            GetDBHelper().Execute(sql, paras);
        }

        /// <summary>
        /// 取得門市分頁資料
        /// </summary>
        /// <param name="pageEntity"></param>
        /// <param name="ivrCode"></param>
        /// <returns></returns>
        public PageResult<StoreProfileDTO> GetPageListStore(PageEntity pageEntity, StoreProfileDTO searchVO)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            if (!string.IsNullOrWhiteSpace(searchVO.IvrCodeLike))
            {
                condition.Append($"AND {nameof(StoreProfileDTO.ivr_code)} ILIKE '%' || @{nameof(searchVO.IvrCodeLike)} || '%' ");
                paras.Add(nameof(searchVO.IvrCodeLike), searchVO.IvrCodeLike);
            }

            if (!string.IsNullOrWhiteSpace(searchVO.ShopNameLike))
            {
                condition.Append($"AND {nameof(StoreProfileDTO.shop_name)} ILIKE '%' || @{nameof(searchVO.ShopNameLike)} || '%' ");
                paras.Add(nameof(searchVO.ShopNameLike), searchVO.ShopNameLike);
            }

            if (!string.IsNullOrWhiteSpace(searchVO.CompanyLeavesLike))
            {
                condition.Append($"AND {nameof(StoreProfileDTO.company_leaves)} ILIKE '%' || @{nameof(searchVO.CompanyLeavesLike)} || '%' ");
                paras.Add(nameof(searchVO.CompanyLeavesLike), searchVO.CompanyLeavesLike);
            }

            if (!string.IsNullOrWhiteSpace(searchVO.ChannelLike))
            {
                condition.Append($"AND {nameof(StoreProfileDTO.channel)} ILIKE '%' || @{nameof(searchVO.ChannelLike)} || '%' ");
                paras.Add(nameof(searchVO.ChannelLike), searchVO.ChannelLike);
            }

            if (!string.IsNullOrWhiteSpace(searchVO.StoreTypeLike))
            {
                condition.Append($"AND {nameof(StoreProfileDTO.store_type)} ILIKE '%' || @{nameof(searchVO.StoreTypeLike)} || '%' ");
                paras.Add(nameof(searchVO.StoreTypeLike), searchVO.StoreTypeLike);
            }

            string sql = $@"
SELECT company_leaves
       , store_type
       , channel
       , area
       , shop_name
       , ivr_code
       , email
       , owner_cname
       , owner_ename
       , as_empno
       , as_cname
       , as_ename
       , owner_tel
       , urgent_tel
       , address
       , owner_cname
         || '('
         || owner_ename
         ||')' AS owner_name
       , as_cname
         || '('
         || as_ename
         ||')' AS as_name
FROM   store_profile
WHERE  1 = 1
AND ivr_code IS NOT NULL 
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

            return GetDBHelper().FindPageList<StoreProfileDTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
        }

        /// <summary>
        /// 取得 form_access_status 資料
        /// </summary>
        /// <returns></returns>
        public List<FormAccessStatusDTO> GetListFormAccessStatus()
        {
            //StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
SELECT status
       , status_name
FROM   form_access_status 
";

            return GetDBHelper().FindList<FormAccessStatusDTO>(sql, paras);
        }
    }
}
