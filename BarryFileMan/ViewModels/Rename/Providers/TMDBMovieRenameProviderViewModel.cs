using BarryFileMan.Managers;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Rename.Providers.TMDB;
using BarryFileMan.Views.Common;
using BarryFileMan.Views.Rename.Providers;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class TMDBMovieRenameProviderViewModel : BaseTMDBRenameProviderViewModel
    {
        private readonly TMDBMovieRenameMatchProvider _tmdbMovieProvider = new(true);

        public TMDBMovieRenameProviderViewModel() : this(new RenameViewModel()) { }

        public TMDBMovieRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel)
        {
            // TODO - load from preset
            MatchPattern = "(?<movie>[^\\\\/]+?)\\W(?:(?<year>\\d{4})(?:\\W(?<resolution>\\d+p)?)|(?<resolution>\\d+p)(?:\\W(?<year>\\d{4}))?)";
            QueryRenamePattern = "<movie.replace('.',' ')>";
            YearRenamePattern = "<year>";
            LanguageRenamePattern = string.Empty;
            RenamePattern = "<tmdbName> (<tmdbReleaseYear>)";
            Input = "\\ParentFolder\\Movie.2000.1080p";
            SelectedMatchTypeIndex = 1;
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

        protected override void OnInputMatchesChangedBefore(IEnumerable<IRenameMatch>? value)
        {
            HandleOutputRename(value, TmdbMatches, RenamePattern);
            base.OnInputMatchesChangedBefore(value);
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
            if(!IsBusy)
            {
                IsBusy = true;

                TmdbMatches = null;
                var (matches, error) = await TMDBMovieFindMatches(QueryOutput, YearOutput, LanguageOutput);
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

        private async Task<(IEnumerable<IRenameMatch>? matches, string? error)> TMDBMovieFindMatches(string query, string year, string language)
        {
            IEnumerable<IRenameMatch>? matches = null;
            string? error = null;
            try
            {
                matches = await _tmdbMovieProvider.MatchAsync(
                    new(
                        AppManager.UserConfig.Config.Tmdb.ApiKey ?? string.Empty, 
                        AppManager.UserConfig.Config.Tmdb.IncludeAdult,
                        query, 
                        year, 
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
            var output = RenameMatches(_tmdbMovieProvider, tmdbMatches, renamePattern, out var error);
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

                    var languageOutput = RenameMatches(_regexProvider, regexMatches, LanguageRenamePattern, out error);
                    if (!string.IsNullOrWhiteSpace(error))
                        throw new Exception(error);

                    var (tmdbMatches, tmdbMatchError) = await TMDBMovieFindMatches(queryOutput, yearOutput, languageOutput);

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
                    file.RenameError = $"{ e.Message }";
                    file.RenamedFileName = null;
                }
            }
        }
    }
}
