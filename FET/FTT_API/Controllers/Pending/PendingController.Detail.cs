
using Core.Utility.Enums;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models;
using FTT_API.Models.Handler;
using FTT_API.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph.Models;
using Npgsql;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.POIFS.Crypt.Agile;
using System.Data;
using System.Data.Common;

namespace FTT_API.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        //protected string mIVRCode = "";
        //protected string SubmitButton = "";
        //protected string Role = "";
        //protected string UpdateField = "";
        //protected string RequireField = "";
        //protected string ActionName = "";

        [HttpGet("[action]")]
        public IActionResult Detail(string formNo)
        {
            FormTableVM vm = new FormTableVM();

            ViewData["FORM_NO"] = formNo;
            var ActionName = "";
            if (LoginSession.Current.empname != LoginSession.Current.engname)
                ActionName = LoginSession.Current.empname + "(" + LoginSession.Current.engname + ")";
            else
                ActionName = LoginSession.Current.empname;

            ViewData["ACTION_NAME"] = ActionName;


            var funcId = WebMethod.SetFuncIdAndClassName(ViewData, HttpContext.Request);

            ftt_formSQL _ftt_formSQL = new ftt_formSQL();
            var dto = _ftt_formSQL.GetInfoByFormNo(formNo);

            if (dto == null)
            {
                return RedirectToAction("Redirection", "AlertMsg", new AlertMsgRedirection()
                {
                    ParasJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { funcId = funcId }),
                    ActionName = "index",
                    ControllerName = "Pending",
                    ClassName = WebMethod.GetClassName(HttpContext.Request),
                    Msgs = new List<string>()
                {
                    "表單編號不存在"
                },
                    AlertType = "info"
                });
            }

            TicketInfoVM _TicketInfo = new TicketInfoVM();

            approve_formSQL _approve_formSQL = new approve_formSQL();
            var approve_form = _approve_formSQL.GetInfoByFormNo(formNo);
            if (approve_form != null)
            {
                if (approve_form.form_type != "")
                {
                    Approve Approve_Auth = new Approve(LoginSession.Current.empno);
                    string[] UserAuth = Approve_Auth.Form_Auth(vm, approve_form.form_type, formNo, approve_form.status, approve_form.form_type + "_PRIOR_STATUS", LoginSession.Current.ivrcode);
                    string SubmitButton = UserAuth[0];
                    string Role = UserAuth[1];
                    string UpdateField = UserAuth[2];
                    string RequireField = UserAuth[3];

                    ViewData["Role"] = Role;
                    ViewData["UpdateField"] = UpdateField;
                    ViewData["RequireField"] = RequireField;

                    ViewData["SubmitButton"] = SubmitButton;

                    // 當狀態為 [已派單] 不使用按鈕，使用下拉式選單選擇狀態與顯示相對應的控制項

                    ViewData["TicketPanelVisible"] = true;
                    _TicketInfo.TTNo = formNo;
                    _TicketInfo.TTStatus = approve_form.status;
                    _TicketInfo.ShowTicketInfo = dto.ticket_info;
                    _TicketInfo.TTType = approve_form.form_type;

                    ViewData["TicketInfo"] = _TicketInfo;
                }

                ViewData["STATUS_NAME"] = approve_form.STATUS_NAME;


                GetTicketInfo(vm, formNo, _TicketInfo.TTType, _TicketInfo.TTStatus, _TicketInfo.ShowTicketInfo);

                if (!string.IsNullOrEmpty(formNo))
                {
                    if (!string.IsNullOrEmpty(_TicketInfo.TTStatus))
                    {   // 判斷狀態是否有傳入
                        string TTStatus = _TicketInfo.TTStatus;
                        //string TTCategory = _TicketInfo.TTCategory; 沒使用

                        if (TTStatus != "TICKET")
                        {   // 如果不是已派單，則顯示資料！

                            Ftt_form_amountSQL _Ftt_form_amountSQL = new Ftt_form_amountSQL();
                            var totalPrice = _Ftt_form_amountSQL.GetTotalPrice(formNo);
                            ViewData["Totals"] = totalPrice;

                            ViewData["Hide_dele"] = true;
                        }

                        // needToCheck 沒被使用到 直接註解
                        //// 2008 12 28 Add - 廠商保固期內完修及拒絕不應填寫金額
                        //if (Request.QueryString["ifwarrant"] != null)
                        //{  // 是否有過保固
                        //    if (Request.QueryString["ifwarrant"] == "Y")
                        //    {   // 保固內，不應填金額
                        //        mRunScript += "needToCheck=false;";
                        //    }
                        //}
                    }

                    Init_Bind(formNo);
                }
                else
                {
                    return RedirectToAction("Redirection", "AlertMsg", new AlertMsgRedirection()
                    {
                        ParasJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { funcId = funcId }),
                        ActionName = "index",
                        ControllerName = "Pending",
                        ClassName = WebMethod.GetClassName(HttpContext.Request),
                        Msgs = new List<string>()
                {
                    "表單編號不正確"
                },
                        AlertType = "info"
                    });
                }
            }

            //vm.Ftt_formDTO = GetTTInfo(formNo);
            //vm.Store_profileDTO = GetStoreInfo(formNo);

            if (vm.Ftt_formDTO == null)
            {
                return RedirectToAction("Redirection", "AlertMsg", new AlertMsgRedirection()
                {
                    ParasJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { funcId = funcId }),
                    ActionName = "index",
                    ControllerName = "Pending",
                    ClassName = WebMethod.GetClassName(HttpContext.Request),
                    Msgs = new List<string>()
                {
                    "查無開單者資訊"
                },
                    AlertType = "info"
                });
            }

            if (vm.Store_profileDTO == null)
            {
                return RedirectToAction("Redirection", "AlertMsg", new AlertMsgRedirection()
                {
                    ParasJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { funcId = funcId }),
                    ActionName = "index",
                    ControllerName = "Pending",
                    ClassName = WebMethod.GetClassName(HttpContext.Request),
                    Msgs = new List<string>()
                {
                    "查無門市資訊"
                },
                    AlertType = "info"
                });
            }

            ViewData["PreHandleDesc"] = GetPreHandleDesc("TT_LAST_DESC", formNo, "", "");

            return View(vm);
        }

        private void Init_Bind(string formNo)
        {


            bool showEXPENSE_DESC_TEXT = true;
            bool showEXPENSE_DESC_SEL = true;

            ftt_formSQL _ftt_formSQL = new ftt_formSQL();
            var dto = _ftt_formSQL.GetInfoByFormNo(formNo);


            ViewData["CATEGORY_ID"] = dto.category_id;

            Ftt_form_amountSQL _Ftt_form_amountSQL = new Ftt_form_amountSQL();
            var dto_Ftt_form_amountSQL = _Ftt_form_amountSQL.GetInfoByFormNo_ENABLE_Y(formNo);

            if (dto_Ftt_form_amountSQL != null)
            {
                var total = _Ftt_form_amountSQL.GetTotalPrice(formNo);
                var _AMOUNT_COST = total;
                ViewData["AMOUNT_COST"] = _AMOUNT_COST;
                ViewData["Totals"] = _AMOUNT_COST;
            }
            else
            {
                var _AMOUNT_COST = 0;
                ViewData["AMOUNT_COST"] = _AMOUNT_COST;
                ViewData["Totals"] = _AMOUNT_COST;
            }

            var dtos = _Ftt_form_amountSQL.GetListByFormNo(formNo);
            ViewData["Ftt_form_amountDTOs"] = dtos;

            amount_selectSQL _amount_selectSQL = new amount_selectSQL();
            var dto_amount_select = _amount_selectSQL.GetInfoByCategory_Id(dto.category_id);

            if (dto_amount_select != null)
            {
                ViewData["AMOUNT_CONFIG"] = "Y";

                var dtos2 = _amount_selectSQL.GetExpenseTypeListByCategoryId(formNo);
                var EXPENSE_TYPE_List = dtos2.Select(s => new SelectListItem() { Text = s.expense_type, Value = s.expense_type }).ToList();
                ViewData["EXPENSE_TYPE_List"] = EXPENSE_TYPE_List;

                showEXPENSE_DESC_TEXT = false;
            }
            else
            {
                ViewData["AMOUNT_CONFIG"] = "N";

                var EXPENSE_TYPE_List = new List<SelectListItem>() {
                    new SelectListItem() {Text="",Value=""} ,
                    new SelectListItem() {Text="工資",Value="工資"} ,
                    new SelectListItem() {Text="材料費",Value="材料費"} ,
                };
                ViewData["EXPENSE_TYPE_List"] = EXPENSE_TYPE_List;

                showEXPENSE_DESC_SEL = false;
            }

            ViewData["showEXPENSE_DESC_SEL"] = showEXPENSE_DESC_SEL;
            ViewData["showEXPENSE_DESC_TEXT"] = showEXPENSE_DESC_TEXT;
        }

        //private void GetTicketInfo(string formNo, string mTTType, string TTStatus, string mTicketInfo)
        //{
        //    string tempStr = "";
        //    string mRunScript = "";
        //    if (formNo != "")
        //    {
        //        bool disableOffer = true;  // 是否為保固內


        //        store_profileSQL _store_profileSQL = new store_profileSQL();
        //        var dto_store = _store_profileSQL.GetInfoByFormNo(formNo);

        //        if (dto_store != null)
        //            if (dto_store.store_type.ToUpper().IndexOf("FRANCHISE") == -1)
        //            {   // 為直營店

        //                ci_exception_configSQL _ci_exception_configSQL = new ci_exception_configSQL();
        //                var dto_ci_exception_config = _ci_exception_configSQL.GetInfoByFormNo(formNo);

        //                if (dto_ci_exception_config == null)
        //                {
        //                    var dtoTemp = _store_profileSQL.GetListByFormNoDate365(formNo);
        //                    if (dtoTemp == null)
        //                    {  // 保固外
        //                        disableOffer = false;
        //                    }
        //                }
        //            }
        //            else //12/1新需求，加盟[影音/招牌]也要報價
        //            {
        //                var dtoTemp = _store_profileSQL.GetListByFormNoNotIn1278And1260(formNo);
        //                if (dtoTemp == null)
        //                {
        //                    mRunScript += "//is FRANCHISE\r\n";
        //                    disableOffer = false;
        //                }

        //            }

        //        {
        //            mRunScript += "document.all.SELECTSTATUS.options[2].text='故障排除';";
        //        }

        //        if (disableOffer == true)
        //        {
        //            mRunScript += "document.all.SELECTSTATUS.remove(3);\r\n";
        //        }
        //        bool mShowAmount = true;
        //        if (mTTType == "SECURITY_FORM")
        //        {
        //            mShowAmount = false;
        //        }
        //        ViewData["mShowAmount"] = mShowAmount;

        //        if (TTStatus == "TICKET")
        //        {
        //            mRunScript += "document.all.SELECTSTATUS_PANEL.style.display = \"\";\r\n";

        //            ftt_form_logSQL _ftt_form_logSQL = new ftt_form_logSQL();
        //            var dtoTemp = _ftt_form_logSQL.GetInfoByFormNo(formNo, "STATUS", "AGREE", "ASSIGN");
        //            if (dtoTemp != null)
        //            {
        //                dtoTemp = _ftt_form_logSQL.GetInfoByFormNo(formNo, "STATUS", "ASSIGN", "PRWP");
        //                if (dtoTemp != null)
        //                {
        //                    dtoTemp = _ftt_form_logSQL.GetInfoByFormNo(formNo, "STATUS", "PRWP", "TICKET");
        //                    if (dtoTemp != null)
        //                    {
        //                        mRunScript += "document.all.SELECTSTATUS.remove(2);\r\ndocument.all.SELECTSTATUS.remove(2);\r\ndocument.all.SELECTSTATUS.remove(2);\r\n";
        //                    }
        //                }
        //            }
        //        }

        //        if (mTicketInfo != "")
        //        {
        //            mRunScript += "document.all." + mTicketInfo + "_PANEL.style.display = \"\";\r\n";
        //            mRunScript += "document.all.TICKET_INFO.value = \"" + mTicketInfo + "\";\r\n";

        //            ftt_formSQL _ftt_formSQL = new ftt_formSQL();
        //            var dtoFttFrom = _ftt_formSQL.GetInfoByFormNo(formNo);

        //            if (mTicketInfo == "COMPLETE")
        //            {   // 完修則取得完成日期
        //                mRunScript += "document.all.AMOUNT_PANEL.style.display = \"\";\r\n";
        //                //mRunScript += "document.all.VENDOR_ARRIVE_DATE.value = \"" + baseHandler.GetDBHelper().FindScalar<string>("to_char(VENDOR_ARRIVE_DATE,'yyyy/mm/dd')", "FTT_FORM", "FORM_NO=" + mTTNo) + "\";\r\n";
        //                mRunScript += "document.all.COMPLETETIME.value = \"" + dtoFttFrom.completetime + "\";\r\n";
        //            }

        //            if (mTicketInfo == "PENDING")
        //            {   // 待料則取得預計完成日期
        //                mRunScript += "document.all.AMOUNT_PANEL.style.display = \"none\";\r\n";
        //                mRunScript += "document.all.PRECOMPLETETIME.value = \"" + dtoFttFrom.precompletetime + "\";\r\n";
        //            }

        //            if (mTicketInfo == "OFFER")
        //            {   // 報價則取得報價資訊
        //                mRunScript += "document.all.AMOUNT_PANEL.style.display = \"\";\r\n";
        //            }

        //            Ftt_form_amountSQL _Ftt_form_amountSQL = new Ftt_form_amountSQL();
        //            var dtoTEMP = _Ftt_form_amountSQL.GetInfoByFormNo_ENABLE_Y(formNo);

        //            // 2008 12 28 Add - 只要金額彙總欄有資料就秀出來
        //            if (dtoTEMP != null)
        //            {  // 判斷是否已有顯示
        //                if (mRunScript.IndexOf("document.all.AMOUNT_PANEL.style.display = \"\";") == -1)
        //                    mRunScript += "document.all.AMOUNT_PANEL.style.display = \"\";\r\n";
        //            }
        //        }
        //    }
        //}
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
                        mCIName = Path.Combine(_hostingEnvironment.WebRootPath, "Images/Item/" + mCIName + ".jpg");
                        if (System.IO.File.Exists(Path.Combine(_hostingEnvironment.WebRootPath, "Images/Item/" + mCIName + ".jpg")) == true)
                            mResult = "/Images/Item/" + mCIName;
                        else
                            mResult = "/images/item/no-product.gif";
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
                            mResult = "'<img src=\"/images/icon/date.gif\" align=\"absmiddle\" />'" + dtoTemp.create_date.Value.ToString("yyyy/MM/dd HH:mm") + "'&nbsp;&nbsp;&nbsp;<img src=\"/images/icon/emp.gif\" align=\"absmiddle\" />' " + dtoTemp.action_name + " '&nbsp;&nbsp;&nbsp;<img src=\"/images/icon/edit.gif\" align=\"absmiddle\" />' " + dtoTemp.description;
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



        public class SelectDescVM
        {
            public string categoryID { get; set; }
            public string ExpenseType { get; set; }
        }

        [HttpPost("[action]")]
        public ActionResult SelectDesc(SelectDescVM vm)
        {
            string ip = Method.GetClientIPAddress();
            try
            {
                string sSQL = string.Format("SELECT DISTINCT ID, DECODE(L2_DESC,NULL,L1_DESC,L1_DESC || '-' || L2_DESC) as dataValue FROM AMOUNT_SELECT WHERE ENABLE='Y' AND CHK_CI_LIST('{0}'::text,category_id::text)='Y' AND EXPENSE_TYPE='{1}' ORDER BY ID", vm.categoryID, vm.ExpenseType);

                BaseDBHandler baseHandler = new BaseDBHandler();
                DataTable m_table = baseHandler.GetDBHelper().FindDataTable(sSQL, null);

                var selectLists = new List<SelectListItem>();
                for (int i = 0; i < m_table.Rows.Count; i++)
                {
                    selectLists.Add(new SelectListItem() { Text = m_table.Rows[i]["dataValue"].ToString(), Value = m_table.Rows[i]["ID"].ToString() });
                }

                return JsonOK(selectLists);
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統發生異常");
            }
        }

        public class TemplateConfigVM
        {
            public string ID { get; set; }
            public string UNIT { get; set; }
            public string QTY { get; set; }
            public string PRICE { get; set; }
            public string REMARK { get; set; }
        }

        [HttpPost("[action]")]
        public ActionResult ShowDesc(TemplateConfigVM vm)
        {
            try
            {
                List<TemplateConfigVM> result = new List<TemplateConfigVM>();
                string sSQL = string.Format("SELECT ID, UNIT, QTY, PRICE, REMARK FROM AMOUNT_SELECT WHERE ENABLE='Y' AND ID={0}", vm.ID);

                BaseDBHandler baseHandler = new BaseDBHandler();
                DataTable m_table = baseHandler.GetDBHelper().FindDataTable(sSQL, null);

                for (int i = 0; i < m_table.Rows.Count; i++)
                {
                    TemplateConfigVM config = new TemplateConfigVM();
                    config.ID = m_table.Rows[i]["ID"].ToString();
                    config.UNIT = m_table.Rows[i]["UNIT"].ToString();
                    config.QTY = m_table.Rows[i]["QTY"].ToString();
                    config.PRICE = m_table.Rows[i]["PRICE"].ToString();
                    config.REMARK = m_table.Rows[i]["REMARK"].ToString();
                    result.Add(config);
                }

                return JsonOK(result);
            }
            catch (Exception ex)
            {
                return JsonValidFail("系統發生異常");
            }
        }

        public class Add_Ftt_form_amount_VM
        {
            public List<Ftt_form_amountDTO> vms { get; set; }
            public string FORM_ACTION { get; set; }
            public string form_no { get; set; }
        }

        //[HttpPost("[action]")]
        //public ActionResult Add_Ftt_form_amount(Add_Ftt_form_amount_VM vm)
        //{
        //    //m_Logger.Debug("FORM_ACTION：" + Request.QueryString["FORM_ACTION"]);

        //    if (vm != null && !string.IsNullOrEmpty(vm.FORM_ACTION))
        //    {
        //        if (vm.FORM_ACTION == "INSERT")
        //        {
        //            try
        //            {
        //                string inserSQL = "";
        //                BaseDBHandler baseHandler = new BaseDBHandler();
        //                baseHandler.GetDBHelper().Execute("DELETE FROM Ftt_form_amount WHERE FORM_NO = '" + vm.form_no + "'", null);

        //                foreach (var item in vm.vms)
        //                {
        //                    inserSQL = string.Format(@"INSERT INTO Ftt_form_amount (FORM_NO, EXPENSE_TYPE, EXPENSE_DESC, QTY, PRICE, SUBTOTAL, ORDERID, UNIT, FAULT_REASON, REPAIR_ACTION) VALUES ( {0}, '{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}', '{8}', '{9}' );",
        //                                    vm.form_no, item.expense_type, item.expense_desc, item.qty, item.price, item.subtotal, item.orderid, item.unit, item.fault_reason, item.repair_action);

        //                    baseHandler.GetDBHelper().Execute(inserSQL, null);
        //                }

        //                baseHandler.GetDBHelper().Commit();

        //                //DB db = new DB();
        //                //// 取得資料庫目前時間，藉以查詢資料取得入庫產生的 SystemID
        //                //DbConnection conn = db.CreateConnection();
        //                //DbTransaction tran = null;
        //                //DbCommand dbComm = null;
        //                //DataTable m_table = new DataTable();

        //                // 開啟 DB Transaction 機制
        //                //conn.Open();
        //                //tran = conn.BeginTransaction();

        //                //int attMaxIdx = int.Parse(m_Request["AMOUNT_MAX_IDX"]);

        //                //m_Logger.Debug("attMaxIdx:" + attMaxIdx);

        //                //db.ExecuteNonQuery(tran, "DELETE FROM Ftt_form_amount WHERE FORM_NO='" + m_Request["FORM_NO"] + "'");

        //                //for (int index = 1; index <= attMaxIdx; index++)
        //                //{
        //                //    string inserSQL = string.Format(@"INSERT INTO Ftt_form_amount (FORM_NO, EXPENSE_TYPE, EXPENSE_DESC, QTY, PRICE, SUBTOTAL, ORDERID, UNIT, FAULT_REASON, REPAIR_ACTION) VALUES ( {0}, '{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}', '{8}', '{9}' );",
        //                //        m_Request["FORM_NO"], m_Request["EXPENSE_TYPE_" + index].Replace("'", "''"), m_Request["EXPENSE_DESC_" + index].Replace("'", "''"), m_Request["QTY_" + index], m_Request["PRICE_" + index], m_Request["SUBTOTAL_" + index], index, m_Request["UNIT_" + index], m_Request["FAULT_REASON_" + index].Replace("'", "''"), m_Request["REPAIR_ACTION_" + index].Replace("'", "''"));

        //                //    m_Logger.Debug("inserSQL:" + inserSQL);
        //                //    db.ExecuteNonQuery(tran, inserSQL);
        //                //}

        //                //Trace.Write("Form No:" + m_Request["FORM_NO"] + "，Finished.");
        //                //tran.Commit();

        //                return JsonSuccess("新增完成");
        //            }
        //            catch (Exception err)
        //            {
        //                return JsonValidFail("系統異常");
        //            }
        //            finally
        //            {

        //            }



        //        }

        //        return JsonValidFail("不是INSERT方法");
        //    }

        //    return JsonValidFail("工單不存在");
        //}


    }
}