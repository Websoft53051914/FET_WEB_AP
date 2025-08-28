using Core.Utility.Helper.DB.Entity;
using FTT_WEB.Common.ConfigurationHelper;
using FTT_WEB.Common.OriginClass.EntiityClass;
using FTT_WEB.Models.ViewModel;
using System.Text;

namespace FTT_WEB.Models.Handler
{
    /// <summary>
    /// 共用
    /// </summary>
    public partial class CommonHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        public CommonHandler(ConfigurationHelper confighelper)
        {
            _configHelper = confighelper;
        }

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

        public bool CheckExistIvrCode(string ivrCode)
        {
            StringBuilder condition = new();
            Dictionary<string, object> paras = new()
            {
                {"IVR_CODE", ivrCode }
            };

            return CheckDataExist("STORE_PROFILE", paras);
        }

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
    }
}
