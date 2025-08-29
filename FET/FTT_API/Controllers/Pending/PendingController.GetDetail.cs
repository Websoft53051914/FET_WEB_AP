
using Core.Utility.Enums;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_API.Common;
using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models;
using FTT_API.Models.Handler;
using FTT_API.Models.Partial;
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
        [HttpGet("[action]")]
        public IActionResult GetDetail(string form_no)
        {
            try
            {
                FormTableVM vm = new FormTableVM();

                vm.ActionName = LoginSession.Current.empname;
                if (LoginSession.Current.empname != LoginSession.Current.engname)
                    vm.ActionName = LoginSession.Current.empname + "(" + LoginSession.Current.engname + ")";

                vm.PreHandleDesc = GetPreHandleDesc("TT_LAST_DESC", form_no, "", "");

                HandleForm_Load(vm, form_no);






                return JsonSuccess(vm);
            }
            catch (Exception ex)
            {

                return JsonValidFail("系統異常");
            }
        }


        protected void HandleForm_Load(FormTableVM vm, string form_no)
        {
            PenddingHanlder _PenddingHanlder = new PenddingHanlder(_config, HttpContext);

            var mIVRCode = _PenddingHanlder.GetIVRCode(form_no);

            var _StoreClass = GetStoreInfo(vm, form_no);
            vm.store_profileDTO = _StoreClass;

            var _TTInfo = GetTTInfo(vm, form_no);
            vm.ftt_formDTO = _TTInfo;



//            string APPROVE_FORM = "";

//            SRData = db.ExecuteDataTable("SELECT * FROM FTT_FORM WHERE FORM_NO=" + Server.HtmlEncode(Request.QueryString["FORM_NO"]));

//            DataTable Form_Status = db.ExecuteDataTable(@"
//SELECT 
//STATUS,PRIOR_STATUS,STATUS_ORDERID,FORM_TYPE,(SELECT STATUS_NAME FROM FORM_ACCESS_STATUS WHERE FORM_TYPE='FTT_FORM' AND FORM_ACCESS_STATUS.STATUS=APPROVE_FORM.STATUS) as STATUS_NAME 
//FROM APPROVE_FORM WHERE FORM_NO=" + Server.HtmlEncode(Request.QueryString["FORM_NO"]));
//            if (Form_Status.Rows.Count > 0)
//            {
//                APPROVE_FORM = Form_Status.Rows[0]["FORM_TYPE"].ToString();
//                SRData.Columns.Add("FORM", System.Type.GetType("System.String"));
//                SRData.Columns.Add(APPROVE_FORM, System.Type.GetType("System.String"));
//                SRData.Columns.Add(APPROVE_FORM + "_PRIOR_STATUS", System.Type.GetType("System.String"));
//                SRData.Columns.Add(APPROVE_FORM + "_STATUS_ORDERID", System.Type.GetType("System.String"));
//                SRData.Columns.Add("STATUS", System.Type.GetType("System.String"));
//                SRData.Columns.Add("STATUS_DESC", System.Type.GetType("System.String"));

//                SRData.Rows[0]["FORM"] = APPROVE_FORM;
//                SRData.Rows[0][APPROVE_FORM] = Form_Status.Rows[0][0].ToString();
//                SRData.Rows[0][APPROVE_FORM + "_PRIOR_STATUS"] = (Form_Status.Rows[0][1].ToString() == "") ? "16" : Form_Status.Rows[0][1].ToString();
//                SRData.Rows[0][APPROVE_FORM + "_STATUS_ORDERID"] = (Form_Status.Rows[0][2].ToString() == "") ? "16" : Form_Status.Rows[0][2].ToString();
//                SRData.Rows[0]["STATUS"] = Form_Status.Rows[0]["STATUS"].ToString();
//                SRData.Rows[0]["STATUS_DESC"] = Form_Status.Rows[0]["STATUS_NAME"].ToString();
//            }
//            Form_Status.Dispose();

//            Approve Approve_Auth = new Approve(Context.User.Identity.Name.ToString());
//            if (APPROVE_FORM != "")
//            {
//                string[] UserAuth = Approve_Auth.Form_Auth(APPROVE_FORM, Server.HtmlEncode(Request.QueryString["FORM_NO"]), SRData.Rows[0][APPROVE_FORM].ToString(), APPROVE_FORM + "_PRIOR_STATUS", Session["IVRCODE"].ToString());
//                SubmitButton += UserAuth[0];
//                Role += UserAuth[1];
//                UpdateField += UserAuth[2];
//                RequireField += UserAuth[3];

//                // 當狀態為 [已派單] 不使用按鈕，使用下拉式選單選擇狀態與顯示相對應的控制項
//                TicketPanel.Visible = true;
//                TicketInfo.TTNo = Server.HtmlEncode(Request.QueryString["FORM_NO"]);
//                TicketInfo.TTStatus = SRData.Rows[0]["STATUS"].ToString();
//                TicketInfo.ShowTicketInfo = SRData.Rows[0]["TICKET_INFO"].ToString();
//                TicketInfo.TTType = APPROVE_FORM;
//                if (SRData.Rows[0]["STATUS"].ToString() == "TICKET")
//                {
//                    SubmitButton = "";
//                    SubmitForm.Visible = true;
//                    SubmitForm.Attributes["onClick"] = @"event.returnValue = CheckFormInTicket();";
//                }
//            }

//            db.Dispose();

//            /* 測試用 */
//            //RequireField = "EMPNAME,";
        }


        protected StoreClass GetStoreInfo(FormTableVM vm, string form_No)
        {
            StoreClass mStoreData = new StoreClass();

            if (!string.IsNullOrEmpty(form_No))
            {
                PenddingHanlder _PenddingHanlder = new PenddingHanlder(_config, HttpContext);
                var create_time = _PenddingHanlder.GetCreateTime(form_No);

                var mIVRCode = _PenddingHanlder.GetIVRCode(form_No);
                var mSTOREName = _PenddingHanlder.GetShopName(mIVRCode);
                LoginSession.Current.shop_name = mSTOREName;

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

                        //mRunScript += "ifwarrant = \"N\";\r\n";
                    }
                }
                //mRunScript += "isNewTT = false;\r\n";
            }
            else if (!string.IsNullOrEmpty(LoginSession.Current.ivrcode))
            {
                vm.Create_Time = System.DateTime.Now.ToString("yyyy/MM/dd");
                mStoreData = new StoreClass(LoginSession.Current.ivrcode);
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

                            //mRunScript += "ifwarrant = \"N\";\r\n";
                        }
                    }
                }
                //mRunScript += "isNewTT = true;\r\n";
            }

            return mStoreData;
        }

        protected ftt_formDTO GetTTInfo(FormTableVM vm, string formNo)
        {
            ftt_formDTO _ftt_formDTO = new ftt_formDTO();
            if (!string.IsNullOrEmpty(formNo))
            {
                PenddingHanlder _PenddingHanlder = new PenddingHanlder(_config, HttpContext);
                _ftt_formDTO = _PenddingHanlder.GetFttFormInfo(formNo);

                if (_ftt_formDTO != null)
                {
                    vm.Create_Time = _ftt_formDTO.createtime;

                    if (!string.IsNullOrEmpty(_ftt_formDTO.CIDesc))
                    {
                        string filePath = $"images/Item/{_ftt_formDTO.CIDesc.Trim()}.jpg";
                        string path = Path.Combine(_hostingEnvironment.WebRootPath, filePath);
                        if (System.IO.File.Exists(path))
                        {
                            vm.hasTT_IMAGE = true;
                            vm.newImageSRC = filePath;
                        }
                    }

                    //mRunScript += "setCATEGORY_LABEL(\"" + _ftt_formDTO.category_id + "\"); \r\n";

                    if (_ftt_formDTO.selfconfig == "N")
                    {
                        //mRunScript += "showVenderInfo(\"" + _ftt_formDTO.order_id + "\",\"" + _ftt_formDTO.merchant_name + "\",\"" + _ftt_formDTO.cp_name + "\",\"" + _ftt_formDTO.cp_tel + "\");\r\n";
                    }
                    else
                    {
                        var storeName = _PenddingHanlder.GetShopName(_ftt_formDTO.ivrcode);
                        vm.storename = storeName;
                    }
                }
            }

            return _ftt_formDTO;
        }
    }
}