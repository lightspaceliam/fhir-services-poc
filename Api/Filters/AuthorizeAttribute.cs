using Microsoft.AspNetCore.Mvc;

namespace Api.Filters
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(string[] roles) : base(typeof(AuthorizeRolesFilter))
        {
            Arguments = new object[] { roles };
        }
    }
}
