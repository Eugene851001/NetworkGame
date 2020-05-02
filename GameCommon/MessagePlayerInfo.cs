using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessagePlayerInfo: GameMessage
    {
        public PlayerInfo PlayerInfo;
        public MessagePlayerInfo()
        {
            MessageType = MessageType.PlayerInfo;
        }
    }
}
