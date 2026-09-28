using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Domain.Models;
using ImageSearch.Presentation.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ImageSearch.Presentation.Views
{
    public sealed class SearchScreenView : MonoBehaviour, ISearchScreenView
    {
        [SerializeField] private TMP_InputField _searchInput;
        [SerializeField] private Button _searchButton;
        [SerializeField] private RectTransform _gridContent;
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private TextMeshProUGUI _statusText;

        private SearchScreenPresenter _presenter;
        private IThumbnailLoader _thumbnailLoader;
        private readonly List<ResultCardView> _activeCards = new();

        public void Configure(
            TMP_InputField searchInput,
            Button searchButton,
            RectTransform gridContent,
            GameObject cardPrefab,
            TextMeshProUGUI statusText)
        {
            _searchInput = searchInput;
            _searchButton = searchButton;
            _gridContent = gridContent;
            _cardPrefab = cardPrefab;
            _statusText = statusText;
        }

        public void Initialize(SearchScreenPresenter presenter, IThumbnailLoader thumbnailLoader)
        {
            _presenter = presenter;
            _thumbnailLoader = thumbnailLoader;
            _searchButton.onClick.RemoveAllListeners();
            _searchButton.onClick.AddListener(HandleSearchButtonClicked);
        }

        private void HandleSearchButtonClicked()
        {
            _presenter.OnSearchRequested(_searchInput.text).Forget();
        }

        public void ShowLoading()
        {
            _statusText.gameObject.SetActive(true);
            _statusText.text = "검색 중입니다...";
            ClearCards();
        }

        public void ShowEmpty()
        {
            _statusText.gameObject.SetActive(true);
            _statusText.text = "결과가 없습니다";
            ClearCards();
        }

        public void ShowError(string message)
        {
            _statusText.gameObject.SetActive(true);
            _statusText.text = message;
            ClearCards();
        }

        public void ShowResults(IReadOnlyList<ImageItem> items, CancellationToken thumbnailToken)
        {
            _statusText.gameObject.SetActive(false);
            ClearCards();

            foreach (var item in items)
            {
                var instance = Instantiate(_cardPrefab, _gridContent);
                var card = instance.GetComponent<ResultCardView>();
                card.Bind(item, _thumbnailLoader, thumbnailToken).Forget();
                _activeCards.Add(card);
            }
        }

        private void ClearCards()
        {
            foreach (var card in _activeCards)
            {
                card.Release();
                Destroy(card.gameObject);
            }

            _activeCards.Clear();
        }
    }
}
