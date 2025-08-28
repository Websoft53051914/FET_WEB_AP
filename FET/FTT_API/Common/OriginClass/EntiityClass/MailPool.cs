namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class MailPool
    {
        public int Id {  get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public DateTime EstimateSendTime { get; set; }
        public DateTime? RealSendTime { get; set; }

        public int? SendStatus { get; set; }
        public string? ErrorMsg { get; set; }
        public int? Status { get; set; }

        public string? DestinationEmail { get; set; }

        public int? Creator {  get; set; }
        public DateTime? CreateTime { get; set; }

        public int? Updater { get; set; }
        public DateTime? UpdateTime { get; set; }

    }
}
