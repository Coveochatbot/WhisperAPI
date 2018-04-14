using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Controllers
{
    public class ContextController : Controller
    {
        private readonly IContexts _contexts;

        public ContextController(IContexts contexts)
        {
            this._contexts = contexts;
        }

        protected ConversationContext ConversationContext { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            if (!this.ModelState.IsValid)
            {
                actionExecutingContext.Result = this.BadRequest(this.ModelState);
                return;
            }

            var searchQuery = (SearchQuery)actionExecutingContext.ActionArguments["searchQuery"];
            Guid chatKey = searchQuery.ChatKey.Value;
            this.ConversationContext = this._contexts[chatKey];

            base.OnActionExecuting(actionExecutingContext);
        }
    }
}
