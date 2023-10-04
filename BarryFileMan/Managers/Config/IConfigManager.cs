using BarryFileMan.Models.Config;
using System.Threading.Tasks;

namespace BarryFileMan.Managers.Config
{
    public interface IConfigManager<T>
    {
        public T Config { get; }

        public T SetConfig(T config);
        public Task<T> SetConfigAsync(T config);
        public T GetConfig();
        public Task<T> GetConfigAsync();
        public T RestoreDefaultConfig();
        public Task<T> RestoreDefaultConfigAsync();
        public T UpdateTheme(Theme theme);
        public Task<T> UpdateThemeAsync(Theme theme);
    }
}
