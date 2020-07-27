using System.Collections.Generic;

using KenticoKontent.Models.Management.Elements;

namespace KenticoKontent.Models.Management
{
    public class LanguageVariant
    {
        public IList<AbstractElement> Elements { get; set; } = new List<AbstractElement>();
    }
}