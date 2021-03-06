﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using GeekSyncServer.Internal;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace GeekSyncServer.Controllers
{
    [ApiVersion( "0.2" )]
    [ApiVersion( "0.3" )]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class ChannelController : ControllerBase
    {
      

        private readonly ILogger<AboutController> _logger;

        public ChannelController(ILogger<AboutController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{channelID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Get(Guid channelID)
        {
            Channel channel=ChannelManager.Instance[channelID];
            if (channel==null)
            {
                return NotFound();
            }
            else
            {
                return Ok();
            }
        }


        [HttpPut("{channelID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> Register(Guid channelID)
        {
            // TOTO: logger: Console.WriteLine("Got Connect request on "+channelID.ToString());
            Channel channel = ChannelManager.Instance[channelID];
            if (channel == null)
            {
                ChannelManager.Instance.CreateChannel(channelID);
            }
            return Ok();
        }

        [HttpDelete("{channelID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> UnRegister(Guid channelID)
        {
            // TOTO: logger: Console.WriteLine("Got Disconnect request on "+channelID.ToString());
            ChannelManager.Instance.DeleteChannel(channelID);
            return Ok();
        }



        [HttpPost("{channelID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion("0.2")]
        public async Task<ActionResult<string>> SendCompat(Guid channelID, [FromBody] string message)
        {
            
            // TOTO: logger: Console.WriteLine("Got Send request on "+channelID.ToString());
            


            Channel channel=ChannelManager.Instance[channelID];
            if (channel==null)
            {
                return NotFound();
            }
            else
            {
                await channel.SendToReceiver(message);
                return Ok();
            }
            

             
        }
        [HttpPost("{channelID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [MapToApiVersion("0.3")]
        public async Task<ActionResult<string>> Send(Guid channelID, [FromBody] object message)
        {
            
            // TOTO: logger: Console.WriteLine("Got Send request on "+channelID.ToString());
            


            Channel channel=ChannelManager.Instance[channelID];
            if (channel==null)
            {
                return NotFound();
            }
            else
            {
                string msg=JsonSerializer.Serialize(message);
              /*  Console.WriteLine("----");
                Console.WriteLine(msg);
                Console.WriteLine("----");*/
                await channel.SendToReceiver(msg);
                return Ok();
            }
            

             
        }


    }
}
