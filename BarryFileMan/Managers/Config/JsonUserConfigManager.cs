using BarryFileMan.Models.Config;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading.Tasks;

namespace BarryFileMan.Managers.Config
{
    public class JsonUserConfigManager : IConfigManager<UserConfig>
    {
        private readonly string _filePath = "user.json";
        private readonly UserConfig _defaultUserConfig = new();

        private Subject<UserConfig> _configSubject = new Subject<UserConfig>();
        public IObservable<UserConfig> ConfigObservable => _configSubject.AsObservable();
        public UserConfig Config { get; private set; }

        public JsonUserConfigManager()
        {
            Config = _defaultUserConfig;
            UpdateConfigSubject(GetConfig());
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
            try
            {
                File.WriteAllText(_filePath, JsonSerializer.Serialize(config));
                UpdateConfigSubject(config);
            }
            catch { }
            return config;
        }

        public async Task<UserConfig> SetConfigAsync(UserConfig config)
        {
            try
            {
                await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(config));
                UpdateConfigSubject(config);
            }
            catch { }
            return config;
        }

        public UserConfig UpdateTheme(Theme theme)
        {
            Config.Theme = theme;
            return SetConfig(Config);
        }

        public async Task<UserConfig> UpdateThemeAsync(Theme theme)
        {
            Config.Theme = theme;
            return await SetConfigAsync(Config);
        }

        private void UpdateConfigSubject(UserConfig config)
        {
            Config = config;
            _configSubject.OnNext(config);
        }
    }
}
