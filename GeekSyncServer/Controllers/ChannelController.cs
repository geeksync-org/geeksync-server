using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using GeekSyncServer.Internal;



namespace GeekSyncServer.Controllers
{
    [ApiVersion( "0.2" )]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ChannelController : ControllerBase
    {
      

        private readonly ILogger<AboutController> _logger;

        public ChannelController(ILogger<AboutController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{channelID}")]
        public ActionResult Get(Guid channelID)
        {
            Channel channel=ChannelManager.Instance[channelID];
            if (channel==null)
            {
                return NotFound();
            }
            else
            {
                return Ok("OK");
            }
        }


        [HttpPut("{channelID}")]
        public ActionResult<string> Register(Guid channelID)
        {
            Channel channel = ChannelManager.Instance[channelID];
            if (channel == null)
            {
                ChannelManager.Instance.CreateChannel(channelID);
            }
            return Ok("OK");
        }

        [HttpDelete("{channelID}")]
        public ActionResult<string> UnRegister(Guid channelID)
        {
            ChannelManager.Instance.DeleteChannel(channelID);
            return Ok("OK");
        }



        [HttpPost("{channelID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> Send(Guid ChannelID, string message)
        {

            Channel channel=ChannelManager.Instance[ChannelID];
            if (channel==null)
            {
                return NotFound();
            }
            else
            {
                await channel.SendToReceiver(message);
                return Ok("OK");
            }
            

             
        }


    }
}
