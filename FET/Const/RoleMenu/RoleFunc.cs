using Const.RoleMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Const.Enums;

namespace Const
{
    public static class RoleFunc
    {
        public static List<FuncID> Vender = new List<FuncID>
        {
            FuncID.Home_View,
            FuncID.Pending_View,
            FuncID.InProcess_View,
            FuncID.CaseClosed_View,
            FuncID.Query_View,
            FuncID.Dispatching_View,
            FuncID.Dispatched_View
        };
        public static List<FuncID> Admin = new List<FuncID>
        {
            FuncID.Home_View,
            FuncID.NewOrder_View,
            FuncID.NewOrderSelfVendor_View,
            FuncID.Pending_View,
            FuncID.OnsitePrint_View,
            FuncID.InProcess_View,
            FuncID.CaseClosed_View,
            FuncID.Query_View,
            FuncID.QuoteMgt_View,
            FuncID.StoreMgt_View,
            FuncID.SupplierMgt_View,
            FuncID.DispatchRuleMgt_View,
            FuncID.CIConfig_View,
            FuncID.SecurityMgt_View,
            FuncID.FTTGroupMgt_View,
            FuncID.CIMgt_View,
            FuncID.MailServerSetting_View,
        };


        // 全部功能的定義
        public static Dictionary<string, List<MenuModel>> allTreeData = new Dictionary<string, List<MenuModel>>
        {
            ["門市報修管理"] = new List<MenuModel>
            {
                new MenuModel { FuncId=FuncID.NewOrder_View, FuncName="新開單", Url="/NewOrder" },
                new MenuModel { FuncId=FuncID.NewOrderSelfVendor_View, FuncName="自行尋商開單", Url="/NewOrderSelfVendor" },
                new MenuModel { FuncId=FuncID.Pending_View, FuncName="待處理", Url="/Pending", DataCount = 1 },
                new MenuModel { FuncId=FuncID.OnsitePrint_View, FuncName="列印維修單", Url="/OnsitePrint", DataCount = 1 },
                new MenuModel { FuncId=FuncID.InProcess_View, FuncName="處理中", Url="/InProcess", DataCount = 1 },
                new MenuModel { FuncId=FuncID.CaseClosed_View, FuncName="已結案", Url="/CaseClosed", DataCount = 1 },
                new MenuModel { FuncId=FuncID.Query_View, FuncName="查詢", Url="/Query" }
            },
            ["報價維護"] = new List<MenuModel>
            {
                new MenuModel { FuncId=FuncID.QuoteMgt_View, FuncName="報價維護", Url="/QuoteMgt" }
            },
            ["廠商派工管理"] = new List<MenuModel>
            {
                new MenuModel { FuncId=FuncID.Dispatching_View, FuncName="派工中", Url="/Dispatching" },
                new MenuModel { FuncId=FuncID.Dispatched_View, FuncName="已派工", Url="/" },
            },
            ["後端管理"] = new List<MenuModel>
            {
                new MenuModel { FuncId=FuncID.StoreMgt_View, FuncName="門市資料維護", Url="/StoreMgt" },
                new MenuModel { FuncId=FuncID.SupplierMgt_View, FuncName="廠商資料維護", Url="/SupplierMgt" },
                new MenuModel { FuncId=FuncID.DispatchRuleMgt_View, FuncName="派工規則維護", Url="/DispatchRuleMgt" },
                new MenuModel { FuncId=FuncID.CIConfig_View, FuncName="例外派工維護", Url="/CIConfig" },
                new MenuModel { FuncId=FuncID.SecurityMgt_View, FuncName="保全廠商維護", Url="/SecurityMgt" },
                new MenuModel { FuncId=FuncID.FTTGroupMgt_View, FuncName="角色權限維護", Url="/FTTGroupMgt" },
                new MenuModel { FuncId=FuncID.CIMgt_View, FuncName="報修品項維護", Url="/CIMgt" },
                new MenuModel { FuncId=FuncID.MailServerSetting_View, FuncName="Mail Server 設定", Url="/MailServerSetting" }

            }
        };

        public static Dictionary<string, List<MenuModel>> GetMenuByFuncIds(List<FuncID> allowedFuncIds)
        {
            var allowedSet = new HashSet<FuncID>(allowedFuncIds);

            return allTreeData
                .Select(group => new
                {
                    Category = group.Key,
                    Menus = group.Value
                        .Where(menu => allowedSet.Contains(menu.FuncId))
                        .ToList()
                })
                .Where(g => g.Menus.Any()) // 過濾掉空分類
                .ToDictionary(g => g.Category, g => g.Menus);
        }
    }
}
