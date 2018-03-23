using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class IndexSearchTest
    {
        private IIndexSearch _indexSearch;
        private HttpClient _httpClient;

        private SearchResult _searchResult;

        [SetUp]
        public void SetUp()
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"totalCount\": 4,\"results\": [{\"title\": \"Available Coveo Cloud V2 Source Types\",\"uri\": \"https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm\",\"printableUri\": \"https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm\",\"score\": 4280       },{\"title\": \"Coveo Cloud Query Syntax Reference\",\"uri\": \"https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm\",\"printableUri\": \"https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm\",\"score\": 3900},{\"title\": \"Events\",\"uri\": \"https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573\",\"printableUri\": \"https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573\",\"score\": 2947},{\"title\": \"Coveo Facet Component (CoveoFacet)\",\"uri\": \"https://coveo.github.io/search-ui/components/facet.html\",\"printableUri\": \"https://coveo.github.io/search-ui/components/facet.html\",\"score\": 2932}]}")
                }));

            this._httpClient = new HttpClient(httpMessageHandler.Object);

            this._indexSearch = new IndexSearch(null, this._httpClient);

            this._searchResult = new SearchResult
            {
                NbrElements = 4,
                Elements = new List<SearchResultElement>
                {
                    new SearchResultElement
                    {
                        Title = "Available Coveo Cloud V2 Source Types",
                        Uri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                        PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                        Summary = null,
                        Score = 4280
                    },
                    new SearchResultElement
                    {
                        Title = "Coveo Cloud Query Syntax Reference",
                        Uri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                        PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                        Summary = null,
                        Score = 3900
                    },
                    new SearchResultElement
                    {
                        Title = "Events",
                        Uri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                        PrintableUri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                        Summary = null,
                        Score = 2947
                    },
                    new SearchResultElement
                    {
                        Title = "Coveo Facet Component (CoveoFacet)",
                        Uri = "https://coveo.github.io/search-ui/components/facet.html",
                        PrintableUri = "https://coveo.github.io/search-ui/components/facet.html",
                        Summary = null,
                        Score = 2932
                    }
                }
            };
        }

        [Test]
        [TestCase("test")]
        public void Receive_response_from_post_then_return_result(string querry)
        {
            this._indexSearch.Search(querry).Should().BeEquivalentTo(this._searchResult);
        }
    }
}
