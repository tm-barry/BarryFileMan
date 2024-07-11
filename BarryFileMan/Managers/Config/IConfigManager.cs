using BarryFileMan.Models.Config;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace BarryFileMan.Managers.Config
{
    public interface IConfigManager<T>
    {
        public IObservable<(T config, string? key)> ConfigObservable { get; }
        public T Config { get; }
        public string FolderPath { get; }
        public string FilePath { get; }

        public T SetConfig(T config, string? key = null);
        public Task<T> SetConfigAsync(T config, string? key = null);
        public T GetConfig();
        public Task<T> GetConfigAsync();
        public T RestoreDefaultConfig();
        public Task<T> RestoreDefaultConfigAsync();
    }
}
