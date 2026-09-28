using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Data.Dtos;
using ImageSearch.Data.Mapping;
using ImageSearch.Data.Sources;
using ImageSearch.Domain.Models;
using ImageSearch.Domain.Repositories;
using ImageSearch.Domain.ValueObjects;

namespace ImageSearch.Data.Repositories
{
    public sealed class PixabayImageRepository : IImageSearchRepository
    {
        private readonly IPixabayDataSource _dataSource;

        public PixabayImageRepository(IPixabayDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async UniTask<Result<IReadOnlyList<ImageItem>, NetworkError>> SearchAsync(
            ImageSearchQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _dataSource.FetchAsync(query, cancellationToken);

            return result switch
            {
                Result<PixabaySearchResponseDto, NetworkError>.Success success =>
                    new Result<IReadOnlyList<ImageItem>, NetworkError>.Success(
                        PixabayResponseMapper.ToModels(success.Value.Hits)),
                Result<PixabaySearchResponseDto, NetworkError>.Error error =>
                    new Result<IReadOnlyList<ImageItem>, NetworkError>.Error(error.Value),
                _ => new Result<IReadOnlyList<ImageItem>, NetworkError>.Error(NetworkError.Unknown)
            };
        }
    }
}
