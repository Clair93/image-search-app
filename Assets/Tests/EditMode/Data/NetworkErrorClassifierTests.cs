using System;
using System.Net;
using System.Net.Http;
using ImageSearch.Core;
using ImageSearch.Data.Sources;
using NUnit.Framework;

namespace ImageSearch.Data.Tests
{
    public class NetworkErrorClassifierTests
    {
        [Test]
        public void FromStatusCode_500_ReturnsServerError()
        {
            Assert.That(
                NetworkErrorClassifier.FromStatusCode(HttpStatusCode.InternalServerError),
                Is.EqualTo(NetworkError.ServerError));
        }

        [Test]
        public void FromStatusCode_404_ReturnsNotFound()
        {
            Assert.That(
                NetworkErrorClassifier.FromStatusCode(HttpStatusCode.NotFound),
                Is.EqualTo(NetworkError.NotFound));
        }

        [Test]
        public void FromException_HttpRequestException_ReturnsNoInternet()
        {
            Assert.That(
                NetworkErrorClassifier.FromException(new HttpRequestException()),
                Is.EqualTo(NetworkError.NoInternet));
        }

        [Test]
        public void FromException_UnrecognizedException_ReturnsUnknown()
        {
            Assert.That(
                NetworkErrorClassifier.FromException(new FormatException()),
                Is.EqualTo(NetworkError.Unknown));
        }
    }
}
