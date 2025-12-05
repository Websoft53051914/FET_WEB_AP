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

		public int? order_id { get; set; }

		public int? login_count { get; set; }

		public string? locked { get; set; }

		public TimeSpan? kpi_days { get; set; }
    }
}
