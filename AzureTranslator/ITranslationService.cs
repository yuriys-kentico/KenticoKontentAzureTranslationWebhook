using System.Threading.Tasks;

using AzureTranslator.Models;

namespace AzureTranslator
{
    public interface ITranslationService
    {
        Task<TranslationResult> Translate(string text, string language);
    }
}