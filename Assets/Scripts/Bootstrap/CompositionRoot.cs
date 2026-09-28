using System.Net.Http;
using ImageSearch.Data.Repositories;
using ImageSearch.Data.Sources;
using ImageSearch.Presentation;
using ImageSearch.Presentation.Contracts;
using ImageSearch.Presentation.Loading;
using ImageSearch.Presentation.Views;
using UnityEngine;

namespace ImageSearch.Bootstrap
{
    public sealed class CompositionRoot : MonoBehaviour
    {
        [SerializeField] private SearchScreenView _view;
        [SerializeField] private MockImageDataSource _mockDataSource;
        [SerializeField]
        [Tooltip("체크하면 API 키가 있어도 항상 MockImageDataSource를 사용합니다 (오류 화면 확인용).")]
        private bool _forceMock;

        public void Configure(SearchScreenView view, MockImageDataSource mockDataSource)
        {
            _view = view;
            _mockDataSource = mockDataSource;
        }

        private void Awake()
        {
            IPixabayDataSource dataSource;
            IThumbnailLoader thumbnailLoader;

            if (!_forceMock && ApiKeyProvider.TryGetPixabayApiKey(out var apiKey))
            {
                dataSource = new PixabayImageDataSource(new HttpClient(), apiKey);
                thumbnailLoader = new UnityWebRequestThumbnailLoader();
            }
            else
            {
                if (_forceMock)
                {
                    Debug.LogWarning("Force Mock 옵션이 켜져 있어 API 키가 있어도 Mock 데이터로 실행합니다.");
                }
                else
                {
                    Debug.LogWarning(
                        "Pixabay API 키를 찾을 수 없어 Mock 데이터로 실행합니다. " +
                        "Assets/Secrets~/pixabay-api-key.txt.example를 참고해 키를 등록하세요.");
                }

                dataSource = _mockDataSource;
                thumbnailLoader = new MockThumbnailLoader();
            }

            var repository = new PixabayImageRepository(dataSource);
            var presenter = new SearchScreenPresenter(repository);

            _view.Initialize(presenter, thumbnailLoader);
            presenter.AttachView(_view);
        }
    }
}
