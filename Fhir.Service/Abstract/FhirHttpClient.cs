using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhir.Service.Abstract
{
    public class FhirHttpClient
    {
        public HttpClient Client { get; private set; }

        public FhirHttpClient(HttpClient httpClient)
        {
            Client = httpClient;
        }
    }
}
