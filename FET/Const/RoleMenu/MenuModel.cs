using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Const.Enums;

namespace Const.RoleMenu
{
    public class MenuModel
    {
        public string ClassName { set; get; }

        public int PermissionCode { set; get; }
        public string FuncName { set; get; }
        public FuncID FuncId { set; get; }
        public string Url { set; get; }

        public int? DataCount { set; get; }

        public bool IsActive { set; get; } = false;
    }
}
