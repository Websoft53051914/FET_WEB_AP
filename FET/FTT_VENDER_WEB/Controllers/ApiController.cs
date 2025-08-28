using Core.Utility.Helper.DB.Entity;
using Core.Utility.Utility;
using Core.Utility.Web.EX;
using FTT_VENDER_WEB.Common;
using FTT_VENDER_WEB.Common.ConfigurationHelper;
using FTT_VENDER_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using static Const.Enums;

namespace FTT_VENDER_WEB.Controllers
{
    public class ApiController : BaseProjectController
    {
        private readonly ConfigurationHelper _config;
        public ApiController(ConfigurationHelper config) 
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult GetCiDataSelfVendorPageList(DataSourceRequest request)
        {
            try
            {
                PageEntity pageEntity = base.GetPageEntity(request);

                //CISID  CINAME ACINAME
                var data = new List<dynamic>
                {
                    new
                    {
                        CISID = "534",
                        CINAME = "27吋監控電視螢幕(飛利浦 273V5LHSB)",
                        ACINAME = "停車監控設備 -27吋監控電視螢幕(飛利浦 273V5LHSB)"
                    },
                    new
                    {
                        CISID = "4535",
                        CINAME = "停車監控主機(型號：G5004)",
                        ACINAME = "停車監控設備 -停車監控主機(型號：G5004)"
                    },
                    new
                    {
                        CISID = "44564",
                        CINAME = "停車監控主機(型號：G5004)",
                        ACINAME = "停車監控設備 -停車監控主機(型號：G5004)"
                    },
                };

                return JsonPage(new DataSourceResult
                {
                    Data = data,
                    Total = 2
                });
            }
            catch (Exception ex)
            {
                return JsonValidFail(_config.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
