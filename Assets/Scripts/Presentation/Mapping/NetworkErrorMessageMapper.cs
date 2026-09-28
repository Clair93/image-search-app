using ImageSearch.Core;

namespace ImageSearch.Presentation.Mapping
{
    public static class NetworkErrorMessageMapper
    {
        public static string ToMessage(NetworkError error)
        {
            return error switch
            {
                NetworkError.NoInternet => "인터넷 연결을 확인해주세요",
                NetworkError.ServerError => "일시적인 서버 오류입니다. 잠시 후 다시 시도해주세요",
                NetworkError.NotFound => "요청하신 정보를 찾을 수 없습니다",
                NetworkError.Unknown => "알 수 없는 오류가 발생했습니다",
                _ => "알 수 없는 오류가 발생했습니다"
            };
        }
    }
}
