namespace FTT_API.Models.ViewModel.MailServerSetting
{
    public class MailServerSettingVM
    {
        public int Id { get; set; }
        public string? Server { get; set; }
        public string? Port { get; set; }
        public string? Sender { get; set; }
        public string? SenderAddress { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool EnableSSL { get; set; }
        public bool IsEnabled { get; set; }


    }
}
