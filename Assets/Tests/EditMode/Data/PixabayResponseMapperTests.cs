using System.Collections.Generic;
using ImageSearch.Data.Dtos;
using ImageSearch.Data.Mapping;
using NUnit.Framework;

namespace ImageSearch.Data.Tests
{
    public class PixabayResponseMapperTests
    {
        [Test]
        public void ToModel_MapsFieldsExactly()
        {
            var dto = new PixabayImageDto
            {
                Id = 123,
                Tags = "a, b",
                WebformatUrl = "http://x/webformat.jpg",
                User = "alice"
            };

            var model = PixabayResponseMapper.ToModel(dto);

            Assert.That(model.Id, Is.EqualTo(123));
            Assert.That(model.AuthorName, Is.EqualTo("alice"));
            Assert.That(model.ThumbnailUrl, Is.EqualTo("http://x/webformat.jpg"));
            Assert.That(model.Tags, Is.EqualTo(new[] { "a", "b" }));
        }

        [Test]
        public void ToModel_EmptyTagString_ReturnsEmptyTagList()
        {
            var dto = new PixabayImageDto { Id = 1, Tags = "", WebformatUrl = "u", User = "u" };

            var model = PixabayResponseMapper.ToModel(dto);

            Assert.That(model.Tags, Is.Empty);
        }

        [Test]
        public void ToModel_SingleTag_ReturnsOneItem()
        {
            var dto = new PixabayImageDto { Id = 1, Tags = "cat", WebformatUrl = "u", User = "u" };

            var model = PixabayResponseMapper.ToModel(dto);

            Assert.That(model.Tags, Is.EqualTo(new[] { "cat" }));
        }

        [Test]
        public void ToModel_MessyTagSpacing_TrimsWhitespace()
        {
            var dto = new PixabayImageDto { Id = 1, Tags = "cat, animal , cute", WebformatUrl = "u", User = "u" };

            var model = PixabayResponseMapper.ToModel(dto);

            Assert.That(model.Tags, Is.EqualTo(new[] { "cat", "animal", "cute" }));
        }

        [Test]
        public void ToModels_PreservesOrder()
        {
            var dtos = new List<PixabayImageDto>
            {
                new() { Id = 1, Tags = "", WebformatUrl = "u1", User = "a" },
                new() { Id = 2, Tags = "", WebformatUrl = "u2", User = "b" },
                new() { Id = 3, Tags = "", WebformatUrl = "u3", User = "c" }
            };

            var models = PixabayResponseMapper.ToModels(dtos);

            Assert.That(models.Count, Is.EqualTo(3));
            Assert.That(models[0].Id, Is.EqualTo(1));
            Assert.That(models[1].Id, Is.EqualTo(2));
            Assert.That(models[2].Id, Is.EqualTo(3));
        }

        [Test]
        public void ToModels_EmptyList_ReturnsEmptyNotNull()
        {
            var models = PixabayResponseMapper.ToModels(new List<PixabayImageDto>());

            Assert.That(models, Is.Not.Null);
            Assert.That(models, Is.Empty);
        }
    }
}
