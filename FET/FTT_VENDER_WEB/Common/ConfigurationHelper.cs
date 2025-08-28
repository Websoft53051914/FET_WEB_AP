using Const;

namespace FTT_VENDER_WEB.Common.ConfigurationHelper
{
    public class ConfigurationHelper
    {
        public ConfigurationHelper(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public IConfiguration Config => _configuration;

        public string GetMessage(string key, string defaultVal = "")
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return defaultVal;
            }

            return _configuration[$"Message:{LocaleConst.ZH_TW}:{key}"] ?? defaultVal;
        }

    }
}
