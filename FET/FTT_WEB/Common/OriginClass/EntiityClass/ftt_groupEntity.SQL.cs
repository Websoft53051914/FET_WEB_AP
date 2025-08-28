using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class ftt_groupSQL
    {
        public PageResult<ftt_groupDTO> FindPageList(PageEntity pageEntity, ftt_groupDTO dto)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("cname", @$"%{dto.CName}%");

            string originSQL = @"
select * from public.ftt_group 
where cname like @cname 
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

            var result = baseHandler.GetDBHelper().FindPageList<ftt_groupDTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal void Delete(string empno)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("EmpNo", empno);

            string sqlWhere = "";

            string qrySQL = $@"
delete from ftt_group where empno=@EmpNo
";

            baseHandler.GetDBHelper().Execute(qrySQL, paras);
            baseHandler.GetDBHelper().Commit();
        }

        internal List<ftt_groupDTO> GetGroupList()
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();

            string sqlWhere = "";

            string qrySQL = $@"
select ftt_group from ftt_group group by ftt_group
";

            return baseHandler.GetDBHelper().FindList<ftt_groupDTO>(qrySQL, paras);
        }

        internal ftt_groupDTO GetInfoByEmpno(string empno)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("empno", empno);

            string sqlWhere = "";

            string qrySQL = $@"
select * from ftt_group 
where empno=@empno
";

            return baseHandler.GetDBHelper().Find<ftt_groupDTO>(qrySQL, paras);
        }

        internal void Insert(ftt_groupDTO dto)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("EmpNo", dto.EmpNo);
            paras.Add("FTT_Group", dto.FTT_Group);
            paras.Add("CName", dto.CName);
            paras.Add("EName", dto.EName);
            paras.Add("Ext", dto.Ext);

            string sqlWhere = "";

            string qrySQL = $@"
INSERT INTO ftt_group(
	empno, ftt_group, cname, ename, ext)
	VALUES (@empno,@ftt_group, @cname, @ename, @ext);
";

            baseHandler.GetDBHelper().Execute(qrySQL, paras);
            baseHandler.GetDBHelper().Commit();
        }
    }
}
