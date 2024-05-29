using BarryFileMan.Attributes.Validation;
using BarryFileMan.Managers;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Providers.TMDB;
using BarryFileMan.Views.Common;
using BarryFileMan.Views.Rename.Providers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class TMDBTvRenameProviderViewModel : BaseTMDBRenameProviderViewModel
    {
        private readonly TMDBTvRenameMatchProvider _tmdbTvProvider = new(true);

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
        partial void OnSeasonOutputChanged(string value)
        {
            TMDBParamsChanged();
        }

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
        partial void OnEpisodeOutputChanged(string value)
        {
            TMDBParamsChanged();
        }

        public TMDBTvRenameProviderViewModel() : this(new RenameViewModel()) { }

        public TMDBTvRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel)
        {
            // TODO - load from preset
            MatchPattern = "(?<show>[^(?:\\\\|/)]+)\\W(?:s|S)(?<season>\\d+)(?:e|E)(?<episode>\\d+)";
            QueryRenamePattern = "<show.replace('.',' ')>";
            YearRenamePattern = string.Empty;
            SeasonRenamePattern = "<season>";
            EpisodeRenamePattern = "<episode>";
            LanguageRenamePattern = string.Empty;
            RenamePattern = "";
            Input = "\\ParentFolder\\Show.Name.S01E01";
            SelectedMatchTypeIndex = 1;
        }

        protected override void OnInputMatchesChangedBefore(IEnumerable<IRenameMatch>? value)
        {
            HandleSeasonRenamePatternChanged(SeasonRenamePattern, value);
            HandleEpisodeRenamePatternChanged(EpisodeRenamePattern, value);
            HandleOutputRename(value, TmdbMatches, RenamePattern);

            base.OnInputMatchesChangedBefore(value);
        }

        private void HandleSeasonRenamePatternChanged(string value, IEnumerable<IRenameMatch>? inputMatches)
        {
            SeasonOutput = RenameMatches(_regexProvider, inputMatches, value, out var error);
            SeasonRenamePatternError = error;
        }

        private void HandleEpisodeRenamePatternChanged(string renamePattern, IEnumerable<IRenameMatch>? inputMatches)
        {
            EpisodeOutput = RenameMatches(_regexProvider, inputMatches, renamePattern, out var error);
            EpisodeRenamePatternError = error;
        }

        protected override void OnIsBusyChangedBefore(bool value)
        {
            FindInputMatchesCommand.NotifyCanExecuteChanged();
            base.OnIsBusyChangedBefore(value);
        }

        protected override void OnQueryOutputChangedBefore(string value)
        {
            TMDBParamsChanged();
            base.OnQueryOutputChangedBefore(value);
        }

        protected override void OnYearOutputChangedBefore(string value)
        {
            TMDBParamsChanged();
            base.OnYearOutputChangedBefore(value);
        }

        protected override void OnLanguageOutputChangedBefore(string value)
        {
            TMDBParamsChanged();
            base.OnLanguageOutputChangedBefore(value);
        }

        protected override void OnTmdbMatchesChangedBefore(IEnumerable<IRenameMatch>? value)
        {
            HandleOutputRename(InputMatches, value, RenamePattern);
            base.OnTmdbMatchesChangedBefore(value);
        }

        protected override void OnRenamePatternChangedBefore(string value)
        {
            HandleOutputRename(InputMatches, TmdbMatches, value);
            base.OnRenamePatternChangedBefore(value);
        }

        private void TMDBParamsChanged()
        {
            TmdbMatches = null;
        }

        private bool CanFindInputMatches() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanFindInputMatches))]
        private async Task FindInputMatches()
        {
            if (!IsBusy)
            {
                IsBusy = true;

                TmdbMatches = null;
                var (matches, error) = await TMDBTvFindMatches(QueryOutput, YearOutput, SeasonOutput, EpisodeOutput, LanguageOutput);
                if (string.IsNullOrEmpty(error))
                {
                    if (matches != null && matches.Any())
                    {
                        if (matches.Count() > 1)
                        {
                            var match = await TMDBMatchSelect.ShowAsync(matches, AppManager.MainWindow);
                            if (match != null)
                            {
                                TmdbMatches = new List<IRenameMatch>() { match };
                            }
                            else
                            {
                                TmdbMatches = matches;
                            }

                        }
                        else
                        {
                            TmdbMatches = matches;
                        }

                        MatchesTabIndex = 0;
                    }
                    else
                    {
                        await AppManager.MsgBoxShowWindowDialogAsync(
                            Resources.Resources.Error, Resources.Resources.NoMatchesFound, MsgBoxButtons.Ok, MsgBoxIcons.Error);
                    }
                }
                else
                {
                    await AppManager.MsgBoxShowWindowDialogAsync(
                        Resources.Resources.Error, error, MsgBoxButtons.Ok, MsgBoxIcons.Error);
                }

                IsBusy = false;
            }
        }

        private async Task<(IEnumerable<IRenameMatch>? matches, string? error)> TMDBTvFindMatches(string query, string year, string season, string episode, string language)
        {
            IEnumerable<IRenameMatch>? matches = null;
            string? error = null;
            try
            {
                matches = await _tmdbTvProvider.MatchAsync(
                    new(
                        AppManager.UserConfig.Config.Tmdb.ApiKey ?? string.Empty,
                        AppManager.UserConfig.Config.Tmdb.IncludeAdult,
                        query,
                        year,
                        season,
                        episode,
                        language
                    ));
            }
            catch (Exception ex)
            {
                error = ex.Message;
                matches = null;
            }

            return (matches, error);
        }

        private (string output, string? error) GetRenameFromMatches(IEnumerable<IRenameMatch>? regexMatches, IEnumerable<IRenameMatch>? tmdbMatches, string? renamePattern)
        {
            var output = RenameMatches(_tmdbTvProvider, tmdbMatches, renamePattern, out var error);
            output = RenameMatches(_regexProvider, regexMatches, output, out error);

            return (output, error);
        }

        private void HandleOutputRename(IEnumerable<IRenameMatch>? regexMatches, IEnumerable<IRenameMatch>? tmdbMatches, string? renamePattern)
        {
            if (tmdbMatches != null)
            {
                var (output, error) = GetRenameFromMatches(regexMatches, tmdbMatches, renamePattern);
                Output = output;
                RenamePatternError = error;
            }
            else
            {
                Output = string.Empty;
                RenamePatternError = null;
            }
        }

        public override async Task ApplyFileRenames()
        {
            foreach (var file in ViewModel.Files)
            {
                try
                {
                    string? error;
                    var regexMatches = RegexFindMatches(GetFileMatchInput(file), MatchPattern, out error);
                    if (!string.IsNullOrWhiteSpace(error))
                        throw new Exception(error);

                    var queryOutput = RenameMatches(_regexProvider, regexMatches, QueryRenamePattern, out error);
                    if (!string.IsNullOrWhiteSpace(error))
                        throw new Exception(error);

                    var yearOutput = RenameMatches(_regexProvider, regexMatches, YearRenamePattern, out error);
                    if (!string.IsNullOrWhiteSpace(error))
                        throw new Exception(error);

                    var seasonOutput = RenameMatches(_regexProvider, regexMatches, SeasonRenamePattern, out error);
                    if (!string.IsNullOrWhiteSpace(error))
                        throw new Exception(error);

                    var episodeOutput = RenameMatches(_regexProvider, regexMatches, EpisodeRenamePattern, out error);
                    if (!string.IsNullOrWhiteSpace(error))
                        throw new Exception(error);

                    var languageOutput = RenameMatches(_regexProvider, regexMatches, LanguageRenamePattern, out error);
                    if (!string.IsNullOrWhiteSpace(error))
                        throw new Exception(error);

                    var (tmdbMatches, tmdbMatchError) = await TMDBTvFindMatches(queryOutput, yearOutput, seasonOutput, episodeOutput, languageOutput);

                    if (tmdbMatches != null && tmdbMatches.Count() > 1)
                    {
                        var match = await TMDBMatchSelect.ShowAsync(tmdbMatches, AppManager.MainWindow);
                        if (match != null)
                        {
                            tmdbMatches = new List<IRenameMatch>() { match };
                        }
                    }

                    var (renamedFileName, renameError) = GetRenameFromMatches(regexMatches, tmdbMatches, RenamePattern);
                    file.RenameError = renameError;
                    file.RenamedFileName = string.IsNullOrEmpty(renameError) ? renamedFileName : null;
                }
                catch (Exception e)
                {
                    file.RenameError = $"{e.Message}";
                    file.RenamedFileName = null;
                }
            }
        }
    }
}
