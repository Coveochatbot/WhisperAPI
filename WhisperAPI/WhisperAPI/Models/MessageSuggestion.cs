using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhisperAPI.Models
{
    /// <summary>
    ///  Wrapper arround message string because entity framework doesn't allow primitive type in collection
    /// </summary>
    public class MessageSuggestion
    {
        public MessageSuggestion(string message)
        {
            this.Message = message;
        }

        public MessageSuggestion()
        {
        }

        [Key]
        public int Id { get; set; }

        public string Message { get; set; }
    }
}