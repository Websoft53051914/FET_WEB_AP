using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using System.Text;

namespace FTT_API.Models.Handler
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
        /// 
        /// </summary>
        /// <param name="ivrCode"></param>
        /// <returns></returns>
        public List<StoreDTO> GetListStoreVM(string ivrCode)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"IVR_CODE", ivrCode }
            };

            string sql = $@"
SELECT *
FROM STORE_PROFILE 
WHERE IVR_CODE = @IVR_CODE
";

            return GetDBHelper().FindList<StoreDTO>(sql, paras);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ivrCode"></param>
        /// <returns></returns>
        public StoreVM? GetStoreData(string ivrCode)
        {
            StoreVM? result = null;
            if (string.IsNullOrEmpty(ivrCode))
            {
                return result;
            }

            List<StoreDTO> dataList = GetListStoreVM(ivrCode);
            if (dataList.Count == 0)
            {
                return result;
            }
            else if (dataList.Count > 1)
            {
                GetMessage().SetAlert($"以 IVR Code [\" {ivrCode} \"] 搜尋出來門市資料太多，請檢視資料是否正確！");
                return result;
            }

            StoreDTO dto = dataList.First();
            result = new StoreVM(dto);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentSid"></param>
        /// <param name="reqSrc"></param>
        /// <returns></returns>
        public List<CIRelationsDTO> GetListCIRelations(int parentSid, string reqSrc)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"PARENTSID", parentSid },
                {"REQSRC", reqSrc },
            };

            string sql = $@"
SELECT ci.*
, circ.NOTES
, circ.DESCR
, (SELECT CINAME FROM CI_RELATIONS ci3 WHERE ci3.CISID = ci.PARENTSID AND ROWNUM=1)||'-'||ci.CINAME AS FULLNAME
, EXISTS(
    SELECT 1
    FROM CI_RELATIONS ci2
    WHERE ci2.PARENTSID = ci.CISID AND (DECODE(ci2.DISABLE,'',NULL,ci2.DISABLE) IS NULL OR ci2.DISABLE='N') AND (INSTR(','||ci2.REQSRC||',',@REQSRC) > 0 OR INSTR(ci2.REQSRC,'ALL,') > 0) 
) AS HasChildren
FROM CI_RELATIONS ci
LEFT JOIN CI_RELATIONS_CATEGORY circ ON circ.CISID  = ci.CISID
WHERE PARENTSID=@PARENTSID AND (DECODE(DISABLE,'',NULL,DISABLE) IS NULL OR DISABLE='N') AND (INSTR(','||REQSRC||',',@REQSRC) > 0 OR INSTR(REQSRC,'ALL,') > 0) 
ORDER BY CINAME
";

            return GetDBHelper().FindList<CIRelationsDTO>(sql, paras);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageEntity"></param>
        /// <returns></returns>
        public PageResult<CIRelationsDTO> GetPageListCiDataSelfVendor(PageEntity pageEntity)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = [];

            string sql = $@"
select a.cisid, a.ciname, b.aciname, b.l1name, b.l2name, b.l3name, b.l4name
, circ.NOTES
, circ.DESCR
FROM ci_relations a
INNER JOIN ci_data b ON b.cisid=a.cisid
LEFT JOIN CI_RELATIONS_CATEGORY circ ON circ.CISID  = a.CISID
where a.cicategory=1006 and (decode(a.disable,'',null,a.disable) is null or a.disable='N') and a.cisid in (select circ2.cisid from ci_relations_category circ2 where instr(circ2.actype,'RETAIL')>0 and circ2.selfconfig='Y')
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

            return GetDBHelper().FindPageList<CIRelationsDTO>(sql, sqlCount, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
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
    }
}
