using System;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public interface IContexts
    {
        ConversationContext this[Guid chatkey] { get; }
    }
}
