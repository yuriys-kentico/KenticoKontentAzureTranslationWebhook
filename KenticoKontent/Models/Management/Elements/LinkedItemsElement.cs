using System.Collections.Generic;

namespace KenticoKontent.Models.Management.Elements
{
    public class LinkedItemsElement : AbstractElement
    {
        public IEnumerable<Reference>? Value { get; set; }
    }
}