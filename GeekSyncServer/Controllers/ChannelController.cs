using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using GeekSyncServer.Internal;
using GeekSyncServer.Model;


namespace GeekSyncServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChannelController : ControllerBase
    {
      

        private readonly ILogger<AboutController> _logger;

        public ChannelController(ILogger<AboutController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{pairingID}")]
        public ChannelInfo Get(Guid pairingID)
        {
            return new ChannelInfo(ChannelManager.Instance[pairingID]);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ChannelInfo>> Send(MessagePayload payload)
        {

            Channel channel=ChannelManager.Instance[payload.PairingID];
            if (channel==null)
            {
                return NotFound();
            }
            else
            {
                await channel.SentToAllDesktops(payload.Message);
                return Ok();
            }
            

             
        }


    }
}
