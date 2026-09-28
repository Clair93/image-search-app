using ImageSearch.Data.Dtos;
using Newtonsoft.Json;

namespace ImageSearch.Data.Sources
{
    public static class PixabayResponseParser
    {
        public static PixabaySearchResponseDto Parse(string json)
        {
            return JsonConvert.DeserializeObject<PixabaySearchResponseDto>(json);
        }
    }
}
