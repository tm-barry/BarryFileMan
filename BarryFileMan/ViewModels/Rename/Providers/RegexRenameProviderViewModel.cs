using BarryFileMan.Enums.Rename;
using BarryFileMan.Interfaces;
using BarryFileMan.Managers;
using BarryFileMan.Models.Presets;
using BarryFileMan.Rename.Interfaces;
using BarryFileMan.Views.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class RegexRenameProviderViewModel : BaseRegexRenameProviderViewModel
    {
        private static readonly string _presetKey = "rename-regex";

        [ObservableProperty]
        private string _output = string.Empty;

        public RegexRenameProviderViewModel() : this(new RenameViewModel()) { }
        public RegexRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel) { }

        protected override void OnPresetsConfigChanged((Presets presets, string? key) value)
        {
            if (value.key == null || value.key == _presetKey)
            {
                var selectedPresetName = SelectedPreset?.Name;
                var regexPresets = value.presets.Rename.RegexPresets;

                // Add system defaults
                var newPresets = new List<IPresetViewModel>()
                {
                    new PresetViewModel<RenameRegexPreset>($"-- { Resources.Resources.Movie } --", true, new()
                    {
                        MatchPattern = "(?<movie>[^\\\\/]+?)\\W(?:(?<year>\\d{4})(?:\\W(?<resolution>\\d+p)?)|(?<resolution>\\d+p)(?:\\W(?<year>\\d{4}))?)",
                        RenamePattern = "<movie.replace('.',' ').trim(both)> (<year>)",
                        Input = "\\ParentFolder\\Movie.Name.2000.1080p",
                        SelectedMatchTypeIndex = 1,
                    }),
                    new PresetViewModel<RenameRegexPreset>($"-- { Resources.Resources.TV } --", true, new()
                    {
                        MatchPattern = "(?<show>[^\\\\/]+)\\W(?:s|S)(?<season>\\d+)(?:e|E)(?<episode>\\d+)",
                        RenamePattern = "<show{-1}.replace(\'.\',\' \').append(' - ')>S<season{-1}.pad(left,\'0\',2)>E<episode{-1}.pad(left,\'0\',2)>",
                        Input = "\\ParentFolder\\Show.Name.S01E01",
                        SelectedMatchTypeIndex = 1,
                    })
                };

                if (regexPresets != null)
                {
                    foreach (var presetKey in regexPresets.Keys)
                    {
                        newPresets.Add(new PresetViewModel<RenameRegexPreset>(presetKey, false, regexPresets[presetKey]));
                    }
                }

                Presets = newPresets.OrderBy((preset) => preset.Name).ToList();
                SelectedPreset = Presets.FirstOrDefault((preset) => preset.Name == selectedPresetName) ?? Presets.FirstOrDefault();
            }
        }

        protected override void OnSelectedPresetChanged(IPresetViewModel? value)
        {
            OnSelectedPresetChanged(value as PresetViewModel<RenameRegexPreset>);
            base.OnSelectedPresetChanged(value);
        }

        protected override void OnInputMatchesChangedBefore(IEnumerable<IRenameMatch>? value)
        {
            HandleOutputRename(value, RenamePattern);
        }

        protected override void OnRenamePatternChangedBefore(string value)
        {
            HandleOutputRename(InputMatches, value);
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
                    if(presets.Rename.RegexPresets == null)
                    {
                        presets.Rename.RegexPresets = new();
                    }

                    bool save = true;
                    presets.Rename.RegexPresets.TryGetValue(seletedPresetName, out var preset);
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
                                presets.Rename.RegexPresets.Remove(seletedPresetName);
                            }
                            else
                            {
                                save = false;
                            }
                            break;
                        case RenamePresetChangeType.Rename:
                            var renameResult = await GetPresetName(seletedPresetName, presets.Rename.RegexPresets);
                            if (renameResult.success && preset != null)
                            {
                                presets.Rename.RegexPresets.Remove(seletedPresetName);
                                presets.Rename.RegexPresets.Add(renameResult.name, preset);

                                if(SelectedPreset != null)
                                    SelectedPreset.Name = renameResult.name;
                            }
                            else
                            {
                                save = false;
                            }
                            break;
                        case RenamePresetChangeType.SaveNew:
                            var newNameResult = await GetPresetName(string.Empty, presets.Rename.RegexPresets);
                            if (newNameResult.success)
                            {
                                var newPreset = new RenameRegexPreset();
                                UpdatePreset(newPreset);
                                presets.Rename.RegexPresets.Add(newNameResult.name, newPreset);

                                if(SelectedPreset != null)
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

        private void UpdatePreset(RenameRegexPreset? preset)
        {
            if (preset != null)
            {
                preset.MatchPattern = MatchPattern;
                preset.RenamePattern = RenamePattern;
                preset.Input = Input;
                preset.SelectedMatchTypeIndex = SelectedMatchTypeIndex;
            }
        }

        private async Task<(string name, bool success)> GetPresetName(string name, Dictionary<string, RenameRegexPreset> existingPresets)
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

        private void OnSelectedPresetChanged(PresetViewModel<RenameRegexPreset>? preset)
        {
            MatchPattern = preset?.Preset.MatchPattern ?? string.Empty;
            RenamePattern = preset?.Preset.RenamePattern ?? string.Empty;
            Input = preset?.Preset.Input ?? string.Empty;
            SelectedMatchTypeIndex = preset?.Preset.SelectedMatchTypeIndex ?? 0;
        }

        private void HandleOutputRename(IEnumerable<IRenameMatch>? matches, string? renamePattern)
        {
            Output = RenameMatches(_regexProvider, matches, renamePattern, out var error);
            RenamePatternError = error;
        }
    }
}
