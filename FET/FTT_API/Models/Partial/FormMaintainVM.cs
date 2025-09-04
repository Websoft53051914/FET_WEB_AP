using FTT_API.Common.OriginClass.EntiityClass;
using FTT_API.Models.ViewModel;

namespace FTT_API.Models.Partial
{
    public class FormMaintainVM
    {
        public string form_no { get; set; }
        public string StatusId { get; set; }

        public List<v_access_roleView> vms { get; set; }
    }
}
