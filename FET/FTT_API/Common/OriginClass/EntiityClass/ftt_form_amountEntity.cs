using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class Ftt_form_amountEntity
    {
        public decimal form_no { get; set; }

        public string expense_type { get; set; }

        public string expense_desc { get; set; }

        public string qty { get; set; }

        public string price { get; set; }

        public string subtotal { get; set; }

        public decimal? orderid { get; set; }

        public string enable { get; set; } = "Y";

        public DateTime? disable_time { get; set; }

        public DateTime? create_time { get; set; }

        public string unit { get; set; }

        public string fault_reason { get; set; }

        public string repair_action { get; set; }
    }

    public class Ftt_form_amountDTO : Ftt_form_amountEntity
    {

    }


}
