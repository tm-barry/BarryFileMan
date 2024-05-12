using BarryFileMan.Rename.Exceptions;
using BarryFileMan.Rename.Models.TMDB;
using RestSharp;
using System.Text.Json;

namespace BarryFileMan.Rename.Repositories
{
    public class TMDBRepository
    {
        private string? _key;

        public TMDBRepository() { }
        public TMDBRepository(string? key)
        {
            SetKey(key);
        }

        public void SetKey(string? key)
        {
            _key = key;
        }

        public Task<TMDBResponse?> ValidateKeyAsync(string key) 
            => GetAsync<TMDBResponse?>("https://api.themoviedb.org/3/authentication", null, key);

        public Task<TMDBConfiguration?> GetConfigurationAsync() 
            => GetAsync<TMDBConfiguration?>("https://api.themoviedb.org/3/configuration");

        public Task<TMDBGenreContainer?> GetGenreMovieList(string? language = null) 
            => GetGenreList("https://api.themoviedb.org/3/genre/movie/list", language);

        public Task<TMDBGenreContainer?> GetGenreTvList(string? language = null) 
            => GetGenreList("https://api.themoviedb.org/3/genre/tv/list", language);

        public Task<TMDBMovie?> GetMovieDetails(int movieId, string? language = null, string? appendToResponse = null)
        {
            var queryParams = new List<Parameter>();
            if (!string.IsNullOrWhiteSpace(language))
                queryParams.Add(new QueryParameter("language", language));
            if (!string.IsNullOrWhiteSpace(appendToResponse))
                queryParams.Add(new QueryParameter("append_to_response", appendToResponse));

            return GetAsync<TMDBMovie?>($"https://api.themoviedb.org/3/movie/{movieId}", queryParams);
        }

        public Task<TMDBTv?> GetTvSeriesDetails(int seriesId, string? language = null, string? appendToResponse = null)
        {
            var queryParams = new List<Parameter>();
            if (!string.IsNullOrWhiteSpace(language))
                queryParams.Add(new QueryParameter("language", language));
            if (!string.IsNullOrWhiteSpace(appendToResponse))
                queryParams.Add(new QueryParameter("append_to_response", appendToResponse));

            return GetAsync<TMDBTv?>($"https://api.themoviedb.org/3/tv/{seriesId}", queryParams);
        }

        public Task<TMDBTvEpisode?> GetTvEpisodeDetails(int seriesId, int seasonNumber, int episodeNumber, string? language = null, string? appendToResponse = null)
        {
            var queryParams = new List<Parameter>();
            if (!string.IsNullOrWhiteSpace(language))
                queryParams.Add(new QueryParameter("language", language));
            if (!string.IsNullOrWhiteSpace(appendToResponse))
                queryParams.Add(new QueryParameter("append_to_response", appendToResponse));

            return GetAsync<TMDBTvEpisode?>($"https://api.themoviedb.org/3/tv/{seriesId}/season/{seasonNumber}/episode/{episodeNumber}", queryParams);
        }

        public Task<TMDBSearchCollection?> SearchCollection(string query, bool? includeAdult = null, string? language = null,
            int? page = null, string? region = null)
        {
            var queryParams = new List<Parameter>();
            if (!string.IsNullOrWhiteSpace(query))
                queryParams.Add(new QueryParameter("query", query));
            if (includeAdult != null)
                queryParams.Add(new QueryParameter("include_adult", includeAdult.Value.ToString()));
            if (!string.IsNullOrWhiteSpace(language))
                queryParams.Add(new QueryParameter("language", language));
            if (page != null)
                queryParams.Add(new QueryParameter("page", page.Value.ToString()));
            if (!string.IsNullOrWhiteSpace(region))
                queryParams.Add(new QueryParameter("region", region));

            return GetAsync<TMDBSearchCollection?>("https://api.themoviedb.org/3/search/collection", queryParams);
        }

        public Task<TMDBSearchMovieTV?> SearchMovie(string query, bool? includeAdult = null, string? language = null, 
            string? primaryReleaseYear = null, int? page = null, string? region = null)
        {
            var queryParams = new List<Parameter>();
            if (!string.IsNullOrWhiteSpace(query))
                queryParams.Add(new QueryParameter("query", query));
            if (includeAdult != null)
                queryParams.Add(new QueryParameter("include_adult", includeAdult.Value.ToString()));
            if (!string.IsNullOrWhiteSpace(language))
                queryParams.Add(new QueryParameter("language", language));
            if (!string.IsNullOrWhiteSpace(primaryReleaseYear))
                queryParams.Add(new QueryParameter("primary_release_year", primaryReleaseYear));
            if (page != null)
                queryParams.Add(new QueryParameter("page", page.Value.ToString()));
            if (!string.IsNullOrWhiteSpace(region))
                queryParams.Add(new QueryParameter("region", region));

            return GetAsync<TMDBSearchMovieTV?>("https://api.themoviedb.org/3/search/movie", queryParams);
        }

        public Task<TMDBSearchMovieTV?> SearchTv(string query, int? firstAirDateYear = null, bool? includeAdult = null, string ? language = null,
            int? page = null, int? year = null)
        {
            var queryParams = new List<Parameter>();
            if (!string.IsNullOrWhiteSpace(query))
                queryParams.Add(new QueryParameter("query", query));
            if (firstAirDateYear != null)
                queryParams.Add(new QueryParameter("first_air_date_year", firstAirDateYear.Value.ToString()));
            if (includeAdult != null)
                queryParams.Add(new QueryParameter("include_adult", includeAdult.Value.ToString()));
            if (!string.IsNullOrWhiteSpace(language))
                queryParams.Add(new QueryParameter("language", language));
            if (page != null)
                queryParams.Add(new QueryParameter("page", page.Value.ToString()));
            if (year != null)
                queryParams.Add(new QueryParameter("year", year.Value.ToString()));

            return GetAsync<TMDBSearchMovieTV?>("https://api.themoviedb.org/3/search/tv", queryParams);
        }

        private Task<TMDBGenreContainer?> GetGenreList(string url, string? language = null)
        {
            var queryParams = new List<Parameter>();
            if (!string.IsNullOrWhiteSpace(language))
                queryParams.Add(new QueryParameter("language", language));

            return GetAsync<TMDBGenreContainer?>(url, queryParams);
        }

        private async Task<T?> GetAsync<T>(string url, IEnumerable<Parameter>? queryParams = null, string? key = null)
        {
            var options = new RestClientOptions(url);
            var client = new RestClient(options);
            var request = new RestRequest("");

            if(queryParams != null)
                foreach (var param in queryParams)
                    request.AddParameter(param);

            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Bearer { key ?? _key }");
            var response = await client.GetAsync(request);

            if (response?.Content == null || !response.IsSuccessful)
                throw HandleBadResponseException(response?.Content);
            else
                return JsonSerializer.Deserialize<T>(response.Content);
        }

        private TMDBBadResponseException HandleBadResponseException(string? response)
        {
            var tmdbResponse = response != null
                ? JsonSerializer.Deserialize<TMDBResponse>(response)
                : null;

            return new TMDBBadResponseException(tmdbResponse?.StatusMessage ?? "Bad response!", tmdbResponse?.StatusCode ?? -1);
        }
    }
}
