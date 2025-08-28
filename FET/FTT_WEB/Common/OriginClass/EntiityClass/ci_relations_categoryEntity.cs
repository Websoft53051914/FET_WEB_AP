using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class ci_relations_categoryEntity
    {
        public decimal cisid { get; set; }

        public string descr { get; set; }

        public string notes { get; set; }

        public string picture_path { get; set; }

        public string actype { get; set; }

        public decimal? kpitime { get; set; } = 1;

        public string selfconfig { get; set; } = "N";
    }

    public class ci_relations_categoryDTO : ci_relations_categoryEntity
    {
        public int No { get; set; }

    }


}
