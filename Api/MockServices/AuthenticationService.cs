using Api.MockServices.Abstract;
using Api.Models;

namespace Api.MockServices
{
    public class AuthenticationService : IAuthenticationService
    {
        //  Mock user collection.
        private readonly List<HealthcareProvider> _healthcareProviders = new List<HealthcareProvider>
        {
            new HealthcareProvider
            { 
                Id = Guid.Parse("5D494E5F-C0D7-414E-86CB-920FE84D27BC"), 
                Email = "doogie@rmh.com.au", 
                Password = "Password", 
                FirstName = "Doogie", 
                LastName = "Howser"
            }
        };

        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(ILogger<AuthenticationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// MOCK...
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public HealthcareProvider? FindProvider(string email)
        {
            var provider = _healthcareProviders
                .FirstOrDefault(p => p.Email == email);

            if(provider == null)
            {
                _logger.LogInformation($"Healcare Provider not found with email: {email}.");
            }

            return provider;
        }
    }
}
