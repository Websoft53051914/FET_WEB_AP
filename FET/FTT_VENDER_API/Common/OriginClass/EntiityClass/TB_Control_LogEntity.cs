namespace FTT_VENDER_API.Common.OriginClass.EntiityClass
{
    public class TB_Control_LogEntity
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
        public string? Token { get; set; }
    }

    public class TB_Control_LogDTO : TB_Control_LogEntity
    {
        public int No { get; set; }
    }


}
