using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class CI : IDisposable
    {
        private string m_cisId;

        private string m_ciName;

        private CICategory m_ciCategory;

        private string m_ciFunction;

        private string m_parentsId;

        private string m_horizontalId;

        private string m_referenceId;

        private string m_handleGroup;

        private string m_contactList;

        private string m_authorizationId;

        private bool m_disable;

        private string m_remark;

        private string m_fullDesc;

        private DateTime m_createTime;

        private Employee m_createOp;

        private DateTime m_updateTime;

        private Employee m_updateOp;

        protected bool m_Result;

        protected long m_rowsCount;

        private bool IsDisposed = false;

        private Container components = null;

        protected RetrieveData m_retrieveData = new RetrieveCIData();

        public string CISID
        {
            get
            {
                return m_cisId;
            }
            set
            {
                m_cisId = value;
            }
        }

        public string CIName
        {
            get
            {
                return m_ciName;
            }
            set
            {
                m_ciName = value;
            }
        }

        public CICategory CICategory
        {
            get
            {
                return m_ciCategory;
            }
            set
            {
                m_ciCategory = value;
            }
        }

        public string CIFunction
        {
            get
            {
                return m_ciFunction;
            }
            set
            {
                m_ciFunction = value;
            }
        }

        public string ParentSID
        {
            get
            {
                return m_parentsId;
            }
            set
            {
                m_parentsId = value;
            }
        }

        public string HorizontalID
        {
            get
            {
                return m_horizontalId;
            }
            set
            {
                m_horizontalId = value;
            }
        }

        public string ReferenceId
        {
            get
            {
                return m_referenceId;
            }
            set
            {
                m_referenceId = value;
            }
        }

        public string HandleGroupList
        {
            get
            {
                return m_handleGroup;
            }
            set
            {
                m_handleGroup = value;
            }
        }

        public string ContactList
        {
            get
            {
                return m_contactList;
            }
            set
            {
                m_contactList = value;
            }
        }

        public string AuthorizationList
        {
            get
            {
                return m_authorizationId;
            }
            set
            {
                m_authorizationId = value;
            }
        }

        public bool Disable
        {
            get
            {
                return m_disable;
            }
            set
            {
                m_disable = value;
            }
        }

        public string Remark
        {
            get
            {
                return m_remark;
            }
            set
            {
                m_remark = value;
            }
        }

        public string FullDesc
        {
            get
            {
                return m_fullDesc;
            }
            set
            {
                m_fullDesc = value;
            }
        }

        public DateTime CreateTime
        {
            get
            {
                return m_createTime;
            }
            set
            {
                m_createTime = value;
            }
        }

        public Employee CreateOp
        {
            get
            {
                return m_createOp;
            }
            set
            {
                m_createOp = value;
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                return m_updateTime;
            }
            set
            {
                m_updateTime = value;
            }
        }

        public Employee UpdateOp
        {
            get
            {
                return m_updateOp;
            }
            set
            {
                m_updateOp = value;
            }
        }

        public CI()
        {
        }

        public CI(string CISID)
        {
            if (CISID != "")
            {
                m_Result = GetCIData(CISID);
            }
            else
            {
                m_Result = false;
            }

            Trace.WriteLine("Retrieve CI Data : " + m_Result);
        }

        ~CI()
        {
            Trace.WriteLine("Destructor CI Class.");
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
            Trace.WriteLine("Dispose CI Class.");
        }

        public bool hasData()
        {
            if (m_rowsCount > 0)
            {
                return true;
            }

            return false;
        }

        public void SetRetrieveData(RetrieveData m_retrieveData)
        {
            this.m_retrieveData = m_retrieveData;
        }

        protected bool GetCIData(string sData)
        {
            DataTable dataTable = m_retrieveData.RetrieveDBData(sData);
            m_rowsCount = dataTable.Rows.Count;
            if (m_rowsCount == 0)
            {
                return false;
            }

            Trace.WriteLine("Retrieve Rows Count : " + dataTable.Rows.Count);
            m_cisId = dataTable.Rows[0]["cisid"].ToString();
            m_ciName = dataTable.Rows[0]["ciname"].ToString();
            m_ciCategory = new CICategory(dataTable.Rows[0]["cicategory"].ToString());
            m_ciFunction = dataTable.Rows[0]["cifunction"].ToString();
            m_parentsId = dataTable.Rows[0]["parentsid"].ToString();
            m_disable = ((dataTable.Rows[0]["disable"].ToString() == "Y") ? true : false);
            m_remark = dataTable.Rows[0]["remark"].ToString();
            m_handleGroup = dataTable.Rows[0]["handlegroup"].ToString();
            m_contactList = dataTable.Rows[0]["contactlist"].ToString();
            if (dataTable.Columns.Contains("authorizationId"))
            {
                m_authorizationId = dataTable.Rows[0]["authorizationId"].ToString();
            }

            if (dataTable.Columns.Contains("referenceId"))
            {
                m_referenceId = dataTable.Rows[0]["referenceId"].ToString();
            }

            if (dataTable.Columns.Contains("fulldesc"))
            {
                m_fullDesc = dataTable.Rows[0]["fulldesc"].ToString();
            }

            if (dataTable.Columns.Contains("Create_Time") && !dataTable.Rows[0]["Create_Time"].Equals(DBNull.Value))
            {
                m_createTime = Convert.ToDateTime(dataTable.Rows[0]["Create_Time"].ToString());
            }

            if (dataTable.Columns.Contains("Create_Operator") && !dataTable.Rows[0]["Create_Operator"].Equals(DBNull.Value))
            {
                m_createOp = new Employee(dataTable.Rows[0]["Create_Operator"].ToString());
            }

            if (dataTable.Columns.Contains("Update_Time") && !dataTable.Rows[0]["Update_Time"].Equals(DBNull.Value))
            {
                m_updateTime = Convert.ToDateTime(dataTable.Rows[0]["Update_Time"].ToString());
            }

            if (dataTable.Columns.Contains("Update_Operator") && !dataTable.Rows[0]["Update_Operator"].Equals(DBNull.Value))
            {
                m_updateOp = new Employee(dataTable.Rows[0]["Update_Operator"].ToString());
            }

            Trace.WriteLine("CISID : " + m_cisId);
            Trace.WriteLine("CIName : " + m_ciName);
            Trace.WriteLine("CICategory : " + m_ciCategory.CICategoryID.ToString() + "(" + m_ciCategory.CategoryName + ")");
            Trace.WriteLine("CIFunction : " + m_ciFunction);
            Trace.WriteLine("ParentSID : " + m_parentsId);
            Trace.WriteLine("Handle Group : " + m_handleGroup);
            Trace.WriteLine("Contact List : " + m_contactList);
            Trace.WriteLine("Disabled : " + m_disable);
            Trace.WriteLine("Remark : " + m_remark);
            dataTable.Dispose();
            return true;
        }
    }
}
