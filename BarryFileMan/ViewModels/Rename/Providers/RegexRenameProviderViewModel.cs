using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class RegexRenameProviderViewModel : BaseRegexRenameProviderViewModel
    {
        [ObservableProperty]
        private string _output = string.Empty;

        public RegexRenameProviderViewModel() : this(new RenameViewModel()) { }

        public RegexRenameProviderViewModel(RenameViewModel viewModel, bool loadPreset = true) : base(viewModel)
        {
            // TODO - load from preset
            if (loadPreset)
            {
                MatchPattern = "(?:\\\\|/)(?<title>[^(?:\\\\|/)]+)(?:s|S)(?<season>\\d+)(?:e|E)(?<episode>\\d+)";
                RenamePattern = "<title{-1}.replace(\'.\',\' \')>- S<season{-1}.pad(left,\'0\',2)>E<episode{-1}.pad(left,\'0\',2)>";
                Input = "\\ParentFolder\\Show.Name.S01E01";
                SelectedMatchTypeIndex = 1;
            }
        }

        protected override void OnInputMatchesChangedBefore(IEnumerable<IRenameMatch>? value)
        {
            HandleOutputRename(value, RenamePattern, Input);
        }

        protected override void OnRenamePatternChangedBefore(string value)
        {
            HandleOutputRename(InputMatches, value, Input);
        }

        private void HandleOutputRename(IEnumerable<IRenameMatch>? matches, string? renamePattern, string? fallbackValue)
        {
            Output = RegexRenameMatches(matches, renamePattern, fallbackValue, out var error);
            RenamePatternError = error;
        }
    }
}
