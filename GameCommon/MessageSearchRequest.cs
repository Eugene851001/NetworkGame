using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageSearchRequest: GameMessage
    {
        public string IPAdress;
        public int Port;

        public MessageSearchRequest()
        {
            MessageType = MessageType.SearchRequest;
        }
    }
}
