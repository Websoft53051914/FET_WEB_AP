namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class form_access_statusEntity
    {
        public string form_type { get; set; }

        public string status { get; set; }

        public string status_name { get; set; }
    }

    public class form_access_statusDTO : form_access_statusEntity
    {
        public string StatusId { get; set; }
        public int No { get; set; }
    }


}
