using Api.MockServices.Abstract;
using Api.Models;

namespace Api.MockServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Provider> _providers = new List<Provider>
        {
            new Provider{ 
                Id = Guid.Parse("5D494E5F-C0D7-414E-86CB-920FE84D27BC"), Email = "doogie@rmh.com.au", Password = "Password", FirstName = "Doogie", LastName = "Howser"
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
        public Provider? FindProvider(string email)
        {
            var provider = _providers
                .FirstOrDefault(p => p.Email == email);

            if(provider == null)
            {
                _logger.LogInformation($"Provider not found with email: {email}.");
            }

            return provider;
        }
    }
}
