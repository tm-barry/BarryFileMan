using BarryFileMan.Models.Config;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BarryFileMan.Managers.Config
{
    public class JsonUserConfigManager : IConfigManager<UserConfig>
    {
        private readonly string _filePath = "user.json";
        private readonly UserConfig _defaultUserConfig = new();

        public UserConfig Config { get; private set; }

        public JsonUserConfigManager()
        {
            Config = GetConfig();
        }

        public UserConfig GetConfig()
        {
            string configString = string.Empty;
            UserConfig? config = null;
            try
            {
                if (File.Exists(_filePath))
                {
                    configString = File.ReadAllText(_filePath);
                }

                if (!string.IsNullOrWhiteSpace(configString))
                {
                    config = JsonSerializer.Deserialize<UserConfig>(configString);
                }
                else
                {
                    SetConfig(_defaultUserConfig);
                }
            }
            catch { }

            return config ?? _defaultUserConfig;
        }

        public async Task<UserConfig> GetConfigAsync()
        {
            string configString = string.Empty;
            UserConfig? config = null;
            try
            {
                if (File.Exists(_filePath))
                {
                    configString = await File.ReadAllTextAsync(_filePath);
                }

                if (!string.IsNullOrWhiteSpace(configString))
                {
                    config = JsonSerializer.Deserialize<UserConfig>(configString);
                }
                else
                {
                    await SetConfigAsync(_defaultUserConfig);
                }
            }
            catch { }

            return config ?? _defaultUserConfig;
        }

        public UserConfig RestoreDefaultConfig()
        {
            return SetConfig(_defaultUserConfig);
        }

        public async Task<UserConfig> RestoreDefaultConfigAsync()
        {
            return await SetConfigAsync(_defaultUserConfig);
        }

        public UserConfig SetConfig(UserConfig config)
        {
            File.WriteAllText(_filePath, JsonSerializer.Serialize(config));
            Config = config;
            return config;
        }

        public async Task<UserConfig> SetConfigAsync(UserConfig config)
        {
            await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(config));
            Config = config;
            return config;
        }
    }
}
