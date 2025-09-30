using Core.Utility.Extensions;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.InkML;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Text;
using static Const.Enums;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FTT_API.Models.Handler
{
    public class SecurityMgtHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public SecurityMgtHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal void Create(store_sec_vendor_listDTO vm)
        {

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("merchant_name", vm.merchant_name);
            paras.Add("cp_name", vm.cp_name);
            paras.Add("cp_tel", vm.cp_tel);
            //paras.Add("email", vm.email);
            //paras.Add("merchant_login", vm.merchant_login);
            //paras.Add("merchant_password", vm.merchant_password);

            string sql = @"
INSERT INTO store_sec_vendor_list
(
    merchant_name,
    cp_name,
    cp_tel,
    email,
    merchant_login,
    merchant_password
)
VALUES
(
    @merchant_name,
    @cp_name,
    @cp_tel,
    @email,
    @merchant_login,
    @merchant_password
)";

            dbHelper.Execute(sql, paras);
            dbHelper.Commit();
        }

        internal void Delete(string ivrcode, string empno)
        {

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("ivrcode", ivrcode);
            paras.Add("UPDATE_OPID", empno);

            string originSQL = @"
Update STORE_SEC_VENDOR_LIST Set ENABLE='N', UPDATE_OPID=@UPDATE_OPID, UPDATE_TIME=SYSDATE Where ivrcode=@ivrcode And ENABLE='Y'
";

            dbHelper.Execute(originSQL, paras);
            dbHelper.Commit();
        }

        internal string? Edit(store_sec_vendor_listDTO vm)
        {


            Dictionary<string, object> paras = new Dictionary<string, object>();
            //paras.Add("order_id", vm.order_id);

            paras.Add("merchant_name", vm.merchant_name);
            paras.Add("cp_name", vm.cp_name);
            paras.Add("cp_tel", vm.cp_tel);
            //paras.Add("email", vm.email);
            //paras.Add("merchant_login", vm.merchant_login);
            //paras.Add("merchant_password", vm.merchant_password);

            string sql = @"
UPDATE store_sec_vendor_list
SET
    merchant_name     = @merchant_name,
    cp_name           = @cp_name,
    cp_tel            = @cp_tel,
    email             = @email,
    merchant_login    = @merchant_login,
    merchant_password = @merchant_password
WHERE order_id = @order_id";

            dbHelper.Execute(sql, paras);
            dbHelper.Commit();

            return "";
        }

        internal PageResult<store_sec_vendor_listDTO> FindPageList(PageEntity pageEntity, store_sec_vendor_listDTO dto)
        {

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("shop_name", "%" + dto.shop_name + "%");

            string strWhere = "";

            if (!string.IsNullOrEmpty(dto.shop_name))
            {
                strWhere += " and p.shop_name like @shop_name ";
            }


            string originSQL = $@"
select 
s.ivrcode, 
p.shop_name, 
s.vendor_id, 
v.merchant_name, 
v.cp_name, 
v.cp_tel 
from store_sec_vendor_list s left 
join store_profile p on p.ivr_code=s.ivrcode 
left join store_vender_profile v on v.order_id=s.vendor_id 

where s.enable='Y' 
{strWhere}

order by s.ivrcode
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

            var result = dbHelper.FindPageList<store_sec_vendor_listDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal store_sec_vendor_listDTO GetDetail(string order_id)
        {

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("order_id", order_id);

            string strWhere = "";

            string originSQL = $@"
select * from
store_sec_vendor_list
where order_id=@order_id
";

            var result = dbHelper.Find<store_sec_vendor_listDTO>(originSQL, paras);
            return result;
        }

        internal DataTable GetQueryData()
        {
            string strSql = "SELECT S.IVRCODE AS \"IVRCODE\", P.SHOP_NAME AS 門市名稱, S.VENDOR_ID AS 廠商代碼, V.MERCHANT_NAME AS 廠商名稱, V.CP_NAME AS 聯絡人, V.CP_TEL AS 聯絡人電話, '' AS 刪除註記 FROM STORE_SEC_VENDOR_LIST S LEFT JOIN STORE_PROFILE P ON P.IVR_CODE=S.IVRCODE LEFT JOIN STORE_VENDER_PROFILE V ON V.ORDER_ID=S.VENDOR_ID WHERE S.ENABLE='Y' ORDER BY S.IVRCODE";
            DataTable dtTable = this.dbHelper.FindDataTable(strSql, null);
            return dtTable;
        }

        internal DataTable GetStoreData()
        {
            string strSql = "SELECT IVR_CODE AS \"IVRCODE\", SHOP_NAME AS 店名, STORE_TYPE AS 店格, CHANNEL AS 通路, AREA AS 區域, EMAIL AS \"EMAIL ADDRESS\", ADDRESS AS 地址 FROM STORE_PROFILE ORDER BY STORE_TYPE, CHANNEL, AREA, IVR_CODE";
            DataTable dtTable = this.dbHelper.FindDataTable(strSql, null);
            return dtTable;
        }

        internal DataTable GetVendorData()
        {
            string strSql = " SELECT ORDER_ID AS 廠商代碼, MERCHANT_NAME AS 廠商名稱, CP_NAME AS 聯絡人, CP_TEL AS 聯絡人電話, EMAIL AS 聯絡人EMAIL  FROM STORE_VENDER_PROFILE      ORDER BY ORDER_ID ";
            DataTable dtTable = this.dbHelper.FindDataTable(strSql, null);
            return dtTable;
        }

        internal string? Import(string destFilePath, string empno)
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

                if (hst.SheetName == "Sheet1")
                {
                    IRow hr = hst.GetRow(0);
                    int dLastNum = hr.LastCellNum;

                    Dictionary<string, object> paras = new Dictionary<string, object>();

                    for (int j = 1; j <= hst.LastRowNum; j++)
                    {
                        int line = j + 1;

                        hr = hst.GetRow(j);

                        string strIvrCode = hr.GetCell(0) == null ? "0" : hr.GetCell(0).ToString().Trim();
                        string strVendorID = hr.GetCell(2) == null ? "0" : hr.GetCell(2).ToString().Trim();
                        string strDelFlg = hr.GetCell(6) == null ? "" : hr.GetCell(6).ToString().Trim();

                        if (strDelFlg == "D")
                        {
                            paras = new Dictionary<string, object>();
                            paras.Add("UPDATE_OPID", empno);
                            paras.Add("IVRCODE", strIvrCode);
                            paras.Add("VENDOR_ID", int.Parse(strVendorID));

                            this.dbHelper.Execute("UPDATE STORE_SEC_VENDOR_LIST SET ENABLE='N', UPDATE_TIME=SYSDATE, UPDATE_OPID=@UPDATE_OPID WHERE ENABLE='Y' AND IVRCODE=@IVRCODE AND VENDOR_ID=@VENDOR_ID", paras);
                            this.dbHelper.Commit();
                        }
                        else
                        {
                            if (strIvrCode != "0")
                            {

                                if (this.CheckDataExist("STORE_SEC_VENDOR_LIST", new Dictionary<string, object>() { { "IVRCODE", strIvrCode }, { "ENABLE", "Y" } }) == true)
                                {
                                    if (this.CheckDataExist("STORE_VENDER_PROFILE", new Dictionary<string, object>() { { "ORDER_ID", strVendorID } }) == false || strVendorID == "0")
                                    {
                                        return $"第 {line.ToString()} 列 無此({strVendorID})廠商代碼!";
                                    }
                                    else
                                    {
                                        paras = new Dictionary<string, object>();
                                        paras.Add("UPDATE_OPID", empno);
                                        paras.Add("IVRCODE", strIvrCode);
                                        paras.Add("VENDOR_ID", int.Parse(strVendorID));
                                        this.dbHelper.Execute("UPDATE STORE_SEC_VENDOR_LIST SET VENDOR_ID=@VENDOR_ID, UPDATE_TIME=SYSDATE, UPDATE_OPID=@UPDATE_OPID WHERE ENABLE='Y' AND IVRCODE=@IVRCODE ", paras);
                                        this.dbHelper.Commit();
                                    }
                                }
                                else
                                {
                                    if (this.CheckDataExist("STORE_PROFILE", new Dictionary<string, object>() { { "IVR_CODE", strIvrCode } }) == false)
                                    {
                                        return $"第 {line.ToString()} 列 無此({strIvrCode})門市!";
                                    }
                                    else
                                    {
                                        if (this.CheckDataExist("STORE_VENDER_PROFILE", new Dictionary<string, object>() { { "ORDER_ID", strVendorID } }) == false || strVendorID == "0")
                                        {
                                            return $"第 {line.ToString()} 列 無此({strVendorID})廠商代碼!";
                                        }
                                        else
                                        {
                                            paras = new Dictionary<string, object>();
                                            paras.Add("UPDATE_OPID", empno);
                                            paras.Add("IVRCODE", strIvrCode);
                                            paras.Add("VENDOR_ID", int.Parse(strVendorID));
                                            this.dbHelper.Execute("INSERT INTO STORE_SEC_VENDOR_LIST (IVRCODE,VENDOR_ID,CREATE_OPID) VALUES (@IVRCODE,@VENDOR_ID,@UPDATE_OPID)", paras);
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
    }
}
