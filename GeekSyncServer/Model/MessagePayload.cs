using System;

namespace GeekSyncServer.Model
{
    public class MessagePayload
    {
        public Guid PairingID {get;set;}
        public string Message {get;set;}
       //TODO: find out why it's not available
       // private TimeDate TimeStamp {get;set;}
    }
}