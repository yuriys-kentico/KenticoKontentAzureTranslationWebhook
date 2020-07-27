using System;

namespace KenticoKontent.Models
{
    public class RetrieveLanguageVariantParameters
    {
        public Guid? ItemId { get; set; }

        public Guid? LanguageId { get; set; }

        public void Deconstruct(
            out Guid itemId,
            out Guid languageId
            )
        {
            itemId = ItemId ?? throw new ArgumentNullException(nameof(ItemId));
            languageId = LanguageId ?? throw new ArgumentNullException(nameof(LanguageId));
        }
    }
}