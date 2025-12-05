/**
 * 舊版頁面： "/pool/newopen.aspx", "/Form/SubmitForm.aspx(.cs), "/Form/StoreInfo.ascx", "/Form/TTInfo.ascx"
 */
using Const;
using Const.VO;
using Core.Utility.Utility;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.ConfigurationHelper;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models;
using FTT_API.Models.Handler;
using FTT_API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FTT_API.Controllers.NewOrder
{
    /// <summary>
    /// 新開單 API
    /// </summary>
    [Route("[controller]")]
    [Common.Attribute.CustomAuthorization]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public partial class NewOrderController : BaseProjectController
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configHelper"></param>
        public NewOrderController(ConfigurationHelper configHelper)
        {
            _configHelper = configHelper;
        }

        private ConfigurationHelper _configHelper;
    }

    public partial class NewOrderController
    {
        /// <summary>
        /// 取得頁面資料
        /// </summary>
        [HttpPost("[action]")]
        public IActionResult GetInitData()
        {
            try
            {
                // [SubmitForm.aspx.cs.Page_Load]登入資訊已遺失檢查(統一檢查)
                ArgumentNullException.ThrowIfNullOrWhiteSpace(_sessionVO.ivrcode);

                DateTime now = DateTime.Now;
                CommonHandler commonHandler = new(_configHelper);
                // [newopen.aspx]檢查 IVRCode 是否存在
                bool checkExistIvrCode = commonHandler.CheckExistIvrCode(_sessionVO.ivrcode);
                if (!checkExistIvrCode)
                {
                    this.LogSuccess();
                    List<string> adminNameList = commonHandler.GetListAdminEngName();
                    return JsonValidFail($"門市[{_sessionVO.ivrcode}]尚未完成工程收驗無法報修!\r\n\r\n請聯絡 {string.Join("，", adminNameList)}");
                }

                StoreVO? storeVM = commonHandler.GetStoreData(_sessionVO.ivrcode);
                if (commonHandler.GetMessage().IsError())
                {
                    this.LogSuccess();
                    return JsonValidFail(commonHandler.GetMessage().GetErrMsg());
                }

                NewOrderVO result = new()
                {
                    StoreVO = storeVM ?? new(),
                    IVRCODE = _sessionVO.ivrcode,
                    EMPNAME = _sessionVO.empname,
                    EMPTEL = _sessionVO.ext,
                    Prompt = commonHandler.GetFieldData("CONFIG_VALUE", "MAINTAIN_CONFIG", new Dictionary<string, object>
                    {
                        { "CONFIG_NAME", "MARQUEE" }
                    }),
                    IfWarrant = "Y",
                };

                if (storeVM != null)
                {
                    result.CREATE_TIME = DateTime.Now.ToString(DbConst.FORMAT_DATE2);
                    DateTime? approvalDate = ConvertUtility.DateTimeTryParse(storeVM.ApprovalDate);
                    DateTime? warrantyTime = approvalDate?.AddYears(1);
                    result.APPROVALDATE = approvalDate?.ToString(DbConst.FORMAT_DATE2) ?? string.Empty;
                    result.WARRANTYTIME = warrantyTime?.ToString(DbConst.FORMAT_DATE2) ?? string.Empty;

                    if (warrantyTime.HasValue && now > warrantyTime.Value)
                    {
                        if (storeVM.Channel == "FRANCHISE")
                        {
                            result.WarrantyTimeFlag2 = true;
                        }

                        result.WarrantyTimeFlag1 = true;
                        result.IfWarrant = "N";
                    }
                    result.ReqSrc = storeVM.StoreType;
                    if (result.IVRCODE.Length > 4)
                    {
                        result.AcType = "FRANCHISE";
                        if (result.WarrantyTimeFlag2)
                        {
                            result.ReqSrc = "WARRANTY";
                        }
                    }
                    else
                    {
                        result.AcType = "RETAIL";
                    }
                }

                this.LogSuccess();
                return JsonSuccess(result);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// [Form/SubmitForm.aspx.cs]SubmitForm_Click()
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
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
                        { "fileids", item.FileIds },
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

                MailPoolHandler _MailPoolHandlerHandler = new MailPoolHandler();

                foreach (Dictionary<string, object> data in dataList)
                {
                    var oldStatus = "NEW";
                    if (data["selfconfig"].ToString().ToLower() == "y")
                        oldStatus = "SELF";

                    var result = Method.CreateMailPool(data["form_no"].ToString(), oldStatus, "REVIEW", _MailPoolHandlerHandler);
                    if (!string.IsNullOrEmpty(result))
                    {
                        this.LogError("CreateMailPool 執行失敗");
                    }
                }

                this.LogSuccess("報修單開立成功！");
                return JsonSuccess("報修單開立成功！");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
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
        [HttpPost("[action]")]
        public IActionResult RetrieveTTCount(int cisid, string ivrCode)
        {
            try
            {
                NewOrderHandler newOrderHandler = new(_configHelper);
                string ret = newOrderHandler.RetrieveTTCount(cisid, ivrCode);

                this.LogSuccess();
                return JsonSuccess(ret);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// 取得廠商清單
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
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

                this.LogSuccess();
                return JsonSuccess(selectListItemList);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail(_configHelper.GetMessage("SystemErrorMsg"));
            }
        }

        /// <summary>
        /// [Form/checkdata.asp]檢查報修項目是否已報修
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        public IActionResult CheckRepairReported(int categoryId)
        {
            try
            {
                NewOrderHandler newOrderHandler = new(_configHelper);
                int ret = newOrderHandler.GetCountRepairReported(categoryId, LoginSession.Current.ivrcode);

                this.LogSuccess();
                return JsonSuccess(ret > 0 ? "Y" : string.Empty);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("檢查報修項目是否已報修" + _configHelper.GetMessage("SystemErrorMsg"));
            }
        }
    }
}
