using Hl7.Fhir.Model;

namespace Api.Models
{
    public class PaginatedFhirResponse<T>
        where T : class, IModifierExtendable
    {
        /// <summary>
        /// Total record count for stored Resource.
        /// </summary>
        public int? TotalRecords { get; set; }

        /// <summary>
        /// Date request was executed. 
        /// </summary>
        public DateTimeOffset? LastUpdated { get; set; }

        /// <summary>
        /// Records<Resource>
        /// </summary>
        public List<T?>? Records { get; set; } = new List<T?>();
    }
}
