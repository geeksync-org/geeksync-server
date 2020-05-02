using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GeekSyncServer.Controllers
{
    [ApiVersion( "0.2" )]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AboutController : ControllerBase
    {
      

        private readonly ILogger<AboutController> _logger;

        public AboutController(ILogger<AboutController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        //[MapToApiVersion("0.2")]
        public String Get()
        {
            return "It's me!";
        }
/* keeping to not forget how to do this :)
        [HttpGet]
        [MapToApiVersion("0.3")]
        public String Getv0_3()
        {
            return "It's me 0.3!";
        }
        */
    }
}
