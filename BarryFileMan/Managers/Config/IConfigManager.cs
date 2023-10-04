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
    }
}
