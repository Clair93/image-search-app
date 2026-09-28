using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Domain.Models;
using ImageSearch.Presentation.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ImageSearch.Presentation.Views
{
    public sealed class ResultCardView : MonoBehaviour
    {
        [SerializeField] private Image _thumbnailImage;
        [SerializeField] private TextMeshProUGUI _tagsText;
        [SerializeField] private TextMeshProUGUI _authorText;

        private Texture2D _texture;
        private Sprite _sprite;

        public void Configure(Image thumbnailImage, TextMeshProUGUI tagsText, TextMeshProUGUI authorText)
        {
            _thumbnailImage = thumbnailImage;
            _tagsText = tagsText;
            _authorText = authorText;
        }

        public async UniTask Bind(ImageItem item, IThumbnailLoader loader, CancellationToken cancellationToken)
        {
            _tagsText.text = string.Join(", ", item.Tags);
            _authorText.text = item.AuthorName;

            try
            {
                var texture = await loader.LoadAsync(item.ThumbnailUrl, cancellationToken);

                if (this == null)
                {
                    Destroy(texture);
                    return;
                }

                _texture = texture;
                _sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                _thumbnailImage.sprite = _sprite;
            }
            catch (System.OperationCanceledException)
            {
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"[ResultCardView] Thumbnail load failed for '{item.ThumbnailUrl}': {exception.Message}");
            }
        }

        public void Release()
        {
            if (_sprite != null)
            {
                Destroy(_sprite);
                _sprite = null;
            }

            if (_texture != null)
            {
                Destroy(_texture);
                _texture = null;
            }
        }
    }
}
