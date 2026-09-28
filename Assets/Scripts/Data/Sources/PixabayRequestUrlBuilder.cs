using System.Net;
using ImageSearch.Domain.ValueObjects;

namespace ImageSearch.Data.Sources
{
    public static class PixabayRequestUrlBuilder
    {
        private const string BaseUrl = "https://pixabay.com/api/";

        public static string Build(string apiKey, ImageSearchQuery query)
        {
            return $"{BaseUrl}?key={WebUtility.UrlEncode(apiKey)}" +
                   $"&q={WebUtility.UrlEncode(query.Keyword)}" +
                   $"&page={query.Page}" +
                   $"&per_page={query.PerPage}";
        }
    }
}
