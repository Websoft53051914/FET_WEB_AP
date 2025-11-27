using Const.RoleMenu;

namespace FTT_WEB.Models
{
    public class HomeMenuVM
    {
        public Dictionary<string, List<MenuModel>> TreeData { set; get; } = new Dictionary<string, List<MenuModel>>();
        public string Exception { get; set; }
        public string jwtToken { get; set; }
    }
}
