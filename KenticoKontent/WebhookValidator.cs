using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Core;

using KenticoKontent.Models.Webhook;

namespace KenticoKontent
{
    public class WebhookValidator : IWebhookValidator
    {
        private readonly Settings settings;

        private const string WebhookSignatureHeaderName = "X-KC-Signature";

        public WebhookValidator(Settings settings)
        {
            this.settings = settings;
        }

        public (bool valid, Func<Webhook> getWebhook) ValidateWebhook(string body, IDictionary<string, string> headers)
        {
            headers.TryGetValue(WebhookSignatureHeaderName, out var signatureFromRequest);

            var generatedSignature = GetHashForWebhook(body, settings.WebhookSecret);

            return (generatedSignature == signatureFromRequest, () => CoreHelper.Deserialize<Webhook>(body));
        }

        private static string GetHashForWebhook(string content, string secret)
        {
            var safeUTF8 = new UTF8Encoding(false, true);
            byte[] keyBytes = safeUTF8.GetBytes(secret);
            byte[] messageBytes = safeUTF8.GetBytes(content);

            using var hmacsha256 = new HMACSHA256(keyBytes);

            return Convert.ToBase64String(hmacsha256.ComputeHash(messageBytes));
        }
    }
}