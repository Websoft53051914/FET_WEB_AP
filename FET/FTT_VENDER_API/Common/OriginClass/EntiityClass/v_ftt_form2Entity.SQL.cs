using Core.Utility.Helper.DB.Entity;
using FTT_VENDER_API.Models.Handler;

namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class v_ftt_form2SQL
    {
        public PageResult<v_ftt_form2DTO> FindPageList(PageEntity pageEntity, v_ftt_form2DTO dto)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("USERROLE", dto.USERROLE);
            paras.Add("EMPNO", dto.EMPNO);
            paras.Add("IVRCODE", dto.IVRCODE);

            string originSQL = @"

SELECT form_no                                       ,
       tt_category                                   ,
       l2_desc                                       ,
       ciname                                        ,
       To_char(createtime, 'yyyy/mm/dd hh24:mi:ss') AS createtime,
       shop_name                                    ,
       statusname                                   ,
       To_char(updatetime, 'yyyy/mm/dd hh24:mi:ss') AS updatetime
FROM   v_ftt_form2
WHERE  form_no IN (SELECT form_no
                   FROM   access_role
                   WHERE  action = 'Y'
                          AND ( User_Type = @USERROLE
                                 OR empno = @EMPNO
                                 OR deptcode = @IVRCODE ))
       AND StatusId NOT IN ( 'CONFIRM' )
ORDER  BY updatetime DESC 
";

            //switch (dto.USERROLE)
            //{
            //    case "VENDOR":
            //        originSQL = "SELECT DISTINCT form_no as form_no,tt_category as tt_category,l2_desc as l2_desc,ciname as ciname,to_char(createtime,'yyyy/mm/dd hh24:mi:ss') as createtime,shop_name as shop_name,statusname as statusname,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime FROM v_ftt_form2 WHERE form_no in (select form_no from ACCESS_ROLE where action='Y' and deptcode=@IVRCODE and User_Type=@USERROLE and @EMPNO is not null) order by updatetime desc";
            //        break;
            //    case "MANAGER":
            //        originSQL = "SELECT DISTINCT form_no as form_no,tt_category as tt_category,l2_desc as l2_desc,ciname as ciname,to_char(createtime,'yyyy/mm/dd hh24:mi:ss') as createtime,shop_name as shop_name,statusname as statusname,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime FROM v_ftt_form2 WHERE form_no in (select form_no from ACCESS_ROLE where action='Y' and  empno=@EMPNO and User_Type=@USERROLE and @IVRCODE is not null) order by updatetime desc";
            //        break;
            //    case "SUBMITTER":
            //        originSQL = "SELECT DISTINCT form_no as form_no,tt_category as tt_category,l2_desc as l2_desc,ciname as ciname,to_char(createtime,'yyyy/mm/dd hh24:mi:ss') as createtime,shop_name as shop_name,statusname as statusname,to_char(updatetime,'yyyy/mm/dd hh24:mi:ss') as updatetime FROM v_ftt_form2 WHERE (vender='自行尋商' and ivrcode=@IVRCODE and StatusId in ('DISPATCH','USED')) or (StatusId not in ('CONFIRM') and form_no in (select form_no from ACCESS_ROLE where action='Y' and (User_Type=@USERROLE or empno=@EMPNO or deptcode=@IVRCODE))) order by updatetime desc";
            //        break;
            //    default:
            //        break;
            //}

            string countSQL = @"
  SELECT  
    count(0)
  FROM 
  (
" + originSQL + @"
) as pageData
 where 1=1 
";

            var result = baseHandler.GetDBHelper().FindPageList<v_ftt_form2DTO>(originSQL, countSQL, pageEntity.CurrentPage, pageEntity.PageDataSize, paras);
            return result;
        }

        internal void Delete(string empno)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("EmpNo", empno);

            string sqlWhere = "";

            string qrySQL = $@"
delete from v_ftt_form2 where empno=@EmpNo
";

            baseHandler.GetDBHelper().Execute(qrySQL, paras);
            baseHandler.GetDBHelper().Commit();
        }

        internal List<v_ftt_form2DTO> GetGroupList()
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();

            string sqlWhere = "";

            string qrySQL = $@"
select v_ftt_form2 from v_ftt_form2 group by v_ftt_form2
";

            return baseHandler.GetDBHelper().FindList<v_ftt_form2DTO>(qrySQL, paras);
        }

        internal v_ftt_form2DTO GetInfoByEmpno(string empno)
        {
            BaseDBHandler baseHandler = new BaseDBHandler();
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("empno", empno);

            string sqlWhere = "";

            string qrySQL = $@"
select * from v_ftt_form2 
where empno=@empno
";

            return baseHandler.GetDBHelper().Find<v_ftt_form2DTO>(qrySQL, paras);
        }

    }
}
