using Microsoft.Extensions.Logging;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace Fhir.Service.Abstract
{
    public abstract class FhirResourceService<T> : IFhirResourceService<T>
        where T : Resource
    {
        protected readonly IHttpClientFactory HttpClientFactory;
        
        protected readonly ILogger<FhirResourceService<T>> Logger;
        private readonly string FhirAuth;
        private const string FHIR_NAMED_CLIENT = "FhirService";
        protected readonly string SubUri = string.Empty;

        public FhirResourceService(
            IHttpClientFactory httpClientFactory,
            ILogger<FhirResourceService<T>> logger,
            IConfiguration configuration)
        {
            HttpClientFactory = httpClientFactory;
            Logger = logger;

            FhirAuth = configuration["Fhir:BasicSecret"];
            SubUri = $"{configuration["FhirSettings:SubDirectory"]}";
        }

        protected string UriBuilder(string tenantId)
        {
            return $"{SubUri}{tenantId}/";
        }

        private FhirOperationException GetFhirOperationException(string content, HttpStatusCode httpStatusCode)
        {
            var json = SerializationUtil.JsonReaderFromJsonText(content);
            var fhirJsonParser = new FhirJsonParser();
            var outcome = fhirJsonParser.Parse<OperationOutcome>(json);

            var diagnosticsMessage = outcome.Issue.FirstOrDefault()?.Diagnostics;
            return new FhirOperationException(diagnosticsMessage, httpStatusCode, outcome);
        }

        protected async Task<string> GetResourceAsync(string requestUri, string tenantId)
        {
            var httpClient = HttpClientFactory.CreateClient(FHIR_NAMED_CLIENT);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", FhirAuth);

            using (var httpResponseMessage = await httpClient.GetAsync($"{UriBuilder(tenantId)}{requestUri}"))
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound
                        || httpResponseMessage.StatusCode == HttpStatusCode.Gone)
                    {
                        return "";
                    }

                    var fhirOperationException = GetFhirOperationException(content, httpResponseMessage.StatusCode);
                    throw fhirOperationException;
                }

                return content;
            }
        }

        protected async Task<string> PostResourceAsync(string requestUri, StringContent stringContent, string tenantId)
        {
            var httpClient = HttpClientFactory.CreateClient(FHIR_NAMED_CLIENT);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", FhirAuth);

            using (var httpResponseMessage = await httpClient.PostAsync($"{UriBuilder(tenantId)}{requestUri}", stringContent))
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var fhirOperationException = GetFhirOperationException(content, httpResponseMessage.StatusCode);
                    throw fhirOperationException;
                }

                return content;
            }
        }

        protected async Task<string> PutResourceAsync(string requestUri, StringContent stringContent, string tenantId)
        {
            var httpClient = HttpClientFactory.CreateClient(FHIR_NAMED_CLIENT);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", FhirAuth);

            using (var httpResponseMessage = await httpClient.PutAsync($"{UriBuilder(tenantId)}{requestUri}", stringContent))
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var fhirOperationException = GetFhirOperationException(content, httpResponseMessage.StatusCode);
                    throw fhirOperationException;
                }

                return content;
            }
        }

        /// <summary>
        /// Fhir returns OK even if the resource is not found.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        protected async Task DeleteResourceAsync(string requestUri, string tenantId)
        {
            var httpClient = HttpClientFactory.CreateClient(FHIR_NAMED_CLIENT);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", FhirAuth);

            using (var httpResponseMessage = await httpClient.DeleteAsync($"{UriBuilder(tenantId)}{requestUri}"))
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var fhirOperationException = GetFhirOperationException(content, httpResponseMessage.StatusCode);
                    throw fhirOperationException;
                }
            }
        }

        /* Create   */

        public virtual async Task<(string resourceId, string content)> CreateResourceAsync(T resource, string tenantId)
        {
            try
            {
                var requestUrl = $"{typeof(T).Name}";
                var serializer = new FhirJsonSerializer();
                var payload = new StringContent(serializer.SerializeToString(resource), Encoding.UTF8, "application/json");
                var content = await PostResourceAsync(requestUrl, payload, tenantId);

                var fhirJsonParser = new FhirJsonParser(new ParserSettings
                {
                    AllowUnrecognizedEnums = true,
                    PermissiveParsing = true,
                    AcceptUnknownMembers = true
                });

                var creatResource = await fhirJsonParser.ParseAsync<T>(content);

                return (creatResource.Id, content);
            }
            catch (FhirOperationException ex)
            {
                Logger.LogError($"HttpStatus: {ex.Status}", ex);
                throw new FhirOperationException(ex.Message, ex.Status, ex.Outcome);
            }
        }

        /* Read     */

        public virtual async Task<string> GetPaginatedResourceAsync(string uri, string tenantId)
        {
            try
            {
                var content = await GetResourceAsync($"{typeof(T).Name}{uri}", tenantId);

                return content;
            }
            catch (FhirOperationException ex)
            {
                Logger.LogError($"HttpStatus: {ex.Status}", ex);
                throw new FhirOperationException(ex.Message, ex.Status, ex.Outcome);
            }
        }

        public virtual async Task<string?> GetResourceByIdAsync(string id, string tenantId)
        {
            try
            {
                var requestUrl = $"{typeof(T).Name}/{id}";
                var content = await GetResourceAsync(requestUrl, tenantId);

                return content;
            }
            catch (FhirOperationException ex)
            {
                Logger.LogError($"HttpStatus: {ex.Status}", ex);
                throw new FhirOperationException(ex.Message, ex.Status, ex.Outcome);
            }
        }

        public virtual async Task<string> FindResourceAsync(SearchParams searchParams, string tenantId)
        {
            var httpClient = HttpClientFactory.CreateClient(FHIR_NAMED_CLIENT);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", FhirAuth);
            var fhirClient = new FhirClient($"{httpClient.BaseAddress}", httpClient);

            try
            {
                var bundle = await fhirClient.SearchAsync<T>(searchParams);

                //  TODO: .ForFhir may need to be a Bundle. Test to confirm.
                var options = new JsonSerializerOptions().ForFhir(typeof(T).Assembly).Pretty();
                var json = JsonSerializer.Serialize(bundle, options);

                return json;
            }
            catch (FhirOperationException ex)
            {
                Logger.LogError($"HttpStatus: {ex.Status}", ex);
                throw new FhirOperationException(ex.Message, ex.Status, ex.Outcome);
            }
        }

        /* Update   */

        public virtual async Task<string> UpdateResourceAsync(T resource, string tenantId)
        {
            try
            {
                var requestUrl = $"{typeof(T).Name}/{resource.Id}";
                var serializer = new FhirJsonSerializer();
                var payload = new StringContent(serializer.SerializeToString(resource), Encoding.UTF8, "application/json");

                var content = await PutResourceAsync(requestUrl, payload, tenantId);

                return content;
            }
            catch (FhirOperationException ex)
            {
                Logger.LogError($"HttpStatus: {ex.Status}", ex);
                throw new FhirOperationException(ex.Message, ex.Status, ex.Outcome);
            }
        }

        /* Delete   */

        public virtual async Task DeleteResourceByIdAsync(string id, string tenantId)
        {
            try
            {
                var requestUrl = $"{typeof(T).Name}/{id}";
                await DeleteResourceAsync(requestUrl, tenantId);
            }
            catch (FhirOperationException ex)
            {
                Logger.LogError($"HttpStatus: {ex.Status}", ex);
                throw new FhirOperationException(ex.Message, ex.Status, ex.Outcome);
            }
        }
    }
}
