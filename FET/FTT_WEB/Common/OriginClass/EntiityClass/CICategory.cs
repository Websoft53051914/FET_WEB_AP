using System.ComponentModel;
using System.Data;
using System.Diagnostics;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class CICategory : IDisposable
    {
        private string m_cicategory;

        private string m_categoryname;

        private string m_categoryowner;

        private string m_allow_parentcategory;

        private string m_allow_childcategory;

        private string m_categorymanager;

        private bool m_enable;

        private bool m_isrootcategory;

        private bool m_Result;

        private long m_rowsCount;

        private bool IsDisposed = false;

        private Container components = null;

        private RetrieveData m_retrieveData = new RetrieveCICategoryData();

        public string CICategoryID
        {
            get
            {
                return m_cicategory;
            }
            set
            {
                m_cicategory = value;
            }
        }

        public string CategoryName
        {
            get
            {
                return m_categoryname;
            }
            set
            {
                m_categoryname = value;
            }
        }

        public string CategoryOwner
        {
            get
            {
                return m_categoryowner;
            }
            set
            {
                m_categoryowner = value;
            }
        }

        public string AllowParentCategory
        {
            get
            {
                return m_allow_parentcategory;
            }
            set
            {
                m_allow_parentcategory = value;
            }
        }

        public string AllowChildCategory
        {
            get
            {
                return m_allow_childcategory;
            }
            set
            {
                m_allow_childcategory = value;
            }
        }

        public string CategoryManager
        {
            get
            {
                return m_categorymanager;
            }
            set
            {
                m_categorymanager = value;
            }
        }

        public bool Enable
        {
            get
            {
                return m_enable;
            }
            set
            {
                m_enable = value;
            }
        }

        public bool IsRootCategory
        {
            get
            {
                return m_isrootcategory;
            }
            set
            {
                m_isrootcategory = value;
            }
        }

        public CICategory()
        {
        }

        public CICategory(string CISID)
        {
            if (CISID != "")
            {
                m_Result = GetCICategoryData(CISID);
            }
            else
            {
                m_Result = false;
            }

            Trace.WriteLine("Retrieve CICategory Data : " + m_Result);
        }

        ~CICategory()
        {
            Trace.WriteLine("Destructor CICategory Class.");
            Dispose(Disposing: false);
        }

        public void Dispose()
        {
            Dispose(Disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (!IsDisposed && Disposing)
            {
                m_cicategory = null;
                m_categoryname = null;
                m_categoryowner = null;
                m_allow_parentcategory = null;
                m_allow_childcategory = null;
                m_categorymanager = null;
                m_enable = false;
                m_isrootcategory = false;
                if (components != null)
                {
                    components.Dispose();
                }
            }

            IsDisposed = true;
            Trace.WriteLine("Dispose CICategory Class.");
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

        private bool GetCICategoryData(string sData)
        {
            DataTable dataTable = m_retrieveData.RetrieveDBData(sData);
            m_rowsCount = dataTable.Rows.Count;
            if (m_rowsCount == 0)
            {
                return false;
            }

            Trace.WriteLine("Retrieve Rows Count : " + dataTable.Rows.Count);
            m_cicategory = dataTable.Rows[0]["cicategory"].ToString();
            m_categoryname = dataTable.Rows[0]["categoryname"].ToString();
            m_categoryowner = dataTable.Rows[0]["categoryowner"].ToString();
            m_allow_parentcategory = dataTable.Rows[0]["allow_parentcategory"].ToString();
            m_allow_childcategory = dataTable.Rows[0]["allow_childcategory"].ToString();
            m_categorymanager = dataTable.Rows[0]["categorymanager"].ToString();
            m_enable = ((dataTable.Rows[0]["enable"].ToString() == "Y") ? true : false);
            m_isrootcategory = ((dataTable.Rows[0]["isrootcategory"].ToString() == "Y") ? true : false);
            Trace.WriteLine("CICategory: " + m_cicategory.ToString());
            Trace.WriteLine("CategoryName : " + m_categoryname);
            Trace.WriteLine("CategoryOwner : " + m_categoryowner);
            Trace.WriteLine("AllowParentCategory : " + m_allow_parentcategory);
            Trace.WriteLine("AllowChildCategory : " + m_allow_childcategory);
            Trace.WriteLine("CategoryManager : " + m_categorymanager);
            Trace.WriteLine("Enable : " + m_enable);
            Trace.WriteLine("IsRootCategory : " + m_isrootcategory);
            dataTable.Dispose();
            return true;
        }
    }
}
