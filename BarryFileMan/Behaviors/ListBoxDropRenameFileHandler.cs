using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;
using BarryFileMan.ViewModels;
using System.Linq;

namespace BarryFileMan.Behaviors
{
    public class ListBoxDropRenameFileHandler : DropHandlerBase
    {
        private bool Validate<T>(DragEventArgs e, object? targetContext, bool bExecute) where T : RenameFileViewModel
        {
            var files = e.Data.GetFiles();
            if (files != null
                && files.Any()
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
                                foreach(var file in files)
                                {
                                    var renameFile = new RenameFileViewModel(file);
                                    InsertItem(items, renameFile, items.Count);
                                }
                            }
                            return true;
                        }
                    default:
                        return false;
                }
            }

            return false;
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
