using FTT_VENDER_API.Common.OriginClass.EntiityClass;

namespace FTT_VENDER_API.Models.Partial
{
    public class FormMaintainVM
    {
        public string form_no { get; set; }
        public string StatusId { get; set; }

        public List<v_access_roleView> vms { get; set; }
    }
}
