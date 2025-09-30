using Const.DTO;
using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.ViewModel;
using System.Text;

namespace FTT_VENDER_API.Models.Handler
{
    public class TB_Control_LogHandler : BaseDBHandler
    {
        //private readonly ConfigurationHelper _configHelper;
        //private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        //public TB_Control_LogHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        //{
        //    _configHelper = confighelper;
        //    _httpContext = httpContext;
        //}
        internal void Insert(TB_Control_LogEntity entity)
        {
            Dictionary<string, object> paras = new()
            {

        //public string ID { get; set; }
        //public DateTime LogTime { get; set; }
        //public string IP { get; set; }
        //public string Account { get; set; }
        //public string Name { get; set; }
        //public string Exception { get; set; }
        //public string Status { get; set; }
        //public string ControllerName { get; set; }
        //public string ActionName { get; set; }

                { "LogTime", entity.LogTime},
                { "IP", entity.IP},
                { "Account", entity.Account},
                { "Name", entity.Name},
                { "Exception", entity.Exception},
                { "Status", entity.Status},
                { "ControllerName", entity.ControllerName},
                { "ActionName", entity.ActionName},
                { "Token", entity.Token},
            };

            string strSql = @"
insert into TB_Control_Log
(LogTime ,   IP ,    Account ,   Name ,  Exception ,     Status ,    ControllerName ,    ActionName,Token)
values
(@LogTime ,  @IP ,   @Account ,  @Name , @Exception ,    @Status ,   @ControllerName ,   @ActionName,@Token )
";

            try
            {
                base.dbHelper.Execute(strSql, paras);
                base.dbHelper.Commit();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
