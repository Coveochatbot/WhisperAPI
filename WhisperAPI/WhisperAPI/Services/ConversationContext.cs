using Microsoft.EntityFrameworkCore;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public class ConversationContext : DbContext
    {
        public ConversationContext(DbContextOptions<ConversationContext> options)
            : base(options)
        {
        }

        public DbSet<Conversation> ConversationMessages { get; set; }
    }
}
