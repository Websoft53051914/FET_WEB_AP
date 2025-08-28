namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class MailServerSetting
    {
        public int Id { get; set; }
        public string? Server { get; set; }
        public string? Port { get; set; }
        public string? Sender { get; set; }
        public string? SenderAddress { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool? EnableSSL { get; set; }
        public int? Status { get; set; }

        public int? Creator { get; set; }
        public DateTime? CreateTime { get; set; }

        public int? Updater { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
