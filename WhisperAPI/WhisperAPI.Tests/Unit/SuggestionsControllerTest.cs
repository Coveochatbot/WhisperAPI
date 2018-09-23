using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using WhisperAPI.Controllers;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;
using WhisperAPI.Services.Context;
using WhisperAPI.Services.Suggestions;
using WhisperAPI.Tests.Data.Builders;
using static WhisperAPI.Models.SearchQuery;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class SuggestionsControllerTest
    {
        private List<SearchQuery> _invalidSearchQueryList;
        private List<SearchQuery> _validSearchQueryList;
        private List<SelectQuery> _invalidSelectQueryList;
        private List<SelectQuery> _validSelectQueryList;

        private Mock<ISuggestionsService> _suggestionServiceMock;
        private SuggestionsController _suggestionController;
        private InMemoryContexts _contexts;

        [SetUp]
        public void SetUp()
        {
            var chatKey = new Guid("0f8fad5b-d9cb-469f-a165-708677289501");

            this._contexts = new InMemoryContexts(new TimeSpan(1, 0, 0, 0));
            this._invalidSearchQueryList = new List<SearchQuery>
            {
                null,
                SearchQueryBuilder.Build.WithChatKey(chatKey).WithQuery(null).WithMessageType(MessageType.Customer).Instance,
                SearchQueryBuilder.Build.WithChatKey(chatKey).WithQuery(null).WithMessageType(MessageType.Agent).Instance,
            };

            this._validSearchQueryList = new List<SearchQuery>
            {
                SearchQueryBuilder.Build.WithChatKey(chatKey).WithQuery("test").WithMessageType(MessageType.Customer).Instance,
                SearchQueryBuilder.Build.WithChatKey(chatKey).WithQuery("test").WithMessageType(MessageType.Agent).Instance,
            };

            this._invalidSelectQueryList = new List<SelectQuery>
            {
                null,
                SelectQueryBuilder.Build.WithChatKey(chatKey).WithId(null).Instance,
            };

            this._validSelectQueryList = new List<SelectQuery>
            {
                SelectQueryBuilder.Build.WithChatKey(chatKey).WithId(new Guid("0f8fad7b-d9cb-469f-a165-708677289501")).Instance,
            };
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void When_receive_invalid_or_null_searchQuery_then_return_bad_request(int invalidQueryIndex)
        {
            this._suggestionServiceMock = new Mock<ISuggestionsService>();
            this._suggestionServiceMock
                .Setup(x => x.GetSuggestedDocuments(It.IsAny<ConversationContext>()))
                .Returns(this.GetListOfDocuments());

            this._suggestionController = new SuggestionsController(this._suggestionServiceMock.Object, this._contexts);

            SearchQuery query = this._invalidSearchQueryList[invalidQueryIndex];

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(query));
            this._suggestionController.GetSuggestions(query).Should().BeEquivalentTo(new BadRequestResult());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void When_receive_valid_searchQuery_then_return_Ok_request(int validQueryIndex)
        {
            this._suggestionServiceMock = new Mock<ISuggestionsService>();
            this._suggestionServiceMock
                .Setup(x => x.GetSuggestedDocuments(It.IsAny<ConversationContext>()))
                .Returns(this.GetListOfDocuments());

            this._suggestionController = new SuggestionsController(this._suggestionServiceMock.Object, this._contexts);

            SearchQuery query = this._validSearchQueryList[validQueryIndex];

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(query));
            var result = this._suggestionController.GetSuggestions(query);

            var suggestion = result.As<OkObjectResult>().Value as Suggestion;
            suggestion.SuggestedDocuments.Should().BeEquivalentTo(this.GetListOfDocuments());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void When_receive_invalid_or_null_selectQuery_then_return_bad_request(int invalidQueryIndex)
        {
            this._suggestionServiceMock = new Mock<ISuggestionsService>();
            this._suggestionServiceMock
                .Setup(x => x.UpdateContextWithSelectedSuggestion(It.IsAny<ConversationContext>(), new Guid("c21d07d5-fd5a-42ab-ac2c-2ef6101e58d1")))
                .Returns(false);

            SelectQuery query = this._invalidSelectQueryList[invalidQueryIndex];

            this._suggestionController = new SuggestionsController(this._suggestionServiceMock.Object, this._contexts);

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(query));
            this._suggestionController.SelectSuggestion(query).Should().BeEquivalentTo(new BadRequestResult());
        }

        [Test]
        [TestCase(0)]
        public void When_receive_valid_selectQuery_then_return_Ok_request(int validQueryIndex)
        {
            this._suggestionServiceMock = new Mock<ISuggestionsService>();
            this._suggestionServiceMock
                .Setup(x => x.UpdateContextWithSelectedSuggestion(It.IsAny<ConversationContext>(), this._validSelectQueryList[validQueryIndex].Id.Value))
                .Returns(true);

            SelectQuery query = this._invalidSelectQueryList[validQueryIndex];

            this._suggestionController = new SuggestionsController(this._suggestionServiceMock.Object, this._contexts);

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(query));
            this._suggestionController.SelectSuggestion(this._validSelectQueryList[validQueryIndex]).Should().BeEquivalentTo(new OkResult());
        }

        public ActionExecutingContext GetActionExecutingContext(Query query)
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

        public List<SuggestedDocument> GetListOfDocuments()
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
    }
}
