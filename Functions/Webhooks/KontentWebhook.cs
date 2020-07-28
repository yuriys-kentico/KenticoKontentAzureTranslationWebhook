using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AzureTranslator;

using Core;

using KenticoKontent;
using KenticoKontent.Models;
using KenticoKontent.Models.Management.Elements;
using KenticoKontent.Models.Webhook;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions.Webhooks
{
    public class KontentWebhook : BaseFunction
    {
        private readonly IWebhookValidator webhookValidator;
        private readonly IKontentRepository kontentRepository;
        private readonly ITranslationService translationService;
        private readonly ITextAnalyzer textAnalyzer;

        public KontentWebhook(
            ILogger<KontentWebhook> logger,
            IWebhookValidator webhookValidator,
            IKontentRepository kontentRepository,
            ITranslationService translationService,
            ITextAnalyzer textAnalyzer
            ) : base(logger)
        {
            this.webhookValidator = webhookValidator;
            this.kontentRepository = kontentRepository;
            this.translationService = translationService;
            this.textAnalyzer = textAnalyzer;
        }

        [FunctionName(nameof(KontentWebhook))]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                "post",
                Route = Routes.KontentWebhook
            )] string body,
            IDictionary<string, string> headers,
            string language
            )
        {
            try
            {
                var (valid, getWebhook) = webhookValidator.ValidateWebhook(body, headers);

                if (!valid) return LogUnauthorized();

                var (data, message) = getWebhook();

                if (data.Items == null)
                {
                    throw new ArgumentNullException(nameof(data.Items));
                }

                switch (message.Type)
                {
                    case "content_item_variant":
                        switch (message.Operation)
                        {
                            case "change_workflow_step":
                                var translationLanguage = language.Substring(0, 2);

                                foreach (var item in data.Items)
                                {
                                    await TranslateItem(item, language, translationLanguage);
                                }
                                break;
                        }
                        break;
                }

                return LogOk();
            }
            catch (ArgumentNullException ex)
            {
                return LogOkException(ex);
            }
            catch (ApiException ex)
            {
                return LogOkException(ex);
            }
            catch (Exception ex)
            {
                return LogException(ex);
            }
        }

        private async Task TranslateItem(ItemObject item, string language, string translationLanguage)
        {
            if (item.Item == null)
            {
                throw new ArgumentNullException(nameof(item.Item));
            }

            var languageVariant = await kontentRepository.RetrieveLanguageVariant(new RetrieveLanguageVariantParameters
            {
                ItemId = item.Item.Id,
                LanguageId = item.Language?.Id
            });

            foreach (var element in languageVariant.Elements)
            {
                switch (element)
                {
                    case RichTextElement richTextElement:
                        var value = richTextElement.Value;

                        if (value?.Length >= 5000)
                        {
                            var parts = textAnalyzer.SplitHtml(value);
                            var result = "";

                            foreach (var part in parts)
                            {
                                var (partTranslated, partTranslation) = await Translate(part, translationLanguage);

                                if (partTranslated)
                                {
                                    result += partTranslation;
                                };
                            }

                            if (!string.IsNullOrWhiteSpace(result))
                            {
                                richTextElement.Value = result;
                            }

                            break;
                        }

                        var (translated, translation) = await Translate(richTextElement.Value, translationLanguage);

                        if (translated)
                        {
                            richTextElement.Value = translation;
                        };
                        break;

                    case UrlSlugElement urlSlugElement:
                        (translated, translation) = await Translate(urlSlugElement.Value, translationLanguage);

                        if (translated)
                        {
                            urlSlugElement.Value = translation.Replace(" ", "-");
                        };
                        break;

                    case TextElement textElement:
                        (translated, translation) = await Translate(textElement.Value, translationLanguage);

                        if (translated)
                        {
                            textElement.Value = translation;
                        };
                        break;
                }
            }

            await kontentRepository.UpsertLanguageVariant(new UpsertLanguageVariantParameters
            {
                ItemId = item.Item.Id,
                Language = language,
                Variant = languageVariant
            });
        }

        private async Task<(bool, string)> Translate(string? original, string translationLanguage)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                return (false, string.Empty);
            }

            var translation = (await translationService.Translate(original, translationLanguage)).Translations.First().Text;

            if (string.IsNullOrWhiteSpace(translation))
            {
                return (false, string.Empty);
            }

            return (true, translation);
        }
    }
}