﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using WhisperAPI.Controllers;
using WhisperAPI.Models;
using WhisperAPI.Models.NLPAPI;
using WhisperAPI.Models.Queries;
using WhisperAPI.Services.Context;
using WhisperAPI.Services.MLAPI.Facets;
using WhisperAPI.Services.NLPAPI;
using WhisperAPI.Services.Questions;
using WhisperAPI.Services.Search;
using WhisperAPI.Services.Suggestions;
using WhisperAPI.Tests.Data.Builders;
using BadRequestResult = Microsoft.AspNetCore.Mvc.BadRequestResult;

namespace WhisperAPI.Tests.Integration
{
    [TestFixture]
    public class SuggestionsControllerTest
    {
        private SuggestionsController _suggestionController;

        private Mock<HttpMessageHandler> _indexSearchHttpMessageHandleMock;
        private Mock<HttpMessageHandler> _nlpCallHttpMessageHandleMock;
        private Mock<HttpMessageHandler> _documentFacetsHttpMessageHandleMock;
        private Mock<HttpMessageHandler> _filterDocumentsHttpMessageHandleMock;

        [SetUp]
        public void SetUp()
        {
            this._indexSearchHttpMessageHandleMock = new Mock<HttpMessageHandler>();
            this._nlpCallHttpMessageHandleMock = new Mock<HttpMessageHandler>();
            this._documentFacetsHttpMessageHandleMock = new Mock<HttpMessageHandler>();
            this._filterDocumentsHttpMessageHandleMock = new Mock<HttpMessageHandler>();

            var indexSearchHttpClient = new HttpClient(this._indexSearchHttpMessageHandleMock.Object);
            var nlpCallHttpClient = new HttpClient(this._nlpCallHttpMessageHandleMock.Object);
            var documentFacetHttpClient = new HttpClient(this._documentFacetsHttpMessageHandleMock.Object);
            var filterDocumentHttpClient = new HttpClient(this._filterDocumentsHttpMessageHandleMock.Object);

            var indexSearch = new IndexSearch(null, indexSearchHttpClient, "https://localhost:5000");
            var nlpCall = new NlpCall(nlpCallHttpClient, "https://localhost:5000");
            var documentFacets = new DocumentFacets(documentFacetHttpClient, "https://localhost:5000");
            var filterDocuments = new FilterDocuments(filterDocumentHttpClient, "https://localhost:5000");

            var suggestionsService = new SuggestionsService(indexSearch, nlpCall, documentFacets, filterDocuments, this.GetIrrelevantIntents());

            var contexts = new InMemoryContexts(new TimeSpan(1, 0, 0, 0));
            var questionsService = new QuestionsService();
            this._suggestionController = new SuggestionsController(suggestionsService, questionsService, contexts);
        }

