using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Core;

using KenticoKontent.Models;
using KenticoKontent.Models.Management;

using Newtonsoft.Json.Serialization;

namespace KenticoKontent
{
    public class KontentRepository : IKontentRepository
    {
        private readonly HttpClient httpClient;
        private readonly Settings settings;

        public KontentRepository(
            HttpClient httpClient,
            Settings settings
            )
        {
            this.httpClient = httpClient;
            this.settings = settings;
        }

        private string ConfigureClient(string? endpoint = default)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", settings.ManagementApiKey);

            var projectId = settings.ProjectId;

            return $@"https://manage.kontent.ai/v2/projects/{projectId}/items{endpoint}";
        }

        public async Task<LanguageVariant> RetrieveLanguageVariant(RetrieveLanguageVariantParameters retrieveLanguageVariantParameters)
        {
            var (itemId, languageId) = retrieveLanguageVariantParameters;

            var requestUri = ConfigureClient($"/{itemId}/variants/{languageId}");

            var response = await httpClient.GetAsync(requestUri);

            await ThrowIfNotSuccessStatusCode(response);

            return await response.Content.ReadAsAsync<LanguageVariant>();
        }

        public async Task UpsertLanguageVariant(UpsertLanguageVariantParameters upsertLanguageVariantParameters)
        {
            var (itemId, language, variant) = upsertLanguageVariantParameters;

            var requestUri = ConfigureClient($"/{itemId}/variants/codename/{language}");

            var response = await PutAsJsonAsync(requestUri, variant);

            await ThrowIfNotSuccessStatusCode(response);
        }

        private async Task<HttpResponseMessage> PutAsJsonAsync(string requestUri, object? value = default)
        {
            var response = await httpClient.PutAsync(requestUri, value, new JsonMediaTypeFormatter()
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });

            await ThrowIfNotSuccessStatusCode(response);

            return response;
        }

        private static async Task ThrowIfNotSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsAsync<APIErrorResponse>();
                throw errorContent.GetException();
            }
        }
    }
}