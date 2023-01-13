namespace Api.Models
{
    public class CurrentUserCredential
    {
        /// <summary>
        /// Current user's unique identifier.
        /// </summary>
        public string UserId { get; set; } = default!;

        /// <summary>
        /// Healthcare provider the current user is associated with.
        /// </summary>
        public string HealthCareProviderId { get; set; } = default!;

        /// <summary>
        /// The healthcare providers tenant identifier the current user is assigned to.
        /// </summary>
        public string TenantIdentifier { get; set; } = default!;
    }
}
