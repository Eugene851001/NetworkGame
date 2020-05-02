using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageAddPlayer: GameMessage
    {
        public PlayerInfo PlayerInfo;

        public MessageAddPlayer()
        {
            MessageType = MessageType.AddPlayer;
        }
    }
}
