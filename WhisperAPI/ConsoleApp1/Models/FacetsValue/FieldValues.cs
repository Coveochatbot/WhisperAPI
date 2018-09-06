using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsoleApp1.Models.Facets_value
{
    public class FieldValues
    {
        [JsonProperty("values")]
        public IEnumerable<FieldValue> List { get; set; }
    }
}
