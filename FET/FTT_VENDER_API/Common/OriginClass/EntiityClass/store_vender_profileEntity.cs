namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class store_vender_profileEntity
    {
        public string merchant_name { get; set; }

        public string cp_name { get; set; }

        public string cp_tel { get; set; }

        public string email { get; set; }

        public string construction_category { get; set; }

        public string merchant_login { get; set; }

        public string merchant_password { get; set; }

        public decimal order_id { get; set; }   // PK，不可為 NULL

        public decimal? login_count { get; set; } = 1;

        public string locked { get; set; } = "N";

        public TimeSpan? kpi_days { get; set; }   // PostgreSQL interval → C# TimeSpan

        public short? locked_reason { get; set; }

        public TimeSpan? pw_chgtime { get; set; }   // time without time zone
    }

    public class store_vender_profileDTO : store_vender_profileEntity
    {
        public int No { get; set; }
    }


}
