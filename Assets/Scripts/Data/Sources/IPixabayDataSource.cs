using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Data.Dtos;
using ImageSearch.Domain.ValueObjects;

namespace ImageSearch.Data.Sources
{
    public interface IPixabayDataSource
    {
        UniTask<Result<PixabaySearchResponseDto, NetworkError>> FetchAsync(
            ImageSearchQuery query,
            CancellationToken cancellationToken);
    }
}
