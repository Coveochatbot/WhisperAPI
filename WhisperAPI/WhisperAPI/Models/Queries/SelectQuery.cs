using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Models
{
    public class SelectQuery : Query
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Id is required")]
        [JsonProperty("id")]
        public Guid? Id { get; set; }
    }
}