using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Presentation.Contracts;
using UnityEngine;

namespace ImageSearch.Presentation.Loading
{
    public sealed class MockThumbnailLoader : IThumbnailLoader
    {
        public async UniTask<Texture2D> LoadAsync(string url, CancellationToken cancellationToken)
        {
            await UniTask.Delay(300, cancellationToken: cancellationToken);

            var color = ColorFromString(url);
            var texture = new Texture2D(4, 4);
            var pixels = new Color[16];
            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private static Color ColorFromString(string value)
        {
            var hash = 0;
            foreach (var c in value)
            {
                hash = hash * 31 + c;
            }

            var hue = Mathf.Abs(hash % 360) / 360f;
            return Color.HSVToRGB(hue, 0.45f, 0.85f);
        }
    }
}
