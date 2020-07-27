using System.Threading.Tasks;

using KenticoKontent.Models;
using KenticoKontent.Models.Management;

namespace KenticoKontent
{
    public interface IKontentRepository
    {
        Task<LanguageVariant> RetrieveLanguageVariant(RetrieveLanguageVariantParameters retrieveLanguageVariantParameters);

        Task UpsertLanguageVariant(UpsertLanguageVariantParameters upsertLanguageVariantParameters);
    }
}