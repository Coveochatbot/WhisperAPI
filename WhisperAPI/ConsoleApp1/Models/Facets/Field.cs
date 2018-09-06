using Newtonsoft.Json;

namespace ConsoleApp1.Models.Facets
{
    public class Field
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nativeName")]
        public string NativeName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("fieldType")]
        public string FieldType { get; set; }

        [JsonProperty("filedSourceType")]
        public string FiledSourceType { get; set; }

        [JsonProperty("includeInQuery")]
        public bool IncludeInQuery { get; set; }

        [JsonProperty("includeInResults")]
        public bool IncludeInResults { get; set; }

        [JsonProperty("groupByField")]
        public bool GroupByField { get; set; }

        [JsonProperty("splitGroupByField")]
        public bool SplitGroupByField { get; set; }

        [JsonProperty("sortByField")]
        public bool SortByField { get; set; }
    }
}
