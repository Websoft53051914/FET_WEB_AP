using FTT_API.Models.ViewModel;

namespace FTT_API.Models.Partial
{
    public class FormEditVM
    {
        public string FormNo { get; set; }
        public bool IsEdit { get; set; } = true;
        public NewOrderVM NewOrder { get; set; }
    }
}
