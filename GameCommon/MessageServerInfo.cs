using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageServerInfo: GameMessage
    {
        public string IPAdress;
        public int Port;

        public int MaxPlayersAmount;
        public int CurrentPlayersAmount;


        public string MapName;
        public int MapHeight;
        public int MapWidth;

        public MessageServerInfo()
        {
            MessageType = MessageType.ServerInfo;
        }
    }
}
