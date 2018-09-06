using Newtonsoft.Json;

namespace ConsoleApp1.Models.Facets_value
{
    public class FieldValue
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("lookupValue")]
        public string LookupValue { get; set; }

        [JsonProperty("numberOfResults")]
        public string NumberOfResults { get; set; }
    }
}
