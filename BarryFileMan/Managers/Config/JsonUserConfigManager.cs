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
        private static readonly string _portableFolderPath = "user";
        private static readonly string _appDatafolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BarryFileMan");
        private static readonly string _fileName = "settings.json";

        private readonly UserConfig _defaultUserConfig = new();
        private readonly Subject<UserConfig> _configSubject = new();

        private string FolderPath => Directory.Exists(_portableFolderPath) ? _portableFolderPath : _appDatafolderPath;
        private string FilePath => Path.Combine(FolderPath, _fileName);

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
                if (File.Exists(FilePath))
                {
                    configString = File.ReadAllText(FilePath);
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
                if (File.Exists(FilePath))
                {
                    configString = await File.ReadAllTextAsync(FilePath);
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
                HandleDirectoryCreation();
                File.WriteAllText(FilePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
                UpdateConfigSubject(config);
            }
            catch { }
            return config;
        }

        public async Task<UserConfig> SetConfigAsync(UserConfig config)
        {
            try
            {
                HandleDirectoryCreation();
                await File.WriteAllTextAsync(FilePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
                UpdateConfigSubject(config);
            }
            catch { }
            return config;
        }

        private void HandleDirectoryCreation()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }

        private void UpdateConfigSubject(UserConfig config)
        {
            Config = config;
            _configSubject.OnNext(config);
        }
    }
}
