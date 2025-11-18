using FTT_VENDER_API.Common.OriginClass.EntiityClass;

namespace FTT_VENDER_API.Models.Handler
{
    public class FileHandler : BaseDBHandler
    {
        internal FileDTO FindById(string id)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("id", id);

            string strWhere = "";

            string originSQL = $@"
select * from
tb_file
where id=@id
";

            var result = dbHelper.Find<FileDTO>(originSQL, paras);
            return result;
        }

        internal List<FileDTO> FindListByFormNo(string form_no)
        {
            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("formno", form_no);

            string strWhere = "";

            string originSQL = $@"
select * from
tb_file
where formno=@formno
";

            var result = dbHelper.FindList<FileDTO>(originSQL, paras);
            return result;
        }

        //private readonly ConfigurationHelper _configHelper;
        //private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        //public Control_LogHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        //{
        //    _configHelper = confighelper;
        //    _httpContext = httpContext;
        //}
        internal string Insert(FileEntity entity, string formNo, out int result)
        {
            Dictionary<string, object> paras = new()
            {
                { "Status", int.Parse(entity.Status)},
                { "creator" , entity.creator},
                { "createtime" , entity.createtime},
                { "updater" , entity.updater},
                { "updatetime" , entity.updatetime},
                { "filename", entity.filename},
                { "destfilepath" , entity.destfilepath},
                { "fileext" , entity.fileext},
                { "filesize" , entity.filesize},
                { "formNo" , formNo},
            };

            string strSql = @"
INSERT INTO tb_file
(
status, creator, createtime, updater, updatetime, filename, destfilepath, fileext, filesize,formno
)
VALUES 
(
@status, @creator, @createtime, @updater, @updatetime, @filename, @destfilepath, @fileext, @filesize,@formno
)
RETURNING id;
";

            try
            {
                result = base.dbHelper.FindScalar<int>(strSql, paras);
                base.dbHelper.Commit();
                return "";
            }
            catch (Exception ex)
            {
                result = 0;
                return ex.ToString();
            }
        }


    }
}
