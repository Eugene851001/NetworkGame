using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageAddPlayer: GameMessage
    {
        public Player Player;

        public MessageAddPlayer()
        {
            MessageType = MessageType.AddPlayer;
        }
    }
}
