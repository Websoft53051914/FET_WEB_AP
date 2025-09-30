using Core.Utility.Extensions;
using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;
using static Const.Enums;

namespace FTT_API.Models.Handler
{
    public class CIMgtHandler : BaseDBHandler
    {
        private readonly ConfigurationHelper _configHelper;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        public CIMgtHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            _configHelper = confighelper;
            _httpContext = httpContext;
        }

        internal void Create(ci_relations_categoryDTO vm)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            //paras.Add("merchant_name", vm.merchant_name);
            //paras.Add("cp_name", vm.cp_name);
            //paras.Add("cp_tel", vm.cp_tel);
            //paras.Add("email", vm.email);
            //paras.Add("merchant_login", vm.merchant_login);
            //paras.Add("merchant_password", vm.merchant_password);

            string sql = @"
INSERT INTO ci_relations_category
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

        internal void CreateDetail(ci_relations_categoryDTO vm, string empno)
        {
            DateTime dtTime = DateTime.Now;
            Dictionary<string, object> paras = new Dictionary<string, object>();

            paras.Add("ciname", vm.ciname);
            paras.Add("cicategory", int.Parse(vm.cicategory));
            paras.Add("parentsid", int.Parse(vm.parentsid));
            paras.Add("create_time", dtTime);
            paras.Add("create_operator", empno);
            paras.Add("update_time", dtTime);
            paras.Add("modify_operator", empno);
            paras.Add("remark", vm.remark);
            paras.Add("reqsrc", vm.reqsrc);

            string originSQL = $@"
insert into ci_relations 
(
ciname,
cicategory,
parentsid,
create_time,
create_operator,
update_time,
modify_operator,
remark,
reqsrc
)
VALUES 
(
@ciname,
@cicategory,
@parentsid,
@create_time,
@create_operator,
@update_time,
@modify_operator,
@remark,
@reqsrc
)
RETURNING cisid;
";

            var result = dbHelper.FindScalar<int>(originSQL, paras);

            paras = new Dictionary<string, object>();
            paras.Add("cisid", result);
            paras.Add("descr", vm.descr);
            paras.Add("notes", vm.notes);
            paras.Add("picture_path", vm.picture_path);
            paras.Add("actype", vm.actype);
            paras.Add("kpitime", vm.kpitime);
            paras.Add("selfconfig", vm.selfconfig);

            originSQL = $@"
insert into ci_relations_category
(
cisid,
descr,
notes,
picture_path,
actype,
kpitime,
selfconfig
)
values
(
@cisid,
@descr,
@notes,
@picture_path,
@actype,
@kpitime,
@selfconfig
)

";

            dbHelper.Execute(originSQL, paras);

            dbHelper.Commit();
        }

        internal void Delete(string order_id)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("order_id", order_id);

            string originSQL = @"
delete from ci_relations_category where order_id = @order_id
";

            dbHelper.Execute(originSQL, paras);
            dbHelper.Commit();
        }

        internal void DeleteDetail(int cisid, string empno)
        {
            DateTime dtTime = DateTime.Now;

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("cisid", cisid);
            paras.Add("modify_operator", empno);
            paras.Add("update_time", dtTime);

            string originSQL = @"
update ci_relations
set 
disable ='Y',
update_time=@update_time,
modify_operator=@modify_operator

where cisid=@cisid

";

            dbHelper.Execute(originSQL, paras);
            dbHelper.Commit();
        }

        internal void EditDetail(ci_relations_categoryDTO vm)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("cisid", vm.cisid);
            paras.Add("ciname", vm.ciname);
            paras.Add("remark", vm.remark);
            paras.Add("descr", vm.descr);

            paras.Add("picture_path", vm.picture_path);
            paras.Add("kpitime", vm.kpitime);
            paras.Add("selfconfig", vm.selfconfig);

            paras.Add("reqsrc", vm.reqsrc);
            paras.Add("actype", vm.actype);

            string originSQL = $@"
update ci_relations 
set          
ciname=@ciname,         
remark=@remark,   
reqsrc=@reqsrc    
where
cisid=@cisid
";

            dbHelper.Execute(originSQL, paras);

            originSQL = $@"
update ci_relations_category
set

descr=@descr,          
picture_path=@picture_path,   
kpitime=@kpitime,        
selfconfig=@selfconfig,         
actype=@actype     

where
cisid=@cisid

";

            dbHelper.Execute(originSQL, paras);

            dbHelper.Commit();
        }

        internal List<store_typeDTO> GetActypes()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();

            string strWhere = "";

            string originSQL = $@"
SELECT type_value
FROM   store_type
WHERE  type_name = 'ACTION_TYPE'
ORDER  BY order_id 
";

            var result = dbHelper.FindList<store_typeDTO>(originSQL, paras);
            return result;
        }

        internal ci_relations_categoryDTO GetDetail(string cisid)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("cisid", cisid);

            string strWhere = "";

            string originSQL = $@"
SELECT 

ci.cisid,
ci.ciname,
ci.remark,
circ.descr,
ci.reqsrc,
circ.actype,
circ.picture_path,
circ.kpitime,
circ.selfconfig,
circ.notes


FROM CI_RELATIONS ci
LEFT JOIN CI_RELATIONS_CATEGORY circ ON circ.CISID  = ci.CISID
WHERE 1=1
and
ci.cisid=@cisid
";

            var result = dbHelper.Find<ci_relations_categoryDTO>(originSQL, paras);
            return result;
        }

        internal List<store_typeDTO> GetReqsrcs()
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();

            string strWhere = "";

            string originSQL = $@"
SELECT *
FROM   (SELECT Trim(selectitem) AS STORE_TYPE
        FROM   column_select
        WHERE  columnname = 'STORE_TYPE'
        UNION
        SELECT DISTINCT Trim(type_value) AS STORE_TYPE
        FROM   store_type
        WHERE  type_name = 'STORE_TYPE')
WHERE  store_type <> 'WARRANTY'
ORDER  BY store_type 
";

            var result = dbHelper.FindList<store_typeDTO>(originSQL, paras);
            return result;
        }
    }
}
