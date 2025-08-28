using FTT_WEB.Models.Handler;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class CIFunctions
    {
        private bool IsDisposed = false;

        private Container components = null;

        ~CIFunctions()
        {
            Trace.WriteLine("Destructor CIFunctions Class.");
            Dispose(Disposing: false);
        }

        public void Dispose()
        {
            Dispose(Disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (!IsDisposed && Disposing && components != null)
            {
                components.Dispose();
            }

            IsDisposed = true;
            Trace.WriteLine("Dispose CIFunctions Class.");
        }

        public bool Check_CI_has_Child(string CISID)
        {
            bool flag = false;
            if (CISID == "" || CISID == null)
            {
                flag = false;
            }
            else
            {
                string queryString = "SELECT CISID FROM ci_relations WHERE parentsid = " + CISID + "";
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                flag = ((dataTable.Rows.Count != 0) ? true : false);
                dataTable.Dispose();
            }

            return flag;
        }

        public string GetIncidentTypeDesc(string IncidentTypeID)
        {
            string text = "";
            if (IncidentTypeID == "" || IncidentTypeID == null)
            {
                text = "";
            }
            else
            {
                string queryString = "SELECT incident_type_desc(" + IncidentTypeID + ") as cidesc FROM dual";
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                text = ((dataTable.Rows.Count != 0) ? dataTable.Rows[0]["cidesc"].ToString() : ("(" + IncidentTypeID + ")"));
                dataTable.Dispose();
            }

            return text;
        }

        public string GetCIFullCode(string CISID)
        {
            return GetCIFullCode(CISID, 0) + ",";
        }

        private string GetCIFullCode(string CISID, int RunCount)
        {
            string text = "";
            if (CISID == "" || CISID == null || RunCount > 10)
            {
                text = "";
            }
            else
            {
                text = CISID;
                string queryString = "SELECT parentsid,cicategory FROM ci_relations WHERE cisid=" + CISID;
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["parentsid"].ToString() != dataTable.Rows[0]["cicategory"].ToString())
                {
                    text = GetCIFullCode(dataTable.Rows[0]["parentsid"].ToString(), ++RunCount) + "," + text;
                }

                dataTable.Dispose();
            }

            return text;
        }

        public string GetCIFullDesc(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                string queryString = "SELECT ci_desc(" + CISID + ") as cidesc FROM dual";
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                text = ((dataTable.Rows.Count != 0) ? dataTable.Rows[0]["cidesc"].ToString() : ("(" + CISID + ")"));
                dataTable.Dispose();
            }

            return text;
        }

        public string GetCIContactList(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                CI cI = new CI(CISID);
                text = ((!cI.hasData()) ? "" : cI.ContactList);
                cI.Dispose();
            }

            return text;
        }

        public string GetCIAllContactList(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                return "";
            }

            string queryString = "SELECT contactlist, parentsid, cicategory FROM ci_relations WHERE cisid = " + CISID;
            BaseDBHandler handler = new();
            DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                string text2 = dataTable.Rows[0][0].ToString();
                string text3 = dataTable.Rows[0][1].ToString();
                string text4 = dataTable.Rows[0][2].ToString();
                if (text3 != text4)
                {
                    string text5 = text;
                    text = text5 + "," + text2 + "," + GetCIAllContactList(text3);
                }
                else
                {
                    text = text + "," + text2;
                }
            }

            dataTable.Dispose();
            return text;
        }

        public string Get_CI_Full_Desc(string CISID, string CIDescrStr, int CategorySID)
        {
            string result = "";
            if (CISID == "" || CISID == null)
            {
                result = "";
            }
            else
            {
                CI cI = new CI(CISID);
                if (cI.hasData())
                {
                    if (cI.ParentSID == CategorySID.ToString() || cI.ParentSID == "")
                    {
                        if (CIDescrStr.Length < 2 && cI.ParentSID == CategorySID.ToString())
                        {
                            CIDescrStr = cI.CIName + "-";
                        }

                        result = Show_CISID_Descr(CISID, CIDescrStr);
                    }
                    else
                    {
                        CIDescrStr = cI.CIName + "-" + CIDescrStr;
                        CIDescrStr = Get_CI_Full_Desc(cI.ParentSID, CIDescrStr, CategorySID);
                    }
                }
                else
                {
                    result = Show_CISID_Descr(CISID, CIDescrStr);
                }

                cI.Dispose();
            }

            return result;
        }

        public string GetCIL1(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                string queryString = "SELECT ci_sid_l1(" + CISID + ") as cisid FROM dual";
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                text = ((dataTable.Rows.Count != 0) ? dataTable.Rows[0]["cisid"].ToString() : ("(" + CISID + ")"));
                dataTable.Dispose();
            }

            return text;
        }

        public string GetCIL1Desc(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                string queryString = "SELECT ci_desc(ci_sid_l1(" + CISID + ")) as cidesc FROM dual";
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                text = ((dataTable.Rows.Count != 0) ? dataTable.Rows[0]["cidesc"].ToString() : ("(" + CISID + ")"));
                dataTable.Dispose();
            }

            return text;
        }

        public string GetCILevelDesc(string CILevel, string CISID)
        {
            string text = "";
            string text2 = CISID.ToString();
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                int num = 0;
                switch (CILevel.ToLower())
                {
                    case "l1":
                        text = GetCIL1Desc(CISID);
                        break;
                    case "l2":
                        num = 2;
                        break;
                    case "l3":
                        num = 3;
                        break;
                    default:
                        num = 0;
                        break;
                }

                if (text == "")
                {
                    string text3 = "SELECT * FROM ci_relations WHERE cisid = " + text2;
                    string text4 = "";
                    while (text3 != "")
                    {
                        BaseDBHandler handler = new();
                        DataTable dataTable = handler.GetDBHelper().FindDataTable(text3, []);
                        if (dataTable.Rows.Count == 0)
                        {
                            text3 = "";
                        }
                        else if (dataTable.Rows[0]["parentsid"].ToString() != "")
                        {
                            text2 = dataTable.Rows[0]["parentsid"].ToString();
                            text3 = "SELECT * FROM ci_relations WHERE cisid = " + text2;
                            text4 = "," + text2 + text4;
                        }
                        else
                        {
                            text3 = "";
                        }

                        dataTable.Dispose();
                    }

                    string[] array = (CISID + text4).Split(',');
                    if (num <= array.Length)
                    {
                        text2 = array[num];
                        text3 = "SELECT * FROM ci_relations WHERE cisid = " + text2;
                    }
                    else
                    {
                        text2 = array[array.Length - 1];
                        text3 = "SELECT * FROM ci_relations WHERE cisid = " + text2;
                    }

                    if (text3 != "")
                    {
                        BaseDBHandler handler = new();
                        DataTable dataTable2 = handler.GetDBHelper().FindDataTable(text3, []);
                        text = ((dataTable2.Rows.Count != 0) ? dataTable2.Rows[0]["ciname"].ToString() : "");
                        dataTable2.Dispose();
                    }
                }
            }

            return text;
        }

        public string GetCICategoryDesc(string CategorySID)
        {
            string text = "";
            if (CategorySID == "" || CategorySID == null)
            {
                text = "";
            }
            else
            {
                CICategory cICategory = new CICategory(CategorySID);
                text = ((!cICategory.hasData()) ? "" : cICategory.CategoryName);
                cICategory.Dispose();
            }

            return text;
        }

        public string GetCICategorySID(string CategoryName)
        {
            string text = "";
            if (CategoryName == "" || CategoryName == null)
            {
                text = "";
            }
            else
            {
                string queryString = "SELECT cicategory FROM ci_category WHERE lower(categoryname) = '" + CategoryName.ToLower() + "'";
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                text = ((dataTable.Rows.Count != 0) ? dataTable.Rows[0]["cicategory"].ToString() : "");
                dataTable.Dispose();
            }

            return text;
        }

        public string GetCICategory(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                CI cI = new CI(CISID);
                text = ((!cI.hasData()) ? "" : cI.CICategory.CICategoryID);
                cI.Dispose();
            }

            return text;
        }

        public string GetCIRootSID(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                string queryString = "SELECT ci_sid_root(" + CISID + ") as rootsid FROM dual";
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                text = ((dataTable.Rows.Count != 0) ? dataTable.Rows[0]["rootsid"].ToString() : "");
                dataTable.Dispose();
            }

            return text;
        }

        public string GetCIGroupDesc(string CIList)
        {
            string text = "";
            if (CIList.Length > 0)
            {
                string[] array = CIList.Split(',');
                string[] array2 = array;
                foreach (string cISID in array2)
                {
                    text = text + "," + GetCIFullDesc(cISID);
                }

                UtilFunctions utilFunctions = new UtilFunctions();
                text = utilFunctions.RemoveDuplcate(text);
                utilFunctions.Dispose();
            }

            return text;
        }

        public string Get_CI_Desc(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                CI cI = new CI(CISID);
                text = ((!cI.hasData()) ? "" : cI.CIName);
                cI.Dispose();
            }

            return text;
        }

        public string Get_CI_SID(string CIName)
        {
            string text = "";
            if (CIName == "" || CIName == null)
            {
                text = "";
            }
            else
            {
                string queryString = "SELECT * FROM ci_relations WHERE ciname = '" + CIName + "'";
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                text = ((dataTable.Rows.Count != 0) ? dataTable.Rows[0]["ciname"].ToString() : "");
                dataTable.Dispose();
            }

            return text;
        }

        public string Get_CI_SID(string CIName, string CICategory)
        {
            string text = "";
            if (CIName == "" || CIName == null)
            {
                text = "";
            }
            else
            {
                string queryString = "SELECT * FROM ci_relations WHERE ciname = '" + CIName + "' and cicategory = " + CICategory;
                BaseDBHandler handler = new();
                DataTable dataTable = handler.GetDBHelper().FindDataTable(queryString, []);
                text = ((dataTable.Rows.Count != 0) ? dataTable.Rows[0]["cisid"].ToString() : "");
                dataTable.Dispose();
            }

            return text;
        }

        public string Get_CI_ParentSID(string CISID)
        {
            string text = "";
            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                CI cI = new CI(CISID);
                text = ((!cI.hasData()) ? "" : cI.ParentSID);
                cI.Dispose();
            }

            return text;
        }

        public string Show_CISID_Descr(string CISID, string sString)
        {
            sString = ((CISID == "" || CISID == null) ? "<font color=\"#FF0000\">(尚未歸類)</font>" : ((sString.Length > 1) ? sString.Substring(0, sString.Length - 1) : ((CISID.Length >= 3) ? ("<font color=\"#FF0000\">(" + CISID + ")</font>") : "<font color=\"#FF0000\">(尚未歸類)</font>")));
            return sString;
        }

        public string GetChildCIList(string CISID, bool NodeOnly)
        {
            string text = "";
            if (!NodeOnly)
            {
                text = CISID + ",";
            }

            if (CISID == "" || CISID == null)
            {
                text = "";
            }
            else
            {
                string text2 = "";
                string text3 = "";
                BaseDBHandler handler = new();
                text3 = handler.GetFieldType("PARENTSID", "CI_RELATIONS");
                text2 = "SELECT CISID FROM CI_RELATIONS WHERE PARENTSID=" + handler.ConvertOracleData(CISID, text3);
                DataTable dataTable = handler.GetDBHelper().FindDataTable(text2, []);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (Check_CI_has_Child(dataTable.Rows[i][0].ToString()))
                    {
                        if (!NodeOnly)
                        {
                            text = text + dataTable.Rows[i][0].ToString() + ",";
                        }

                        text += GetChildCIList(dataTable.Rows[i][0].ToString(), NodeOnly);
                    }
                    else
                    {
                        text = text + dataTable.Rows[i][0].ToString() + ",";
                    }
                }

                if (NodeOnly && text.Length == 0)
                {
                    text = CISID + ",";
                }

                dataTable.Dispose();
            }

            return text;
        }
    }
}
