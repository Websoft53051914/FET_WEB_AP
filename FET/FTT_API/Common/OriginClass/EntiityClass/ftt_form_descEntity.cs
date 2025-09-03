using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class ftt_form_descEntity
    {

        public decimal? form_no { get; set; }

        public DateTime? create_date { get; set; }

        public string User_Type { get; set; }

        public string action_name { get; set; }

        public string description { get; set; }

        public string prior_status { get; set; }

        public string status { get; set; }

        public string description_1 { get; set; }
    }

    public class ftt_form_descDTO : ftt_form_descEntity
    {
        public int No { get; set; }
        public string TT_LAST_DESC { get; set; }
    }


}
