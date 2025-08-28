using static Const.Enums;

namespace FTT_VENDER_WEB.Models
{
    public class MenuVM
    {
        public MenuVM()
        {
        }
        public string ClassName { set; get; }

        public int PermissionCode { set; get; }
        public string FuncName { set; get; }
        public FuncID FuncId { set; get; }
        public string Url { set; get; }

        public int? DataCount { set; get; }

        public bool IsActive { set; get; } = false;
    }
}
