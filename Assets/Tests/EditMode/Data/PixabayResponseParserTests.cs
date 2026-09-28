using ImageSearch.Data.Sources;
using ImageSearch.Data.Tests.Fixtures;
using NUnit.Framework;

namespace ImageSearch.Data.Tests
{
    public class PixabayResponseParserTests
    {
        [Test]
        public void Parse_ValidJson_PopulatesFields()
        {
            var dto = PixabayResponseParser.Parse(PixabayJsonFixtures.SuccessTwoHits);

            Assert.That(dto.TotalHits, Is.EqualTo(2));
            Assert.That(dto.Hits.Count, Is.EqualTo(2));
            Assert.That(dto.Hits[0].Id, Is.EqualTo(1));
            Assert.That(dto.Hits[0].Tags, Is.EqualTo("cat, animal"));
            Assert.That(dto.Hits[0].PreviewUrl, Is.EqualTo("http://x/1.jpg"));
            Assert.That(dto.Hits[0].User, Is.EqualTo("alice"));
        }

        [Test]
        public void Parse_EmptyHits_ReturnsEmptyList()
        {
            var dto = PixabayResponseParser.Parse(PixabayJsonFixtures.SuccessEmptyHits);

            Assert.That(dto.TotalHits, Is.EqualTo(0));
            Assert.That(dto.Hits, Is.Empty);
        }
    }
}
