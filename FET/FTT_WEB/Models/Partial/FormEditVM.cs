using FTT_WEB.Models.ViewModel;

namespace FTT_WEB.Models.Partial
{
    public class FormEditVM
    {
        public string FormNo { get; set; }
        public bool IsEdit { get; set; } = true;
        public NewOrderVM NewOrder { get; set; }
    }
}
