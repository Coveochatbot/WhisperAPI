using System.Collections.Generic;
using System.Linq;
using ConsoleApp1.Models.Facets;
using ConsoleApp1.Models.Facets_value;
using ConsoleApp1.Models.File;
using ConsoleApp1.Models.Search;
using ConsoleApp1.Services;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var getFields = new GetFields();
            var getFieldValue = new GetFieldValues();
            var indexSearchWithGroupBy = new IndexSearchSortByField();

            var fields = getFields.Get().List.Where(x => x.GroupByField).OrderBy(x => x.Name);

            var dictionary = new Dictionary<Field, List<FieldValue>>();
            foreach (var field in fields)
            {
                var values = getFieldValue.Get(field.Name);
                dictionary.Add(field, values.List.ToList());
            }

            foreach (var fieldAndValue in dictionary)
            {
                foreach (var fieldValue in fieldAndValue.Value)
                {
                    if (fieldValue.Value.Contains(","))
                    {
                        continue;
                    }

                    var dataToWrite = new DataToWrite
                    {
                        FacetName = fieldAndValue.Key.Name,
                        FacetValue = fieldValue.Value,
                        Documents = new List<Document>()
                    };

                    var rowId = 0;
                    ISearchResult result;
                    do
                    {
                        result = indexSearchWithGroupBy.Search(fieldAndValue.Key.Name, fieldValue.Value, rowId);

                        if (result.NbrElements != 0)
                        {
                            rowId = result.Elements.OrderBy(x => x.rawInformation.RowId).Last().rawInformation.RowId;
                            dataToWrite.Documents.AddRange(result.Elements.Select(x => new Document
                            {
                                Summary = x.summary,
                                Title = x.title,
                                Uri = x.uri,
                                PrintableUri = x.printableUri,
                                Excerpt = x.excerpt,
                                ClickUri = x.clickUri,
                                UniqueId = x.uniqueId
                            }));
                        }
                        else
                        {
                            fieldValue.Value = fieldValue.Value.Replace(": ", " ").Replace("/", "_").Replace("?", string.Empty);

                            if (fieldValue.Value.Length > 100)
                            {
                                fieldValue.Value = fieldValue.Value.Substring(0, 100);
                            }

                            var path = $@"D:\Projet\Facettes\{fieldAndValue.Key.Name}_{fieldValue.Value}.json";
                            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(dataToWrite, Formatting.Indented));
                            rowId = 0;
                        }
                    } while (result.NbrElements != 0);
                }
            }
        }
    }
}
