using System.Collections.Generic;
using WhisperAPI.Models.MLAPI;

namespace WhisperAPI.Tests.Data.Builders
{
    public class FilterDocumentsParametersBuilder
    {
        private List<string> _documents;

        private List<Facet> _mustHaveFacets;

        private List<Facet> _mustNotHaveFacets;

        public static FilterDocumentsParametersBuilder Build => new FilterDocumentsParametersBuilder();

        public FilterDocumentsParameters Instance => new FilterDocumentsParameters
        {
            Documents = this._documents,
            MustHaveFacets = this._mustHaveFacets,
            MustNotHaveFacets = this._mustNotHaveFacets
        };

        private FilterDocumentsParametersBuilder()
        {
            this._documents = new List<string>();
            this._mustHaveFacets = new List<Facet>();
            this._mustNotHaveFacets = new List<Facet>();
        }

        public FilterDocumentsParametersBuilder WithDocuments(List<string> documents)
        {
            this._documents = documents;
            return this;
        }

        public FilterDocumentsParametersBuilder WithMustHaveFacets(List<Facet> mustHaveFacets)
        {
            this._mustHaveFacets = mustHaveFacets;
            return this;
        }

        public FilterDocumentsParametersBuilder WithMustNotHaveFacets(List<Facet> mustNotHaveFacets)
        {
            this._mustNotHaveFacets = mustNotHaveFacets;
            return this;
        }

        public FilterDocumentsParametersBuilder AddDocument(string document)
        {
            this._documents.Add(document);
            return this;
        }

        public FilterDocumentsParametersBuilder AddMustHaveFacet(Facet mustHaveFacet)
        {
            this._mustHaveFacets.Add(mustHaveFacet);
            return this;
        }

        public FilterDocumentsParametersBuilder AddMustNotHaveFacet(Facet mustNotHaveFacet)
        {
            this._mustNotHaveFacets.Add(mustNotHaveFacet);
            return this;
        }
    }
}
