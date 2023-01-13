using Api.Abstract;
using Fhir.Service.Abstract;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Areas.Resources.Controllers
{
    [Route("api/resources/patients")]
    public class PatientController : ResourceController<Patient>
    {
        public PatientController(
            IFhirResourceService<Patient> resourceService,
            ILogger<ResourceController<Patient>> logger) 
            : base(resourceService, logger)
        { }
    }
}
