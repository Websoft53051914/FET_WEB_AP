using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class maintain_configSQL
    {
        public PageResult<maintain_configDTO> FindPageList(PageEntity pageEntity, maintain_configDTO dto)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();

            string originSQL = @"
select * from public.maintain_config 
where 1=1
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

            var result = baseHandler.GetDBHelper().FindPageList<maintain_configDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal void Delete(string empno)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("EmpNo", empno);

            string sqlWhere = "";

            string qrySQL = $@"
delete from maintain_config where empno=@EmpNo
";

            baseHandler.GetDBHelper().Execute(qrySQL, paras);
            baseHandler.GetDBHelper().Commit();
        }

        internal maintain_configDTO FindByConfigName(string config_name)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("config_name", config_name);

            string sqlWhere = "";

            string qrySQL = $@"
select * from maintain_config 
where config_name=@config_name
";

            return baseHandler.GetDBHelper().Find<maintain_configDTO>(qrySQL, paras);
        }

        internal maintain_configDTO GetInfoByEmpno(string empno)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("empno", empno);

            string sqlWhere = "";

            string qrySQL = $@"
select * from maintain_config 
where empno=@empno
";

            return baseHandler.GetDBHelper().Find<maintain_configDTO>(qrySQL, paras);
        }

    }
}
