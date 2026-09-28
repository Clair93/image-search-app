using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ImageSearch.Presentation.Contracts
{
    public interface IThumbnailLoader
    {
        UniTask<Texture2D> LoadAsync(string url, CancellationToken cancellationToken);
    }
}
