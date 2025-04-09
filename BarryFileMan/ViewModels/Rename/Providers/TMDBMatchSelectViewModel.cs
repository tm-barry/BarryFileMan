using Avalonia.Controls;
using BarryFileMan.Managers;
using BarryFileMan.Rename.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class TMDBMatchSelectViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TMDBMatch> _matches = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
        private TMDBMatch? _selectedMatch;

        public TMDBMatch? ConfirmedMatch { get; private set; }

        public TMDBMatchSelectViewModel(IEnumerable<IRenameMatch>? matches)
        {
            PopulateMatchContainers(matches);
        }

        private void PopulateMatchContainers(IEnumerable<IRenameMatch>? matches)
        {
            Matches.Clear();

            if (matches != null && matches.Any())
            {
                foreach (var match in matches)
                {
                    Matches.Add(new(match));
                }
            }
        }

        private bool CanApply => SelectedMatch != null;

        [RelayCommand(CanExecute = nameof(CanApply))]
        private void Apply(Window window)
        {
            ConfirmedMatch = SelectedMatch;
            window.Close();
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            ConfirmedMatch = null;
            window.Close();
        }
    }

    public class TMDBMatch
    {
        private readonly string _secureBaseUrl = "https://image.tmdb.org/t/p/";
        private readonly IEnumerable<string> _posterSizes = new List<string>()
        {
            "w92",
            "w154",
            "w185",
            "w342",
            "w500",
            "w780",
            "original"
        };

        public IRenameMatch RenameMatch { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string OriginalName { get; private set; } = string.Empty;
        public string OriginalLanguage { get; private set; } = string.Empty;
        public string ReleaseDate { get; private set; } = string.Empty;
        public string Overview { get; private set; } = string.Empty;
        public string? PosterUrl { get; private set; }

        public TMDBMatch(IRenameMatch match)
        {
            RenameMatch = match;
            Populate(match);
        }

        private void Populate(IRenameMatch match)
        {
            Name = match.Groups["tmdbName"].FirstOrDefault()?.Value ?? string.Empty;
            OriginalName = match.Groups["tmdbOriginalName"].FirstOrDefault()?.Value ?? string.Empty;
            OriginalLanguage = match.Groups["tmdbOriginalLanguage"].FirstOrDefault()?.Value ?? string.Empty;
            ReleaseDate = match.Groups["tmdbReleaseDate"].FirstOrDefault()?.Value ?? string.Empty;
            Overview = match.Groups["tmdbOverview"].FirstOrDefault()?.Value ?? string.Empty;

            var posterPath = match.Groups["tmdbPosterPath"].FirstOrDefault()?.Value;
            if (!string.IsNullOrWhiteSpace(posterPath))
            {
                var baseUrl = !string.IsNullOrWhiteSpace(AppManager.TMDBConfig?.Images?.SecureBaseUrl)
                    ? AppManager.TMDBConfig.Images.SecureBaseUrl : _secureBaseUrl;
                var posterSizes = AppManager.TMDBConfig?.Images?.PosterSizes != null && AppManager.TMDBConfig.Images.PosterSizes.Any()
                    ? AppManager.TMDBConfig.Images.PosterSizes : _posterSizes;
                var posterSize = posterSizes.FirstOrDefault(ps => ps == "w154") ?? posterSizes.FirstOrDefault();

                PosterUrl = baseUrl + posterSize + posterPath;
            }
        }
    }
}