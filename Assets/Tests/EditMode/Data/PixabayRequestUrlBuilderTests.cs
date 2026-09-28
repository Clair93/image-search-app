using System.Linq;
using System.Net;
using ImageSearch.Data.Sources;
using ImageSearch.Domain.ValueObjects;
using NUnit.Framework;

namespace ImageSearch.Data.Tests
{
    public class PixabayRequestUrlBuilderTests
    {
        [Test]
        public void Build_IncludesApiKeyAndPaging()
        {
            var query = new ImageSearchQuery("cat", 1, 20);

            var url = PixabayRequestUrlBuilder.Build("my-api-key", query);

            Assert.That(url, Does.Contain("key=my-api-key"));
            Assert.That(url, Does.Contain("page=1"));
            Assert.That(url, Does.Contain("per_page=20"));
        }

        [Test]
        public void Build_KeywordRoundTripsThroughEncoding()
        {
            var query = new ImageSearchQuery("red car & friends", 1, 20);

            var url = PixabayRequestUrlBuilder.Build("key", query);
            var qParam = url.Split('&').First(p => p.StartsWith("q="));
            var decodedKeyword = WebUtility.UrlDecode(qParam.Substring("q=".Length));

            Assert.That(decodedKeyword, Is.EqualTo("red car & friends"));
        }
    }
}
