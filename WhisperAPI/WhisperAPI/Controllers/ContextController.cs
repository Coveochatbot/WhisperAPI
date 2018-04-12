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

        public ContextController(Contexts contexts)
        {
            this._contexts = contexts;
        }

        protected ConversationContext ConversationContext { get; set; }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            if (!this.ModelState.IsValid)
            {
                actionExecutingContext.Result = this.BadRequest(this.ModelState);
                return;
            }

            var searchQuerry = (SearchQuerry)actionExecutingContext.ActionArguments["searchQuerry"];
            Guid chatKey = searchQuerry.ChatKey.Value;
            this.ConversationContext = this._contexts[chatKey];

            base.OnActionExecuting(actionExecutingContext);
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            // Save changes that could have been made in controllers
            this._contexts.SaveChangesAsync();

            // Remove context older than the default timespan
            this._contexts.RemoveOldContext();

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
