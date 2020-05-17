using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageRegistration: GameMessage
    {
        public string Name;

        public MessageRegistration()
        {
            MessageType = MessageType.Regitsration;
        }
    }
}
