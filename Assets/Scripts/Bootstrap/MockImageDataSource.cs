using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Data.Dtos;
using ImageSearch.Data.Sources;
using ImageSearch.Domain.ValueObjects;
using UnityEngine;

namespace ImageSearch.Bootstrap
{
    public sealed class MockImageDataSource : MonoBehaviour, IPixabayDataSource
    {
        public enum Scenario
        {
            Success,
            EmptyResult,
            NoInternet,
            ServerError
        }

        [SerializeField] private Scenario _scenario = Scenario.Success;

        private static readonly (string Tags, string User)[] DummyHits =
        {
            ("고양이, 동물", "user01"),
            ("강아지, 동물", "user02"),
            ("풍경, 자연", "user03"),
            ("음식, 요리", "user04"),
            ("자동차, 여행", "user05"),
            ("꽃, 봄", "user06")
        };

        public async UniTask<Result<PixabaySearchResponseDto, NetworkError>> FetchAsync(
            ImageSearchQuery query,
            CancellationToken cancellationToken)
        {
            await UniTask.Delay(500, cancellationToken: cancellationToken);

            switch (_scenario)
            {
                case Scenario.NoInternet:
                    return new Result<PixabaySearchResponseDto, NetworkError>.Error(NetworkError.NoInternet);

                case Scenario.ServerError:
                    return new Result<PixabaySearchResponseDto, NetworkError>.Error(NetworkError.ServerError);

                case Scenario.EmptyResult:
                    return new Result<PixabaySearchResponseDto, NetworkError>.Success(new PixabaySearchResponseDto
                    {
                        Total = 0,
                        TotalHits = 0,
                        Hits = new List<PixabayImageDto>()
                    });

                default:
                    var hits = new List<PixabayImageDto>();
                    for (var i = 0; i < DummyHits.Length; i++)
                    {
                        hits.Add(new PixabayImageDto
                        {
                            Id = i + 1,
                            Tags = DummyHits[i].Tags,
                            PreviewUrl = $"mock://{i + 1}",
                            User = DummyHits[i].User
                        });
                    }

                    return new Result<PixabaySearchResponseDto, NetworkError>.Success(new PixabaySearchResponseDto
                    {
                        Total = hits.Count,
                        TotalHits = hits.Count,
                        Hits = hits
                    });
            }
        }
    }
}
