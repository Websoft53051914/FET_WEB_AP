using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Text;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class StoreMgtHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public StoreMgtHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal string Create(Store_profileDTO vm, string empno)
        {

            var result = this.CheckDataExist("STORE_PROFILE", new Dictionary<string, object>() { { "ivr_code", vm.ivr_code } });
            if (result == true)
            {
                return "IVR Code重複輸入，請重新輸入！";
            }

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("company_leaves", vm.company_leaves);
            paras.Add("store_type", vm.store_type);
            paras.Add("channel", vm.channel);
            paras.Add("area", vm.area);
            paras.Add("shop_name", vm.shop_name);
            paras.Add("ivr_code", vm.ivr_code);
            paras.Add("email", vm.email);
            paras.Add("owner_empno", vm.owner_empno);
            paras.Add("owner_cname", vm.owner_cname);
            paras.Add("owner_ename", vm.owner_ename);
            paras.Add("as_empno", vm.as_empno);
            paras.Add("owner_tel", vm.owner_tel);
            paras.Add("urgent_tel", vm.urgent_tel);
            paras.Add("address", vm.address);
            paras.Add("decoration_condition", vm.decoration_condition);
            paras.Add("approval_date", vm.approval_date.Value.ToString("yyyy/MM/dd"));
            paras.Add("note", vm.note);
            paras.Add("note_owner", empno);
            //paras.Add("note_date", vm.note_date);
            paras.Add("business_hour_range1", vm.business_hour_range1);
            paras.Add("business_hour_range2", vm.business_hour_range2);
            paras.Add("business_hour_range3", vm.business_hour_range3);
            paras.Add("business_hour_range4", vm.business_hour_range4);

            string originSQL = $@"
                            insert into STORE_PROFILE (
                               COMPANY_LEAVES,              
                               STORE_TYPE,                  
                               CHANNEL,                     
                               AREA,                        
                               SHOP_NAME,                   
                               IVR_CODE,                    
                               EMAIL,                       
                               OWNER_EMPNO,                 
                               OWNER_CNAME,                 
                               OWNER_ENAME,                 
                               AS_EMPNO,                    
                               OWNER_TEL,                   
                               URGENT_TEL,                  
                               ADDRESS,                     
                               DECORATION_CONDITION,        
                               APPROVAL_DATE,               
                               NOTE,                        
                               NOTE_OWNER,                  
                               NOTE_DATE,                   
                               BUSINESS_HOUR_RANGE1, 
                               BUSINESS_HOUR_RANGE2, 
                               BUSINESS_HOUR_RANGE3, 
                               BUSINESS_HOUR_RANGE4  
                             ) values (                    
                             @company_leaves,
                             @STORE_TYPE,
                             @CHANNEL,
                             @AREA,
                             @SHOP_NAME,
                             @IVR_CODE,
                             @EMAIL,
                             @OWNER_EMPNO,
                             @OWNER_CNAME,
                             @OWNER_ENAME,
                             @AS_EMPNO,
                             @OWNER_TEL,
                             @URGENT_TEL,
                             @ADDRESS,
                             @DECORATION_CONDITION,
                             to_date(@APPROVAL_DATE,'yyyy/mm/dd'),
                             @NOTE,
                             @NOTE_OWNER,
                             sysdate,
                             @BUSINESS_HOUR_RANGE1,
                             @BUSINESS_HOUR_RANGE2,
                             @BUSINESS_HOUR_RANGE3,
                             @BUSINESS_HOUR_RANGE4
                             )";

            dbHelper.Execute(originSQL, paras);
            dbHelper.Commit();

            return "";
        }

        internal void Delete(string ivr_code)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("ivr_code", ivr_code);

            string originSQL = @"
