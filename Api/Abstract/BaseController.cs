using Microsoft.AspNetCore.Mvc;

namespace Api
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> Logger;

        public BaseController(ILogger<BaseController> logger)
        {
            Logger = logger;
        }
    }
}
