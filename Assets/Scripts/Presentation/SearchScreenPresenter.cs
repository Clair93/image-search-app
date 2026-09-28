using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Domain.Models;
using ImageSearch.Domain.Repositories;
using ImageSearch.Domain.ValueObjects;
using ImageSearch.Presentation.Contracts;
using ImageSearch.Presentation.Mapping;

namespace ImageSearch.Presentation
{
    public sealed class SearchScreenPresenter
    {
        private readonly IImageSearchRepository _repository;
        private ISearchScreenView _view;
        private CancellationTokenSource _searchCts;

        public SearchScreenPresenter(IImageSearchRepository repository)
        {
            _repository = repository;
        }

        public void AttachView(ISearchScreenView view)
        {
            _view = view;
        }

        public async UniTask OnSearchRequested(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return;
            }

            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            _view.ShowLoading();

            try
            {
                var query = new ImageSearchQuery(keyword, 1, 20);
                var result = await _repository.SearchAsync(query, token);

                switch (result)
                {
                    case Result<IReadOnlyList<ImageItem>, NetworkError>.Success success:
                        if (success.Value.Count == 0)
                        {
                            _view.ShowEmpty();
                        }
                        else
                        {
                            _view.ShowResults(success.Value, token);
                        }

                        break;

                    case Result<IReadOnlyList<ImageItem>, NetworkError>.Error error:
                        _view.ShowError(NetworkErrorMessageMapper.ToMessage(error.Value));
                        break;
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
