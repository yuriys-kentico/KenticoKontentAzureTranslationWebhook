using System;

using KenticoKontent.Models.Management;

namespace KenticoKontent.Models
{
    public class UpsertLanguageVariantParameters
    {
        public Guid ItemId { get; set; }

        public string? Language { get; set; }

        public LanguageVariant? Variant { get; set; }

        public void Deconstruct(
            out Guid itemId,
            out string language,
            out LanguageVariant variant
            )
        {
            itemId = ItemId;
            language = Language ?? throw new ArgumentNullException(nameof(Language));
            variant = Variant ?? throw new ArgumentNullException(nameof(Variant));
        }
    }
}