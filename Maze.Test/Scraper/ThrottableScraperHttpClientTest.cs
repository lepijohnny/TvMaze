using Maze.DTO;
using Maze.Scaraper;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Maze.Test.Scraper
{
    public class ThrottableScraperHttpClientTest
    {
        [Fact]
        public async Task Should_brake_when_rate_limited_by_external_api()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(m => m.CreateClient("tvmaze"))
                .Returns(BuildHttpClientStub(new Queue<HttpStatusCode>(new[] { HttpStatusCode.TooManyRequests })));

            var httpRateLimiterMock = new Mock<IHttpRateLimiter>();
            httpRateLimiterMock.Setup(m => m.BrakeAsync()).Verifiable();

            var sut = new TestHttpClient(httpClientFactoryMock.Object, new Mock<IHttpDeserializer<int>>().Object, httpRateLimiterMock.Object);

            // Act
            await Assert.ThrowsAsync<HttpRetriableException>(async () => await sut.GetAsync(""));

            // Assert
            httpRateLimiterMock.Verify();
        }

        [Fact]
        public async Task Should_unbrake_when_external_api_invoked()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(m => m.CreateClient("tvmaze"))
                .Returns(BuildHttpClientStub(new Queue<HttpStatusCode>(new[] { HttpStatusCode.OK })));

            var httpRateLimiterMock = new Mock<IHttpRateLimiter>();
            httpRateLimiterMock.Setup(m => m.UnbrakeAsync()).Verifiable();

            var sut = new TestHttpClient(httpClientFactoryMock.Object, new Mock<IHttpDeserializer<int>>().Object, httpRateLimiterMock.Object);

            // Act
            _ = await sut.GetAsync("");

            // Assert
            httpRateLimiterMock.Verify();
        }

        [Fact]
        public async Task Should_throw_nonretriable_exception_when_other_error_by_external_api()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(m => m.CreateClient("tvmaze"))
                .Returns(BuildHttpClientStub(new Queue<HttpStatusCode>(new[] { HttpStatusCode.BadRequest })));

            var httpRateLimiterMock = new Mock<IHttpRateLimiter>();

            var sut = new TestHttpClient(httpClientFactoryMock.Object, new Mock<IHttpDeserializer<int>>().Object, httpRateLimiterMock.Object);

            // Act
            await Assert.ThrowsAsync<HttpNonRetriableException>(async () => await sut.GetAsync(""));

            // Assert
            httpRateLimiterMock.Verify(m => m.BrakeAsync(), Times.Never);
            httpRateLimiterMock.Verify(m => m.UnbrakeAsync(), Times.Never);
        }

        private static HttpClient BuildHttpClientStub(Queue<HttpStatusCode> codes)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(() => new HttpResponseMessage()
            {
                StatusCode = codes.Dequeue(),
                Content = new StringContent("[{'id':1,'value':'1'}]"),
            })
            .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            return httpClient;
        }
    }

    internal class TestHttpClient : ThrottableScraperHttpClient<int>
    {
        public TestHttpClient(IHttpClientFactory httpClientFactory, IHttpDeserializer<int> httpDeserializer, IHttpRateLimiter httpRateLimiter)
            : base(httpClientFactory, httpDeserializer, httpRateLimiter)
        {
        }
    }  
}
