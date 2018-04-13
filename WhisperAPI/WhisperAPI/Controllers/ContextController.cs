using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Controllers
{
    public class ContextController : Controller
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Contexts _contexts;

        public ContextController(Contexts contexts)
        {
            this._contexts = contexts;
        }

        protected ConversationContext ConversationContext { get; set; }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var searchQuerry = (SearchQuerry)actionExecutingContext.ActionArguments["searchQuerry"];

            log4net.ThreadContext.Properties["requestId"] = Guid.NewGuid();
            if (!this.ModelState.IsValid)
            {
                actionExecutingContext.Result = this.BadRequest(this.ModelState);
                Log.Error($"ChatKey: {searchQuerry.ChatKey}, Query: {searchQuerry.Querry}, Type: {searchQuerry.Type}");
                return;
            }

            Log.Debug($"ChatKey: {searchQuerry.ChatKey}, Query: {searchQuerry.Querry}, Type: {searchQuerry.Type}");

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
