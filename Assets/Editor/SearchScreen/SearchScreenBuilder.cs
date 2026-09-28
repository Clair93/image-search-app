using System.IO;
using ImageSearch.Bootstrap;
using ImageSearch.Presentation.Common;
using ImageSearch.Presentation.Views;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace ImageSearch.EditorTools
{
    public static class SearchScreenBuilder
    {
        private const string CanvasName = "SearchScreenCanvas";
        private const string CompositionRootName = "CompositionRoot";
        private const string CardPrefabPath = "Assets/Prefabs/UI/ResultCard.prefab";
        private const float HeaderHeight = 180f;
        private const float ReferenceWidth = 1080f;
        private const float ReferenceHeight = 1920f;

        [MenuItem("ImageSearch/Build Search Screen")]
        public static void Build()
        {
            KoreanFontAssetSetup.EnsureKoreanFontAsset();

            var existingCanvas = GameObject.Find(CanvasName);
            if (existingCanvas != null)
            {
                Object.DestroyImmediate(existingCanvas);
            }

            var existingRoot = GameObject.Find(CompositionRootName);
            if (existingRoot != null)
            {
                Object.DestroyImmediate(existingRoot);
            }

            EnsureEventSystem();

            var canvasGo = new GameObject(
                CanvasName,
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var safeAreaRect = CreateChild("SafeArea", canvasGo.transform);
            StretchFull(safeAreaRect);
            safeAreaRect.gameObject.AddComponent<SafeArea>();

            var (searchInput, searchButton) = BuildHeader(safeAreaRect);
            var (content, statusText) = BuildBody(safeAreaRect);

            var cardPrefab = EnsureResultCardPrefab();

            var view = canvasGo.AddComponent<SearchScreenView>();
            view.Configure(searchInput, searchButton, content, cardPrefab, statusText);

            var rootGo = new GameObject(CompositionRootName);
            var mockDataSource = rootGo.AddComponent<MockImageDataSource>();
            var compositionRoot = rootGo.AddComponent<CompositionRoot>();
            compositionRoot.Configure(view, mockDataSource);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();

            Debug.Log("[SearchScreenBuilder] Search screen built and wired.");
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        private static RectTransform CreateChild(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static (TMP_InputField input, Button button) BuildHeader(RectTransform parent)
        {
            var header = CreateChild("Header", parent);
            header.anchorMin = new Vector2(0f, 1f);
            header.anchorMax = new Vector2(1f, 1f);
            header.pivot = new Vector2(0.5f, 1f);
            header.sizeDelta = new Vector2(0f, HeaderHeight);
            header.anchoredPosition = Vector2.zero;

            var headerImage = header.gameObject.AddComponent<Image>();
            headerImage.color = new Color(0.95f, 0.95f, 0.95f, 1f);

            var layout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 20, 20);
            layout.spacing = 16f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var inputRect = CreateChild("SearchInputField", header);
            inputRect.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var input = BuildSearchInputField(inputRect);

            var buttonRect = CreateChild("SearchButton", header);
            var buttonLayoutElement = buttonRect.gameObject.AddComponent<LayoutElement>();
            buttonLayoutElement.preferredWidth = 180f;
            buttonLayoutElement.flexibleWidth = 0f;
            var button = BuildSearchButton(buttonRect);

            return (input, button);
        }

        private static TMP_InputField BuildSearchInputField(RectTransform root)
        {
            var background = root.gameObject.AddComponent<Image>();
            background.color = Color.white;

            var inputField = root.gameObject.AddComponent<TMP_InputField>();

            var textAreaRect = CreateChild("TextArea", root);
            StretchFull(textAreaRect);
            textAreaRect.offsetMin = new Vector2(16f, 6f);
            textAreaRect.offsetMax = new Vector2(-16f, -6f);
            textAreaRect.gameObject.AddComponent<RectMask2D>();

            var placeholderRect = CreateChild("Placeholder", textAreaRect);
            StretchFull(placeholderRect);
            var placeholder = placeholderRect.gameObject.AddComponent<TextMeshProUGUI>();
            placeholder.text = "검색어를 입력하세요";
            placeholder.fontSize = 40f;
            placeholder.color = new Color(0f, 0f, 0f, 0.4f);
            placeholder.alignment = TextAlignmentOptions.Left;

            var textRect = CreateChild("Text", textAreaRect);
            StretchFull(textRect);
            var text = textRect.gameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = 40f;
            text.color = Color.black;
            text.alignment = TextAlignmentOptions.Left;

            inputField.textViewport = textAreaRect;
            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.targetGraphic = background;

            return inputField;
        }

        private static Button BuildSearchButton(RectTransform root)
        {
            var image = root.gameObject.AddComponent<Image>();
            image.color = new Color(0.2f, 0.45f, 0.9f, 1f);

            var button = root.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var labelRect = CreateChild("Label", root);
            StretchFull(labelRect);
            var label = labelRect.gameObject.AddComponent<TextMeshProUGUI>();
            label.text = "검색";
            label.fontSize = 40f;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            return button;
        }

        private static (RectTransform content, TextMeshProUGUI status) BuildBody(RectTransform parent)
        {
            var body = CreateChild("Body", parent);
            body.anchorMin = Vector2.zero;
            body.anchorMax = Vector2.one;
            body.offsetMin = Vector2.zero;
            body.offsetMax = new Vector2(0f, -HeaderHeight);

            var scrollViewRect = CreateChild("ScrollView", body);
            StretchFull(scrollViewRect);
            var scrollRect = scrollViewRect.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;

            var viewportRect = CreateChild("Viewport", scrollViewRect);
            StretchFull(viewportRect);
            viewportRect.gameObject.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
            viewportRect.gameObject.AddComponent<Mask>().showMaskGraphic = false;

            var contentRect = CreateChild("Content", viewportRect);
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;

            var grid = contentRect.gameObject.AddComponent<GridLayoutGroup>();
            grid.padding = new RectOffset(24, 24, 24, 24);
            grid.spacing = new Vector2(16f, 16f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            var cellWidth = (ReferenceWidth - grid.padding.left - grid.padding.right - grid.spacing.x) / 2f;
            grid.cellSize = new Vector2(cellWidth, cellWidth * 1.3f);

            var fitter = contentRect.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;

            var statusRect = CreateChild("StatusMessage", body);
            StretchFull(statusRect);
            statusRect.offsetMin = new Vector2(48f, statusRect.offsetMin.y);
            statusRect.offsetMax = new Vector2(-48f, statusRect.offsetMax.y);
            var status = statusRect.gameObject.AddComponent<TextMeshProUGUI>();
            status.text = "검색 중입니다...";
            status.fontSize = 44f;
            status.color = new Color(0f, 0f, 0f, 0.6f);
            status.alignment = TextAlignmentOptions.Center;
            status.enableWordWrapping = true;
            status.overflowMode = TextOverflowModes.Overflow;
            statusRect.gameObject.SetActive(false);

            return (contentRect, status);
        }

        private static GameObject EnsureResultCardPrefab()
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CardPrefabPath);
            if (existingPrefab != null && existingPrefab.GetComponent<ResultCardView>() != null)
            {
                return existingPrefab;
            }

            var cardGo = new GameObject("ResultCard", typeof(RectTransform));
            var cardRect = cardGo.GetComponent<RectTransform>();
            cardGo.AddComponent<Image>().color = Color.white;

            var thumbnailRect = CreateChild("Thumbnail", cardRect);
            thumbnailRect.anchorMin = new Vector2(0f, 0.32f);
            thumbnailRect.anchorMax = new Vector2(1f, 1f);
            thumbnailRect.offsetMin = new Vector2(8f, 4f);
            thumbnailRect.offsetMax = new Vector2(-8f, -8f);
            var thumbnailImage = thumbnailRect.gameObject.AddComponent<Image>();
            thumbnailImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            var tagsRect = CreateChild("Tags", cardRect);
            tagsRect.anchorMin = new Vector2(0f, 0.16f);
            tagsRect.anchorMax = new Vector2(1f, 0.32f);
            tagsRect.offsetMin = new Vector2(12f, 0f);
            tagsRect.offsetMax = new Vector2(-12f, 0f);
            var tagsText = tagsRect.gameObject.AddComponent<TextMeshProUGUI>();
            tagsText.fontSize = 26f;
            tagsText.color = Color.black;
            tagsText.alignment = TextAlignmentOptions.Left;

            var authorRect = CreateChild("Author", cardRect);
            authorRect.anchorMin = new Vector2(0f, 0f);
            authorRect.anchorMax = new Vector2(1f, 0.16f);
            authorRect.offsetMin = new Vector2(12f, 0f);
            authorRect.offsetMax = new Vector2(-12f, 0f);
            var authorText = authorRect.gameObject.AddComponent<TextMeshProUGUI>();
            authorText.fontSize = 24f;
            authorText.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            authorText.alignment = TextAlignmentOptions.Left;

            var cardView = cardGo.AddComponent<ResultCardView>();
            cardView.Configure(thumbnailImage, tagsText, authorText);

            Directory.CreateDirectory("Assets/Prefabs/UI");
            var prefab = PrefabUtility.SaveAsPrefabAsset(cardGo, CardPrefabPath);
            Object.DestroyImmediate(cardGo);

            return prefab;
        }
    }
}
