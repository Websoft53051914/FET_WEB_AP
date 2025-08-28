using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class ftt_formEntity
    {
        public string form_no { get; set; }
        public string ivrcode { get; set; }
        public string category_id { get; set; }
        public string category_name { get; set; }
        public string createtime { get; set; }
        public string empno { get; set; }
        public string empname { get; set; }
        public string emptel { get; set; }
        public string descr { get; set; }
        public string checkitem { get; set; }
        public string closedate { get; set; }
        public string completetime { get; set; }
        public string dispatchtime { get; set; }
        public string tt_category { get; set; }
        public string order_id { get; set; }
        public string tt_no { get; set; }
        public string remark { get; set; }
        public string vender_id { get; set; }
        public string tt_type { get; set; }
        public string change_type { get; set; }
        public string approve_after { get; set; }
        public string price { get; set; }
        public string qty { get; set; }
        public string updatename { get; set; }
        public string ticket_info { get; set; }
        public string precompletetime { get; set; }
        public string repair { get; set; }
        public string resupply { get; set; }
        public string repair_action { get; set; }
        public string vendor_arrive_date { get; set; }
        public string selfconfig { get; set; }
        public string delay_reason { get; set; }
    }

    public partial class ftt_formDTO : ftt_formEntity
    {
        public string cp_name { get; set; }
        public string cp_tel { get; set; }
        public string merchant_name { get; set; }

        public string CIDesc { get; set; }
        public int No { get; set; }
    }


    public partial class ftt_formDTO : ftt_formEntity
    {
        public bool updateCOMPLETETIME { get; set; }
        public bool updatePRECOMPLETETIME { get; set; }

        public string DESCRIPTION { get; set; }
        public string STATUS { get; set; }
        public string FORM_TYPE { get; set; }


        public string TT_COUNT { get; set; }

    }
}
