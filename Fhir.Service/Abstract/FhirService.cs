using Microsoft.Extensions.Logging;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace Fhir.Service.Abstract
{
    public abstract class FhirService<T, R> : IFhirService<T, R>
        where T : class, IModifierExtendable
        where R : Resource
    {
        private FhirHttpClient _hirHttpClient;
        private readonly FhirClient _fhirClient;

        protected readonly ILogger<FhirService<T, R>> Logger;

        protected FhirService(
            FhirHttpClient fhirHttpClient,
            ILogger<FhirService<T, R>> logger)
        {
            _hirHttpClient = fhirHttpClient;
            Logger = logger;

            _fhirClient = new FhirClient(fhirHttpClient.Client.BaseAddress, fhirHttpClient.Client, null, null);
        }

        public async virtual Task<string> GetContentAsync(string uri)
        {
            using (var httpResponseMessage = await _hirHttpClient.Client.GetAsync(uri))
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    Logger.LogError($"Request: {uri} failed. Http status code: {httpResponseMessage.StatusCode}.");
                    throw new Exception(httpResponseMessage.StatusCode.ToString());
                }

                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Returns paginated results and deserializes to T that conforms to IModifierExtendable.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async virtual Task<(List<T?>? results, int? totalRecords, DateTimeOffset? lastUpdated)> GetPaginatedRequestAsync(string uri)
        {
            var content = await GetContentAsync(uri);
            var fhirJsonParser = new FhirJsonParser();
            var bundle = fhirJsonParser.Parse<Bundle>(content);
            var meta = bundle.Meta;
            var lastUpdated = meta.LastUpdated;
            var totalRecords = bundle.Total;
            var results = bundle?.Entry
                    .Select(p => p.Resource as T)
                    .ToList();

            return (results, totalRecords, lastUpdated);
        }

        /// <summary>
        /// Returns single resource result and deserializes to R that conforms to Resource.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async virtual Task<R?> GetResourceAsync(string uri)
        {
            var resource = await _fhirClient.ReadAsync<R>(uri);

            return resource;
        }
    }
}
