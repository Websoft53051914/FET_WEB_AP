using FTT_VENDER_API.Common.OriginClass.EntiityClass;
using FTT_VENDER_API.Models;
using FTT_VENDER_API.Models.Handler;
using log4net;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace FTT_VENDER_API.Common.OriginClass
{
    public class Approve
    {
        public string form_type;
        public string form_no;
        private string org_status;
        private string _EmpNo;
        public string ApproveCommon;
        private string _request_STATUS;
        //public Logger m_Logger = LogManager.GetCurrentClassLogger();

        public Approve(string EmpNo)
        {
            _EmpNo = EmpNo;
            ApproveCommon = "";
        }

        public Approve(string FormType, string FormNo, string request_STATUS)
        {
            form_type = FormType;
            form_no = FormNo;
            _request_STATUS = request_STATUS;
        }


        ~Approve()
        {
            Dispose(false);
        }

        private bool IsDisposed = false;
        private System.ComponentModel.Container components = null;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {

            if (!IsDisposed)
            {
                if (Disposing)
                {
                    //Clean Up managed resources
                    if (components != null)
                        components.Dispose();
                }
                //Clean up unmanaged resources
            }
            IsDisposed = true;

        }

        public string[] Form_Auth(FormTableVM vm, string FormType, string FormNo, string TSTATUS, string PreStatus, string IVRCode)
        {
            string SubmitButton = "", UpdateField = "", RequireField = "", Role = "", Status = "";
            form_access_controlSQL _form_access_controlSQL = new form_access_controlSQL();
            var dto = _form_access_controlSQL.GetInfo(IVRCode, FormType, TSTATUS, FormNo, _EmpNo);

            /* 判斷user對此SR所擁有的權限, 在此會記錄
             * RequireField, OptionField, SubmittBotton 此三項資料
             * 之後會根據這三項, 建立表單權限
             */

            if (dto != null)
            {
                UpdateField = $"{dto.option_field},{dto.require_field},";
                Role += dto.User_Type + ",";
                RequireField += dto.require_field + ",";

                string Temp = dto.allow_status;
                string[] StatusTemp = { };
                string[] StatusName = { };
                if (Temp.Length > 0)
                {
                    StatusTemp = Temp.Split(',');
                    Temp = dto.allow_wording;
                    StatusName = Temp.Split(',');
                    /*AlLOW_WORDING is the word on SUBMIT BUTTON*/

                }

                if (dto.approve == "Y" && ApproveCommon == "")
                {
                    ApproveCommon = "Y";
                    vm.ShowApproveCommon = true;


                    //在此保留原邏輯用來判斷，不拋到前端
                    SubmitButton = "<font id='approvecommon' STYLE='FONT: bold 9pt Arial; COLOR: #000080; TEXT-DECORATION: none;'>建議／說明</font>：<input type=text name=approvecommon maxlength=200 size=80>" + SubmitButton;
                }

                for (int j = 0; j < StatusTemp.Length; j++)
                {
                    if (!(SubmitButton.Contains("value='" + StatusTemp.GetValue(j) + "'>") || Status.Contains("value='" + PreStatus + "'>") || Temp.ToString() == ""))
                    {
                        /*如果可執行的狀態是回到上一個狀態*/

                        if (StatusTemp.GetValue(j).ToString() == "PRIOR_STATUS")
                        {
                            //  SubmitButton += "<input type=button onclick=\"document.all.STATUSWORDING.value=this.value;RequireField+='" + m_Accesscontrol.Rows[i]["REQUIRE_FIELD"].ToString() + "';document.all.FORM_TYPE.value='" + m_Accesscontrol.Rows[i]["FORM_TYPE"].ToString() + "';document.all.STATUS_DESC.value='';document.all.STATUS.value='" + PreStatus + "';\" value='" + StatusName.GetValue(j) + "' Class='customButton'  style='vertical-align:middle;border:none 0px black;' onMouseOver=\"this.className = 'customButtonHover';\" onMouseOut=\"this.className = 'customButton';\">";
                            vm.ShowPriorStatus = true;

                            vm.StatusWording = StatusName.GetValue(j).ToString();
                            vm.Form_Type = dto.form_type;
                            vm.Status = PreStatus;
                            vm.RequireField += dto.require_field;
                            vm.BtnSubmitName = StatusName.GetValue(j).ToString();

                        }
                        else
                        {
                            if (IVRCode.Length <= 7 && StatusTemp.GetValue(j).ToString() != "USED")
                            {
                                if (dto.approve == "Y")
                                {
                                    vm.ApproveY = true;

                                    vm.BtnSubmitName = StatusName.GetValue(j).ToString();
                                    vm.StatusWording = StatusName.GetValue(j).ToString();
                                    vm.RequireField = dto.require_field;
                                    vm.Form_Type = dto.form_type;
                                    vm.Approve = "Y"; //前端沒有找到這個id

                                    vm.User_Type = dto.User_Type;
                                    vm.Status = StatusTemp.GetValue(j).ToString();

                                    SubmitButton += @$"　
<input 
type=submit 
onclick=""

document.all.STATUSWORDING.value=this.value;
RequireField='{dto.require_field}';
document.all.FORM_TYPE.value='{dto.form_type}';
document.all.APPROVE.value='Y';
document.all.User_Type.value='{dto.User_Type}';
document.all.STATUS.value='{StatusTemp.GetValue(j)}';"" 

value='{StatusName.GetValue(j)}' 
Class='customButton'  
style='vertical-align:middle;border:none 0px black;'   >";
                                }
                                else
                                {
                                    vm.BtnSubmitName = StatusName.GetValue(j).ToString();

                                    vm.StatusWording = StatusName.GetValue(j).ToString();
                                    vm.RequireField = dto.require_field;
                                    vm.Form_Type = dto.form_type;
                                    vm.Status = StatusTemp.GetValue(j).ToString();

                                    //m_Logger.Debug(j);
                                    //m_Logger.Debug(StatusTemp.GetValue(j));
                                    SubmitButton += @$"
<input type=submit 
onclick=""
document.all.STATUSWORDING.value=this.value;
RequireField='{dto.require_field}';
document.all.FORM_TYPE.value='{dto.form_type}';
document.all.STATUS.value='{StatusTemp.GetValue(j)}';"" 

value='{StatusName.GetValue(j)}' 
Class='customButton'  
style='vertical-align:middle;border:none 0px black;'  >";
                                }
                            }
                        }
                    }
                }
            }

            string[] temp = { SubmitButton, Role, UpdateField, RequireField, "" };
            return temp;
        }

    }
}
