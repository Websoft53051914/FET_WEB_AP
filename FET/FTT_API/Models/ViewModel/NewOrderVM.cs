namespace FTT_API.Models.ViewModel
{
    public class NewOrderVM
    {
        public string? IVRCODE { get; set; }
        public string? EMPNAME { get; set; }
        public string? EMPTEL { get; set; }
        public string? TT_CATEGORY { get; set; }
        public string? CATEGORY_NAME_TMP { get; set; }
        public string? ChooseCIButton { get; set; }
        public string? REPAIR { get; set; }
        public string? RESUPPLY { get; set; }
        public string? SELFCONFIG { get; set; }
        public StoreVM StoreVM { get; set; } = new();
        public string CREATE_TIME { get; set; } = string.Empty;
        public string APPROVALDATE { get; set; } = string.Empty;
        public string WARRANTYTIME { get; set; } = string.Empty;
        public bool WarrantyTimeFlag1 { get; set; } = false;
        public bool WarrantyTimeFlag2 { get; set; } = false;
        public string Prompt { get; set; } = string.Empty;
        public NewOrderTTItemVM TemplateItem { get; set; } = new();
        public List<NewOrderTTItemVM> TTItemList { get; set; } = [];
    }

    public class NewOrderTTItemVM
    {
        public string? CATEGORY_ID { get; set; }
        public string? CATEGORY_NAME { get; set; }
        public string? VENDER_ID { get; set; }
        public string? VENDER_NAME { get; set; }
        public List<string> ITEMNOTE { get; set; } = [];
        public string? ItemNoteVal { get; set; }
        public List<string> ITEMDESC { get; set; } = [];
        public string? ItemDescVal { get; set; }
        public string? REMARK { get; set; }
    }
}
