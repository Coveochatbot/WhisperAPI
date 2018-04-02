using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Controllers
{
    public class ContextController : Controller
    {
        private Contexts _contexts;
        private TimeSpan _contextLifeSpan;
        private ConversationContext _conversationContext;

        public ContextController(Contexts contexts)
        {
            this._contexts = contexts;
            this._contextLifeSpan = new TimeSpan(1, 0, 0, 0);
        }

        public ContextController(Contexts contexts, TimeSpan contextLifeSpan)
        {
            this._contexts = contexts;
            this._contextLifeSpan = contextLifeSpan;
        }

        public TimeSpan ContextLifeSpan { get => this._contextLifeSpan; set => this._contextLifeSpan = value; }

        protected ConversationContext ConversationContext { get => this._conversationContext; set => this._conversationContext = value; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string chatKey = ((SearchQuerry)context.ActionArguments["searchQuerry"]).ChatKey;
            this._conversationContext = this._contexts[chatKey];

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Save changes that could have been made in controllers
            this._contexts.SaveChangesAsync();

            // Remove context older than the default timespan
            this._contexts.RemoveContextOlderThan(this._contextLifeSpan);

            base.OnActionExecuted(context);
        }
    }
}
