using Fhir.Service.Abstract;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fhir.Service
{
    public class PatientService : FhirResourceService<Patient>
    {
        public PatientService(
            IHttpClientFactory httpClientFactory,
            ILogger<FhirResourceService<Patient>>
            logger, IConfiguration configuration) 
            : base(httpClientFactory, logger, configuration)
        { }
    }
}