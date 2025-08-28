using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass
{
    public class StoreClass
    {
        private string mIVRCode = "";
        private string mCompany = "";
        private string mStoreType = "";
        private string mChannel = "";
        private string mArea = "";
        private string mStoreName = "";
        private string mEMail = "";
        private string mOwner = "";
        private string mManager = "";
        private string mManagerEmpno = "";
        private string mPhone = "";
        private string mPhoneUrgent = "";
        private string mPhoneFax = "";
        private string mAddress = "";
        private string mBusinessTime1 = "";
        private string mBusinessTime2 = "";
        private string mBusinessTime3 = "";
        private string mBusinessTime4 = "";
        private string mDecoration = "";
        private string mNote = "";
        private string mApprovalDate = "";
        private int m_rowsCount = 0;

        public StoreClass()
        {
            //
            // TODO: 在此加入建構函式的程式碼
            //
        }

        public StoreClass(string IVRCode)
        {
            mIVRCode = IVRCode;
            this.GetData();
        }

        #region Public Properties

        /// <summary>
        /// 公司別
        /// </summary>
        public string Company
        {
            get
            {
                return mCompany;
            }
            set
            {
                mCompany = value;
            }
        }

        /// <summary>
        /// 門市類型
        /// </summary>
        public string StoreType
        {
            get
            {
                return mStoreType;
            }
            set
            {
                mStoreType = value;
            }
        }

        /// <summary>
        /// 通路
        /// </summary>
        public string Channel
        {
            get
            {
                return mChannel;
            }
            set
            {
                mChannel = value;
            }
        }

        /// <summary>
        /// 區域
        /// </summary>
        public string Area
        {
            get
            {
                return mArea;
            }
            set
            {
                mArea = value;
            }
        }

        /// <summary>
        /// 店名
        /// </summary>
        public string StoreName
        {
            get
            {
                return mStoreName;
            }
            set
            {
                mStoreName = value;
            }
        }

        /// <summary>
        /// IVR Code
        /// </summary>
        public string IVRCode
        {
            get
            {
                return mIVRCode;
            }
            set
            {
                mIVRCode = value;
                this.GetData();
            }
        }

        /// <summary>
        /// eMail
        /// </summary>
        public string eMail
        {
            get
            {
                return mEMail;
            }
            set
            {
                mEMail = value;
            }
        }

        /// <summary>
        /// 店長
        /// </summary>
        public string Owner
        {
            get
            {
                return mOwner;
            }
            set
            {
                mOwner = value;
            }
        }

        /// <summary>
        /// 區經理
        /// </summary>
        public string Manager
        {
            get
            {
                return mManager;
            }
            set
            {
                mManager = value;
            }
        }

        /// <summary>
        /// 區經理員編
        /// </summary>
        public string ManagerEmpno
        {
            get
            {
                return mManagerEmpno;
            }
            set
            {
                mManagerEmpno = value;
            }
        }

        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string Phone
        {
            get
            {
                return mPhone;
            }
            set
            {
                mPhone = value;
            }
        }

        /// <summary>
        /// 緊急聯絡電話
        /// </summary>
        public string PhoneUrgent
        {
            get
            {
                return mPhoneUrgent;
            }
            set
            {
                mPhoneUrgent = value;
            }
        }

        /// <summary>
        /// 傳真電話
        /// </summary>
        public string PhoneFax
        {
            get { return mPhoneFax; }
            set { mPhoneFax = value; }
        }

        /// <summary>
        /// 住址
        /// </summary>
        public string Address
        {
            get
            {
                return mAddress;
            }
            set
            {
                mAddress = value;
            }
        }

        /// <summary>
        /// 週一至週五
        /// </summary>
        public string BusinessTime1
        {
            get
            {
                return mBusinessTime1;
            }
            set
            {
                mBusinessTime1 = value;
            }
        }

        /// <summary>
        /// 週六
        /// </summary>
        public string BusinessTime2
        {
            get
            {
                return mBusinessTime2;
            }
            set
            {
                mBusinessTime2 = value;
            }
        }

        /// <summary>
        /// 週日
        /// </summary>
        public string BusinessTime3
        {
            get
            {
                return mBusinessTime3;
            }
            set
            {
                mBusinessTime3 = value;
            }
        }

        /// <summary>
        /// 國定假日
        /// </summary>
        public string BusinessTime4
        {
            get
            {
                return mBusinessTime4;
            }
            set
            {
                mBusinessTime4 = value;
            }
        }

        /// <summary>
        /// 裝潢型態
        /// </summary>
        public string DecorationCondition
        {
            get { return mDecoration; }
            set { mDecoration = value; }
        }

        /// <summary>
        /// 備註
        /// </summary>
        public string Note
        {
            get { return mNote; }
            set { mNote = value; }
        }

        /// <summary>
        /// 驗收日
        /// </summary>
        public string ApprovalDate
        {
            get { return mApprovalDate; }
            set { mApprovalDate = value; }
        }

        #endregion

        /// <summary>
        /// 檢查是否有資料存在
        /// </summary>
        /// <returns>存在或不存在</returns>
        public bool hasData()
        {
            if (m_rowsCount > 0)
                return true;
            else
                return false;
        }

        private void GetData()
        {
            if (mIVRCode != "")
            {
                BaseDBHandler baseHandler = new BaseDBHandler();
                DataTable storeData = baseHandler.GetDBHelper().FindDataTable("SELECT * FROM STORE_PROFILE WHERE IVR_CODE='" + mIVRCode + "'", null);
                m_rowsCount = storeData.Rows.Count;
                if (storeData.Rows.Count == 1)
                {
                    mCompany = storeData.Rows[0]["COMPANY_LEAVES"].ToString();
                    mStoreType = storeData.Rows[0]["STORE_TYPE"].ToString();
                    mChannel = storeData.Rows[0]["CHANNEL"].ToString();
                    mArea = storeData.Rows[0]["AREA"].ToString();
                    mStoreName = storeData.Rows[0]["SHOP_NAME"].ToString();
                    mEMail = storeData.Rows[0]["EMAIL"].ToString();
                    mOwner = storeData.Rows[0]["OWNER_CNAME"].ToString();
                    mManager = storeData.Rows[0]["AS_CNAME"].ToString();
                    mManagerEmpno = storeData.Rows[0]["AS_EMPNO"].ToString();
                    mPhone = storeData.Rows[0]["OWNER_TEL"].ToString();
                    mPhoneUrgent = storeData.Rows[0]["URGENT_TEL"].ToString();
                    mPhoneFax = storeData.Rows[0]["FAX_TEL"].ToString();
                    mAddress = storeData.Rows[0]["ADDRESS"].ToString();
                    mBusinessTime1 = storeData.Rows[0]["BUSINESS_HOUR_RANGE1"].ToString();
                    mBusinessTime2 = storeData.Rows[0]["BUSINESS_HOUR_RANGE2"].ToString();
                    mBusinessTime3 = storeData.Rows[0]["BUSINESS_HOUR_RANGE3"].ToString();
                    mBusinessTime4 = storeData.Rows[0]["BUSINESS_HOUR_RANGE4"].ToString();
                    mDecoration = storeData.Rows[0]["DECORATION_CONDITION"].ToString();
                    mNote = storeData.Rows[0]["NOTE"].ToString();
                    mApprovalDate = storeData.Rows[0]["APPROVAL_DATE"].ToString();
                }
                else
                {
                    if (storeData.Rows.Count > 1)
                        throw new Exception("以 IVR Code [" + mIVRCode + "] 搜尋出來門市資料太多，請檢視資料是否正確！");
                }

                storeData.Dispose();
            }
        }
    }
}
