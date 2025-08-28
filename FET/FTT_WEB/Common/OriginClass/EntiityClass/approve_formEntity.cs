using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class approve_formEntity
    {
        public string form_type { get; set; }
        public string form_no { get; set; }
        public string status { get; set; }
        public string updatetime { get; set; }
        public string prior_status { get; set; }
        public string update_empno { get; set; }
        public string update_engname { get; set; }
        public string status_orderid { get; set; }
        public string root_no { get; set; }
    }

    public class approve_formDTO : approve_formEntity
    {
        public int No { get; set; }
        public string STATUS_NAME { get; set; }
        
    }


}
