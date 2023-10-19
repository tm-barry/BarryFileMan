using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactions.DragAndDrop;
using BarryFileMan.ViewModels;
using BarryFileMan.ViewModels.Rename;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.Behaviors
{
    public class ListBoxDropRenameFileHandler : DropHandlerBase
    {
        private bool Validate<T>(DragEventArgs e, object? targetContext, bool bExecute) where T : RenameFileViewModel
        {
            var storageItems = e.Data.GetFiles();
            if (storageItems != null
                && storageItems.Any()
                && targetContext is RenameViewModel vm)
            {
                var items = vm.Files;

                switch (e.DragEffects)
                {
                    case DragDropEffects.Copy:
                    case DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link:
                        {
                            if (bExecute)
                            {
                                AddStorageItems(items, storageItems);
                            }
                            return true;
                        }
                    default:
                        return false;
                }
            }

            return false;
        }

        private void AddStorageItems(IList<RenameFileViewModel> items, IEnumerable<IStorageItem>? storageItems)
        {
            if (storageItems != null)
            {
                foreach (var storageItem in storageItems)
                {
                    if (storageItem is IStorageFile file)
                    {
                        AddFile(items, file);
                    }
                    else if (storageItem is IStorageFolder folder)
                    {
                        AddFolder(items, folder);
                    }
                }
            }
        }

        private void AddFile(IList<RenameFileViewModel> items, IStorageFile file)
        {
            var renameFile = new RenameFileViewModel(file);
            InsertItem(items, renameFile, items.Count);
        }

        private void AddFolder(IList<RenameFileViewModel> items, IStorageFolder folder)
        {
            Task.Run(async () => 
            { 
                var storageItems = await folder.GetItemsAsync().ToListAsync();
                AddStorageItems(items, storageItems);
            }).Wait();
        }

        public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            if (e.Source is Control && sender is ListBox)
            {
                return Validate<RenameFileViewModel>(e, targetContext, false);
            }
            return false;
        }

        public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
        {
            if (e.Source is Control && sender is ListBox)
            {
                return Validate<RenameFileViewModel>(e, targetContext, true);
            }
            return false;
        }
    }
}
