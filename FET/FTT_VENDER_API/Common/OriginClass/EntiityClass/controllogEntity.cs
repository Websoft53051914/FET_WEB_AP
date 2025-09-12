namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class controllogEntity
    {
        public string ID { get; set; }
        public DateTime LogTime { get; set; }
        public string IP { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Exception { get; set; }
        public string Status { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }

    public class controllogDTO : controllogEntity
    {
        public int No { get; set; }
    }


}
