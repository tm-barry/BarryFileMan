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
        private readonly TMDBMovieRenameProvider _tmdbMovieProvider = new();

        public TMDBMovieRenameProviderViewModel() : this(new RenameViewModel()) { }

        public TMDBMovieRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel)
        {
            // TODO - load from preset
            MatchPattern = "(?<movie>[^(?:\\\\|/)]+?)\\W(?:(?<year>\\d{4})(?:\\W(?<resolution>\\d+p)?)|(?<resolution>\\d+p)(?:\\W(?<year>\\d{4}))?)";
            QueryRenamePattern = "<movie.replace('.',' ')>";
            YearRenamePattern = "<year>";
            LanguageRenamePattern = string.Empty;
            RenamePattern = "<tmdb-name> (<tmdb-release-year>)";
            Input = "\\ParentFolder\\Movie.2000.1080p";
            SelectedMatchTypeIndex = 1;
        }

        protected override void OnIsBusyChangedBefore(bool value)
        {
            FindInputMatchesCommand.NotifyCanExecuteChanged();

            base.OnIsBusyChangedBefore(value);
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
                        query.ToLower().Trim(), 
                        year.ToLower().Trim(), 
                        language.ToLower().Trim()
                    ));
            }
            catch (Exception ex)
            {
                error = ex.Message;
                matches = null;
            }

            return (matches, error);
        }
    }
}
