namespace FTT_API.Common
{
    public class JwtConfigVO
    {
        public string Secret { get; set; }

        public string ExpireTimeDuration { get; set; }

        public string Issuer { get; set; }

        public JwtConfigVO()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, false).Build();
            Secret = config["JwtConfig:Secret"]??string.Empty;
            ExpireTimeDuration = config["JwtConfig:ExpireTimeDuration"] ?? string.Empty;
            Issuer = config["JwtConfig:Issuer"] ?? string.Empty;
        }
    }
}
