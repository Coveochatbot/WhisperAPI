using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using WhisperAPI.Models;
using WhisperAPI.Services.MLAPI.Facets;
using WhisperAPI.Tests.Data.Builders;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class DocumentFacetsTest
    {
        private IDocumentFacets _documentFacets;

        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            this._httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        }

        [Test]
        public void When_receive_ok_response_from_post_then_returns_result_correctly()
        {
            var questions = new List<Question>
            {
                QuestionBuilder.Build.Instance
            };

            const string baseAdress = "http://localhost:5000";

            this._httpClient = new HttpClient(this._httpMessageHandlerMock.Object);
            this._documentFacets = new DocumentFacets(this._httpClient, baseAdress);

            this._httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(questions))
                }));

            var result = this._documentFacets.GetQuestions(new List<SuggestedDocument>());

            result.Should().BeEquivalentTo(questions);
        }

        [Test]
        [TestCase(HttpStatusCode.InternalServerError)]
        [TestCase(HttpStatusCode.NotFound)]
        public void When_receive_not_ok_response_from_post_then_throws_exception(HttpStatusCode status)
        {
            const string baseAdress = "http://localhost:5000";

            this._httpClient = new HttpClient(this._httpMessageHandlerMock.Object);
            this._documentFacets = new DocumentFacets(this._httpClient, baseAdress);

            this._httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(string.Empty)
                }));

            Assert.Throws<HttpRequestException>(() => this._documentFacets.GetQuestions(new List<SuggestedDocument>()));
        }

        [Test]
        public void When_receive_ok_response_with_empty_content_from_post_then_returns_null()
        {
            const string baseAdress = "http://localhost:5000";

            this._httpClient = new HttpClient(this._httpMessageHandlerMock.Object);
            this._documentFacets = new DocumentFacets(this._httpClient, baseAdress);

            this._httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(string.Empty))
                }));

            var result = this._documentFacets.GetQuestions(new List<SuggestedDocument>());
            result.Should().BeEquivalentTo((List<Question>)null);
        }
    }
}
