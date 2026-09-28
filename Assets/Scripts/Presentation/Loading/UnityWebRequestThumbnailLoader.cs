using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Presentation.Contracts;
using UnityEngine;
using UnityEngine.Networking;

namespace ImageSearch.Presentation.Loading
{
    public sealed class UnityWebRequestThumbnailLoader : IThumbnailLoader
    {
        public async UniTask<Texture2D> LoadAsync(string url, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequestTexture.GetTexture(url);
            using var registration = cancellationToken.Register(() => request.Abort());

            await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new InvalidOperationException($"Thumbnail request failed ({request.result}): {request.error}");
            }

            return DownloadHandlerTexture.GetContent(request);
        }
    }
}
