using Avalonia.Platform.Storage;
using BarryFileMan.Attributes.Validation;
using BarryFileMan.Managers;
using BarryFileMan.Views.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BarryFileMan.ViewModels.Flatten
{
    public partial class FlattenViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBusy))]
        [NotifyCanExecuteChangedFor(nameof(FlattenFolderCommand))]
        private bool _openingFolder;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBusy))]
        [NotifyCanExecuteChangedFor(nameof(FlattenFolderCommand))]
        private bool _applyingFilters;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBusy))]
        [NotifyCanExecuteChangedFor(nameof(FlattenFolderCommand))]
        private bool _flatteningFolder;

        public bool IsBusy => OpeningFolder || ApplyingFilters || FlatteningFolder;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FolderPath))]
        [NotifyCanExecuteChangedFor(nameof(FlattenFolderCommand))]
        private IStorageFolder? _storageFolder;

        public string? FolderPath => StorageFolder != null ? Uri.UnescapeDataString(StorageFolder.Path.LocalPath) : null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasFiles))]
        [NotifyPropertyChangedFor(nameof(FilteredFiles))]
        [NotifyCanExecuteChangedFor(nameof(FlattenFolderCommand))]
        private ObservableCollection<FlattenFileViewModel> _files = new();

        public bool HasFiles => Files.Any();

        public IEnumerable<FlattenFileViewModel> FilteredFiles 
            => !string.IsNullOrWhiteSpace(FileFilterRegexError) 
            ? Files 
            : Files.Where((file) => _fileFilterRegex.IsMatch(file.FileName)
                || _fileFilterRegex.IsMatch(file.NewFileName ?? string.Empty));

        [ObservableProperty]
        private FlattenFileViewModel? _selectedFile;

        private Regex _fileFilterRegex = new(".*");

        [ObservableProperty]
        [HasErrorProperty(nameof(FileFilterRegexError))]
        private string? _fileFilterRegexPattern;
        partial void OnFileFilterRegexPatternChanged(string? value)
        {
            var regex = string.IsNullOrWhiteSpace(value) ? ".*" : value;
            if (Common.Regex.Extensions.IsValidRegex(regex, out _fileFilterRegex, out var regexError))
            {
                FileFilterRegexError = null;
            }
            else
            {
                FileFilterRegexError = regexError;
            }
            OnPropertyChanged(nameof(FilteredFiles));
            FlattenFolderCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty]
        private string? _fileFilterRegexError;
        partial void OnFileFilterRegexErrorChanged(string? value)
        {
            ValidateProperty(FileFilterRegexPattern, nameof(FileFilterRegexPattern));
        }

        [ObservableProperty]
        private bool _shouldDeleteExcludedFiles = false;

        [ObservableProperty]
        private bool _shouldDeleteEmptyFolders = true;

        [ObservableProperty]
        private bool _flattenOptionsExpanded = true;

        public FlattenViewModel()
        {
            Files.CollectionChanged += Files_CollectionChanged;
        }

        ~FlattenViewModel()
        {
            Files.CollectionChanged -= Files_CollectionChanged;
        }

        private void Files_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasFiles));
            OnPropertyChanged(nameof(FilteredFiles));
        }

        [RelayCommand]
        private async Task OpenFolderDialog()
        {
            OpeningFolder = true;

            try
            {
                var folders = await AppManager.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Select Folder to Flatten",
                    AllowMultiple = false,
                });

                StorageFolder = folders?.FirstOrDefault();

            }
            catch (Exception ex)
            {
                Files.Clear();
                await AppManager.MsgBoxShowWindowDialogAsync(
                        "Error", $"{ ex.Message }\n{ ex.InnerException?.Message }", MsgBoxButtons.Ok, MsgBoxIcons.Error);
            }

            await ApplyFilters();

            OpeningFolder = false;
        }

        [RelayCommand]
        private static async Task CopyOriginalFile(FlattenFileViewModel file)
        {
            await AppManager.CopyText(file.FullPath);
        }

        [RelayCommand]
        private static async Task CopyNewFile(FlattenFileViewModel file)
        {
            await AppManager.CopyText(file.NewFileName);
        }

        [RelayCommand]
        private void RemoveFile(FlattenFileViewModel file)
        {
            Files.Remove(file);
        }

        [RelayCommand(CanExecute = nameof(CanFlattenFolder))]
        private async Task FlattenFolder()
        {
            FlatteningFolder = true;
            List<string> failedFiles = new();
            bool hadError = false;

            var mbResult = await AppManager.MsgBoxShowWindowDialogAsync(
                "Flatten Folder", $"Are you sure you want to flatten the following folder?\n\n{FolderPath}", MsgBoxButtons.YesNo, MsgBoxIcons.Question);

            if (mbResult == MsgBoxResult.Yes)
            {
                // Move files
                foreach (var file in FilteredFiles.Where((ff) => !ff.Exclude))
                {
                    try
                    {
                        if (file.NewFileName != null)
                            File.Move(file.FullPath, Path.Combine(file.BasePath, file.NewFileName));
                    }
                    catch
                    {
                        failedFiles.Add(file.FullPath);
                    }
                }

                // Show failed moved files
                if (failedFiles.Any())
                {
                    var failedListString = string.Join("\n", failedFiles);
                    await AppManager.MsgBoxShowWindowDialogAsync(
                        "Error", $"Failed to move the following files:\n{failedListString}", MsgBoxButtons.Ok, MsgBoxIcons.Error);
                    failedFiles = new();
                    hadError = true;
                }

                // Delete Excluded Files
                if (ShouldDeleteExcludedFiles)
                {
                    foreach (var file in FilteredFiles.Where((ff) => ff.Exclude))
                    {
                        try
                        {
                            File.Delete(file.FullPath);
                        }
                        catch
                        {
                            failedFiles.Add(file.FullPath);
                        }
                    }
                }

                // Show failed delete excluded files
                if (failedFiles.Any())
                {
                    var failedListString = string.Join("\n", failedFiles);
                    await AppManager.MsgBoxShowWindowDialogAsync(
                        "Error", $"Failed to delete the following excluded files:\n{failedListString}", MsgBoxButtons.Ok, MsgBoxIcons.Error);
                    failedFiles = new();
                    hadError = true;
                }

                if (ShouldDeleteEmptyFolders && FolderPath != null)
                {
                    try
                    {
                        DeleteEmptyFolders(FolderPath);
                    }
                    catch (Exception ex)
                    {
                        await AppManager.MsgBoxShowWindowDialogAsync(
                            "Error", $"Failed to delete empty folders!\n{ex.Message}\n{ex.InnerException?.Message}", MsgBoxButtons.Ok, MsgBoxIcons.Error);
                        hadError = true;
                    }
                }

                StorageFolder = null;
                Files.Clear();

                if (!hadError)
                {
                    await AppManager.MsgBoxShowWindowDialogAsync("Success", "Folder successfully flattened!", MsgBoxButtons.Ok, MsgBoxIcons.Success);
                }
            }

            FlatteningFolder = false;
        }

        private bool CanFlattenFolder()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(FolderPath) && FilteredFiles.Any();
        }

        private async Task ApplyFilters()
        {
            ApplyingFilters = true;
            var files = new List<FlattenFileViewModel>();

            try
            {
                if (Directory.Exists(FolderPath))
                {
                    var filePaths = Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories);

                    // Load initial files
                    foreach (var filePath in filePaths)
                    {
                        files.Add(new FlattenFileViewModel(FolderPath, filePath));
                    }

                    // Handle duplicates
                    foreach (var file in files)
                    {
                        if (file.NewFileName == null)
                        {
                            var duplicateFiles = files.Where((f) => f.FileName == file.FileName).ToList();

                            if (duplicateFiles.Count > 1)
                            {
                                foreach (var dupFile in duplicateFiles)
                                {
                                    dupFile.NewFileName = GetNextAvailableFilename(dupFile.FileName, files);
                                    dupFile.IsDuplicate = true;
                                }
                            }
                            else
                            {
                                file.NewFileName = file.FileName;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                files.Clear();
                await AppManager.MsgBoxShowWindowDialogAsync(
                        "Error", $"{ex.Message}\n{ex.InnerException?.Message}", MsgBoxButtons.Ok, MsgBoxIcons.Error);
            }

            Files.Clear();
            if (files != null)
                foreach (var file in files)
                    Files.Add(file);

            ApplyingFilters = false;
        }

        private static string GetNextAvailableFilename(string filename, IEnumerable<FlattenFileViewModel> files)
        {
            if (!files.Any((file) => file.NewFileName == filename)) return filename;

            string alternateFilename;
            int fileNameIndex = 1;
            do
            {
                fileNameIndex += 1;
                alternateFilename = CreateNumberedFilename(filename, fileNameIndex);
            } while (files.Any((file) => file.NewFileName == alternateFilename));

            return alternateFilename;
        }

        private static string CreateNumberedFilename(string filename, int number)
        {
            string plainName = Path.GetFileNameWithoutExtension(filename);
            string extension = Path.GetExtension(filename);
            return string.Format("{0}({1}){2}", plainName, number, extension);
        }

        private void DeleteEmptyFolders(string path)
        {
            if (Directory.Exists(path))
            {
                var rootFolders = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
                foreach (var folder in rootFolders)
                {
                    // Delete empty subfolders
                    DeleteEmptyFolders(folder);

                    // Delete folder if empty
                    var folders = Directory.GetDirectories(folder, "*", SearchOption.TopDirectoryOnly);
                    var files = Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly);
                    if ((files == null || files.Length == 0)
                        && (folders == null || folders.Length == 0))
                    {
                        try
                        {
                            Directory.Delete(folder);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
