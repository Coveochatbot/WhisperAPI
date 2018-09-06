using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsoleApp1.Models.Facets
{
    public class Fields
    {
        [JsonProperty("fields")]
        public IEnumerable<Field> List { get; set; }
    }
}
