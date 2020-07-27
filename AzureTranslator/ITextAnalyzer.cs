using System.Collections.Generic;

namespace AzureTranslator
{
    public interface ITextAnalyzer
    {
        IEnumerable<string> SplitHtml(string input);
    }
}