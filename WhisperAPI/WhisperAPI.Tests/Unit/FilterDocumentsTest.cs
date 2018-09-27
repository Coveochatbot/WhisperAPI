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
using WhisperAPI.Services.MLAPI.Facets;
using WhisperAPI.Tests.Data.Builders;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class FilterDocumentsTest
    {
        private const string BaseAddress = "http://localhost:5000";

        private IFilterDocuments _filterDocuments;

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
            var documents = new List<string>
            {
                "document1",
                "document2",
                "document3"
            };

            this._httpClient = new HttpClient(this._httpMessageHandlerMock.Object);
            this._filterDocuments = new FilterDocuments(this._httpClient, BaseAddress);

            this.HttpMessageHandlerMock(documents, HttpStatusCode.OK);

            var facet = FacetBuilder.Build.Instance;
            var parameters = FilterDocumentsParametersBuilder.Build
                .WithDocuments(documents)
                .AddMustHaveFacet(facet)
                .AddMustNotHaveFacet(facet).Instance;

            var result = this._filterDocuments.FilterDocumentsByFacets(parameters);

            result.Should().BeEquivalentTo(documents);
        }

        [Test]
        [TestCase(HttpStatusCode.InternalServerError)]
        [TestCase(HttpStatusCode.NotFound)]
        public void When_receive_not_ok_response_from_post_then_throws_exception(HttpStatusCode status)
        {
            this._httpClient = new HttpClient(this._httpMessageHandlerMock.Object);
            this._filterDocuments = new FilterDocuments(this._httpClient, BaseAddress);

            this.HttpMessageHandlerMock(null, status);

            var parameters = FilterDocumentsParametersBuilder.Build.Instance;
            Assert.Throws<HttpRequestException>(() => this._filterDocuments.FilterDocumentsByFacets(parameters));
        }

        [Test]
        public void When_receive_ok_response_with_empty_content_from_post_then_returns_null()
        {
            this._httpClient = new HttpClient(this._httpMessageHandlerMock.Object);
            this._filterDocuments = new FilterDocuments(this._httpClient, BaseAddress);

            this.HttpMessageHandlerMock(null, HttpStatusCode.OK);

            var parameters = FilterDocumentsParametersBuilder.Build.Instance;
            var result = this._filterDocuments.FilterDocumentsByFacets(parameters);
            result.Should().BeEquivalentTo((List<string>)null);
        }

        private void HttpMessageHandlerMock(List<string> documents, HttpStatusCode httpStatusCode)
        {
            this._httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode,
                    Content = new StringContent(JsonConvert.SerializeObject(documents))
                }));
        }
    }
}
