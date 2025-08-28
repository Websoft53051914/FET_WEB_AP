using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class store_profileEntity
    {

        public string company_leaves { get; set; }

        public string store_type { get; set; }

        public string channel { get; set; }

        public string area { get; set; }

        public string shop_name { get; set; }

        public string ivr_code { get; set; }

        public string email { get; set; }

        public string owner_cname { get; set; }

        public string owner_ename { get; set; }

        public string as_empno { get; set; }

        public string as_cname { get; set; }

        public string as_ename { get; set; }

        public string owner_tel { get; set; }

        public string urgent_tel { get; set; }

        public string address { get; set; }

        public string decoration_condition { get; set; }

        public DateTime? approval_date { get; set; }

        public string note { get; set; }

        public string note_owner { get; set; }

        public DateTime? note_date { get; set; }

        public string shop_password { get; set; }

        public string business_hour_range1 { get; set; }

        public string owner_empno { get; set; }

        public string business_hour_range2 { get; set; }

        public string business_hour_range3 { get; set; }

        public string business_hour_range4 { get; set; }

        public string fax_tel { get; set; }

        public DateTime? updatetime { get; set; }
    }

    public class store_profileDTO : store_profileEntity
    {

    }


}
