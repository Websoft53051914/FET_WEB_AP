using Core.Utility.Helper.DB.Entity;
using Core.Utility.Web.EX;
using FTT_API.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTT_API.Controllers.FTTGroupMgt
{
    
    public partial class FTTGroupMgtController : BaseProjectController
    {
         
        
        [HttpPost("[action]")]
        public IActionResult Create(ftt_groupDTO vm)
        {
            try
            {
                ftt_groupSQL _ftt_groupSQL = new ftt_groupSQL();
                //判斷是否存在
                var dto = _ftt_groupSQL.GetInfoByEmpno(vm.EmpNo);
                if (dto != null)
                {
                this.LogSuccess();
                    return JsonValidFail("輸入的員工編號已存在");
                }

                fet_user_profileSQL _fet_user_profileSQL = new fet_user_profileSQL();
                //判斷是否存在人員檔
                var dto2 = _fet_user_profileSQL.GetInfoByEmpno(vm.EmpNo);
                if (dto2 == null)
                {
                this.LogSuccess();
                    return JsonValidFail("輸入的員工編號不存在");
                }

                vm.Ext = dto2.ext;
                vm.EName = dto2.engname;
                vm.CName = dto2.empname;
                _ftt_groupSQL.Insert(vm);

                this.LogSuccess("新增完成");
                return JsonSuccess("新增完成");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統異常");
            }
        }
    }
}