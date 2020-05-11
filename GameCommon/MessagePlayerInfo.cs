using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessagePlayerInfo: GameMessage
    {
        public Player Player;
        public int InputNumber;
        public MessagePlayerInfo()
        {
            MessageType = MessageType.PlayerInfo;
        }
    }
}
