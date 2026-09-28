using System;
using System.Net;
using System.Net.Http;
using ImageSearch.Core;

namespace ImageSearch.Data.Sources
{
    public static class NetworkErrorClassifier
    {
        public static NetworkError FromStatusCode(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.NotFound)
            {
                return NetworkError.NotFound;
            }

            if ((int)statusCode >= 500)
            {
                return NetworkError.ServerError;
            }

            return NetworkError.Unknown;
        }

        public static NetworkError FromException(Exception exception)
        {
            if (exception is HttpRequestException)
            {
                return NetworkError.NoInternet;
            }

            return NetworkError.Unknown;
        }
    }
}
