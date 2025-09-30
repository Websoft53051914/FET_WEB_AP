using Const;

namespace FTT_API.Common.ConfigurationHelper
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

        /// <summary>
        /// 取得 OutputPath(匯入檔案存放位置) 參數
        /// </summary>
        /// <returns></returns>
        public string GetOutputPath()
        {
            return _configuration["OutputPath"] ?? string.Empty;
        }

    }
}
