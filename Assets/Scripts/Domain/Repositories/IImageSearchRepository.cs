using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Domain.Models;
using ImageSearch.Domain.ValueObjects;

namespace ImageSearch.Domain.Repositories
{
    public interface IImageSearchRepository
    {
        UniTask<Result<IReadOnlyList<ImageItem>, NetworkError>> SearchAsync(
            ImageSearchQuery query,
            CancellationToken cancellationToken);
    }
}
