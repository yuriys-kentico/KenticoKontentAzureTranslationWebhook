using System.Collections.Generic;

namespace KenticoKontent.Models.Management.Elements
{
    public class TaxonomyElement : AbstractElement
    {
        public IEnumerable<Reference>? Value { get; set; }
    }
}