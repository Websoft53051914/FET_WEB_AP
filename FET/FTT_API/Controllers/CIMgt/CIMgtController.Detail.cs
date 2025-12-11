using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FTT_API.Controllers.CIMgt
{
    public partial class CIMgtController : BaseProjectController
    {
        public class CIMgtDefaultData()
        {
            public List<SelectListItem> reqsrcs { get; set; } = new List<SelectListItem>();
            public List<SelectListItem> actypes { get; set; } = new List<SelectListItem>();
        }
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetDefault()
        {
            try
            {
                var _CIMgtHandler = new CIMgtHandler(_config, HttpContext);
                CIMgtDefaultData _CIMgtDefaultData = new CIMgtDefaultData();
                _CIMgtDefaultData.reqsrcs = _CIMgtHandler.GetReqsrcs().Select(s => new SelectListItem() { Text = s.STORE_TYPE, Value = s.STORE_TYPE }).ToList();
                _CIMgtDefaultData.actypes = _CIMgtHandler.GetActypes().Select(s => new SelectListItem() { Text = s.type_value, Value = s.type_value }).ToList();
                    this.LogSuccess();
                return JsonSuccess(_CIMgtDefaultData);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetDetail(string cisid)
        {
            try
            {
                var _CIMgtHandler = new CIMgtHandler(_config, HttpContext);
                var result = _CIMgtHandler.GetDetail(cisid);
                    this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateDetail(ci_relations_categoryDTO vm)
        {
            try
            {
                var _CIMgtHandler = new CIMgtHandler(_config, HttpContext);
                _CIMgtHandler.CreateDetail(vm, this._sessionVO.empno);
                    this.LogSuccess();
                return JsonSuccess("新增成功");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> EditDetail(ci_relations_categoryDTO vm)
        {
            try
            {
                var _CIMgtHandler = new CIMgtHandler(_config, HttpContext);
                _CIMgtHandler.EditDetail(vm);
                    this.LogSuccess();
                return JsonSuccess("更新成功");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> DeleteDetail(int cisid)
        {
            try
            {
                var _CIMgtHandler = new CIMgtHandler(_config, HttpContext);
                _CIMgtHandler.DeleteDetail(cisid, this._sessionVO.empno);
                    this.LogSuccess();
                return JsonSuccess("刪除成功");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }
    }
}
