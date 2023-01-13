using Api.Extensions;
using Fhir.Service.Abstract;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Api.Abstract
{
    public abstract class ResourceController<T> : BaseController
        where T : Resource
    {
        protected readonly IFhirResourceService<T> ResourceService;

        protected ResourceController(
            IFhirResourceService<T> resourceService,
            ILogger<ResourceController<T>> logger)
            : base(logger)
        {
            ResourceService = resourceService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> Index()
        {
            try
            {
                var queryPairs = HttpContext.Request.Query
                    .ToList();
                var queryString = queryPairs.ToOptimisticQueryString();
                var userCredentials = HttpContext.ToCurrentUserCredentials();

                var content = await ResourceService.GetPaginatedResourceAsync(queryString, userCredentials.TenantIdentifier);

                return Ok(content);
            }
            catch (Exception ex)
            {
                Logger.LogError("Something went wrong", ex);

                if (ex != null && ex is FhirOperationException fhirEx)
                {
                    return BadRequest(fhirEx.Message);
                }
                return BadRequest(ex?.ToProblemDetails());
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> CreateResource([FromBody] T resource)
        {
            try
            {
                var userCredentials = HttpContext.ToCurrentUserCredentials();

                var (resourceId, content) = await ResourceService.CreateResourceAsync(resource, userCredentials.TenantIdentifier);

                return CreatedAtAction(nameof(GetResourceById), new { id = resourceId }, content);
            }
            catch (Exception ex)
            {
                Logger.LogError("Something went wrong", ex);

                if (ex != null && ex is FhirOperationException fhirEx)
                {
                    return BadRequest(fhirEx.Message);
                }
                return BadRequest(ex?.ToProblemDetails());
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> GetResourceById(string id)
        {
            try
            {
                var userCredentials = HttpContext.ToCurrentUserCredentials();

                var content = await ResourceService.GetResourceByIdAsync(id, userCredentials.TenantIdentifier);

                if (string.IsNullOrEmpty(content))
                {
                    return NotFound();
                }

                return Ok(content);
            }
            catch (Exception ex)
            {
                Logger.LogError("Something went wrong", ex);

                if (ex != null && ex is FhirOperationException fhirEx)
                {
                    return BadRequest(fhirEx.Message);
                }
                return BadRequest(ex?.ToProblemDetails());
            }
        }

        [HttpPut("{resourceId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePatientResource(string resourceId, [FromBody] T resource)
        {
            if (resourceId != resource.Id) return BadRequest();

            try
            {
                var userCredentials = HttpContext.ToCurrentUserCredentials();

                var content = await ResourceService.UpdateResourceAsync(resource, userCredentials.TenantIdentifier);

                return Ok(content);
            }
            catch (Exception ex)
            {
                Logger.LogError("Something went wrong", ex);

                if (ex != null && ex is FhirOperationException fhirEx)
                {
                    return BadRequest(fhirEx.Message);
                }
                return BadRequest(ex?.ToProblemDetails());
            }
        }

        [HttpDelete("{resourceId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteResourceById(string resourceId)
        {
            try
            {
                var userCredentials = HttpContext.ToCurrentUserCredentials();

                var resource = await ResourceService.GetResourceByIdAsync(resourceId, userCredentials.TenantIdentifier);

                if (resource == null)
                {
                    return NotFound();
                }

                await ResourceService.DeleteResourceByIdAsync(resourceId, userCredentials.TenantIdentifier);

                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError("Something went wrong", ex);

                if (ex != null && ex is FhirOperationException fhirEx)
                {
                    return BadRequest(fhirEx.Message);
                }
                return BadRequest(ex?.ToProblemDetails());
            }
        }
    }
}
