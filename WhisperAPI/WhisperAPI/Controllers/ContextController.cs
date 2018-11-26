using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;
using WhisperAPI.Services.Context;

namespace WhisperAPI.Controllers
{
    public class ContextController : Controller
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IContexts _contexts;

        public ContextController(IContexts contexts)
        {
            this._contexts = contexts;
        }

        public ConversationContext ConversationContext { get; set; }

        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            Query query = actionExecutingContext.ActionArguments.Values.OfType<Query>().FirstOrDefault();

            log4net.ThreadContext.Properties["requestId"] = Guid.NewGuid();
            if (!this.ModelState.IsValid || query == null)
            {
                actionExecutingContext.Result = this.BadRequest(this.ModelState);
                Log.Error($"Search query:\r\n{JsonConvert.SerializeObject(query, Formatting.Indented)}");
                return;
            }

            Log.Debug($"Search query:\r\n {JsonConvert.SerializeObject(query, Formatting.Indented)}");
            Guid chatKey = query.ChatKey.Value;
            this.ConversationContext = this._contexts[chatKey];
            base.OnActionExecuting(actionExecutingContext);
        }

        protected void ReplaceConversationContext(ConversationContext conversationContext)
        {
            this.ConversationContext = this._contexts[conversationContext.ChatKey] = conversationContext;
        }
    }
}
