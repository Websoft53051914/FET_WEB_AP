/**
 * 舊版頁面： "/pool/newopen.aspx", "/Form/SubmitForm.aspx(.cs), "/Form/StoreInfo.ascx", "/Form/TTInfo.ascx"
 */
using Const;
using Core.Utility.Utility;
using FTT_WEB.Common;
using FTT_WEB.Common.ConfigurationHelper;
using FTT_WEB.Common.OriginClass.EntiityClass;
using FTT_WEB.Models;
using FTT_WEB.Models.Handler;
using FTT_WEB.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FTT_WEB.Controllers.NewOrder
{
    /// <summary>
    /// 新開單
    /// </summary>
    public partial class NewOrderController : BaseProjectController
    {
        public NewOrderController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        private ConfigurationHelper _configHelper;
    }

    public partial class NewOrderController
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
                    return View("StopByLunarNewYear");
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

        /// <summary>
        /// [Form/SubmitForm.aspx.cs]SubmitForm_Click()
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(NewOrderVM vm)
        {
            try
            {
                Trace.Write("Prepare to Convert Form Collection ...");
                NewOrderHandler newOrderHandler = new(_configHelper);

                // 取得報修單主單單號
                int TT_NO = newOrderHandler.GetNextTTNo();
                Trace.Write("報修單主單單號：" + TT_NO);

                List<Dictionary<string, object>> dataList = [];
                for (int i = 0; i < vm.TTItemList.Count; i++)
                {
                    Trace.Write("Order ID：" + (i + 1).ToString());
                    NewOrderTTItemVM item = vm.TTItemList[i];
                    Dictionary<string, object> data = new()
                    {
                        { "form_no", newOrderHandler.GetNextTTNo() },
                        { "ivrcode", vm.IVRCODE ?? string.Empty },
                        { "category_id", int.Parse(item.CATEGORY_ID ?? string.Empty) },
                        { "category_name", item.CATEGORY_NAME ?? string.Empty },
                        { "createtime", DateTime.Now },
                        { "empname", vm.EMPNAME ?? string.Empty },
                        { "emptel", vm.EMPTEL ?? string.Empty },
                        { "descr", (item.ItemDescVal ?? string.Empty) + " " },
                        { "checkitem", (item.ItemNoteVal ?? string.Empty) + " " },
                        { "tt_category", vm.TT_CATEGORY ?? string.Empty },
                        { "order_id", i + 1 },
                        { "tt_no", TT_NO },
                        { "remark", item.REMARK ?? string.Empty },
                        { "vender_id", int.Parse(item.VENDER_ID ?? string.Empty) },
                        { "tt_type", "FTT" },
                        { "repair", vm.REPAIR ?? string.Empty },
                        { "resupply", vm.RESUPPLY ?? string.Empty },
                        { "selfconfig", vm.SELFCONFIG ?? string.Empty },
                    };

                    string formType = "FTT_FORM";
                    if (newOrderHandler.GetValCIDescL1(ConvertUtility.ConvertToInt32(item.CATEGORY_ID ?? string.Empty, 0)).IndexOf("保全") > -1)
                    {
                        formType = "SECURITY_FORM";
                    }
                    data["formtype"] = formType;
                    dataList.Add(data);
                }

                newOrderHandler.DoCreateFttForm(dataList);

                return JsonSuccess("報修單開立成功！");
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }

    public partial class NewOrderController
    {
        /// <summary>
        /// [FormControlGenerate.aspx.cs]Page_Load FormAction=TT_COUNT
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RetrieveTTCount(int cisid, string ivrCode)
        {
            try
            {
                NewOrderHandler newOrderHandler = new(_configHelper);
                string ret = newOrderHandler.RetrieveTTCount(cisid, ivrCode);

                return JsonSuccess(ret);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        public IActionResult GetSelectListVender(int cisid, string ivrCode, string ifWarrant)
        {
            try
            {
                NewOrderHandler newOrderHandler = new(_configHelper);
                int retGetCountVDispatchList = newOrderHandler.GetCountVDispatchList(cisid, ivrCode);
                string warrant = retGetCountVDispatchList > 0 ? "N" : "Y";
                string ruleId = ivrCode.Length > 4 ? "Y" : "N";

                List<FormDispatchGetDTO> dtoList = newOrderHandler.GetListFormDispatchGet(cisid, ConvertUtility.ConvertToInt32(ivrCode, 0), ifWarrant);
                List<SelectListItemCustom> selectListItemList = [];
                if (dtoList.Count == 0)
                {
                    if (ruleId == "N")
                    {
                        selectListItemList.Add(new SelectListItemCustom("系統尋商", "20"));
                    }
                    else
                    {
                        if (warrant == "Y")
                        {
                            return JsonValidFail("系統無設定-自行尋商！！");
                        }
                        else
                        {
                            return JsonValidFail("已過保固期-自行尋商！！");
                        }
                    }
                }
                else
                {
                    foreach (FormDispatchGetDTO dto in dtoList)
                    {
                        SelectListItemCustom item = new(dto.MERCHANT_NAME ?? string.Empty, dto.ORDER_ID.ToString())
                        {
                            OtherAttr = new() {
                                { "CP_NAME" , dto.CP_NAME ?? string.Empty },
                                { "CP_TEL" , dto.CP_TEL ?? string.Empty },
                            }
                        };
                        selectListItemList.Add(item);
                    }
                }

                return JsonSuccess(selectListItemList);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg") + ex.ToString());
            }
        }

        /// <summary>
        /// [Form/checkdata.asp]檢查報修項目是否已報修
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CheckRepairReported(int categoryId)
        {
            try
            {
                NewOrderHandler newOrderHandler = new(_configHelper);
                int ret = newOrderHandler.GetCountRepairReported(categoryId, LoginSession.Current.ivrcode);

                return JsonSuccess(ret > 0 ? "Y" : string.Empty);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return JsonValidFail("檢查報修項目是否已報修" + _configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
