using System.Collections.Generic;

namespace ConsoleApp1.Models.File
{
    public class DataToWrite
    {
        public string FacetName { get; set; }

        public string FacetValue { get; set; }

        public List<Document> Documents { get; set; }
    }
}
