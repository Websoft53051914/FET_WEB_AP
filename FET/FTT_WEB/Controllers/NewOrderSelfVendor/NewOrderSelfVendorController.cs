/**
 * 舊版頁面： "/pool/newopen2.aspx", "/Form/SubmitForm2.aspx(.cs), "/Form/StoreInfo.ascx", "/Form/TTInfo2.ascx"
 */
using Const;
using Core.Utility.Utility;
using FTT_WEB.Common;
using FTT_WEB.Common.ConfigurationHelper;
using FTT_WEB.Models.Handler;
using FTT_WEB.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace FTT_WEB.Controllers.NewOrderSelfVendor
{
    /// <summary>
    /// 自行尋商開單
    /// </summary>
    public partial class NewOrderSelfVendorController : BaseProjectController
    {
        public NewOrderSelfVendorController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        private ConfigurationHelper _configHelper;
    }

    public partial class NewOrderSelfVendorController
    {
        public IActionResult Index()
        {
            try
            {
                // [SubmitForm.aspx.cs.Page_Load]登入資訊已遺失檢查(統一檢查)
                ArgumentNullException.ThrowIfNullOrWhiteSpace(LoginSession.Current.ivrcode);

                DateTime now = DateTime.Now;

                CommonHandler commonHandler = new(_configHelper);
                // [newopen.aspx]檢查 IVRCode 是否存在
                bool checkExistIvrCode = commonHandler.CheckExistIvrCode(LoginSession.Current.ivrcode);
                if (!checkExistIvrCode)
                {
                    List<string> adminNameList = commonHandler.GetListAdminEngName();
                    return RedirectToAlertMsg("Index", "Home", $"門市[{LoginSession.Current.ivrcode}]尚未完成工程收驗無法報修!\r\n\r\n請聯絡 {string.Join("，", adminNameList)}", "warning");
                }
                // [SubmitForm.aspx.cs.Page_Load]年節期間暫停設備報修
                if (now < Common.Const.LUNAR_NEW_YEAR_END && now > Common.Const.LUNAR_NEW_YEAR_START)
                {
                    return View("NewOrder/StopByLunarNewYear");
                }

                StoreVM? storeVM = commonHandler.GetStoreData(LoginSession.Current.ivrcode);
                if (commonHandler.GetMessage().IsError())
                {
                    return RedirectToAlertMsg("Index", "Home", commonHandler.GetMessage().GetErrMsg(), "error");
                }
                NewOrderVM vm = new()
                {
                    StoreVM = storeVM ?? new(),
                    IVRCODE = LoginSession.Current.ivrcode,
                    EMPNAME = LoginSession.Current.empname,
                    EMPTEL = LoginSession.Current.ext,
                    Prompt = commonHandler.GetFieldData("CONFIG_VALUE", "MAINTAIN_CONFIG", new Dictionary<string, object>
                    {
                        { "CONFIG_NAME", "MARQUEE" }
                    }),
                };

                if (storeVM != null)
                {
                    vm.CREATE_TIME = DateTime.Now.ToString(DbConst.FORMAT_DATE2);
                    DateTime? approvalDate = ConvertUtility.DateTimeTryParse(storeVM.ApprovalDate);
                    DateTime? warrantyTime = approvalDate?.AddYears(1);
                    vm.APPROVALDATE = approvalDate?.ToString(DbConst.FORMAT_DATE2) ?? string.Empty;
                    vm.WARRANTYTIME = warrantyTime?.ToString(DbConst.FORMAT_DATE2) ?? string.Empty;
                    if (warrantyTime.HasValue && now > warrantyTime.Value)
                    {
                        if (storeVM.Channel == "FRANCHISE")
                        {
                            vm.WarrantyTimeFlag2 = true;
                        }

                        vm.WarrantyTimeFlag1 = true;
                    }
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return RedirectToAlertMsg("Index", "Home", _configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
