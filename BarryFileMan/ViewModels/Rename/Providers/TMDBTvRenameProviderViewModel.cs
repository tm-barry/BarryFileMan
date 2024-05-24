using BarryFileMan.Attributes.Validation;
using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class TMDBTvRenameProviderViewModel : BaseTMDBRenameProviderViewModel
    {
        [ObservableProperty]
        [HasErrorProperty(nameof(SeasonRenamePatternError))]
        private string _seasonRenamePattern = string.Empty;
        partial void OnSeasonRenamePatternChanged(string value)
        {
            HandleSeasonRenamePatternChanged(value, InputMatches);
        }

        [ObservableProperty]
        private string? _seasonRenamePatternError;
        partial void OnSeasonRenamePatternErrorChanged(string? value)
        {
            ValidateProperty(SeasonRenamePattern, nameof(SeasonRenamePattern));
        }

        [ObservableProperty]
        private string _seasonOutput = string.Empty;

        [ObservableProperty]
        [HasErrorProperty(nameof(EpisodeRenamePatternError))]
        private string _episodeRenamePattern = string.Empty;
        partial void OnEpisodeRenamePatternChanged(string value)
        {
            HandleEpisodeRenamePatternChanged(value, InputMatches);
        }

        [ObservableProperty]
        private string? _episodeRenamePatternError;
        partial void OnEpisodeRenamePatternErrorChanged(string? value)
        {
            ValidateProperty(EpisodeRenamePattern, nameof(EpisodeRenamePattern));
        }

        [ObservableProperty]
        private string _episodeOutput = string.Empty;

        public TMDBTvRenameProviderViewModel() : this(new RenameViewModel()) { }

        public TMDBTvRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel)
        {
            // TODO - load from preset
            MatchPattern = "(?<show>[^(?:\\\\|/)]+)\\W(?:s|S)(?<season>\\d+)(?:e|E)(?<episode>\\d+)";
            RenamePattern = "";
            Input = "\\ParentFolder\\Show.Name.S01E01";
            SelectedMatchTypeIndex = 1;
        }

        protected override void OnInputMatchesChangedBefore(IEnumerable<IRenameMatch>? value)
        {
            HandleSeasonRenamePatternChanged(SeasonRenamePattern, value);
            HandleEpisodeRenamePatternChanged(EpisodeRenamePattern, value);

            base.OnInputMatchesChangedBefore(value);
        }

        private void HandleSeasonRenamePatternChanged(string value, IEnumerable<IRenameMatch>? inputMatches)
        {
            SeasonOutput = RegexRenameMatches(inputMatches, value, string.Empty, out var error);
            SeasonRenamePatternError = error;
        }

        private void HandleEpisodeRenamePatternChanged(string renamePattern, IEnumerable<IRenameMatch>? inputMatches)
        {
            EpisodeOutput = RegexRenameMatches(inputMatches, renamePattern, string.Empty, out var error);
            EpisodeRenamePatternError = error;
        }
    }
}
