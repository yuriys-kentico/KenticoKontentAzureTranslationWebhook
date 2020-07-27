using System;

namespace Core
{
    public class Settings
    {
        public Guid ProjectId { get; set; }

        public string WebhookSecret { get; set; } = string.Empty;

        public string ManagementApiKey { get; set; } = string.Empty;

        public string TranslationApiKey { get; set; } = string.Empty;

        public string TranslationApiRegion { get; set; } = string.Empty;
    }
}