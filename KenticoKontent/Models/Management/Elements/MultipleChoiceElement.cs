using System.Collections.Generic;

namespace KenticoKontent.Models.Management.Elements
{
    public class MultipleChoiceElement : AbstractElement
    {
        public IEnumerable<Reference>? Value { get; set; }
    }
}