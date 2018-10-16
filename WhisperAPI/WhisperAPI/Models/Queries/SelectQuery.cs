using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WhisperAPI.Models.Queries
{
    public class SelectQuery : Query
    {
        [Required(ErrorMessage = "Id is required")]
        [JsonProperty("id")]
        public Guid? Id { get; set; }
    }
}