delete from STORE_PROFILE where ivr_code = @ivr_code
";

            dbHelper.Execute(originSQL, paras);
            dbHelper.Commit();
        }

        internal string? Edit(Store_profileDTO vm, string empno)
        {

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("ivr_code", vm.ivr_code);

            paras.Add("company_leaves", vm.company_leaves);
            paras.Add("store_type", vm.store_type);
            paras.Add("channel", vm.channel);
            paras.Add("area", vm.area);
            paras.Add("shop_name", vm.shop_name);
            paras.Add("email", vm.email);
            paras.Add("owner_empno", vm.owner_empno);
            paras.Add("owner_cname", vm.owner_cname);
            paras.Add("owner_ename", vm.owner_ename);
            paras.Add("as_empno", vm.as_empno);
            paras.Add("owner_tel", vm.owner_tel);
            paras.Add("urgent_tel", vm.urgent_tel);
            paras.Add("address", vm.address);
            paras.Add("decoration_condition", vm.decoration_condition);
            paras.Add("approval_date", vm.approval_date.Value.ToString("yyyy/MM/dd"));
            paras.Add("note", vm.note);
            paras.Add("note_owner", empno);
            //paras.Add("note_date", vm.note_date);
            paras.Add("business_hour_range1", vm.business_hour_range1);
            paras.Add("business_hour_range2", vm.business_hour_range2);
            paras.Add("business_hour_range3", vm.business_hour_range3);
            paras.Add("business_hour_range4", vm.business_hour_range4);

            string originSQL = $@"
                            update STORE_PROFILE  set
 COMPANY_LEAVES= @company_leaves,
 STORE_TYPE= @STORE_TYPE,
 CHANNEL = @CHANNEL,
 AREA= @AREA,
 SHOP_NAME = @SHOP_NAME,
 EMAIL = @EMAIL,
 OWNER_EMPNO = @OWNER_EMPNO,
 OWNER_CNAME = @OWNER_CNAME,
 OWNER_ENAME = @OWNER_ENAME,
 AS_EMPNO= @AS_EMPNO,
 OWNER_TEL = @OWNER_TEL,
 URGENT_TEL= @URGENT_TEL,
 ADDRESS = @ADDRESS,
 DECORATION_CONDITION= @DECORATION_CONDITION,
 APPROVAL_DATE = to_date(@APPROVAL_DATE,'yyyy/mm/dd'),
 NOTE= @NOTE,
 NOTE_OWNER= @NOTE_OWNER,
 NOTE_DATE = sysdate,
 BUSINESS_HOUR_RANGE1= @BUSINESS_HOUR_RANGE1,
 BUSINESS_HOUR_RANGE2= @BUSINESS_HOUR_RANGE2,
 BUSINESS_HOUR_RANGE3= @BUSINESS_HOUR_RANGE3,
 BUSINESS_HOUR_RANGE4= @BUSINESS_HOUR_RANGE4

where ivr_code=@ivr_code


                             ";

            dbHelper.Execute(originSQL, paras);
            dbHelper.Commit();

            return "";
        }

        internal PageResult<Store_profileDTO> FindPageList(PageEntity pageEntity, Store_profileDTO dto)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("USERROLE", dto.USERROLE);
            paras.Add("EMPNO", dto.EMPNO);
            paras.Add("IVRCODE", dto.IVRCODE);

            paras.Add("shop_name", "%" + dto.shop_name + "%");
            paras.Add("company_leaves", dto.company_leaves);
            paras.Add("store_type", dto.store_type);
            paras.Add("channel", dto.channel);
            paras.Add("area", dto.area);
            paras.Add("as_cname", "%" + dto.as_cname + "%");

            string strWhere = "";

            if (!string.IsNullOrEmpty(dto.shop_name))
            {
                strWhere += " and shop_name like @shop_name ";
            }

            if (!string.IsNullOrEmpty(dto.company_leaves))
            {
                strWhere += " and company_leaves = @company_leaves ";
            }
            if (!string.IsNullOrEmpty(dto.store_type))
            {

                strWhere += " and store_type = @store_type ";
            }
            if (!string.IsNullOrEmpty(dto.channel))
            {
                strWhere += " and channel = @channel ";
            }
            if (!string.IsNullOrEmpty(dto.area))
            {
                strWhere += " and area = @area ";
            }
            if (!string.IsNullOrEmpty(dto.as_cname))
            {
                strWhere += " and as_cname like @as_cname ";
            }

            string originSQL = $@"
