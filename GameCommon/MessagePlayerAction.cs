using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{

    public enum PlayerActionType {Move, Shoot};
    [Serializable]
    public class MessagePlayerAction: GameMessage
    {
        public Vector2D Direction;
        public MessagePlayerAction()
        {
            MessageType = MessageType.PlayerAction;
        }
    }
}
