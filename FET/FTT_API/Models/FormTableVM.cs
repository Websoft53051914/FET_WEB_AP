using FTT_API.Common.OriginClass;
using FTT_API.Common.OriginClass.EntiityClass;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public string Form { get; set; }
        public string Status { get; set; }
        public string Status_Desc { get; set; }


        public bool ShowApproveCommon { get; set; }
        public bool ShowPriorStatus { get; set; }
        public string StatusWording { get; set; }
        public string Form_Type { get; set; }

        public string BtnSubmitName { get; set; }

        public string User_Type { get; set; }


        public string Approve { get; set; }

        public bool ApproveY { get; set; }
        public bool ShowSubmitButton { get; set; }
        //public bool ShowTicketInfo { get; set; }
        public bool ShowOriginSubmitForm { get; set; }

        public bool ShowAmount { get; set; }
        public bool UpdateSELECTSTATUSOption2 { get; set; }
        public string ApproveForm { get; set; }

        public bool DeleteSELECTSTATUSOption3 { get; set; }

        public string TempStatus { get; set; }
        public bool DeleteSELECTSTATUS2ThreeTimes { get; set; }

        public bool ShowAmountPanel { get; set; }

        public string Total { get; set; }

        public bool HideAmountDel { get; set; }
        public bool HideNewData { get; set; }

        public bool Ifwarrant { get; set; } = true;

        public string Category_Id { get; set; }
        public string Amount_Cost { get; set; }
        public string Amount_Config { get; set; }
        public bool UpdateAmount_Config { get; set; }


        public List<SelectListItem> Amount_SelectList { get; set; }

        public StoreClass Store_profileDTO { get; set; }
        public Ftt_formDTO Ftt_formDTO { get; set; }
        public List<Ftt_form_amountDTO> Ftt_form_amountDTOs { get; set; }

    }
}
