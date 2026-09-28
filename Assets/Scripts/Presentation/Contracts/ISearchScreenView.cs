using System.Collections.Generic;
using System.Threading;
using ImageSearch.Domain.Models;

namespace ImageSearch.Presentation.Contracts
{
    public interface ISearchScreenView
    {
        void ShowLoading();

        void ShowResults(IReadOnlyList<ImageItem> items, CancellationToken thumbnailToken);

        void ShowEmpty();

        void ShowError(string message);
    }
}
