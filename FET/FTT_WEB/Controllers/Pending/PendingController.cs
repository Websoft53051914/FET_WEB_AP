using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_WEB.Common;
using FTT_WEB.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.X86;

namespace FTT_WEB.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PendingController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _config = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            maintain_configSQL _maintain_configSQL = new maintain_configSQL();
            maintain_configDTO dto = _maintain_configSQL.FindByConfigName("HANDLER");
            if (dto != null)
            {
                ViewData["HandlerDesc"] = dto.config_value;
            }
            return View();
        }

        public async Task<IActionResult> GetPageList(DataSourceRequest request, v_ftt_form2DTO vm)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                v_ftt_form2SQL _v_ftt_form2SQL = new v_ftt_form2SQL();

                vm.USERROLE = LoginSession.Current.userrole;
                vm.IVRCODE = LoginSession.Current.ivrcode;
                vm.EMPNO = LoginSession.Current.empno;

                var list = _v_ftt_form2SQL.FindPageList(pageEntity, vm);

                for (int i = 0; i < list.Results.Count; i++)
                {
                    var item = list.Results[i];
                    item.No = (request.pageIndex - 1) * request.pageSize + i + 1;
                }

                return Json(new DataSourceResult
                {
                    Data = list.Results,
                    Total = list.DataCount
                });
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統錯誤");
            }
        }
    }
}
