using Hl7.Fhir.Rest;

namespace Fhir.Service.Abstract
{
    public interface IFhirResourceService<T>
    {
        Task<(string resourceId, string content)> CreateResourceAsync(T resource, string tenantId);

        /* Read     */

        Task<string> GetPaginatedResourceAsync(string uri, string tenantId);
        Task<string?> GetResourceByIdAsync(string id, string tenantId);
        Task<string> FindResourceAsync(SearchParams searchParams, string tenantId);

        /* Update   */

        Task<string> UpdateResourceAsync(T resource, string tenantId);

        /* Delete   */

        Task DeleteResourceByIdAsync(string id, string tenantId);
    }
}
