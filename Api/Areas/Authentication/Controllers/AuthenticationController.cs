using Api.Areas.Authentication.Extensions;
using Api.Areas.Authentication.Models;
using Api.MockServices;
using Api.MockServices.Abstract;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Areas.Authentication.Controllers
{
    [Authorize]
    [Route("api/authentication")]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _service;
        private readonly IConfiguration _configuration;

        public AuthenticationController(
            IAuthenticationService service,
            IConfiguration configuration,
            ILogger<BaseController> logger)
            : base(logger)
        {
            _service = service;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            var provider = ((AuthenticationService)_service).FindProvider(request.Username);

            if (provider == null)
            {
                Logger.LogWarning($"{request.Username} could not be found.");
                return Unauthorized();
            }

            //  TODO: Add further stringent validation.
            if (provider.Password != request.Password)
            {
                Logger.LogWarning($"{request.Username} failed authentication.");
                return Unauthorized();
            }

            var user = new UserDto
            {
                Id = provider.Id,
                Name = $"{provider.FirstName} {provider.LastName}"
            };
            var token = user.CreateJwtToken(_configuration["Jwt:Secret"], _configuration["Jwt:Audience"], _configuration["Jwt:Authority"]);

            return Ok(new { Token = token });
        }
    }
}
