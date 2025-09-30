using Const.DTO;
using Core.Utility.Helper.DB.Entity;
using Core.Utility.Utility;
using DocumentFormat.OpenXml.Office2010.Excel;
using FTT_API.Common.ConfigurationHelper;
using System.Text;

namespace FTT_API.Models.Handler
{
    /// <summary>
    /// 例外派工維護
    /// </summary>
    public class CIConfigHandler : BaseDBHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CIConfigHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }
        private readonly ConfigurationHelper _configHelper;

        /// <summary>
        /// 取得分頁資料
        /// </summary>
        public PageResult<CIExceptionConfigDTO> GetPageList(PageEntity pageEntity, CIExceptionConfigDTO searchVO)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];
            if (!string.IsNullOrWhiteSpace(searchVO.ShopNameLike))
            {
                condition.Append($"AND {nameof(CIExceptionConfigDTO.shop_name)} LIKE CONCAT('%', @{nameof(searchVO.ShopNameLike)}, '%') ");
                paras.Add(nameof(searchVO.ShopNameLike), searchVO.ShopNameLike);
            }

            string sql = $@"
SELECT CFG.cisid
       , CI.aciname
       , CFG.vendor_id
       , SV.merchant_name
       , CFG.ivrcode
       , SP.shop_name
       , TO_CHAR(CFG.approval_date, 'YYYY/MM/DD') AS approval_date_text
FROM   ci_exception_config CFG
       left join ci_data CI
              ON CI.cisid = CFG.cisid
       left join store_profile SP
              ON SP.ivr_code = CFG.ivrcode
       left join store_vender_profile SV
              ON SV.order_id = CFG.vendor_id
WHERE  CFG.enable = 'Y'
{condition}
ORDER  BY CFG.cisid
          , CFG.ivrcode
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

            return GetDBHelper().FindPageList<CIExceptionConfigDTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
        }

        /// <summary>
        /// [/Storemgt/CIConfig.aspx.cs]GetStoreData()
        /// </summary>
        /// <returns></returns>
        public List<StoreProfileDTO> GetListStoreProfile()
        {
            //StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
SELECT ivr_code
       , shop_name
       , store_type
       , channel
       , area
       , email
       , address
FROM   store_profile
ORDER  BY store_type
          , channel
          , area
          , ivr_code    
";

            return GetDBHelper().FindList<StoreProfileDTO>(sql, paras);
        }

        /// <summary>
        /// [/Storemgt/CIConfig.aspx.cs]GetStoreData()
        /// </summary>
        /// <returns></returns>
        public List<CIDataDTO> GetListCIData()
        {
            //StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
SELECT cisid
       , aciname
       , l1name
       , l2name
       , l3name
       , l4name
FROM   ci_data
ORDER  BY acisid 
";

            return GetDBHelper().FindList<CIDataDTO>(sql, paras);
        }

        /// <summary>
        /// 檢查報修品項是否存在
        /// </summary>
        /// <returns></returns>
        public bool CheckExistCisId(string cisId)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"CATEGORY_ID", cisId },
            };

            string sql = $@"
SELECT 1
WHERE EXISTS (SELECT 1
                FROM   ci_relations
                WHERE  cisid = @cisid
                        AND ( disable IS NULL
                                OR disable = '' )) 
";

            return GetDBHelper().FindScalar<int>(sql, paras) == 1;
        }

        /// <summary>
        /// 批次更新
        /// </summary>
        /// <param name="dtoList"></param>
        public void BatchUpdate(List<CIExceptionConfigDTO> dtoList)
        {
            DateTime now = DateTime.Now;
            string sqlInsert = @"
INSERT INTO ci_exception_config
            (cisid
             , vendor_id
             , ivrcode
             , approval_date
             , update_opid)
VALUES      (@cisid
             , @vendor_id
             , @ivrcode
             , @approval_date
             , @updateopid)
";
            string sqlDelete = @"
UPDATE ci_exception_config
SET    enable = 'N'
       , updatetime = @updatetime
       , update_opid = @updateopid
WHERE  cisid = @cisid
       AND ivrcode = @ivrcode
       AND enable = 'Y' 
";
            string sqlUpdate = @"
UPDATE ci_exception_config
SET    vendor_id = @vendor_id
       , approval_date = @approval_date
       , updatetime = @updatetime
       , update_opid = @updateopid
WHERE  cisid = @cisid
       AND ivrcode = @ivrcode
       AND enable = 'Y' 
";

            foreach(CIExceptionConfigDTO dto in dtoList)
            {
                if(dto.flag == "A")
                {
                    Dictionary<string, object> paras = new()
                    {
                        {"cisid", ConvertUtility.ConvertToInt32(dto.cisid ?? string.Empty,0) },
                        {"vendor_id", ConvertUtility.ConvertToInt32(dto.vendor_id ?? string.Empty,0) },
                        {"ivrcode", dto.ivrcode ?? string.Empty },
                        {"approval_date", dto.approval_date ?? now },
                        {"updateopid", SessionVO?.empno ?? string.Empty },
                    };
                    GetDBHelper().Execute(sqlInsert, paras);
                }
                else if(dto.flag == "D")
                {
                    Dictionary<string, object> paras = new()
                    {
                        {"cisid", ConvertUtility.ConvertToInt32(dto.cisid ?? string.Empty,0) },
                        {"updateopid", SessionVO?.empno ?? string.Empty },
                        {"updatetime", now },
                    };
                    GetDBHelper().Execute(sqlDelete, paras);
                }
                else if(dto.flag == "U")
                {
                    Dictionary<string, object> paras = new()
                    {
                        {"cisid", ConvertUtility.ConvertToInt32(dto.cisid ?? string.Empty,0) },
                        {"vendor_id", ConvertUtility.ConvertToInt32(dto.vendor_id ?? string.Empty,0) },
                        {"ivrcode", dto.ivrcode ?? string.Empty },
                        {"approval_date", dto.approval_date ?? now },
                        {"updateopid", SessionVO?.empno ?? string.Empty },
                        {"updatetime", now },
                    };
                    GetDBHelper().Execute(sqlUpdate, paras);
                }
            }

            GetDBHelper().Commit();
        }
    }
}
