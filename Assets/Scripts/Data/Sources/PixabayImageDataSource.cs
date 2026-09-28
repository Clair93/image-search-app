using System;
using System.Net.Http;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Data.Dtos;
using ImageSearch.Domain.ValueObjects;

namespace ImageSearch.Data.Sources
{
    public sealed class PixabayImageDataSource : IPixabayDataSource
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public PixabayImageDataSource(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async UniTask<Result<PixabaySearchResponseDto, NetworkError>> FetchAsync(
            ImageSearchQuery query,
            CancellationToken cancellationToken)
        {
            var url = PixabayRequestUrlBuilder.Build(_apiKey, query);

            try
            {
                using var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return new Result<PixabaySearchResponseDto, NetworkError>.Error(
                        NetworkErrorClassifier.FromStatusCode(response.StatusCode));
                }

                var json = await response.Content.ReadAsStringAsync();
                var dto = PixabayResponseParser.Parse(json);
                return new Result<PixabaySearchResponseDto, NetworkError>.Success(dto);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                return new Result<PixabaySearchResponseDto, NetworkError>.Error(
                    NetworkErrorClassifier.FromException(exception));
            }
        }
    }
}
