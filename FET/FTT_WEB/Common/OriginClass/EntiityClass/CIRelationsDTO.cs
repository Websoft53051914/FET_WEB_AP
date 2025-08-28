namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class CIRelationsDTO
    {
        public int CISID { get; set; }
        public string? CINAME { get; set; }
        public string? ACINAME { get; set; }
        public string? CICATEGORY { get; set; }

        public string? FULLNAME { get; set; }
        public string? NOTES { get; set; }
        public string? DESCR { get; set; }
        public bool HasChildren { get; set; } 
    }
}
