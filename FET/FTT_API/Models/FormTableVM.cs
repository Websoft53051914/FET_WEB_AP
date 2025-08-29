using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using System.Drawing;

namespace FTT_API.Models
{
    public class FormTableVM
    {
        public string ActionName { get; set; }
        public string RequireField { get; set; }
        public string UpdateField { get; set; }
        public string PreHandleDesc { get; set; }
        public string ApprovalDate { get; set; }
        public string WarrantyTime { get; set; }
        public Color WarrantyTimeForeColor { get; set; }
        public string FranchiseMsg { get; set; }
        public string Create_Time { get; set; }
        public bool hasTT_IMAGE { get; set; }
        public string newImageSRC { get; set; }
        public string storename { get; set; }
        


        public StoreClass store_profileDTO { get; set; }
        public ftt_formDTO ftt_formDTO { get; set; }

    }
}
