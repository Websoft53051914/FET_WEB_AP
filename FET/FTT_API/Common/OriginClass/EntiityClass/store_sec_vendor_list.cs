using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class store_sec_vendor_listEntity
    {
        public string ivrcode { get; set; }

        public decimal vendor_id { get; set; }

        public DateTime? createtime { get; set; }

        public DateTime? update_time { get; set; }

        public string create_opid { get; set; }

        public string update_opid { get; set; }

        public string enable { get; set; } = "Y";
    }

    public class store_sec_vendor_listDTO : store_sec_vendor_listEntity
    {
        public string shop_name { get; set; }
        public string merchant_name { get; set; }
        public string cp_name { get; set; }
        public string cp_tel { get; set; }



        public int No { get; set; }

        public string USERROLE { get; set; }
        public string EMPNO { get; set; }
        //public string IVRCODE { get; set; }
    }
}
