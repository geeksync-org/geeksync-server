using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GeekSyncServer.Internal;
using GeekSyncServer.Model;

namespace GeekSyncServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DesktopController : ControllerBase
    {


        private readonly ILogger<AboutController> _logger;

        public DesktopController(ILogger<AboutController> logger)
        {
            _logger = logger;
        }


        [HttpGet("{desktopID}/connect/{pairingID}/{desktopName}")]
        public ActionResult<ChannelInfo> ConnectDesktop(Guid pairingID, Guid desktopID, string desktopName)
        {
            Channel channel = ChannelManager.Instance[pairingID];
            if (channel == null)
            {
                channel = ChannelManager.Instance.CreateChannel(pairingID);
            }
            channel.ConnectDesktop(desktopID, desktopName);
            return new ChannelInfo(channel);
        }

        [HttpGet("{desktopID}/disconnect/{pairingID}")]
        public ActionResult<ChannelInfo> DisconnectDesktop(Guid pairingID, Guid desktopID)
        {
            Channel channel = ChannelManager.Instance[pairingID];
            if (channel != null)
            {

                channel.DisconnectDesktop(desktopID);
             
            }
            return new ChannelInfo(ChannelManager.Instance[pairingID]);
        }


    }
}
