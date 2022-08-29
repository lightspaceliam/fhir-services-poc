using Api.Models;
using Fhir.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Api.Areas.Resources.Controllers
{
    [Route("api/resources/patients")]
    public class PatientController : BaseController
    {
        private readonly IFhirService<Hl7.Fhir.Model.Patient, Hl7.Fhir.Model.Patient> _service;
        public PatientController(
            IFhirService<Hl7.Fhir.Model.Patient, Hl7.Fhir.Model.Patient> service,
            ILogger<BaseController> logger) : base(logger)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedFhirResponse<Hl7.Fhir.Model.Patient>>> Index(int page = 1, int count = 20)
        {
            try
            {
                var (patients, totalRecords, lastUpdated) = await _service.GetPaginatedRequestAsync($"Patient?_page={page}&_count={count}");

                return Ok(new PaginatedFhirResponse<Hl7.Fhir.Model.Patient>
                {
                    TotalRecords = totalRecords,
                    LastUpdated = lastUpdated,
                    Records = patients
                });
            }
            catch(Exception ex)
            {
                Logger.LogError($"{typeof(PatientController).Name} Index exception: {ex}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Hl7.Fhir.Model.Patient>> Edit(string id)
        {
            try
            {
                var patient = await _service.GetResourceAsync($"Patient/{id}");

                if(patient == null) return NotFound();

                return Ok(patient);
            }
            catch (Exception ex)
            {
                Logger.LogError($"{typeof(PatientController).Name} Edit exception: {ex}");
                return BadRequest(ex.Message);
            }
        }
    }
}