select * from STORE_PROFILE
where 1=1
{strWhere}
";

            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = dbHelper.FindPageList<Store_profileDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal Store_profileDTO GetDetail(string ivrcode)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("IVR_CODE", ivrcode);

            string strWhere = "";

            string originSQL = $@"
select * from
STORE_PROFILE
where IVR_CODE=@IVR_CODE
";

            var result = dbHelper.Find<Store_profileDTO>(originSQL, paras);
            return result;
        }

        internal PageResult<fet_user_profileDTO> GetEmpPageList(PageEntity pageEntity, fet_user_profileDTO vm)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();

            if (vm.engname == null) vm.engname = "";
            if (vm.empname == null) vm.empname = "";
            if (vm.empno == null) vm.empno = "";
            if (vm.deptname == null) vm.deptname = "";

            paras.Add("engname", "%" + vm.engname.ToUpper() + "%");
            paras.Add("empname", "%" + vm.empname.ToUpper() + "%");
            paras.Add("empno", "%" + vm.empno.ToUpper() + "%");
            paras.Add("deptname", "%" + vm.deptname.ToUpper() + "%");

            string strWhere = "";

            string originSQL = $@"
SELECT *
FROM   (SELECT deptcode,
               Get_dept_desc(deptcode) AS deptname,
               Get_dept_desc(deptcode) AS deptengname,
               empno,
               empname,
               engname,
               ext,
               aliasname,
               costcenter,
               email,
               mobile
        FROM   fet_user_profile
        WHERE  
            Upper(engname) LIKE @engname
        and Upper(empname) LIKE @empname
        and Upper(empno) LIKE @empno
        and ( ( emptype NOT IN ( 'T', 'Y', 'U' ) )
       OR ( emptype IN ( 'T', 'Y', 'U' )
           AND offdate IS NULL ) ))
WHERE  Upper(deptname) LIKE @deptname
ORDER  BY deptcode,
          engname 
";

            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = dbHelper.FindPageList<fet_user_profileDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal DataTable GetRetailData()
        {
            string strSql = "SELECT DISTINCT IVR_CODE as \"IVRCODE\", SHOP_NAME as 店名, AREA as 區域, OWNER_EMPNO as 店長員編, OWNER_CNAME as 店長名字, AS_EMPNO as 區主管員編, AS_CNAME as 區主管名字, BUSINESS_HOUR_RANGE1 as \"週一~五\", BUSINESS_HOUR_RANGE2 as \"星期六\", BUSINESS_HOUR_RANGE3 as \"星期日\", BUSINESS_HOUR_RANGE4 as \"國定假日\" FROM STORE_PROFILE WHERE CHANNEL='RETAIL' ORDER BY IVR_CODE";
            DataTable dtTable = dbHelper.FindDataTable(strSql, null);
            return dtTable;
        }

        internal List<Store_profileDTO> GetSTORE_PROFILE_AREA()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();

            string strWhere = "";

            string originSQL = $@"
select distinct AREA from STORE_PROFILE A,FET_USER_PROFILE B where A.AS_EMPNO=B.EMPNO order by area
";

            var result = dbHelper.FindList<Store_profileDTO>(originSQL, paras);
            return result;
        }

        internal List<store_typeDTO> GetSTORE_TYPE_TYPE_VALUE(string type_name)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("type_name", type_name);

            string strWhere = "";

            string originSQL = $@"
