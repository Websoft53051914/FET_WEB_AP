using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class amount_selectEntity
    {
        public decimal id { get; set; }

        public decimal? category_id { get; set; }

        public string category_name { get; set; }

        public string expense_type { get; set; }

        public string l1_desc { get; set; }

        public string l2_desc { get; set; }

        public string l3_desc { get; set; }

        public string unit { get; set; }

        public decimal? qty { get; set; }   // numeric(5,1)

        public decimal? price { get; set; }

        public string remark { get; set; }

        public string enable { get; set; } = "Y";

        public DateTime create_time { get; set; }

        public DateTime update_time { get; set; }

        public string modify_operator { get; set; }
    }

    public class amount_selectDTO : amount_selectEntity
    {

    }


}
