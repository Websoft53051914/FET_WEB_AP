using FTT_WEB.Common.OriginClass.EntiityClass;
using FTT_WEB.Models.Handler;
using log4net;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace FTT_WEB.Common.OriginClass
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

        public string[] Form_Auth(string FormType, string FormNo, string TSTATUS, string PreStatus, string IVRCode)
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
                            SubmitButton += "<input type=submit onclick=\"document.all.STATUSWORDING.value=this.value;RequireField+='" + dto.require_field + "';document.all.FORM_TYPE.value='" + dto.form_type + "';document.all.STATUS.value='" + PreStatus + "';\" value='" + StatusName.GetValue(j) + "' Class='customButton'  style='vertical-align:middle;border:none 0px black;'  >";
                        }
                        else
                        {
                            if (IVRCode.Length <= 7 && StatusTemp.GetValue(j).ToString() != "USED")
                            {
                                if (dto.approve == "Y")
                                {
                                    SubmitButton += "　<input type=submit onclick=\"document.all.STATUSWORDING.value=this.value;RequireField='" + dto.require_field + "';document.all.FORM_TYPE.value='" + dto.form_type + "';document.all.APPROVE.value='Y';document.all.User_Type.value='" + dto.User_Type + "';document.all.STATUS.value='" + StatusTemp.GetValue(j) + "';\" value='" + StatusName.GetValue(j) + "' Class='customButton'  style='vertical-align:middle;border:none 0px black;'   >";
                                }
                                else
                                {
                                    //m_Logger.Debug(j);
                                    //m_Logger.Debug(StatusTemp.GetValue(j));
                                    SubmitButton += "　<input type=submit onclick=\"document.all.STATUSWORDING.value=this.value;RequireField='" + dto.require_field + "';document.all.FORM_TYPE.value='" + dto.form_type + "';document.all.STATUS.value='" + StatusTemp.GetValue(j) + "';\" value='" + StatusName.GetValue(j) + "' Class='customButton'  style='vertical-align:middle;border:none 0px black;'  >";
                                }
                            }
                        }
                    }
                }
            }

            string[] temp = { SubmitButton, Role, UpdateField, RequireField, "" };
            return temp;
        }

        private string Apporve_Status_Change(string Status)
        {
            Regex r = new Regex(@"(.*)#(.*)#(.*)", RegexOptions.IgnoreCase);
            string Status_Change = "N";
            string getresponse = "";
            string getresponse1 = "";
            DataTable Access_Control;

            getresponse1 += "<br>" + Status + "<br>";
            //清空待處理人員
            //DBtable.ExecuteNonQuery("UPDATE ACCESS_ROLE SET ACTION='' WHERE FORM_TYPE='" + form_type + "' AND FORM_NO='" + form_no + "'");
            getresponse1 += "<br>1" + "UPDATE ACCESS_ROLE SET ACTION='' WHERE FORM_TYPE='" + form_type + "' AND FORM_NO='" + form_no + "'";
            Match m = r.Match(Status);

            getresponse1 += m.Success.ToString() + "<br>";

            //如果為並行處理
            if (m.Success)
            {
                form_type = m.Groups[1].ToString();
                Status = m.Groups[2].ToString();
                org_status = m.Groups[3].ToString();
                //DBtable.ExecuteNonQuery("UPDATE ACCESS_ROLE SET ACTION='' WHERE FORM_TYPE='" + form_type + "' AND FORM_NO='" + form_no + "'");
            }
            else
            {
                org_status = "";
            }

            getresponse1 += Status + "<br>";
            BaseDBHandler baseHandler = new BaseDBHandler();

            if ((_request_STATUS.Contains("+") || _request_STATUS.Contains("-1")) && !m.Success)
            {
                //簽核
                Access_Control = baseHandler.GetDBHelper().FindDataTable("SELECT User_Type,ORDERID,STATUS FROM ACCESS_CONTROL WHERE  FORM_TYPE='" + form_type + "' AND ORDERID IN (SELECT ORDERID" + _request_STATUS + " from ACCESS_CONTROL WHERE FORM_TYPE='" + form_type + "' AND STATUS='" + Status + "') ", null);
                getresponse1 += "<br>2SELECT User_Type,ORDERID,STATUS FROM ACCESS_CONTROL WHERE  FORM_TYPE='" + form_type + "' AND ORDERID IN (SELECT ORDERID" + _request_STATUS + " from ACCESS_CONTROL WHERE FORM_TYPE='" + form_type + "' AND STATUS='" + Status + "') ";
            }
            else
            {
                Access_Control = baseHandler.GetDBHelper().FindDataTable("SELECT User_Type,ORDERID,STATUS FROM ACCESS_CONTROL WHERE  FORM_TYPE='" + form_type + "' and STATUS='" + Status + "' order by orderid", null);
                getresponse1 += "<br>3SELECT User_Type,ORDERID,STATUS FROM ACCESS_CONTROL WHERE  STATUS='" + Status + "' AND FORM_TYPE='" + form_type + "'";
            }

            for (int i = 0; i < Access_Control.Rows.Count; i++)
            {
                //判斷簽核人員是否為空值, 若為空值是否要stop
                DataTable Access_Role = baseHandler.GetDBHelper().FindDataTable("SELECT *  FROM ACCESS_ROLE WHERE FORM_TYPE='" + form_type + "' AND FORM_NO='" + form_no + "' AND User_Type='" + Access_Control.Rows[i]["User_Type"].ToString() + "'", null);
                getresponse1 += "<br>4SELECT *  FROM ACCESS_ROLE WHERE FORM_TYPE='" + form_type + "' AND FORM_NO='" + form_no + "' AND User_Type='" + Access_Control.Rows[i]["User_Type"].ToString() + "'";
                //2-12 UPDATE BY LING  
                if ((Access_Role.Rows[0]["IFNULLSKIP"].ToString() == "N" || Access_Role.Rows[0]["EMPNO"].ToString() + Access_Role.Rows[0]["DEPTCODE"].ToString() != "") && Access_Role.Rows[0]["APPROVE_STATUS"].ToString() != "同意")
                {
                    //DBtable.ExecuteNonQuery("UPDATE ACCESS_ROLE SET ACTION='Y' WHERE  FORM_TYPE='" + form_type + "' AND FORM_NO='" + form_no + "' AND User_Type='" + Access_Control.Rows[i]["User_Type"].ToString() + "'");
                    getresponse1 += "<br>5UPDATE ACCESS_ROLE SET ACTION='Y' WHERE  FORM_TYPE='" + form_type + "' AND FORM_NO='" + form_no + "' AND User_Type='" + Access_Control.Rows[i]["User_Type"].ToString() + "'";
                    //當找到下個執行人員或沒找到但並須stop(IFNULLSKIP==N) 
                    Status_Change = "Y";//==>找到下個執行狀態, 不須再往下找
                }
                Access_Role.Dispose();
            }

            if (Access_Control.Rows.Count > 0)
            {
                if (Status_Change == "N") //未找到, 但還有其他狀態可判斷
                {
                    DataTable Access_Control_temp = baseHandler.GetDBHelper().FindDataTable("SELECT STATUS,ALLOW_STATUS,APPROVE,ORDERID FROM ACCESS_CONTROL WHERE FORM_TYPE='" + form_type + "' AND ORDERID=" + Access_Control.Rows[0]["ORDERID"].ToString(), null);
                    getresponse1 += "<br>6SELECT STATUS FROM ACCESS_CONTROL WHERE ORDERID=" + Access_Control.Rows[0]["ORDERID"].ToString();
                    /*
                    if (Access_Control_temp.Rows.Count > 0)
                    {
                        getresponse = Apporve_Status_Change(Access_Control_temp.Rows[0][0].ToString()); //往下循找
                    }
                    */
                    if (_request_STATUS.Contains("+") || _request_STATUS.Contains("-"))
                    {
                        getresponse = Apporve_Status_Change(Access_Control_temp.Rows[0][0].ToString()); //往下循找
                    }
                    else
                    {
                        getresponse = _request_STATUS;
                    }

                    Access_Control_temp.Dispose();
                }
                else
                {
                    //DBtable.ExecuteNonQuery("UPDATE " + form_type + " SET status='" + Access_Control.Rows[0]["status"].ToString() + "' where " + columnname + "='" + form_no + "'");
                    getresponse = Access_Control.Rows[0]["status"].ToString();
                }
            }
            else
            {
                if (_request_STATUS.Contains("+") || _request_STATUS.Contains("-1")) //若為簽核, 判斷頂端或尾端的狀態
                {
                    if (_request_STATUS.Contains("+")) //已經到了該類別的底端, 抓取EOF值
                    {
                        Access_Control = baseHandler.GetDBHelper().FindDataTable("SELECT EOF FROM ACCESS_CONTROL WHERE  FORM_TYPE='" + form_type + "' AND STATUS='" + Status + "' ", null);
                        getresponse1 += "<br>7SELECT EOF FROM ACCESS_CONTROL WHERE  FORM_TYPE='" + form_type + "' AND STATUS='" + Status + "' ";
                    }
                    else //已經到了該類別的頂端, 抓取BOF值
                    {
                        Access_Control = baseHandler.GetDBHelper().FindDataTable("SELECT BOF FROM ACCESS_CONTROL WHERE  FORM_TYPE='" + form_type + "' AND STATUS='" + Status + "'", null);
                        getresponse1 += "<br>8SELECT BOF FROM ACCESS_CONTROL WHERE  FORM_TYPE='" + form_type + "' AND STATUS='" + Status + "' ";
                    }

                    if (Access_Control.Rows.Count > 0 && Access_Control.Rows[0][0].ToString() != "")
                    {
                        getresponse = Apporve_Status_Change(Access_Control.Rows[0][0].ToString());
                    }
                    else
                    {
                        getresponse = _request_STATUS;
                    }
                }
                else
                {
                    getresponse = _request_STATUS;
                }
            }

            return getresponse;
            //return getresponse1;
        }


    }
}
