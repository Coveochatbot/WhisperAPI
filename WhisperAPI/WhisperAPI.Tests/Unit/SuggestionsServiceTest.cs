using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using WhisperAPI.Models;
using WhisperAPI.Models.NLPAPI;
using WhisperAPI.Services;
using WhisperAPI.Tests.Data.Builders;
using static WhisperAPI.Models.SearchQuerry;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class SuggestionsServiceTest
    {
        private ISuggestionsService _suggestionsService;

        private Mock<IIndexSearch> _indexSearchMock;
        private Mock<INlpCall> _nlpCallMock;

        [SetUp]
        public void SetUp()
        {
            this._indexSearchMock = new Mock<IIndexSearch>();
            this._nlpCallMock = new Mock<INlpCall>();

            this._suggestionsService = new SuggestionsService(this._indexSearchMock.Object, this._nlpCallMock.Object, this.GetIrrelevantsIntents());
        }

        [Test]
        [TestCase]
        public void When_receive_valid_searchresult_from_search_then_return_list_of_suggestedDocuments()
        {
            var intents = new List<Intent>
            {
                new IntentBuilder().WithName("Need Help").Build()
            };

            var nlpAnalysis = new NlpAnalysisBuilder().WithIntents(intents).Build();

            this._indexSearchMock
                .Setup(x => x.Search(It.IsAny<string>()))
                .Returns(this.GetSearchResult());

            this._nlpCallMock
                .Setup(x => x.GetNlpAnalysis(It.IsAny<string>()))
                .Returns(nlpAnalysis);

            this._suggestionsService.GetSuggestions(this.GetSearchQueries()).Should().BeEquivalentTo(this.GetSuggestedDocuments());
        }

        [Test]
        [TestCase]
        public void When_receive_valid_searchresult_from_search_then_return_list_of_suggestedDocuments_filter_sent_suggestion()
        {
            var intents = new List<Intent>
            {
                new IntentBuilder().WithName("Need Help").Build()
            };

            var nlpAnalysis = new NlpAnalysisBuilder().WithIntents(intents).Build();

            this._indexSearchMock
                .Setup(x => x.Search(It.IsAny<string>()))
                .Returns(this.GetSearchResult());

            this._nlpCallMock
                .Setup(x => x.GetNlpAnalysis(It.IsAny<string>()))
                .Returns(nlpAnalysis);

            this._suggestionsService.GetSuggestions(this.GetSearchQueriesForFilterChoosenSuggestions()).Should()
                .NotContain(this.GetSuggestedDocuments()[0]);
        }

        [Test]
        [TestCase]
        public void When_receive_empty_searchresult_from_search_then_return_empty_list_of_suggestedDocuments()
        {
            var intents = new List<Intent>
            {
                new IntentBuilder().WithName("Need Help").Build()
            };

            var nlpAnalysis = new NlpAnalysisBuilder().WithIntents(intents).Build();

            this._indexSearchMock
                .Setup(x => x.Search(It.IsAny<string>()))
                .Returns(new SearchResult());

            this._nlpCallMock
                .Setup(x => x.GetNlpAnalysis(It.IsAny<string>()))
                .Returns(nlpAnalysis);

            this._suggestionsService.GetSuggestions(this.GetSearchQueries()).Should().BeEquivalentTo(new List<SuggestedDocument>());
        }

        [Test]
        [TestCase]
        public void When_receive_null_searchresult_from_search_then_return_empty_list_of_suggestedDocuments()
        {
            var intents = new List<Intent>
            {
                new IntentBuilder().WithName("Need Help").Build()
            };

            var nlpAnalysis = new NlpAnalysisBuilder().WithIntents(intents).Build();

            this._indexSearchMock
                .Setup(x => x.Search(It.IsAny<string>()))
                .Returns((ISearchResult)null);

            this._nlpCallMock
                .Setup(x => x.GetNlpAnalysis(It.IsAny<string>()))
                .Returns(nlpAnalysis);

            this._suggestionsService.GetSuggestions(this.GetSearchQueries()).Should().BeEquivalentTo(new List<SuggestedDocument>());
        }

        [Test]
        [TestCase]
        public void When_receive_irrelevant_intent_then_returns_empty_list_of_suggestedDocuments()
        {
            var intents = new List<Intent>
            {
                new IntentBuilder().WithName("Greetings").Build()
            };

            var nlpAnalysis = new NlpAnalysisBuilder().WithIntents(intents).Build();

            this._nlpCallMock
                .Setup(x => x.GetNlpAnalysis(It.IsAny<string>()))
                .Returns(nlpAnalysis);

            this._suggestionsService.GetSuggestions(this.GetSearchQueries()).Should().BeEquivalentTo(new List<SuggestedDocument>());
        }

        public SearchResult GetSearchResult()
        {
            return new SearchResult
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

        public List<SuggestedDocument> GetSuggestedDocuments()
        {
            return new List<SuggestedDocument>
            {
                new SuggestedDocument
                {
                    Title = "Available Coveo Cloud V2 Source Types",
                    Uri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                    PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                    Summary = null
                },
                new SuggestedDocument
                {
                    Title = "Coveo Cloud Query Syntax Reference",
                    Uri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                    PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                    Summary = null
                },
                new SuggestedDocument
                {
                    Title = "Events",
                    Uri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                    PrintableUri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                    Summary = null
                },
                new SuggestedDocument
                {
                    Title = "Coveo Facet Component (CoveoFacet)",
                    Uri = "https://coveo.github.io/search-ui/components/facet.html",
                    PrintableUri = "https://coveo.github.io/search-ui/components/facet.html",
                    Summary = null
                }
            };
        }

        public List<string> GetIrrelevantsIntents()
        {
            return new List<string>
            {
                "Greetings"
            };
        }

        public List<SearchQuerry> GetSearchQueries()
        {
            return new List<SearchQuerry>
            {
                new SearchQuerry { ChatKey = new Guid("0f8fad5b-d9cb-469f-a165-708677289501"), Querry = "rest api", Type = MessageType.Customer }
            };
        }

        public List<SearchQuerry> GetSearchQueriesForFilterChoosenSuggestions()
        {
            return new List<SearchQuerry>
            {
                new SearchQuerry { ChatKey = new Guid("0f8fad5b-d9cb-469f-a165-708677289501"), Querry = "rest api", Type = MessageType.Customer },
                new SearchQuerry { ChatKey = new Guid("0f8fad5b-d9cb-469f-a165-708677289501"), Querry = "Available Coveo Cloud V2 Source Types", Type = MessageType.Agent }
            };
        }
    }
}
