using Api.Constants;
using Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Api.Filters
{
    public class AuthorizeRolesFilter : AuthorizeFilter
    {
        private readonly string[] _roles;

        public AuthorizeRolesFilter(string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.GetClaims().UserHasRole(_roles) == false)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
