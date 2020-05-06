using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageChat: GameMessage
    {
        public string Content;
        public MessageChat()
        {
            MessageType = MessageType.Chat;
        }
    }
}
