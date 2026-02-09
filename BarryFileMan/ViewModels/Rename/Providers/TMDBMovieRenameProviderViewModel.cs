using BarryFileMan.Enums.Rename;
using BarryFileMan.Interfaces;
using BarryFileMan.Managers;
using BarryFileMan.Models.Presets;
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
        private static readonly string _presetKey = "rename-tmdb-movie";
        private readonly TMDBMovieRenameMatchProvider _tmdbMovieProvider = new(true);

        public TMDBMovieRenameProviderViewModel() : this(new RenameViewModel()) { }
        public TMDBMovieRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel) { }

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
                    file.RenameError = $"{e.Message}";
                    file.RenamedFileName = null;
                }
            }

            HandleDuplicateFilenames();
        }

        protected override void OnPresetsConfigChanged((Presets presets, string? key) value)
        {
            if (value.key == null || value.key == _presetKey)
            {
                // Add system defaults
                var newPresets = new List<IPresetViewModel>()
                {
                    new PresetViewModel<RenameTMDBMoviePreset>($"-- { Resources.Resources.Movie } --", true, new()
                    {
                        MatchPattern = "(?<movie>[^\\\\/]+?)\\W(?:(?<year>\\d{4})(?:\\W(?<resolution>\\d+p)?)|(?<resolution>\\d+p)(?:\\W(?<year>\\d{4}))?)",
                        QueryRenamePattern = "<movie.replace('.',' ').separate()>",
                        YearRenamePattern = "<year ?? ''>",
                        LanguageRenamePattern = string.Empty,
                        RenamePattern = "<tmdbName> (<tmdbReleaseYear>)",
                        Input = "\\ParentFolder\\Movie.2000.1080p",
                        SelectedMatchTypeIndex = 1,
                    })
                };

                var selectedPresetName = SelectedPreset?.Name;
                var regexPresets = value.presets.Rename.TmdbMoviePresets;

                if (regexPresets != null)
                {
                    foreach (var presetKey in regexPresets.Keys)
                    {
                        newPresets.Add(new PresetViewModel<RenameTMDBMoviePreset>(presetKey, false, regexPresets[presetKey]));
                    }
                }

                Presets = newPresets.OrderBy((preset) => preset.Name).ToList();
                SelectedPreset = Presets.FirstOrDefault((preset) => preset.Name == selectedPresetName) ?? Presets.FirstOrDefault();
            }
        }

        protected override void OnSelectedPresetChanged(IPresetViewModel? value)
        {
            OnSelectedPresetChanged(value as PresetViewModel<RenameTMDBMoviePreset>);
            base.OnSelectedPresetChanged(value);
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

        protected override async Task SavePreset()
        {
            await ChangePreset(RenamePresetChangeType.Save);
        }

        protected override async Task SaveNewPreset()
        {
            await ChangePreset(RenamePresetChangeType.SaveNew);
        }

        protected override async Task RenamePreset()
        {
            await ChangePreset(RenamePresetChangeType.Rename);
        }

        protected override async Task DeletePreset()
        {
            await ChangePreset(RenamePresetChangeType.Delete);
        }

        private async Task ChangePreset(RenamePresetChangeType changeType)
        {
            IsBusy = true;

            try
            {
                var seletedPresetName = SelectedPreset?.Name ?? string.Empty;
                var presets = AppManager.PresetsConfig.Config;
                if (presets != null)
                {
                    if (presets.Rename.TmdbMoviePresets == null)
                    {
                        presets.Rename.TmdbMoviePresets = new();
                    }

                    bool save = true;
                    presets.Rename.TmdbMoviePresets.TryGetValue(seletedPresetName, out var preset);
                    switch (changeType)
                    {
                        case RenamePresetChangeType.Save:
                            UpdatePreset(preset);
                            break;
                        case RenamePresetChangeType.Delete:
                            var deletePromptResult = await AppManager.MsgBoxShowWindowDialogAsync(
                                Resources.Resources.DeletePreset, Resources.Resources.DeletePresetPrompt, MsgBoxButtons.YesNo, MsgBoxIcons.Question);
                            if (deletePromptResult.result == MsgBoxResult.Yes)
                            {
                                presets.Rename.TmdbMoviePresets.Remove(seletedPresetName);
                            }
                            else
                            {
                                save = false;
                            }
                            break;
                        case RenamePresetChangeType.Rename:
                            var renameResult = await GetPresetName(seletedPresetName, presets.Rename.TmdbMoviePresets);
                            if (renameResult.success && preset != null)
                            {
                                presets.Rename.TmdbMoviePresets.Remove(seletedPresetName);
                                presets.Rename.TmdbMoviePresets.Add(renameResult.name, preset);

                                if (SelectedPreset != null)
                                    SelectedPreset.Name = renameResult.name;
                            }
                            else
                            {
                                save = false;
                            }
                            break;
                        case RenamePresetChangeType.SaveNew:
                            var newNameResult = await GetPresetName(string.Empty, presets.Rename.TmdbMoviePresets);
                            if (newNameResult.success)
                            {
                                var newPreset = new RenameTMDBMoviePreset();
                                UpdatePreset(newPreset);
                                presets.Rename.TmdbMoviePresets.Add(newNameResult.name, newPreset);

                                if (SelectedPreset != null)
                                    SelectedPreset.Name = newNameResult.name;
                            }
                            else
                            {
                                save = false;
                            }
                            break;
                        default:
                            throw new NotImplementedException(changeType.ToString());
                    }

                    if (save)
                    {
                        await AppManager.PresetsConfig.SetConfigAsync(presets, _presetKey);
                    }
                }
            }
            catch (Exception ex)
            {
                await AppManager.ExceptionMsgBoxShowWindowDialogAsync(ex);
            }

            IsBusy = false;
        }

        private void UpdatePreset(RenameTMDBMoviePreset? preset)
        {
            if (preset != null)
            {
                preset.MatchPattern = MatchPattern;
                preset.QueryRenamePattern = QueryRenamePattern;
                preset.YearRenamePattern = YearRenamePattern;
                preset.LanguageRenamePattern = LanguageRenamePattern;
                preset.RenamePattern = RenamePattern;
                preset.Input = Input;
                preset.SelectedMatchTypeIndex = SelectedMatchTypeIndex;
            }
        }

        private async Task<(string name, bool success)> GetPresetName(string name, Dictionary<string, RenameTMDBMoviePreset> existingPresets)
        {
            var success = false;
            string presetName = string.Empty;
            var done = false;
            while (!done)
            {
                var msgResult = await AppManager.MsgBoxShowWindowDialogAsync(Resources.Resources.PresetName, Resources.Resources.PresetNamePrompt,
                        MsgBoxButtons.OkCancel, null, true, name, 100);

                if (msgResult.result == MsgBoxResult.Ok)
                {
                    if (!string.IsNullOrWhiteSpace(msgResult.input))
                    {
                        if (!existingPresets.ContainsKey(msgResult.input))
                        {
                            presetName = msgResult.input;
                            success = true;
                            done = true;
                        }
                        else
                        {
                            await AppManager.MsgBoxShowWindowDialogAsync(Resources.Resources.Error, Resources.Resources.PresetNameAlreadyExistsError,
                                MsgBoxButtons.Ok, MsgBoxIcons.Error);
                        }
                    }
                    else
                    {
                        await AppManager.MsgBoxShowWindowDialogAsync(Resources.Resources.Error, Resources.Resources.PresetNameEmptyError,
                            MsgBoxButtons.Ok, MsgBoxIcons.Error);
                    }
                }
                else
                {
                    done = true;
                }
            }
            return (presetName, success);
        }

        private void OnSelectedPresetChanged(PresetViewModel<RenameTMDBMoviePreset>? preset)
        {
            MatchPattern = preset?.Preset.MatchPattern ?? string.Empty;
            QueryRenamePattern = preset?.Preset.QueryRenamePattern ?? string.Empty;
            YearRenamePattern = preset?.Preset.YearRenamePattern ?? string.Empty;
            LanguageRenamePattern = preset?.Preset.LanguageRenamePattern ?? string.Empty;
            RenamePattern = preset?.Preset.RenamePattern ?? string.Empty;
            Input = preset?.Preset.Input ?? string.Empty;
            SelectedMatchTypeIndex = preset?.Preset.SelectedMatchTypeIndex ?? 0;
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
            string? error = null;
            IEnumerable<IRenameMatch>? matches;
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
    }
}
