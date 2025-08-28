using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class ci_exception_configEntity
    {
        public decimal cisid { get; set; }

        public decimal vendor_id { get; set; }

        public string ivrcode { get; set; }

        public DateTime? approval_date { get; set; }

        public DateTime? createtime { get; set; }

        public string update_opid { get; set; }

        public DateTime? updatetime { get; set; }

        public string enable { get; set; } = "Y";
    }

    public class ci_exception_configDTO : ci_exception_configEntity
    {

    }


}
