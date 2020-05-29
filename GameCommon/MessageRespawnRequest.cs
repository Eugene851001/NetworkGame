using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageRespawnRequest: GameMessage
    {
        public MessageRespawnRequest()
        {
            MessageType = MessageType.RespawnRequest;
        }
    }
}
