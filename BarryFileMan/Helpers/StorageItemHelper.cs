using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.Helpers
{
    public static class StorageItemHelper
    {
        public static async Task<IEnumerable<IStorageFile>> GetStorageFiles(IEnumerable<IStorageItem>? items)
        {
            List<IStorageFile> files = new();
            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    if (item is IStorageFile file)
                    {
                        files.Add(file);
                    }
                    else if (item is IStorageFolder folder)
                    {
                        files.AddRange(await GetStorageFolderFiles(folder));
                    }
                }
            }

            return files;
        }

        private static async Task<IEnumerable<IStorageFile>> GetStorageFolderFiles(IStorageFolder folder)
        {
            var storageItems = await folder.GetItemsAsync().ToListAsync();
            return await GetStorageFiles(storageItems);
        }
    }
}
