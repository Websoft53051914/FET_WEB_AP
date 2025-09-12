using Const.DTO;
using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using FTT_VENDER_API.Common.ConfigurationHelper;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models.ViewModel;
using System.Text;

namespace FTT_VENDER_API.Models.Handler
{
    public class ControlLogHandler : BaseDBHandler
    {
        //private readonly ConfigurationHelper _configHelper;
        //private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;
        //public ControlLogHandler(ConfigurationHelper confighelper, Microsoft.AspNetCore.Http.HttpContext httpContext)
        //{
        //    _configHelper = confighelper;
        //    _httpContext = httpContext;
        //}
        internal void Insert(controllogEntity entity)
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

            };

            string strSql = @"
insert into controllog
(LogTime ,   IP ,    Account ,   Name ,  Exception ,     Status ,    ControllerName ,    ActionName)
values
(@LogTime ,  @IP ,   @Account ,  @Name , @Exception ,    @Status ,   @ControllerName ,   @ActionName )
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
