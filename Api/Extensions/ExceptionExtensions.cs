using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Authentication;

namespace Api.Extensions
{
    public static class ExceptionExtensions
    {
        public static ProblemDetails? ToProblemDetails(this Exception ex)
        {
            if (ex == null) return default;

            return new ProblemDetails()
            {
                Status = (int)GetErrorCode(ex.InnerException ?? ex),
                Title = ex.Message
            };
        }

        private static HttpStatusCode GetErrorCode(Exception ex)
        {
            switch (ex)
            {
                case ValidationException _:
                    return HttpStatusCode.BadRequest;

                case FormatException _:
                    return HttpStatusCode.BadRequest;

                case AuthenticationException _:
                    return HttpStatusCode.Forbidden;

                case NotImplementedException _:
                    return HttpStatusCode.NotImplemented;

                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
    }
}
