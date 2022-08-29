namespace Fhir.Service.Abstract
{
    public interface IFhirService<T, R>
    {
        Task<string> GetContentAsync(string uri);
        Task<(List<T?>? results, int? totalRecords, DateTimeOffset? lastUpdated)> GetPaginatedRequestAsync(string uri);
        Task<R?> GetResourceAsync(string uri);
    }
}
