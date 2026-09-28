using System.Collections;
using System.Collections.Generic;
using ImageSearch.Core;
using ImageSearch.Domain.Models;
using ImageSearch.Presentation.Loading;
using ImageSearch.Presentation.Tests.Mocks;
using ImageSearch.Presentation.Views;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace ImageSearch.Presentation.Tests
{
    public class SearchScreenPlayModeTests
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const float TimeoutSeconds = 3f;

        private SearchScreenView _view;
        private TMP_InputField _input;
        private Button _searchButton;
        private RectTransform _content;
        private GameObject _statusObject;
        private TextMeshProUGUI _statusText;
        private TestImageSearchRepository _repository;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
#if UNITY_EDITOR
            EditorSceneManager.LoadSceneInPlayMode(ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
#else
            SceneManager.LoadScene(ScenePath);
#endif
            yield return null;
            yield return null;

            var canvas = GameObject.Find("SearchScreenCanvas");
            _view = canvas.GetComponent<SearchScreenView>();
            _input = canvas.transform.Find("SafeArea/Header/SearchInputField").GetComponent<TMP_InputField>();
            _searchButton = canvas.transform.Find("SafeArea/Header/SearchButton").GetComponent<Button>();
            _content = (RectTransform)canvas.transform.Find("SafeArea/Body/ScrollView/Viewport/Content");
            _statusObject = canvas.transform.Find("SafeArea/Body/StatusMessage").gameObject;
            _statusText = _statusObject.GetComponent<TextMeshProUGUI>();

            _repository = new TestImageSearchRepository();
            var presenter = new SearchScreenPresenter(_repository);
            _view.Initialize(presenter, new MockThumbnailLoader());
            presenter.AttachView(_view);
        }

        [UnityTest]
        public IEnumerator InitialState_ShowsNoCardsAndNoStatus()
        {
            Assert.That(_content.childCount, Is.EqualTo(0));
            Assert.That(_statusObject.activeSelf, Is.False);
            yield break;
        }

        [UnityTest]
        public IEnumerator Search_Success_ShowsCardsWithCorrectFirstCardText()
        {
            _input.text = "고양이";
            _searchButton.onClick.Invoke();

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _repository.Calls.Count > 0, TimeoutSeconds, "repository call to be made");

            _repository.Calls[0].Complete(SuccessWith(
                (1, "mock://1", new[] { "고양이", "동물" }, "user01"),
                (2, "mock://2", new[] { "강아지" }, "user02")));

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _content.childCount == 2, TimeoutSeconds, "2 result cards to appear");

            var firstCard = _content.GetChild(0);
            Assert.That(firstCard.Find("Tags").GetComponent<TextMeshProUGUI>().text, Is.EqualTo("고양이, 동물"));
            Assert.That(firstCard.Find("Author").GetComponent<TextMeshProUGUI>().text, Is.EqualTo("user01"));
        }

        [UnityTest]
        public IEnumerator Search_EmptyResult_ShowsEmptyMessage()
        {
            _input.text = "없는검색어";
            _searchButton.onClick.Invoke();

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _repository.Calls.Count > 0, TimeoutSeconds, "repository call to be made");

            _repository.Calls[0].Complete(SuccessWith());

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _statusObject.activeSelf, TimeoutSeconds, "empty status to show");

            Assert.That(_statusText.text, Is.EqualTo("결과가 없습니다"));
            Assert.That(_content.childCount, Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator Search_NoInternet_ShowsNoInternetMessage()
        {
            _input.text = "고양이";
            _searchButton.onClick.Invoke();

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _repository.Calls.Count > 0, TimeoutSeconds, "repository call to be made");

            _repository.Calls[0].Complete(
                new Result<IReadOnlyList<ImageItem>, NetworkError>.Error(NetworkError.NoInternet));

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _statusObject.activeSelf, TimeoutSeconds, "error status to show");

            Assert.That(_statusText.text, Is.EqualTo("인터넷 연결을 확인해주세요"));
        }

        [UnityTest]
        public IEnumerator Search_ServerError_ShowsServerErrorMessage()
        {
            _input.text = "고양이";
            _searchButton.onClick.Invoke();

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _repository.Calls.Count > 0, TimeoutSeconds, "repository call to be made");

            _repository.Calls[0].Complete(
                new Result<IReadOnlyList<ImageItem>, NetworkError>.Error(NetworkError.ServerError));

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _statusObject.activeSelf, TimeoutSeconds, "error status to show");

            Assert.That(_statusText.text, Is.EqualTo("일시적인 서버 오류입니다. 잠시 후 다시 시도해주세요"));
        }

        [UnityTest]
        public IEnumerator Search_BlankKeyword_MakesNoRequest()
        {
            _input.text = "   ";
            _searchButton.onClick.Invoke();

            yield return null;
            yield return null;

            Assert.That(_repository.Calls.Count, Is.EqualTo(0));
            Assert.That(_statusObject.activeSelf, Is.False);
            Assert.That(_content.childCount, Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator Search_Again_ReplacesOldCardsWithNew()
        {
            _input.text = "cat";
            _searchButton.onClick.Invoke();
            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _repository.Calls.Count > 0, TimeoutSeconds, "first call to be made");
            _repository.Calls[0].Complete(SuccessWith(
                (1, "mock://1", new[] { "a" }, "user01"),
                (2, "mock://2", new[] { "b" }, "user02")));
            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _content.childCount == 2, TimeoutSeconds, "first round cards to appear");

            var firstRoundIds = new List<int>();
            for (var i = 0; i < _content.childCount; i++)
            {
                firstRoundIds.Add(_content.GetChild(i).GetInstanceID());
            }

            _input.text = "dog";
            _searchButton.onClick.Invoke();
            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _repository.Calls.Count > 1, TimeoutSeconds, "second call to be made");
            _repository.Calls[1].Complete(SuccessWith(
                (3, "mock://3", new[] { "c" }, "user03"),
                (4, "mock://4", new[] { "d" }, "user04"),
                (5, "mock://5", new[] { "e" }, "user05")));
            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _content.childCount == 3, TimeoutSeconds, "second round cards to appear");

            for (var i = 0; i < _content.childCount; i++)
            {
                CollectionAssert.DoesNotContain(firstRoundIds, _content.GetChild(i).GetInstanceID());
            }
        }

        [UnityTest]
        public IEnumerator RapidDoubleSearch_ShowsOnlyLatestResult()
        {
            _input.text = "first";
            _searchButton.onClick.Invoke();
            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _repository.Calls.Count > 0, TimeoutSeconds, "first call to be made");

            _input.text = "second";
            _searchButton.onClick.Invoke();
            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _repository.Calls.Count > 1, TimeoutSeconds, "second call to be made");

            // The first call's CancellationToken was already cancelled when the second search
            // started, so this Complete() silently no-ops (TrySetResult loses the race).
            _repository.Calls[0].Complete(
                new Result<IReadOnlyList<ImageItem>, NetworkError>.Error(NetworkError.ServerError));
            _repository.Calls[1].Complete(SuccessWith((1, "mock://1", new[] { "a" }, "user01")));

            yield return PlayModeTestUtilities.WaitUntilOrFail(
                () => _content.childCount == 1, TimeoutSeconds, "final result to settle");

            Assert.That(_statusObject.activeSelf, Is.False);
            Assert.That(_content.childCount, Is.EqualTo(1));
        }

        private static Result<IReadOnlyList<ImageItem>, NetworkError>.Success SuccessWith(
            params (int Id, string ThumbnailUrl, string[] Tags, string Author)[] items)
        {
            var list = new List<ImageItem>();
            foreach (var item in items)
            {
                list.Add(new ImageItem(item.Id, item.ThumbnailUrl, item.Tags, item.Author));
            }

            return new Result<IReadOnlyList<ImageItem>, NetworkError>.Success(list);
        }
    }
}
