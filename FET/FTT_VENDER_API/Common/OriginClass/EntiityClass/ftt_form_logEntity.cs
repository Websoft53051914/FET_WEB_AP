namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class ftt_form_logEntity
    {
        public decimal? form_no { get; set; }

        public string oldvalue { get; set; } = "<<NULL>>";

        public string newvalue { get; set; } = "<<NULL>>";

        public string update_empno { get; set; } = "SYSTEM";

        public string update_engname { get; set; }

        public string fieldname { get; set; }

        public DateTime? updatetime { get; set; }

        public string action { get; set; } = "CHANGE";

        public string change_reason { get; set; }

        public string form_type { get; set; }

        public decimal? root_no { get; set; }
    }

    public class ftt_form_logDTO : ftt_form_logEntity
    {
        public int No { get; set; }
        public string RowCount { get; set; }



    }


}
