using Newtonsoft.Json;

namespace ImageSearch.Data.Dtos
{
    public sealed class PixabayImageDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("previewURL")]
        public string PreviewUrl { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }
    }
}
