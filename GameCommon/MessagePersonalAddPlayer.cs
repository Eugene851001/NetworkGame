using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessagePersonalAddPlayer: GameMessage
    {
        public TileMap Map;

        public MessagePersonalAddPlayer()
        {
            MessageType = MessageType.PersonalAddPlayer;
        }
    }
}
