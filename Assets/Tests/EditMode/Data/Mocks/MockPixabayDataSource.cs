using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Data.Dtos;
using ImageSearch.Data.Sources;
using ImageSearch.Domain.ValueObjects;

namespace ImageSearch.Data.Tests.Mocks
{
    public sealed class MockPixabayDataSource : IPixabayDataSource
    {
        private Result<PixabaySearchResponseDto, NetworkError> _resultToReturn;
        private Exception _exceptionToThrow;

        public ImageSearchQuery LastQuery { get; private set; }
        public int CallCount { get; private set; }

        public void SetupSuccess(PixabaySearchResponseDto response)
        {
            _resultToReturn = new Result<PixabaySearchResponseDto, NetworkError>.Success(response);
            _exceptionToThrow = null;
        }

        public void SetupFailure(NetworkError error)
        {
            _resultToReturn = new Result<PixabaySearchResponseDto, NetworkError>.Error(error);
            _exceptionToThrow = null;
        }

        public void SetupException(Exception exception)
        {
            _exceptionToThrow = exception;
            _resultToReturn = null;
        }

        public UniTask<Result<PixabaySearchResponseDto, NetworkError>> FetchAsync(
            ImageSearchQuery query,
            CancellationToken cancellationToken)
        {
            LastQuery = query;
            CallCount++;

            if (_exceptionToThrow != null)
            {
                throw _exceptionToThrow;
            }

            return UniTask.FromResult(_resultToReturn);
        }
    }
}
