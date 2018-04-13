using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using WhisperAPI.Controllers;
using WhisperAPI.Models;
using WhisperAPI.Services;
using static WhisperAPI.Models.SearchQuerry;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class SuggestionsControllerTest
    {
        private List<SearchQuerry> _invalidSearchQuerryList;
        private List<SearchQuerry> _validSearchQuerryList;

        private Mock<ISuggestionsService> _suggestionServiceMock;
        private SuggestionsController _suggestionController;
        private InMemoryContexts _contexts;

        [SetUp]
        public void SetUp()
        {
            this._contexts = new InMemoryContexts(new TimeSpan(1, 0, 0, 0));
            this._invalidSearchQuerryList = new List<SearchQuerry>
            {
                null,
                new SearchQuerry { ChatKey = new Guid("0f8fad5b-d9cb-469f-a165-708677289501"), Querry = null, Type = MessageType.Customer },
                new SearchQuerry { ChatKey = new Guid("0f8fad5b-d9cb-469f-a165-708677289501"), Querry = null, Type = MessageType.Agent }
            };

            this._validSearchQuerryList = new List<SearchQuerry>
            {
                 new SearchQuerry { ChatKey = new Guid("0f8fad5b-d9cb-469f-a165-708677289501"), Querry = "test", Type = MessageType.Customer },
                 new SearchQuerry { ChatKey = new Guid("0f8fad5b-d9cb-469f-a165-708677289501"), Querry = "test", Type = MessageType.Agent }
            };
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void When_receive_invalid_or_null_searchQuerry_then_return_bad_request(int invalidQuerryIndex)
        {
            this._suggestionServiceMock = new Mock<ISuggestionsService>();
            this._suggestionServiceMock
                .Setup(x => x.GetSuggestions(It.IsAny<ConversationContext>()))
                .Returns(this.GetListOfDocuments());

            this._suggestionController = new SuggestionsController(this._suggestionServiceMock.Object, null);

            this._suggestionController.GetSuggestions(this._invalidSearchQuerryList[invalidQuerryIndex]).Should().BeEquivalentTo(new BadRequestResult());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void When_receive_valid_searchQuerry_then_return_Ok_request(int validQuerryIndex)
        {
            this._suggestionServiceMock = new Mock<ISuggestionsService>();
            this._suggestionServiceMock
                .Setup(x => x.GetSuggestions(It.IsAny<ConversationContext>()))
                .Returns(this.GetListOfDocuments());

            this._suggestionController = new SuggestionsController(this._suggestionServiceMock.Object, this._contexts);

            this._suggestionController.OnActionExecuting(this.GetActionExecutingContext(validQuerryIndex));
            this._suggestionController.GetSuggestions(this._validSearchQuerryList[validQuerryIndex]).Should().BeEquivalentTo(new OkObjectResult(this.GetListOfDocuments()));
        }

        public ActionExecutingContext GetActionExecutingContext(int indexOfTest)
        {
            var actionContext = new ActionContext(
                new Mock<HttpContext>().Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object
            );

            var actionExecutingContext = new Mock<ActionExecutingContext>(
                MockBehavior.Strict,
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                this._suggestionController);

            actionExecutingContext
                .Setup(x => x.ActionArguments["searchQuerry"])
                .Returns(this._validSearchQuerryList[indexOfTest]);

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
