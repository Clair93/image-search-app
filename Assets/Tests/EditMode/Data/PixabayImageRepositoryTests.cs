using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImageSearch.Core;
using ImageSearch.Data.Dtos;
using ImageSearch.Data.Repositories;
using ImageSearch.Data.Tests.Fixtures;
using ImageSearch.Data.Tests.Mocks;
using ImageSearch.Domain.Models;
using ImageSearch.Domain.ValueObjects;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ImageSearch.Data.Tests
{
    public class PixabayImageRepositoryTests
    {
        [Test]
        public async Task SearchAsync_Success_MapsDtoHitsToImageItems()
        {
            var dataSource = new MockPixabayDataSource();
            dataSource.SetupSuccess(JsonConvert.DeserializeObject<PixabaySearchResponseDto>(PixabayJsonFixtures.SuccessTwoHits));
            var repository = new PixabayImageRepository(dataSource);

            var result = await repository.SearchAsync(new ImageSearchQuery("cat", 1, 20), CancellationToken.None);

            var success = (Result<IReadOnlyList<ImageItem>, NetworkError>.Success)result;
            Assert.That(success.Value.Count, Is.EqualTo(2));
            Assert.That(success.Value[0].AuthorName, Is.EqualTo("alice"));
        }

        [Test]
        public async Task SearchAsync_ForwardsQueryToDataSource()
        {
            var dataSource = new MockPixabayDataSource();
            dataSource.SetupSuccess(JsonConvert.DeserializeObject<PixabaySearchResponseDto>(PixabayJsonFixtures.SuccessEmptyHits));
            var repository = new PixabayImageRepository(dataSource);
            var query = new ImageSearchQuery("cat", 1, 20);

            await repository.SearchAsync(query, CancellationToken.None);

            Assert.That(dataSource.LastQuery, Is.EqualTo(query));
            Assert.That(dataSource.CallCount, Is.EqualTo(1));
        }

        [Test]
        public async Task SearchAsync_EmptyHits_ReturnsSuccessWithEmptyList()
        {
            var dataSource = new MockPixabayDataSource();
            dataSource.SetupSuccess(JsonConvert.DeserializeObject<PixabaySearchResponseDto>(PixabayJsonFixtures.SuccessEmptyHits));
            var repository = new PixabayImageRepository(dataSource);

            var result = await repository.SearchAsync(new ImageSearchQuery("cat", 1, 20), CancellationToken.None);

            var success = (Result<IReadOnlyList<ImageItem>, NetworkError>.Success)result;
            Assert.That(success.Value, Is.Empty);
        }

        [Test]
        public async Task SearchAsync_BlankKeyword_ForwardsAsIs()
        {
            var dataSource = new MockPixabayDataSource();
            dataSource.SetupSuccess(JsonConvert.DeserializeObject<PixabaySearchResponseDto>(PixabayJsonFixtures.SuccessEmptyHits));
            var repository = new PixabayImageRepository(dataSource);
            var query = new ImageSearchQuery("   ", 1, 20);

            await repository.SearchAsync(query, CancellationToken.None);

            Assert.That(dataSource.LastQuery.Keyword, Is.EqualTo("   "));
        }

        [Test]
        public async Task SearchAsync_DataSourceFailure_ReturnsSameErrorWithoutMapping()
        {
            var dataSource = new MockPixabayDataSource();
            dataSource.SetupFailure(NetworkError.ServerError);
            var repository = new PixabayImageRepository(dataSource);

            var result = await repository.SearchAsync(new ImageSearchQuery("cat", 1, 20), CancellationToken.None);

            var error = (Result<IReadOnlyList<ImageItem>, NetworkError>.Error)result;
            Assert.That(error.Value, Is.EqualTo(NetworkError.ServerError));
        }

        [Test]
        public void SearchAsync_DataSourceCancels_PropagatesException()
        {
            var dataSource = new MockPixabayDataSource();
            dataSource.SetupException(new System.OperationCanceledException());
            var repository = new PixabayImageRepository(dataSource);

            Assert.CatchAsync<System.OperationCanceledException>(async () =>
                await repository.SearchAsync(new ImageSearchQuery("cat", 1, 20), CancellationToken.None));
        }
    }
}
