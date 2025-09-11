using Const.DTO;
using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.ViewModel;
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
        /// 取得管理員名稱
        /// </summary>
        /// <returns></returns>
        public List<string> GetListAdminEngName()
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
SELECT SUBSTR(ENGNAME, INSTR(ENGNAME,' ')+1, LENGTH(ENGNAME)-1) || ', ' || SUBSTR(ENGNAME,1,INSTR(ENGNAME,' ')-1) || ' ' || EMPNAME || ' (' || EXT || ')' 
FROM FET_USER_PROFILE 
WHERE EMPNO IN(SELECT UNNEST(STRING_TO_ARRAY((SELECT CONFIG_VALUE FROM MAINTAIN_CONFIG WHERE CONFIG_NAME='ADMIN'),',')))
";

            return GetDBHelper().FindList<string>(sql, paras);
        }

        /// <summary>
        /// 檢查 ivr_code 是否存在
        /// </summary>
        /// <param name="ivrCode"></param>
        /// <returns></returns>
        public bool CheckExistIvrCode(string ivrCode)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"IVR_CODE", ivrCode }
            };

            return CheckDataExist("STORE_PROFILE", paras);
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
        /// 取得門市分頁資料
        /// </summary>
        /// <param name="pageEntity"></param>
        /// <param name="ivrCode"></param>
        /// <returns></returns>
        public PageResult<StoreVenderProfileDTO> GetPageListVender(PageEntity pageEntity, StoreVenderProfileDTO searchVO)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            if (!string.IsNullOrWhiteSpace(searchVO.MerchantNameLike))
            {
                condition.Append($"AND {nameof(StoreVenderProfileDTO.merchant_name)} ILIKE '%' || @{nameof(searchVO.MerchantNameLike)} || '%' ");
                paras.Add(nameof(searchVO.MerchantNameLike), searchVO.MerchantNameLike);
            }

            if (!string.IsNullOrWhiteSpace(searchVO.CpNameLike))
            {
                condition.Append($"AND {nameof(StoreVenderProfileDTO.cp_name)} ILIKE '%' || @{nameof(searchVO.CpNameLike)} || '%' ");
                paras.Add(nameof(searchVO.CpNameLike), searchVO.CpNameLike);
            }

            string sql = $@"
SELECT *
FROM   store_vender_profile
WHERE  1 = 1
AND order_id IS NOT NULL
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

            return GetDBHelper().FindPageList<StoreVenderProfileDTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
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

        /// <summary>
        /// 取得 store_type 資料
        /// </summary>
        /// <returns></returns>
        public List<StoreTypeDTO> GetListStoreType(StoreTypeDTO searchVO)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            if (!string.IsNullOrWhiteSpace(searchVO.TypeNameEq))
            {
                condition.Append($"AND {nameof(StoreTypeDTO.type_name)} = @{nameof(searchVO.TypeNameEq)} ");
                paras.Add(nameof(searchVO.TypeNameEq), searchVO.TypeNameEq);
            }

            string sql = $@"
SELECT type_value
FROM   store_type
WHERE  1 = 1
{condition}
ORDER  BY order_id  
";

            return GetDBHelper().FindList<StoreTypeDTO>(sql, paras);
        }

        /// <summary>
        /// 取得區域資料
        /// </summary>
        /// <returns></returns>
        public List<string> GetListArea()
        {
            //StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
SELECT DISTINCT area
FROM   store_profile
WHERE  decode(area, '', NULL,
                    area) IS NOT NULL
ORDER  BY area   
";

            return GetDBHelper().FindList<string>(sql, paras);
        }

        /// <summary>
        /// 取得區經理/業務資料
        /// </summary>
        /// <returns></returns>
        public List<StoreProfileDTO> GetListAsEmp()
        {
            //StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
SELECT DISTINCT as_empno
                , as_cname
FROM   store_profile    
";

            return GetDBHelper().FindList<StoreProfileDTO>(sql, paras);
        }
    }
}
