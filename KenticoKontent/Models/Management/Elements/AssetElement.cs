using System.Collections.Generic;

namespace KenticoKontent.Models.Management.Elements
{
    public class AssetElement : AbstractElement
    {
        public IEnumerable<Reference>? Value { get; set; }
    }
}