using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsoleApp1.Models.Search
{
    public class RawInformation
    {
        [JsonProperty("rowid")]
        public int RowId { get; set; }

        [JsonProperty("systitle")]
        public string systitle { get; set; }

        [JsonProperty("cfspacekey")]
        public string cfspacekey { get; set; }

        [JsonProperty("sysauthor")]
        public string sysauthor { get; set; }

        [JsonProperty("sysurihash")]
        public string sysurihash { get; set; }

        [JsonProperty("urihash")]
        public string urihash { get; set; }

        [JsonProperty("parents")]
        public string parents { get; set; }

        [JsonProperty("sysuri")]
        public string sysuri { get; set; }

        [JsonProperty("systransactionid")]
        public int systransactionid { get; set; }

        [JsonProperty("sysconcepts")]
        public string sysconcepts { get; set; }

        [JsonProperty("concepts")]
        public string concepts { get; set; }

        [JsonProperty("sysindexeddate")]
        public long sysindexeddate { get; set; }

        [JsonProperty("permanentid")]
        public string permanentid { get; set; }

        [JsonProperty("syslanguage")]
        public List<string> syslanguage { get; set; }

        [JsonProperty("transactionid")]
        public int transactionid { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("date")]
        public long date { get; set; }

        [JsonProperty("objecttype")]
        public string objecttype { get; set; }

        [JsonProperty("audience")]
        public List<string> audience { get; set; }

        [JsonProperty("sysconnectortype")]
        public string sysconnectortype { get; set; }

        [JsonProperty("cfspacename")]
        public string cfspacename { get; set; }

        [JsonProperty("size")]
        public int size { get; set; }

        [JsonProperty("clickableuri")]
        public string clickableuri { get; set; }

        [JsonProperty("syssource")]
        public string syssource { get; set; }

        [JsonProperty("syssize")]
        public int syssize { get; set; }

        [JsonProperty("sysdate")]
        public long sysdate { get; set; }

        [JsonProperty("sysparents")]
        public string sysparents { get; set; }

        [JsonProperty("syscfspacename")]
        public string syscfspacename { get; set; }

        [JsonProperty("author")]
        public string author { get; set; }

        [JsonProperty("source")]
        public string source { get; set; }

        [JsonProperty("cfspacetype")]
        public string cfspacetype { get; set; }

        [JsonProperty("collection")]
        public string collection { get; set; }

        [JsonProperty("indexeddate")]
        public long indexeddate { get; set; }

        [JsonProperty("connectortype")]
        public string connectortype { get; set; }

        [JsonProperty("filetype")]
        public string filetype { get; set; }

        [JsonProperty("sysclickableuri")]
        public string sysclickableuri { get; set; }

        [JsonProperty("sysfiletype")]
        public string sysfiletype { get; set; }

        [JsonProperty("language")]
        public List<string> language { get; set; }

        [JsonProperty("sitename")]
        public string sitename { get; set; }

        [JsonProperty("sysrowid")]
        public int sysrowid { get; set; }

        [JsonProperty("uri")]
        public string uri { get; set; }

        [JsonProperty("syscollection")]
        public string syscollection { get; set; }

        [JsonProperty("sitelanguage")]
        public List<string> sitelanguage { get; set; }
    }
}
