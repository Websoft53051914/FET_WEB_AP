namespace FTT_VENDER_API.Models.ViewModel.StoreVenderProfile
{
    public class StoreVenderProfileVM
    {

        public string? merchant_name { get; set; }
        public string? cp_name { get; set; }
        public string? cp_tel { get; set; }
        public string? email { get; set; }
        public string? construction_category { get; set; }
        public string? merchant_login { get; set; }
        public string? tempcolumn { get; set; }

        public int? order_id { get; set; }

        public int? login_count { get; set; }

        public string? locked { get; set; }
        public DateTime locked_time { get; set; }
        public short? locked_reason { get; set; }
        public TimeSpan? kpi_days { get; set; }


        public DateTime LastUrlTime { get; set; }
        public string? LastUrlKey { get; set; }
        public string? MERCHANT_PASSWORD { get; set; }

        public DateTime pw_chgtime { get; set; }
        
    }
}
