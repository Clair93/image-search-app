namespace ImageSearch.Data.Tests.Fixtures
{
    public static class PixabayJsonFixtures
    {
        public const string SuccessTwoHits = @"{
            ""total"": 2,
            ""totalHits"": 2,
            ""hits"": [
                { ""id"": 1, ""tags"": ""cat, animal"", ""previewURL"": ""http://x/1.jpg"", ""webformatURL"": ""http://x/1-web.jpg"", ""user"": ""alice"" },
                { ""id"": 2, ""tags"": ""dog"", ""previewURL"": ""http://x/2.jpg"", ""webformatURL"": ""http://x/2-web.jpg"", ""user"": ""bob"" }
            ]
        }";

        public const string SuccessEmptyHits = @"{
            ""total"": 0,
            ""totalHits"": 0,
            ""hits"": []
        }";

        public const string SuccessEmptyTag = @"{
            ""total"": 1,
            ""totalHits"": 1,
            ""hits"": [
                { ""id"": 3, ""tags"": """", ""previewURL"": ""http://x/3.jpg"", ""webformatURL"": ""http://x/3-web.jpg"", ""user"": ""carol"" }
            ]
        }";

        public const string SuccessMessyTags = @"{
            ""total"": 1,
            ""totalHits"": 1,
            ""hits"": [
                { ""id"": 4, ""tags"": ""cat, animal , cute"", ""previewURL"": ""http://x/4.jpg"", ""webformatURL"": ""http://x/4-web.jpg"", ""user"": ""dave"" }
            ]
        }";
    }
}
