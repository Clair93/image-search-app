using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Domain.Models;
using ImageSearch.Domain.Repositories;
using ImageSearch.Domain.ValueObjects;

namespace ImageSearch.Presentation.Tests.Mocks
{
    public sealed class TestImageSearchRepository : IImageSearchRepository
    {
        public sealed class PendingCall
        {
            private readonly UniTaskCompletionSource<Result<IReadOnlyList<ImageItem>, NetworkError>> _completion;

            public PendingCall(
                ImageSearchQuery query,
                UniTaskCompletionSource<Result<IReadOnlyList<ImageItem>, NetworkError>> completion)
            {
                Query = query;
                _completion = completion;
            }

            public ImageSearchQuery Query { get; }

            public void Complete(Result<IReadOnlyList<ImageItem>, NetworkError> result)
            {
                _completion.TrySetResult(result);
            }
        }

        public List<PendingCall> Calls { get; } = new();

        public UniTask<Result<IReadOnlyList<ImageItem>, NetworkError>> SearchAsync(
            ImageSearchQuery query,
            CancellationToken cancellationToken)
        {
            var completion = new UniTaskCompletionSource<Result<IReadOnlyList<ImageItem>, NetworkError>>();
            cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken));

            Calls.Add(new PendingCall(query, completion));

            return completion.Task;
        }
    }
}