        [Test]
        public void When_getting_suggestions_with_good_query_then_returns_suggestions_correctly()
        {
            var questions = GetQuestions();

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetRelevantNlpAnalysis()))
                }));

            this._indexSearchHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = this.GetSearchResultStringContent()
                }));

            this._documentFacetsHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(questions))
                }));

            var searchQuery = SearchQueryBuilder.Build.Instance;

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            var result = this._suggestionController.GetSuggestions(searchQuery);

            var suggestion = result.As<OkObjectResult>().Value as Suggestion;

            var questionsToClient = questions.Select(q => QuestionToClient.FromQuestion(q)).ToList();

            suggestion.SuggestedDocuments.Should().BeEquivalentTo(GetSuggestedDocuments());
            suggestion.Questions.Should().BeEquivalentTo(questionsToClient);
        }

        [Test]
        public void When_getting_suggestions_with_null_query_then_returns_bad_request()
        {
            var searchQuery = SearchQueryBuilder.Build.WithQuery(null).Instance;

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            var suggestions = this._suggestionController.GetSuggestions(searchQuery);

            suggestions.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Test]
        public void When_getting_suggestions_with_irrelevant_query_then_returns_empty_list()
        {
            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetIrrelevantNlpAnalysis()))
                }));

            this._documentFacetsHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new List<string>()))
                }));

            var searchQuery = SearchQueryBuilder.Build.Instance;
            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            var result = this._suggestionController.GetSuggestions(searchQuery);

            var suggestion = result.As<OkObjectResult>().Value as Suggestion;

            suggestion.SuggestedDocuments.Should().BeEquivalentTo(new List<SuggestedDocument>());
        }

        [Test]
        [TestCase(System.Net.HttpStatusCode.NotFound)]
        [TestCase(System.Net.HttpStatusCode.BadRequest)]
        [TestCase(System.Net.HttpStatusCode.InternalServerError)]
        public void When_getting_suggestions_but_nlp_returns_Error_then_throws_exception(System.Net.HttpStatusCode httpStatusCode)
        {
            var searchQuery = SearchQueryBuilder.Build
                .WithQuery("Hello")
                .Instance;

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode,
                    Content = new StringContent(string.Empty)
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            Assert.Throws<HttpRequestException>(() => this._suggestionController.GetSuggestions(searchQuery));
        }

        [Test]
        [TestCase(System.Net.HttpStatusCode.NotFound)]
        [TestCase(System.Net.HttpStatusCode.BadRequest)]
        [TestCase(System.Net.HttpStatusCode.InternalServerError)]
        public void When_getting_suggestions_but_coveo_returns_Error_then_throws_exception(System.Net.HttpStatusCode httpStatusCode)
        {
            var searchQuery = SearchQueryBuilder.Build
                .WithQuery("I need help with CoveoSearch API")
                .Instance;

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetRelevantNlpAnalysis()))
                }));

            this._indexSearchHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = httpStatusCode,
                    Content = this.GetSearchResultStringContent()
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            Assert.Throws<HttpRequestException>(() => this._suggestionController.GetSuggestions(searchQuery));
        }

        [Test]
        public void When_getting_suggestions_with_agent_sending_uri_then_suggestion_is_filter_then_returns_suggestions_correctly()
        {
            // Customer says: Hello
            var searchQuery = SearchQueryBuilder.Build
                .WithQuery("Hello")
                .Instance;

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetIrrelevantNlpAnalysis()))
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            var result = this._suggestionController.GetSuggestions(searchQuery);

            var suggestion = result.As<OkObjectResult>().Value as Suggestion;

            suggestion.SuggestedDocuments.Should().BeEquivalentTo(new List<SuggestedDocument>());
            suggestion.Questions.Should().BeEquivalentTo(null);

            // Customer says: I need help with CoveoSearch API
            searchQuery = SearchQueryBuilder.Build
                .WithQuery("I need help with CoveoSearch API")
                .WithChatKey(searchQuery.ChatKey)
                .Instance;

            var questions = GetQuestions();

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetRelevantNlpAnalysis()))
                }));

            this._indexSearchHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = this.GetSearchResultStringContent()
                }));

            this._documentFacetsHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(questions))
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            result = this._suggestionController.GetSuggestions(searchQuery);

            suggestion = result.As<OkObjectResult>().Value as Suggestion;

            var questionsToClient = questions.Select(q => QuestionToClient.FromQuestion(q)).ToList();

            suggestion.SuggestedDocuments.Should().BeEquivalentTo(GetSuggestedDocuments());
            suggestion.Questions.Should().BeEquivalentTo(questionsToClient);

            // Agent says: Maybe this could help: https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm
            searchQuery = SearchQueryBuilder.Build
                .WithQuery("Maybe this can help: https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm")
                .WithMessageType(SearchQuery.MessageType.Agent)
                .WithChatKey(searchQuery.ChatKey)
                .Instance;

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetRelevantNlpAnalysis()))
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            result = this._suggestionController.GetSuggestions(searchQuery);

            result.Should().NotBeNull();

            suggestion = result.As<OkObjectResult>().Value as Suggestion;

            suggestion.SuggestedDocuments.Count.Should().NotBe(0);
            suggestion.SuggestedDocuments.Select(x => x.Uri)
                .Contains("https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm").Should().BeFalse();
        }

        [Test]
        public void When_getting_suggestions_with_customer_sending_uri_then_suggestion_is_filter_then_returns_suggestions_correctly()
        {
            // Customer says: Hello
            var searchQuery = SearchQueryBuilder.Build
                .WithQuery("Hello")
                .Instance;

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetIrrelevantNlpAnalysis()))
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            var result = this._suggestionController.GetSuggestions(searchQuery);

            var suggestion = result.As<OkObjectResult>().Value as Suggestion;

            suggestion.SuggestedDocuments.Should().BeEquivalentTo(new List<SuggestedDocument>());
            suggestion.Questions.Should().BeEquivalentTo(null);

            // Customer says: I need help with CoveoSearch API
            searchQuery = SearchQueryBuilder.Build
                .WithQuery("I need help with CoveoSearch API and I look at this: https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm")
                .WithChatKey(searchQuery.ChatKey)
                .Instance;

            var questions = GetQuestions();

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetRelevantNlpAnalysis()))
                }));

            this._indexSearchHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = this.GetSearchResultStringContent()
                }));

            this._documentFacetsHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(questions))
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            result = this._suggestionController.GetSuggestions(searchQuery);

            result.Should().NotBeNull();

            suggestion = result.As<OkObjectResult>().Value as Suggestion;

            suggestion.SuggestedDocuments.Count.Should().NotBe(0);
            suggestion.SuggestedDocuments.Select(x => x.Uri)
                .Contains("https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm").Should().BeFalse();
        }

        [Test]
        public void When_getting_suggestions_with_more_attribute_then_required_then_returns_correctly()
        {
            var jsonSearchQuery = "{\"chatkey\": \"aecaa8db-abc8-4ac9-aa8d-87987da2dbb0\",\"Query\": \"Need help with CoveoSearch API\",\"Type\": 1,\"command\": \"sudo ls\"}";
            var questions = GetQuestions();

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetRelevantNlpAnalysis()))
                }));

            this._indexSearchHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = this.GetSearchResultStringContent()
                }));

            this._documentFacetsHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(questions))
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(JsonConvert.DeserializeObject<SearchQuery>(jsonSearchQuery)));
            var result = this._suggestionController.GetSuggestions(JsonConvert.DeserializeObject<SearchQuery>(jsonSearchQuery));

            var suggestion = result.As<OkObjectResult>().Value as Suggestion;

            suggestion.SuggestedDocuments.Should().BeEquivalentTo(GetSuggestedDocuments());
        }

        [Test]
        public void When_getting_suggestions_then_refresh_result_are_the_same()
        {
            var queryChatkeyRefresh = new Query
            {
                ChatKey = new Guid("aecaa8db-abc8-4ac9-aa8d-87987da2dbb0")
            };
            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetRelevantNlpAnalysis()))
                }));

            this._indexSearchHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = this.GetSearchResultStringContent()
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(queryChatkeyRefresh));
            var resultSuggestions = this._suggestionController.GetSuggestions(queryChatkeyRefresh);

            var suggestion = (Suggestion)resultSuggestions.As<OkObjectResult>().Value;
            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(queryChatkeyRefresh));
            var resultLastSuggestions = this._suggestionController.GetSuggestions(queryChatkeyRefresh);

            var lastSuggestion = (Suggestion) resultLastSuggestions.As<OkObjectResult>().Value;
            lastSuggestion.SuggestedDocuments.Should().BeEquivalentTo(suggestion.SuggestedDocuments);
            lastSuggestion.Questions.Should().BeEquivalentTo(suggestion.Questions);
        }

        [Test]
        public void When_getting_suggestions_and_agent_click_on_question_and_agent_asks_question_to_custommer_then_question_is_filter_out()
        {
            // Customer says: I need help with CoveoSearch API
            var searchQuery = SearchQueryBuilder.Build
                .WithQuery("I need help with CoveoSearch API")
                .Instance;

            var questions = GetQuestions();

            this._nlpCallHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(this.GetRelevantNlpAnalysis()))
                }));

            this._indexSearchHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = this.GetSearchResultStringContent()
                }));

            this._documentFacetsHttpMessageHandleMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(questions))
                }));

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(searchQuery));
            var result = this._suggestionController.GetSuggestions(searchQuery);

            var suggestion = result.As<OkObjectResult>().Value as Suggestion;

            var questionsToClient = questions.Select(q => QuestionToClient.FromQuestion(q)).ToList();

            suggestion.SuggestedDocuments.Should().BeEquivalentTo(GetSuggestedDocuments());
            suggestion.Questions.Should().BeEquivalentTo(questionsToClient);

            // Agent click on a question in the UI
            var selectQuery = SelectQueryBuilder.Build.WithChatKey(searchQuery.ChatKey).WithId(questions[0].Id).Instance;
            this._suggestionController.SelectSuggestion(selectQuery);

            // Agent asks the question he clicked to the custommer
            searchQuery.Type = SearchQuery.MessageType.Agent;
            searchQuery.Query = questions[0].Text;
            result = this._suggestionController.GetSuggestions(searchQuery);
            suggestion = result.As<OkObjectResult>().Value as Suggestion;

            suggestion.SuggestedDocuments.Should().BeEquivalentTo(GetSuggestedDocuments());
            suggestion.Questions.Count.Should().Be(1);
            suggestion.Questions.Single().Should().BeEquivalentTo(questionsToClient[1]);
        }

        private static List<SuggestedDocument> GetSuggestedDocuments()
        {
            return new List<SuggestedDocument>
            {
                new SuggestedDocument
                {
                    Title = "Available Coveo Cloud V2 Source Types",
                    Uri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                    PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                    Summary = null,
                    Excerpt = null
                },
                new SuggestedDocument
                {
                    Title = "Coveo Cloud Query Syntax Reference",
                    Uri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                    PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                    Summary = null,
                    Excerpt = null
                },
                new SuggestedDocument
                {
                    Title = "Events",
                    Uri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                    PrintableUri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                    Summary = null,
                    Excerpt = null
                },
                new SuggestedDocument
                {
                    Title = "Coveo Facet Component (CoveoFacet)",
                    Uri = "https://coveo.github.io/search-ui/components/facet.html",
                    PrintableUri = "https://coveo.github.io/search-ui/components/facet.html",
                    Summary = null,
                    Excerpt = null
                }
            };
        }

        private static List<Question> GetQuestions()
        {
            return new List<Question>
            {
                FacetQuestionBuilder.Build.WithFacetName("Dummy").WithFacetValues("A", "B", "C").Instance,
                FacetQuestionBuilder.Build.WithFacetName("Dummy").WithFacetValues("C", "D", "E").Instance,
            };
        }

        private ActionExecutingContext GetActionExecutingContext(Query query)
        {
            var actionContext = new ActionContext(
                new Mock<HttpContext>().Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object);

            var actionExecutingContext = new Mock<ActionExecutingContext>(
                MockBehavior.Strict,
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                this._suggestionController);

            actionExecutingContext
                .Setup(x => x.ActionArguments.Values)
                .Returns(new[] { query });

            IActionResult result = new OkResult();

            actionExecutingContext
                .SetupSet(x => x.Result = It.IsAny<IActionResult>())
                .Callback<IActionResult>(value => result = value);

            actionExecutingContext
                .SetupGet(x => x.Result)
                .Returns(result);

            return actionExecutingContext.Object;
        }

        private NlpAnalysis GetRelevantNlpAnalysis()
        {
            var intents = new List<Intent>
            {
                IntentBuilder.Build.WithName("Need Help").WithConfidence(98).Instance,
                IntentBuilder.Build.WithName("Greetings").WithConfidence(2).Instance
            };

            return NlpAnalysisBuilder.Build.WithIntents(intents).Instance;
        }

        private NlpAnalysis GetIrrelevantNlpAnalysis()
        {
            var intents = new List<Intent>
            {
                IntentBuilder.Build.WithName("Need Help").WithConfidence(2).Instance,
                IntentBuilder.Build.WithName("Greetings").WithConfidence(98).Instance
            };

            return NlpAnalysisBuilder.Build.WithIntents(intents).Instance;
        }

        private StringContent GetSearchResultStringContent()
        {
            return new StringContent("{\"totalCount\": 4,\"results\": [{\"title\": \"Available Coveo Cloud V2 Source Types\", \"excerpt\": \"This is the excerpt\", \"clickUri\": \"https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm\",\"printableUri\": \"https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm\",\"score\": 4280       },{\"title\": \"Coveo Cloud Query Syntax Reference\",\"excerpt\": \"This is the excerpt\", \"clickUri\": \"https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm\",\"printableUri\": \"https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm\",\"score\": 3900},{\"title\": \"Events\", \"excerpt\": \"This is the excerpt\", \"clickUri\": \"https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573\",\"printableUri\": \"https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573\",\"score\": 2947},{\"title\": \"Coveo Facet Component (CoveoFacet)\", \"excerpt\": \"This is the excerpt\", \"clickUri\": \"https://coveo.github.io/search-ui/components/facet.html\",\"printableUri\": \"https://coveo.github.io/search-ui/components/facet.html\",\"score\": 2932}]}");
        }

        private List<string> GetIrrelevantIntents()
        {
            return new List<string>
            {
                "Greetings"
            };
        }

        private Mock<ActionExecutingContext> CreateActionContextMock()
        {
            var actionContext = new ActionContext(
                new Mock<HttpContext>().Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object);

            return new Mock<ActionExecutingContext>(
                MockBehavior.Strict,
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                this._suggestionController);
        }
    }
}
