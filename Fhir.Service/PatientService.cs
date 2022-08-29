using Fhir.Service.Abstract;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;

namespace Fhir.Service
{
    public class PatientService : FhirService<Patient, Patient>
    {
        public PatientService(
            FhirHttpClient fhirHttpClient,
            ILogger<FhirService<Patient, Patient>> logger)
            : base(fhirHttpClient, logger) 
        { }
    }
}