SELECT TYPE_VALUE FROM STORE_TYPE WHERE TYPE_NAME=@type_name ORDER BY ORDER_ID
{strWhere}
";

            var result = dbHelper.FindList<store_typeDTO>(originSQL, paras);
            return result;
        }

        internal DataTable GetVassData()
        {
            string strSql = "SELECT DISTINCT IVR_CODE as \"IVRCODE\", SHOP_NAME as 店名, AREA as 區域, AS_EMPNO as 業務員編, AS_CNAME as 業務名字, BUSINESS_HOUR_RANGE1 as \"週一~五\", BUSINESS_HOUR_RANGE2 as \"星期六\", BUSINESS_HOUR_RANGE3 as \"星期日\", BUSINESS_HOUR_RANGE4 as \"國定假日\" FROM STORE_PROFILE WHERE CHANNEL='FRANCHISE' ORDER BY IVR_CODE";
            DataTable dtTable = dbHelper.FindDataTable(strSql, null);
            return dtTable;
        }

        internal string? ImportRetail(string destFilePath)
        {

            IWorkbook wk;

            string ext = Path.GetExtension(destFilePath).ToLower();

            // 判斷副檔名
            using (FileStream fs = new FileStream(destFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                if (ext == ".xlsx")
                {
                    wk = new XSSFWorkbook(fs); // XSSF 讀取 .xlsx
                }
                else if (ext == ".xls")
                {
                    wk = new HSSFWorkbook(fs); // HSSF 讀取 .xls
                }
                else
                {
                    return "檔案格式錯誤，只能上傳 Excel (.xls / .xlsx)";
                }
            }

            for (int k = 0; k < wk.NumberOfSheets; k++)
            {
                ISheet hst = wk.GetSheetAt(k);

                if (hst.SheetName == "直營門市")
                {
                    IRow hr = hst.GetRow(0);
                    int dLastNum = hr.LastCellNum;

                    Dictionary<string, object> paras = new Dictionary<string, object>();

                    for (int j = 1; j <= hst.LastRowNum; j++)
                    {
                        int line = j + 1;

                        hr = hst.GetRow(j);

                        string strIVRCODE = hr.GetCell(0) == null ? "" : hr.GetCell(0).ToString().Trim();
                        string strAREA = hr.GetCell(2) == null ? "" : hr.GetCell(2).ToString().Trim();
                        string strOWNER = hr.GetCell(3) == null ? "" : hr.GetCell(3).ToString().Trim();
                        string strMANAGER = hr.GetCell(5) == null ? "" : hr.GetCell(5).ToString().Trim();
                        string strRange1 = hr.GetCell(7) == null ? "" : hr.GetCell(7).ToString().Trim();
                        string strRange2 = hr.GetCell(8) == null ? "" : hr.GetCell(8).ToString().Trim();
                        string strRange3 = hr.GetCell(9) == null ? "" : hr.GetCell(9).ToString().Trim();
                        string strRange4 = hr.GetCell(10) == null ? "" : hr.GetCell(10).ToString().Trim();

                        if (strIVRCODE != "")
                        {
                            if (this.CheckDataExist("STORE_PROFILE", new Dictionary<string, object>() { { "IVR_CODE", strIVRCODE } }) == false)
                            {
                                return $"第 {line.ToString()} 列 無此({strIVRCODE}) IVRCODE!";
                            }
                            else
                            {
                                if (strAREA == "")
                                {
                                    return $"第 {line.ToString()} 列 區域不能是空白!";
                                }
                                else
                                {
                                    if (strOWNER == "")
                                    {
                                        return $"第 {line.ToString()} 列 店長員編不能是空白!";
                                    }
                                    else
                                    {
                                        if (strMANAGER == "")
                                        {
                                            return $"第 {line.ToString()} 列 區主管員編不能是空白!";
                                        }
                                        else
                                        {
                                            paras = new Dictionary<string, object>();
                                            paras.Add("AREA", strAREA);
                                            paras.Add("OWNER_EMPNO", strOWNER);
                                            paras.Add("AS_EMPNO", strMANAGER);
                                            paras.Add("BUSINESS_HOUR_RANGE1", strRange1);
                                            paras.Add("BUSINESS_HOUR_RANGE2", strRange2);
                                            paras.Add("BUSINESS_HOUR_RANGE3", strRange3);
                                            paras.Add("BUSINESS_HOUR_RANGE4", strRange4);
                                            paras.Add("IVR_CODE", strIVRCODE);

                                            string updateSql = @"
UPDATE STORE_PROFILE 
SET 

AREA=@AREA,
OWNER_EMPNO=@OWNER_EMPNO, 
AS_EMPNO=@AS_EMPNO, 
BUSINESS_HOUR_RANGE1=@BUSINESS_HOUR_RANGE1, 
BUSINESS_HOUR_RANGE2=@BUSINESS_HOUR_RANGE2, 
BUSINESS_HOUR_RANGE3=@BUSINESS_HOUR_RANGE3, 
BUSINESS_HOUR_RANGE4=@BUSINESS_HOUR_RANGE4 

WHERE IVR_CODE=@IVR_CODE ";

                                            this.dbHelper.Execute(updateSql, paras);
                                            this.dbHelper.Commit();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return "";
        }

        internal string? ImportVass(string destFilePath)
        {

            IWorkbook wk;

            string ext = Path.GetExtension(destFilePath).ToLower();

            // 判斷副檔名
            using (FileStream fs = new FileStream(destFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                if (ext == ".xlsx")
                {
                    wk = new XSSFWorkbook(fs); // XSSF 讀取 .xlsx
                }
                else if (ext == ".xls")
                {
                    wk = new HSSFWorkbook(fs); // HSSF 讀取 .xls
                }
                else
                {
                    return "檔案格式錯誤，只能上傳 Excel (.xls / .xlsx)";
                }
            }

            for (int k = 0; k < wk.NumberOfSheets; k++)
            {
                ISheet hst = wk.GetSheetAt(k);

                if (hst.SheetName == "加盟門市")
                {
                    IRow hr = hst.GetRow(0);
                    int dLastNum = hr.LastCellNum;

                    Dictionary<string, object> paras = new Dictionary<string, object>();

                    for (int j = 1; j <= hst.LastRowNum; j++)
                    {
                        int line = j + 1;

                        hr = hst.GetRow(j);

                        string strIVRCODE = hr.GetCell(0) == null ? "" : hr.GetCell(0).ToString().Trim();
                        string strAREA = hr.GetCell(2) == null ? "" : hr.GetCell(2).ToString().Trim();
                        string strMANAGER = hr.GetCell(3) == null ? "" : hr.GetCell(3).ToString().Trim();
                        string strRange1 = hr.GetCell(5) == null ? "" : hr.GetCell(5).ToString().Trim();
                        string strRange2 = hr.GetCell(6) == null ? "" : hr.GetCell(6).ToString().Trim();
                        string strRange3 = hr.GetCell(7) == null ? "" : hr.GetCell(7).ToString().Trim();
                        string strRange4 = hr.GetCell(8) == null ? "" : hr.GetCell(8).ToString().Trim();

                        if (strIVRCODE != "")
                        {
                            if (this.CheckDataExist("STORE_PROFILE", new Dictionary<string, object>() { { "IVR_CODE", strIVRCODE } }) == false)
                            {
                                return $"第 {line.ToString()} 列 無此({strIVRCODE}) IVRCODE!";
                            }
                            else
                            {
                                if (strAREA == "")
                                {
                                    return $"第 {line.ToString()} 列 區域不能是空白!";
                                }
                                else
                                {
                                    if (strMANAGER == "")
                                    {
                                        return $"第 {line.ToString()} 列 業務員編不能是空白!";
                                    }
                                    else
                                    {

                                        paras = new Dictionary<string, object>();
                                        paras.Add("AREA", strAREA);
                                        paras.Add("AS_EMPNO", strMANAGER);
                                        paras.Add("BUSINESS_HOUR_RANGE1", strRange1);
                                        paras.Add("BUSINESS_HOUR_RANGE2", strRange2);
                                        paras.Add("BUSINESS_HOUR_RANGE3", strRange3);
                                        paras.Add("BUSINESS_HOUR_RANGE4", strRange4);
                                        paras.Add("IVR_CODE", strIVRCODE);

                                        string updateSql = @"
UPDATE STORE_PROFILE 
SET 

AREA=@AREA,
AS_EMPNO=@AS_EMPNO, 
BUSINESS_HOUR_RANGE1=@BUSINESS_HOUR_RANGE1, 
BUSINESS_HOUR_RANGE2=@BUSINESS_HOUR_RANGE2, 
BUSINESS_HOUR_RANGE3=@BUSINESS_HOUR_RANGE3, 
BUSINESS_HOUR_RANGE4=@BUSINESS_HOUR_RANGE4 
WHERE IVR_CODE=@IVR_CODE ";

                                        this.dbHelper.Execute(updateSql, paras);
                                        this.dbHelper.Commit();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return "";
        }
    }
}
