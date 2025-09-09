namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
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
