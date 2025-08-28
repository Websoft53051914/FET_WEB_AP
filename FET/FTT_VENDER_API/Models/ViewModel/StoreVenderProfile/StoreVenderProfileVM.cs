namespace FTT_VENDER_API.Models.ViewModel.StoreVenderProfile
{
    public class StoreVenderProfileVM
    {
 //       merchant_name varchar(30) NULL,
	//cp_name varchar(20) NULL,
	//cp_tel varchar(150) NULL,
	//email varchar(500) NULL,
	//construction_category varchar(10) NULL,
	//merchant_login varchar(20) NULL,
	//merchant_password varchar(10) NULL,
	//order_id numeric(10) NOT NULL,
	//login_count numeric DEFAULT 1 NULL,
	//"locked" varchar(1) DEFAULT 'N'::character varying NULL,
	//kpi_days interval NULL,

		public string? merchant_name { get; set; }
		public string? cp_name { get; set; }
		public string? cp_tel { get; set; }
		public string? email { get; set; }
		public string? construction_category { get; set; }
		public string? merchant_login { get; set; }
		public string? merchant_password { get; set; }

		public int? order_id { get; set; }

		public int? login_count { get; set; }

		public string? locked { get; set; }

		public TimeSpan? kpi_days { get; set; }
    }
}
