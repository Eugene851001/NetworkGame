using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageDeletePlayer: GameMessage
    {
        public MessageDeletePlayer()
        {
            MessageType = MessageType.DeletePlayer;
        }
    }
}
