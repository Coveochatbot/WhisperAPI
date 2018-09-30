using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhisperAPI.Models
{
    public class FacetQuestion : Question
    {
        public string FacetName { get; set; }

        public List<string> FacetValues { get; set; }

        public string Answer { get; set; }

        public override string Text
        {
            get
            {
                return $"What {this.FacetName} is it? Is it {string.Join(", ", this.FacetValues)} ?";
            }
        }
    }
}
