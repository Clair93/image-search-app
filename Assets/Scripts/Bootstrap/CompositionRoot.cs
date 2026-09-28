using System.Net.Http;
using ImageSearch.Data.Repositories;
using ImageSearch.Data.Sources;
using ImageSearch.Presentation;
using ImageSearch.Presentation.Loading;
using ImageSearch.Presentation.Views;
using UnityEngine;

namespace ImageSearch.Bootstrap
{
    public sealed class CompositionRoot : MonoBehaviour
    {
        [SerializeField] private SearchScreenView _view;
        [SerializeField] private MockImageDataSource _mockDataSource;

        public void Configure(SearchScreenView view, MockImageDataSource mockDataSource)
        {
            _view = view;
            _mockDataSource = mockDataSource;
        }

        private void Awake()
        {
            IPixabayDataSource dataSource;
            if (ApiKeyProvider.TryGetPixabayApiKey(out var apiKey))
            {
                dataSource = new PixabayImageDataSource(new HttpClient(), apiKey);
            }
            else
            {
                Debug.LogWarning(
                    "Pixabay API 키를 찾을 수 없어 Mock 데이터로 실행합니다. " +
                    "Assets/Secrets~/pixabay-api-key.txt.example를 참고해 키를 등록하세요.");
                dataSource = _mockDataSource;
            }

            var repository = new PixabayImageRepository(dataSource);
            var thumbnailLoader = new MockThumbnailLoader();
            var presenter = new SearchScreenPresenter(repository);

            _view.Initialize(presenter, thumbnailLoader);
            presenter.AttachView(_view);
        }
    }
}
