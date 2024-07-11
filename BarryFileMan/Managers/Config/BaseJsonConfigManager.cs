using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading.Tasks;

namespace BarryFileMan.Managers.Config
{
    public class BaseJsonConfigManager<T> : IConfigManager<T> where T : new()
    {
        private static readonly string _executableFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        private static readonly string? _executableDirectory = Path.GetDirectoryName(_executableFilePath);
        private static readonly string _portableFolderPath = Path.Combine(_executableDirectory ?? string.Empty, "user");
        private static readonly string _appDatafolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BarryFileMan");

        private readonly T _defaultConfig = new();
        private readonly BehaviorSubject<(T config, string? key)> _configSubject = new(new());

        public string FolderPath => Directory.Exists(_portableFolderPath) ? _portableFolderPath : _appDatafolderPath;
        public string FilePath => Path.Combine(FolderPath, FileName);

        protected virtual string FileName => "config.json";

        public IObservable<(T config, string? key)> ConfigObservable => _configSubject.AsObservable();
        public T Config => _configSubject.Value.config;

        public BaseJsonConfigManager()
        {
            UpdateConfigSubject(GetConfig());
        }

        public T GetConfig()
        {
            string configString = string.Empty;
            T? config = default;
            try
            {
                if (File.Exists(FilePath))
                {
                    configString = File.ReadAllText(FilePath);
                }

                if (!string.IsNullOrWhiteSpace(configString))
                {
                    config = JsonSerializer.Deserialize<T>(configString);
                }
                else
                {
                    SetConfig(_defaultConfig);
                }
            }
            catch { }

            return config ?? _defaultConfig;
        }

        public async Task<T> GetConfigAsync()
        {
            string configString = string.Empty;
            T? config = default;
            try
            {
                if (File.Exists(FilePath))
                {
                    configString = await File.ReadAllTextAsync(FilePath);
                }

                if (!string.IsNullOrWhiteSpace(configString))
                {
                    config = JsonSerializer.Deserialize<T>(configString);
                }
                else
                {
                    await SetConfigAsync(_defaultConfig);
                }
            }
            catch { }

            return config ?? _defaultConfig;
        }

        public T RestoreDefaultConfig()
        {
            return SetConfig(_defaultConfig);
        }

        public async Task<T> RestoreDefaultConfigAsync()
        {
            return await SetConfigAsync(_defaultConfig);
        }

        public T SetConfig(T config, string? key = null)
        {
            try
            {
                HandleDirectoryCreation();
                File.WriteAllText(FilePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
                UpdateConfigSubject(config, key);
            }
            catch { }
            return config;
        }

        public async Task<T> SetConfigAsync(T config, string? key = null)
        {
            try
            {
                HandleDirectoryCreation();
                await File.WriteAllTextAsync(FilePath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
                UpdateConfigSubject(config, key);
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

        private void UpdateConfigSubject(T config, string? key = null)
        {
            _configSubject.OnNext((config, key));
        }
    }
}
