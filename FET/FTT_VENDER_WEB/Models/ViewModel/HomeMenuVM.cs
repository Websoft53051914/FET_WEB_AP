using Const.RoleMenu;

namespace FTT_VENDER_WEB.Models
{
    public class HomeMenuVM
    {
        public Dictionary<string, List<MenuModel>> TreeData { set; get; } = new Dictionary<string, List<MenuModel>>();
    }
}
