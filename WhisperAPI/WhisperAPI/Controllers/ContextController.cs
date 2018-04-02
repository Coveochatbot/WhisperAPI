using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Controllers
{
    public class ContextController : Controller
    {
        private Contexts _contexts;
        private ConversationContext _conversationContext;

        public ContextController(Contexts contexts)
        {
            this._contexts = contexts;
        }

        protected ConversationContext ConversationContext { get => this._conversationContext; set => this._conversationContext = value; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string chatKey = ((SearchQuerry)context.ActionArguments["searchQuerry"]).ChatKey;
            this._conversationContext = this._contexts[chatKey];

            base.OnActionExecuting(context);
        }
    }
}
