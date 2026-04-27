using Const.VO;
using FTT_VENDER_API.Common;
using FTT_VENDER_API.Common.OriginClass;
using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models;
using FTT_VENDER_API.Models.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using System.Data;

namespace FTT_VENDER_API.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        /// <summary>
        /// 取得報修單明細資料
        /// </summary>
        /// <param name="form_no"></param>
        /// <returns></returns>

        [ValidateAntiForgeryToken]
        [HttpGet("[action]")]
        public IActionResult GetDetail(string form_no)
        {
            try
            {
                FormTableVM vm = new FormTableVM();
                vm.FileItems = new List<SelectListItem>() { };
                FileHandler _FileHandler = new FileHandler();
                var dtos = _FileHandler.FindListByFormNo(form_no);
                if (dtos != null && dtos.Count > 0)
                {
                    vm.FileItems = dtos.Select(s => new SelectListItem()
                    {
                        Text = s.filename,
                        Value = s.ID
                    }).ToList();
                }

                vm.Form_Type = "FTT_FORM";
                vm.ActionName = _sessionVO.empname;
                if (_sessionVO.empname != _sessionVO.engname)
                    vm.ActionName = _sessionVO.empname + "(" + _sessionVO.engname + ")";

                vm.PreHandleDesc = GetPreHandleDesc("TT_LAST_DESC", form_no, "", "");

                HandleForm_Load(vm, form_no);

                if (_sessionVO.userrole.ToLower() == "admin")
                {
                    vm.IsAdmin = true;
                }

                this.LogSuccess();
                return JsonSuccess(vm);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統異常");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="form_no"></param>
        protected void HandleForm_Load(FormTableVM vm, string form_no)
        {
            PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

            var mIVRCode = _PenddingHanlder.GetIVRCode(form_no);

            var _StoreClass = GetStoreInfo(vm, form_no);
            vm.Store_profileDTO = _StoreClass;

            var _TTInfo = GetTTInfo(vm, form_no);
            vm.Ftt_formDTO = _TTInfo;

            string APPROVE_FORM = "";
            var ftt_Form = _PenddingHanlder.GetFttFormInfo(form_no);
            vm.KpiDays = ftt_Form.kpi_days ?? 0;

            var approve_FormDTO = _PenddingHanlder.GetApproveFormInfo(form_no);

            var tempStatus = ftt_Form.STATUS;

            if (approve_FormDTO != null)
            {
                APPROVE_FORM = approve_FormDTO.form_type;
                vm.Form = APPROVE_FORM;
                vm.Status = approve_FormDTO.status;
                vm.Status_Desc = approve_FormDTO.STATUS_NAME;

                tempStatus = approve_FormDTO.status;
            }

            vm.TempStatus = tempStatus;

            if (APPROVE_FORM != "")
            {
                vm.ApproveForm = APPROVE_FORM;

                // 舊版程式 preStatus 傳入的值有點奇怪
                HandleFormAuth(vm, APPROVE_FORM, form_no, tempStatus, APPROVE_FORM + "_PRIOR_STATUS", _sessionVO.ivrcode);

                // 當狀態為 [已派單] 不使用按鈕，使用下拉式選單選擇狀態與顯示相對應的控制項
                //vm.ShowTicketInfo = true;

                GetTicketInfo(vm, form_no, APPROVE_FORM, tempStatus, ftt_Form.ticket_info);

                ////TicketPanel.Visible = true;
                //TicketInfo.TTNo = form_no;
                //TicketInfo.TTStatus = tempStatus;
                //TicketInfo.ShowTicketInfo = ftt_Form.ticket_info;
                //TicketInfo.TTType = APPROVE_FORM;
                if (tempStatus == "TICKET")
                {
                    //SubmitButton = "";
                    vm.ShowOriginSubmitForm = true;
                    //SubmitForm.Visible = true;
                    //SubmitForm.Attributes["onClick"] = @"event.returnValue = CheckFormInTicket();";
                }
            }

            vm.SelectListDelayReason = _PenddingHanlder.GetListDelayReason()
                .Select(x => new SelectListItem(x.selectitem, x.selectindex?.ToString()))
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="formNo"></param>
        /// <param name="mTTType"></param>
        /// <param name="tempStatus"></param>
        /// <param name="ticket_info"></param>
        private void GetTicketInfo(FormTableVM vm, string formNo, string mTTType, string tempStatus, string ticket_info)
        {
            //vm.ShowAmount = true;

            if (!string.IsNullOrEmpty(formNo))
            {
                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

                bool disableOffer = true;  // 是否為保固內
                var storeProfile = _PenddingHanlder.GetStoreProfileInfoByFormNo(formNo);

                string StoreType = storeProfile.store_type;

                if (StoreType.ToUpper().IndexOf("FRANCHISE") == -1)
                {   // 為直營店
                    if (_PenddingHanlder.CheckDataExist_CI_EXCEPTION_CONFIG(formNo) == false)//"CI_EXCEPTION_CONFIG", "ENABLE='Y' AND CISID IN (SELECT CATEGORY_ID FROM FTT_FORM WHERE FORM_NO=" + formNo + ") AND IVRCODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=" + formNo + ") AND SYSDATE-APPROVAL_DATE<=365") == false)
                    {
                        if (_PenddingHanlder.CheckDataExist_STORE_PROFILE(formNo) == false) //("STORE_PROFILE", "IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=" + formNo + ") AND SYSDATE BETWEEN APPROVAL_DATE AND APPROVAL_DATE+365") == false)
                        {  // 保固外
                            disableOffer = false;
                        }
                    }
                }
                else //12/1新需求，加盟[影音/招牌]也要報價
                {
                    if (_PenddingHanlder.CheckDataExist_STORE_PROFILENotIn1278And1260(formNo)) //db.CheckDataExist("STORE_PROFILE", "IVR_CODE IN (SELECT IVRCODE FROM FTT_FORM WHERE FORM_NO=" + formNo + " and ci_sid_l1 (category_id) not in (1278,1260))") == false)
                    {
                        disableOffer = false;
                    }
                }

                string FormType = _PenddingHanlder.GetFormTypeByFormNo(formNo); //db.GetFieldData("FORM_TYPE", "APPROVE_FORM", "FORM_NO=" + formNo + " GROUP BY FORM_TYPE");
                if (FormType == "SECURITY_FORM")
                {
                    vm.UpdateSELECTSTATUSOption2 = true;
                    //mRunScript += "document.all.SELECTSTATUS.options[2].text='故障排除';";
                }

                if (disableOffer == true)
                {
                    vm.DeleteSELECTSTATUSOption3 = true;
                    //mRunScript += "document.all.SELECTSTATUS.remove(3);\r\n";
                }

                if (mTTType == "SECURITY_FORM")
                {
                    //vm.ShowAmount = false;
                    //mShowAmount = "none";
                }

                if (tempStatus == "TICKET")
                {
                    //mRunScript += "document.all.SELECTSTATUS_PANEL.style.display = \"\";\r\n";

                    if (_PenddingHanlder.CheckDataExist_FTT_FORM_LOG(formNo, "STATUS", "AGREE", "ASSIGN")) //db.CheckDataExist("FTT_FORM_LOG", "FORM_NO=" + formNo + " AND FIELDNAME='STATUS' AND OLDVALUE='AGREE' AND NEWVALUE='ASSIGN'") == true)
                    {
                        if (_PenddingHanlder.CheckDataExist_FTT_FORM_LOG(formNo, "STATUS", "ASSIGN", "PRWP")) //db.CheckDataExist("FTT_FORM_LOG", "FORM_NO=" + formNo + " AND FIELDNAME='STATUS' AND OLDVALUE='ASSIGN' AND NEWVALUE='PRWP'") == true)
                        {
                            if (_PenddingHanlder.CheckDataExist_FTT_FORM_LOG(formNo, "STATUS", "PRWP", "TICKET")) //db.CheckDataExist("FTT_FORM_LOG", "FORM_NO=" + formNo + " AND FIELDNAME='STATUS' AND OLDVALUE='PRWP' AND NEWVALUE='TICKET'") == true)
                            {
                                vm.DeleteSELECTSTATUS2ThreeTimes = true;
                                //                                mRunScript += @"
                                //document.all.SELECTSTATUS.remove(2);\r\n
                                //document.all.SELECTSTATUS.remove(2);\r\n
                                //document.all.SELECTSTATUS.remove(2);\r\n";
                            }
                        }
                    }
                }
                else
                {
                    // [/Form/AmountForm.aspx.cs]Page_Load()
                    var totalPrice = _PenddingHanlder.GetTotalPrice(formNo);

                    // 如果不是已派單，則顯示資料！
                    string total = totalPrice;
                    vm.Total = total;
                    vm.HideAmountDel = true;
                    vm.HideNewData = true;
                    //NewData.Visible = false;
                    // 2025/09/02 查看 沒有用到
                    //// 2008 12 28 Add - 廠商保固期內完修及拒絕不應填寫金額
                    //if (Request.QueryString["ifwarrant"] != null)
                    //{  // 是否有過保固
                    //    if (Request.QueryString["ifwarrant"] == "Y")
                    //    {   // 保固內，不應填金額
                    //        mRunScript += "needToCheck=false;";
                    //    }
                    //}
                }
                // [/Form/AmountForm.aspx.cs]Init_Bind()
                GetAmountInfo(vm, formNo);
                // [/Form/TicketInfo.ascx]GetStoreInfo()
                if (ticket_info != "")
                {
                    //mRunScript += "document.all." + ticket_info + "_PANEL.style.display = \"\";\r\n";
                    //mRunScript += "document.all.TICKET_INFO.value = \"" + ticket_info + "\";\r\n";

                    if (ticket_info == "COMPLETE")
                    {   // 完修則取得完成日期
                        //mRunScript += "document.all.AMOUNT_PANEL.style.display = \"\";\r\n";
                        //mRunScript += "document.all.VENDOR_ARRIVE_DATE.value = \"" + db.GetFieldData("to_char(VENDOR_ARRIVE_DATE,'yyyy/mm/dd')", "FTT_FORM", "FORM_NO=" + formNo) + "\";\r\n";
                        if (!string.IsNullOrEmpty(vm.Ftt_formDTO.completetime) && vm.Ftt_formDTO.completetime != "null")
                        {
                            vm.Ftt_formDTO.completetime = DateTime.Parse(vm.Ftt_formDTO.completetime).ToString("yyyy/MM/dd");
                        }
                        //mRunScript += "document.all.COMPLETETIME.value = \"" + db.GetFieldData("to_char(COMPLETETIME,'yyyy/mm/dd')", "FTT_FORM", "FORM_NO=" + formNo) + "\";\r\n";
                    }

                    if (ticket_info == "PENDING")
                    {   // 待料則取得預計完成日期
                        //mRunScript += "document.all.AMOUNT_PANEL.style.display = \"none\";\r\n";
                        if (!string.IsNullOrEmpty(vm.Ftt_formDTO.precompletetime) && vm.Ftt_formDTO.precompletetime != "null")
                        {
                            vm.Ftt_formDTO.precompletetime = DateTime.Parse(vm.Ftt_formDTO.precompletetime).ToString("yyyy/MM/dd");
                        }
                        //mRunScript += "document.all.PRECOMPLETETIME.value = \"" + db.GetFieldData("to_char(PRECOMPLETETIME,'yyyy/mm/dd')", "FTT_FORM", "FORM_NO=" + formNo) + "\";\r\n";
                    }

                    if (ticket_info == "OFFER")
                    {
                        // 報價則取得報價資訊
                        //QTY.Text = db.GetFieldData("QTY", "FTT_FORM", "FORM_NO=" + formNo);
                        //PRICE.Text = db.GetFieldData("PRICE", "FTT_FORM", "FORM_NO=" + formNo);
                        //mRunScript += "document.all.AMOUNT_PANEL.style.display = \"\";\r\n";
                    }

                    // 2008 12 28 Add - 只要金額彙總欄有資料就秀出來
                    if (_PenddingHanlder.CheckDataExist_Ftt_form_amount(formNo) == true)
                    {  // 判斷是否已有顯示
                        vm.ShowAmountPanel = true;
                    }

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="formNo"></param>
        private void GetAmountInfo(FormTableVM vm, string formNo)
        {
            PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

            var fttFormDto = _PenddingHanlder.GetFttFormInfo(formNo);

            vm.Category_Id = fttFormDto.category_id;


            if (_PenddingHanlder.CheckDataExist_FTT_FORM_AMOUNT(formNo) == true)
            {
                vm.Amount_Cost = _PenddingHanlder.GetTotalPrice(formNo);
            }
            else
            {
                vm.Amount_Cost = "0";
            }

            vm.Ftt_form_amountDTOs = _PenddingHanlder.GetList_FttFormAmount(formNo) ?? [];

            if (_PenddingHanlder.CheckDataExist_AMOUNT_SELECT(fttFormDto.category_id) == true)
            {
                vm.UpdateAmount_Config = true;
                vm.Amount_Config = "Y";
                vm.Amount_SelectList = _PenddingHanlder.GetListAMOUNT_SELECT(fttFormDto.category_id).Select(s => new SelectListItem() { Text = s.expense_type, Value = s.expense_type }).ToList();

                //mRunScript += @"$('.input_class').hide();";
            }
            else
            {
                vm.UpdateAmount_Config = false;
                vm.Amount_Config = "N";
                vm.Amount_SelectList = new List<SelectListItem>() { new SelectListItem() { Text = "工資", Value = "工資" }, new SelectListItem() { Text = "材料費", Value = "材料費" } };
            }
        }

        private StoreClass GetStoreInfo(FormTableVM vm, string form_No)
        {
            StoreClass mStoreData = new StoreClass();

            if (!string.IsNullOrEmpty(form_No))
            {
                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);
                var create_time = _PenddingHanlder.GetCreateTime(form_No);

                var mIVRCode = _PenddingHanlder.GetIVRCode(form_No);
                //var mSTOREName = _PenddingHanlder.GetShopName(mIVRCode);
                //_sessionVO.shop_name = mSTOREName;

                mStoreData = new StoreClass(mIVRCode);
                if (mStoreData.hasData() == true)
                {
                    vm.ApprovalDate = mStoreData.ApprovalDate;
                    if (vm.ApprovalDate != "")
                    {
                        vm.ApprovalDate = Convert.ToDateTime(vm.ApprovalDate).ToString("yyyy/MM/dd");
                        vm.WarrantyTime = Convert.ToDateTime(vm.ApprovalDate).AddYears(1).ToString("yyyy/MM/dd");
                        if (System.DateTime.Now > Convert.ToDateTime(vm.ApprovalDate).AddYears(1))
                        {
                            if (mStoreData.Channel == "FRANCHISE")
                                vm.WarrantyTimeForeColor = System.Drawing.Color.Red;
                        }

                        vm.Ifwarrant = false;
                        //mRunScript += "ifwarrant = \"N\";\r\n";
                    }
                }
                //mRunScript += "isNewTT = false;\r\n";
            }
            else if (!string.IsNullOrEmpty(_sessionVO.ivrcode))
            {
                vm.Create_Time = System.DateTime.Now.ToString("yyyy/MM/dd");
                mStoreData = new StoreClass(_sessionVO.ivrcode);
                if (mStoreData.hasData() == true)
                {
                    vm.ApprovalDate = mStoreData.ApprovalDate;
                    if (vm.ApprovalDate != "")
                    {
                        vm.ApprovalDate = Convert.ToDateTime(vm.ApprovalDate).ToString("yyyy/MM/dd");
                        vm.WarrantyTime = Convert.ToDateTime(vm.ApprovalDate).AddYears(1).ToString("yyyy/MM/dd");
                        if (System.DateTime.Now > Convert.ToDateTime(vm.ApprovalDate).AddYears(1))
                        {
                            if (mStoreData.Channel == "FRANCHISE")
                            {
                                vm.WarrantyTimeForeColor = System.Drawing.Color.Red;
                                vm.FranchiseMsg += "除招牌與影音設備外,您報修的品項已超過保固！！";
                            }

                            vm.Ifwarrant = false;
                            //mRunScript += "ifwarrant = \"N\";\r\n";
                        }
                    }
                }
                //mRunScript += "isNewTT = true;\r\n";
            }

            return mStoreData;
        }

        private Ftt_formDTO GetTTInfo(FormTableVM vm, string formNo)
        {
            Ftt_formDTO _Ftt_formDTO = new Ftt_formDTO();
            if (!string.IsNullOrEmpty(formNo))
            {
                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);
                _Ftt_formDTO = _PenddingHanlder.GetFttFormInfo(formNo);

                if (_Ftt_formDTO != null)
                {
                    DateTime tempDt;
                    vm.Create_Time = _Ftt_formDTO.createtime;
                    if (DateTime.TryParse(_Ftt_formDTO.createtime, out tempDt))
                    {
                        vm.Create_Time = tempDt.ToString("yyyy/MM/dd");
                    }


                    if (!string.IsNullOrEmpty(_Ftt_formDTO.CIDesc))
                    {
                        string ciDesc = _Ftt_formDTO.CIDesc.Trim();
                        string filePath = null;
                        string path = null;
                        
                        // Debug log - 記錄原始 CIDesc
                        this.LogError($"[DEBUG] 原始 CIDesc: '{ciDesc}'");
                        
                        // 處理可能的檔案名稱格式
                        string[] possibleNames = {
                            ciDesc,  // 原始名稱
                            ciDesc.Replace("傢俱本體-", ""),  // 移除前綴
                            ciDesc.Split('-').LastOrDefault()?.Trim() ?? ciDesc  // 取最後一段
                        };
                        
                        // 嘗試不同的檔案副檔名 (.jpg, .JPG)
                        string[] extensions = { ".jpg", ".JPG" };
                        
                        foreach (string possibleName in possibleNames)
                        {
                            if (string.IsNullOrEmpty(possibleName)) continue;
                            
                            foreach (string ext in extensions)
                            {
                                filePath = $"Item/{possibleName}{ext}";
                                path = Path.Combine(_hostingEnvironment.WebRootPath, filePath);
                                
                                this.LogError($"[DEBUG] 檢查檔案: '{path}', 存在: {System.IO.File.Exists(path)}");
                                
                                if (System.IO.File.Exists(path))
                                {
                                    vm.hasTT_IMAGE = true;
                                    vm.newImageSRC = filePath;
                                    this.LogError($"[DEBUG] 找到圖片檔案: '{filePath}'");
                                    goto FoundImage;
                                }
                            }
                        }
                        
                        FoundImage:
                        if (!vm.hasTT_IMAGE)
                        {
                            this.LogError($"[DEBUG] 未找到任何圖片檔案，原始 CIDesc: '{ciDesc}'");
                        }
                    }

                    //mRunScript += "setCATEGORY_LABEL(\"" + _Ftt_formDTO.category_id + "\"); \r\n";
                    // 廠商無顯示「門市自行尋商」(一律為 "N")
                    //if (_Ftt_formDTO.selfconfig == "N")
                    //{
                    //    //mRunScript += "showVenderInfo(\"" + _Ftt_formDTO.order_id + "\",\"" + _Ftt_formDTO.merchant_name + "\",\"" + _Ftt_formDTO.cp_name + "\",\"" + _Ftt_formDTO.cp_tel + "\");\r\n";
                    //}
                    //else
                    //{
                    //    var storeName = _PenddingHanlder.GetShopName(_Ftt_formDTO.ivrcode);
                    //    vm.storename = storeName;
                    //}
                }
            }

            return _Ftt_formDTO;
        }

        /// <summary>
        /// 處理表單權限<para/>
        /// [App_Code/Approve.cs]Form_Auth()
        /// </summary>
        private void HandleFormAuth(FormTableVM vm, string formType, string formNo, string tStatus, string preStatus, string ivrCode)
        {
            ivrCode ??= string.Empty;
            string submitButton = string.Empty;
            string updateField = string.Empty;
            string requireField = string.Empty;
            string role = string.Empty;
            string status = string.Empty;
            form_access_controlSQL _form_access_controlSQL = new();
            var dtoList = _form_access_controlSQL.GetInfoList(ivrCode, formType, tStatus, formNo, _sessionVO.empno);

            /* 判斷user對此SR所擁有的權限, 在此會記錄
             * RequireField, OptionField, SubmittBotton 此三項資料
             * 之後會根據這三項, 建立表單權限
             */
            vm.FormTableButtons = [];
            foreach (form_access_controlDTO dto in dtoList)
            {
                updateField += $"{dto.option_field},{dto.require_field},";
                role += dto.User_Type + ",";
                requireField += dto.require_field + ",";

                List<string> allowStatusList = string.IsNullOrWhiteSpace(dto.allow_status)
                    ? [] : dto.allow_status.Split(',').ToList();
                List<string> allowWordingList = string.IsNullOrWhiteSpace(dto.allow_wording)
                    ? [] : dto.allow_wording.Split(',').ToList();

                if (dto.approve == "Y" && !vm.FormTableButtons.Any(x => x.IsApproveCommon))
                {
                    vm.FormTableButtons.Add(new FormTableButtonVO
                    {
                        IsApproveCommon = true,
                    });

                    //在此保留原邏輯用來判斷，不拋到前端
                    //submitButton = "<font id='approvecommon' STYLE='FONT: bold 9pt Arial; COLOR: #000080; TEXT-DECORATION: none;'>建議／說明</font>：<input type=text name=approvecommon maxlength=200 size=80>" + submitButton;
                }

                for (int j = 0; j < allowStatusList.Count; j++)
                {
                    string allowStatus = allowStatusList[j];
                    string allowWording = j < allowWordingList.Count ? allowWordingList[j] : string.Empty;
                    // || Status.Contains("value='" + PreStatus + "'>") 條件永遠不會觸發
                    if (!(vm.FormTableButtons.Any(x => x.Status == allowStatus)
                        || string.IsNullOrWhiteSpace(allowWording))
                        )
                    {
                        if (allowStatus == "PRIOR_STATUS")
                        {
                            vm.FormTableButtons.Add(new FormTableButtonVO
                            {
                                StatusWording = allowWording,
                                Status = preStatus,
                                RequireField = dto.require_field,
                                FormType = dto.form_type,
                            });
                        }
                        else if (ivrCode.Length <= 7 && allowStatus != "USED")
                        {// allowStatus != "USED" 的條件僅廠商版本有
                            if (dto.approve == "Y")
                            {
                                vm.FormTableButtons.Add(new FormTableButtonVO
                                {
                                    StatusWording = allowWording,
                                    Status = allowStatus,
                                    RequireField = dto.require_field,
                                    FormType = dto.form_type,
                                    Approve = "Y",
                                    UserType = dto.usertype,
                                });
                            }
                            else
                            {
                                vm.FormTableButtons.Add(new FormTableButtonVO
                                {
                                    StatusWording = allowWording,
                                    Status = allowStatus,
                                    RequireField = dto.require_field,
                                    FormType = dto.form_type,
                                });
                            }
                        }
                    }
                }
            }

            vm.User_Type = role;
            vm.UpdateField = updateField;
            vm.RequireField = requireField;
        }

        /// <summary>
        /// 金額彙整欄送出
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>

        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public ActionResult Add_Ftt_form_amount(Add_Ftt_form_amount_VM vm)
        {
            if (vm != null && !string.IsNullOrEmpty(vm.FORM_ACTION))
            {
                if (vm.FORM_ACTION == "INSERT")
                {
                    try
                    {
                        string inserSQL = "";

                        PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);

                        _PenddingHanlder.DeleteFttFormAmount(vm.form_no);
                        foreach (var item in vm.vms)
                        {
                            _PenddingHanlder.InsertFttFormAmount(int.Parse(vm.form_no), item.expense_type, item.expense_desc, item.qty, item.price, item.subtotal, item.orderid, item.unit, item.fault_reason, item.repair_action);
                        }

                        _PenddingHanlder.Commit();

                        this.LogError("新增完成");
                        return JsonSuccess("新增完成");
                    }
                    catch (Exception ex)
                    {
                        this.LogError(ex.ToString());
                        return JsonValidFail("系統異常");
                    }
                    finally
                    {

                    }
                }

                this.LogSuccess();
                return JsonValidFail("不是INSERT方法");
            }

            return JsonValidFail("工單不存在");
        }

        /// <summary>
        /// 表單送出
        /// </summary>
        /// <returns></returns>

        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult Detail(Ftt_formDTO vm)
        {
            try
            {

                var ttt = vm.form_no;

                BaseDBHandler baseHandler = new BaseDBHandler();
                CommonHandler commonHandler = new CommonHandler(_config, baseHandler.GetDBHelper());

                string updateSQL = "  ";

                Dictionary<string, object> dic = new();
                dic.Add("category_id", decimal.Parse(vm.category_id));
                dic.Add("category_name", vm.category_name);

                dic.Add("ticket_info", vm.ticket_info);


                //dic.Add("selfconfig", vm.selfconfig);
                dic.Add("remark", vm.remark);

                dic.Add("form_no", vm.form_no);
                DateTime tempTime = DateTime.MinValue;
                if (vm.updateCOMPLETETIME == true)
                {
                    if (DateTime.TryParse(vm.completetime, out tempTime) == false)
                    {
                        this.LogSuccess();
                        return JsonValidFail("完成日期格式錯誤");
                    }
                    updateSQL += " completetime=@completetime, ";
                    dic.Add("completetime", tempTime);
                }

                if (vm.updatePRECOMPLETETIME == true)
                {
                    if (DateTime.TryParse(vm.precompletetime, out tempTime) == false)
                    {
                        return JsonValidFail("預計完成日期格式錯誤");
                    }
                    updateSQL += " precompletetime=@precompletetime, ";
                    dic.Add("precompletetime", tempTime);
                }

                if (vm.updateVENDOR_ARRIVE_DATE == true)
                {
                    if (DateTime.TryParse(vm.vendor_arrive_date, out tempTime) == false)
                    {
                        this.LogSuccess();
                        return JsonValidFail("到場日期格式錯誤");
                    }
                    updateSQL += " vendor_arrive_date=@vendor_arrive_date, ";
                    dic.Add("vendor_arrive_date", tempTime);
                }

                if (!string.IsNullOrEmpty(vm.delay_reason))
                {
                    updateSQL += " delay_reason=@delay_reason, ";
                    dic.Add("delay_reason", vm.delay_reason);
                }

                //先取得當下的狀態
                var oldEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                baseHandler.GetDBHelper().Execute($@" 
update ftt_form set 
ticket_info=@ticket_info,

category_id=@category_id,
category_name=@category_name,
{updateSQL}


remark=@remark

where 
form_no=@form_no
", dic);


                Dictionary<string, object> dic2 = new();
                dic2.Add("form_no", int.Parse(vm.form_no));
                dic2.Add("DESCRIPTION", vm.DESCRIPTION);
                dic2.Add("action_name", $@"{_sessionVO.empname}({_sessionVO.engname})");
                dic2.Add("status", vm.STATUS);

                string sql = @"INSERT INTO ftt_form_desc 
                       (form_no,user_type,action_name,description,prior_status,status) 
                       VALUES (@form_no,'',@action_name,@description,null,@status)";

                baseHandler.GetDBHelper().Execute(sql, dic2);
                baseHandler.GetDBHelper().Commit();

                //                baseHandler.GetDBHelper().Execute($@" 
                //update FTT_FORM_DESC set 

                //DESCRIPTION=@DESCRIPTION,
                //action_name=@action_name

                //where 
                //form_no=@form_no
                //", dic2);

                commonHandler.ExecSetStatus(vm.FORM_TYPE, int.Parse(vm.form_no), vm.STATUS, _sessionVO.empno);
                baseHandler.GetDBHelper().Commit();

                baseHandler.GetDBHelper().ExecStoredProcedureWithTransation("SET_STATUS('" + vm.FORM_TYPE + "','" + vm.form_no + "','" + vm.STATUS + "','" + _sessionVO.empno + "','','')");

                //取得更新完的狀態
                var newEntity = baseHandler.GetDBHelper().Find<approve_formEntity>("select * from approve_form where form_no=@form_no ", dic);

                if (newEntity != null && oldEntity != null)
                {
                    MailPoolHandler _MailPoolHandler = new MailPoolHandler();
                    var result = Method.CreateMailPool(vm.form_no, oldEntity.status, newEntity.status, _MailPoolHandler);
                    if (!string.IsNullOrEmpty(result))
                    {
                        this.LogError("CreateMailPool 執行失敗");
                        this.LogSuccess("申請單單號【" + vm.form_no + "】更新成功！");
                        return JsonSuccess("申請單單號【" + vm.form_no + "】更新成功！");
                    }
                }

                ////TODO 不知道甚麼時候未有 APPROVE="Y" 的參數
                //if (Request.QueryString["APPROVE"] == "Y")
                //{
                //    db.ExecuteNonQuery(tran, "INSERT INTO APPROVE_FORM_LOG (FORM_TYPE,FORM_NO,User_Type,STATUS,AGENT,COMMON,ROOT_NO) VALUES ('" + m_Request["FORM_TYPE"] + "','" + m_Request["FORM_NO"] + "','" + m_Request["User_Type"] + "','" + m_Request["STATUSWORDING"] + "','" + Context.User.Identity.Name + "','" + m_Request["APPROVECOMMON"].Replace("'", "’").ToString() + "'),");
                //}

                this.LogSuccess();
                return JsonSuccess("申請單單號【" + vm.form_no + "】更新成功！");
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統異常");
            }
        }

        private string GetPreHandleDesc(string mFormAction, string formNo, string mCIID, string mCIName)
        {
            //string mFormAction = "TT_LAST_DESC";
            string mResult = "";
            //string mCIID = "";
            //string mCIName = "";
            //string mFormNo = "";

            //if (Request.QueryString["CIID"] != null)
            //    mCIID = Server.HtmlEncode(Request.QueryString["CIID"].ToString());
            //if (Request.QueryString["CIName"] != null)
            //    mCIName = Server.HtmlEncode(Request.QueryString["CIName"].ToString());
            //if (Request.QueryString["FORM_NO"] != null)
            //    mFormNo = Server.HtmlEncode(Request.QueryString["FORM_NO"].ToString());

            if (mFormAction != "")
            {
                ci_relations_categorySQL _ci_relations_categorySQL = new ci_relations_categorySQL();
                var dtoTEMP = _ci_relations_categorySQL.GetInfoByCISID(mCIID);
                switch (mFormAction)
                {
                    case "TT_IMAGE":
                        string ciName = mCIName.Trim();
                        
                        this.LogError($"[DEBUG GetPreHandleDesc] 原始 ciName: '{ciName}'");
                        
                        // 處理可能的檔案名稱格式
                        string[] possibleNames = {
                            ciName,  // 原始名稱
                            ciName.Replace("傢俱本體-", ""),  // 移除前綴
                            ciName.Split('-').LastOrDefault()?.Trim() ?? ciName  // 取最後一段
                        };
                        
                        string[] extensions = { ".jpg", ".JPG" };
                        bool fileFound = false;
                        
                        foreach (string possibleName in possibleNames)
                        {
                            if (string.IsNullOrEmpty(possibleName)) continue;
                            
                            foreach (string ext in extensions)
                            {
                                string testPath = Path.Combine(_hostingEnvironment.WebRootPath, "Item/" + possibleName + ext);
                                this.LogError($"[DEBUG GetPreHandleDesc] 檢查檔案: '{testPath}', 存在: {System.IO.File.Exists(testPath)}");
                                
                                if (System.IO.File.Exists(testPath))
                                {
                                    mResult = "/Item/" + possibleName + ext;
                                    fileFound = true;
                                    this.LogError($"[DEBUG GetPreHandleDesc] 找到圖片: '{mResult}'");
                                    goto FoundImagePreHandle;
                                }
                            }
                        }
                        
                        FoundImagePreHandle:
                        if (!fileFound)
                        {
                            mResult = "/Item/no-product.gif";
                            this.LogError($"[DEBUG GetPreHandleDesc] 使用預設圖片: '{mResult}'");
                        }
                        break;
                    case "TT_CATEGORY_NOTE":
                        mResult = dtoTEMP.notes;
                        break;
                    case "TT_CATEGORY_DESC":
                        mResult = dtoTEMP.descr;
                        break;
                    case "TT_LAST_DESC":
                        ftt_form_descSQL _ftt_form_descSQL = new ftt_form_descSQL();
                        var dtoTemp = _ftt_form_descSQL.GetInfoByFormNo(formNo);
                        if (dtoTemp != null)
                        {
                            if (!string.IsNullOrEmpty(dtoTemp.description))
                                mResult = $"{DateTime.Parse(dtoTemp.create_date).ToString("yyyy/MM/dd HH:mm")}-{dtoTemp.action_name}【{dtoTemp.description}】";

                            //mResult = "'<img src=\"/images/icon/date.gif\" align=\"absmiddle\" />'" + dtoTemp.create_date.Value.ToString("yyyy/MM/dd HH:mm") + "'&nbsp;&nbsp;&nbsp;<img src=\"/images/icon/emp.gif\" align=\"absmiddle\" />' " + dtoTemp.action_name + " '&nbsp;&nbsp;&nbsp;<img src=\"/images/icon/edit.gif\" align=\"absmiddle\" />' " + dtoTemp.description;
                        }
                        break;
                    case "TT_COUNT":
                        ftt_formSQL _ftt_formSQL = new ftt_formSQL();
                        var dtoftt_form = _ftt_formSQL.GetTT_COUNTByCATEGORY_ID(formNo, mCIID);
                        mResult = dtoftt_form.TT_COUNT;
                        break;
                    case "TT_CATEGORY_SELFCONFIG":
                        mResult = dtoTEMP.selfconfig;
                        break;
                    default:
                        break;
                }

                return mResult;
            }

            return "";
        }

        public class TemplateConfigVM
        {
            public string ID { get; set; }
            public string UNIT { get; set; }
            public string QTY { get; set; }
            public string PRICE { get; set; }
            public string REMARK { get; set; }
        }
        public class Add_Ftt_form_amount_VM
        {
            public List<Ftt_form_amountDTO> vms { get; set; }
            public string FORM_ACTION { get; set; }
            public string form_no { get; set; }
        }
        public class SelectDescVM
        {
            public string categoryID { get; set; }
            public string ExpenseType { get; set; }
        }


        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public ActionResult SelectDesc(SelectDescVM vm)
        {
            string ip = Method.GetClientIPAddress();
            try
            {
                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);
                var list = _PenddingHanlder.GetAmountSelectInfo(vm.categoryID, vm.ExpenseType);
                var selectLists = list.Select(s => new SelectListItem() { Text = s.dataValue, Value = s.id.ToString() }).ToList();

                this.LogSuccess();
                return JsonOK(selectLists);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統發生異常");
            }
        }


        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public ActionResult ShowDesc(TemplateConfigVM vm)
        {
            try
            {
                PendingHanlder _PenddingHanlder = new PendingHanlder(_config, HttpContext);
                var list = _PenddingHanlder.GetAmountSelectInfoById(vm.ID);
                var result = list.Select(s => new TemplateConfigVM()
                {
                    ID = s.id.ToString(),
                    UNIT = s.unit,
                    QTY = s.qty.ToString(),
                    PRICE = s.price.ToString(),
                    REMARK = s.remark
                }).ToList();

                this.LogSuccess();
                return JsonOK(result);
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統發生異常");
            }
        }


        [ValidateAntiForgeryToken]
        [HttpPost("[action]")]
        public IActionResult FileUpload(IFormFile file, string formNo)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return JsonValidFail("未選擇檔案");
                }

                //紀錄舊檔名
                var originFileName = file.FileName;
                //新檔案名稱
                var newFileName = _sessionVO.empno + "_" + DateTime.Now.ToString("HHmmss") + "_" + file.FileName;
                //檔案路徑
                string destFilePath = _config.Config["OutputPath"] + System.IO.Path.GetFileName(_config.Config["OutputPath"] + newFileName);

                // 檢查資料夾是否存在
                if (!Directory.Exists(_config.Config["OutputPath"]))
                {
                    Directory.CreateDirectory(_config.Config["OutputPath"]);
                }

                // 儲存檔案
                using (var stream = new FileStream(destFilePath, FileMode.Create))
                {
                    //file.CopyToAsync(stream); 非同步導致檔案是 0 kb 時就被使用
                    file.CopyTo(stream);
                }

                DateTime dtNow = DateTime.Now;

                var fileOriginName = Path.GetFileName(file.FileName);
                var fileExt = Path.GetExtension(fileOriginName);

                //存至TABLE
                FileHandler _FileHandler = new FileHandler();

                int fileId = 0;

                var msg = _FileHandler.Insert(new FileEntity()
                {
                    destfilepath = destFilePath,
                    Status = "1",
                    createtime = dtNow,
                    creator = _sessionVO.empno,
                    filename = originFileName,
                    filesize = file.Length,
                    updater = _sessionVO.empno,
                    updatetime = dtNow,
                    fileext = fileExt,
                },
                formNo,
                out fileId
                );


                if (string.IsNullOrEmpty(msg) == false)
                {
                    this.LogError(msg);
                    return JsonValidFail("新增檔案時發生異常");
                }
                else
                {
                    this.LogSuccess("新增檔案成功");
                    return JsonSuccess(new SelectListItem() { Text = originFileName, Value = fileId.ToString() }); ;
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }



        [ValidateAntiForgeryToken]
        [HttpGet("[action]")]
        public IActionResult DownloadFile(string id)
        {
            try
            {
                FileHandler _FileHandler = new FileHandler();
                var dto = _FileHandler.FindById(id);
                if (dto != null)
                {
                    var filePath = dto.destfilepath;

                    if (!System.IO.File.Exists(filePath))
                        return NotFound("檔案不存在");

                    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(filePath, out var contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    return File(stream, contentType, dto.filename);
                }
                else
                {
                    return JsonValidFail("查無檔案");
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex.ToString());
                return JsonValidFail("系統錯誤");
            }
        }
    }
}