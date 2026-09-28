using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageSearch.Data.Dtos
{
    public sealed class PixabaySearchResponseDto
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("totalHits")]
        public int TotalHits { get; set; }

        [JsonProperty("hits")]
        public List<PixabayImageDto> Hits { get; set; }
    }
}
