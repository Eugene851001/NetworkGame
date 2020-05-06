using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{

    public enum PlayerActionType {MoveFront, MoveBack, RotateRight, RotateLeft, Shoot};
    [Serializable]
    public class MessagePlayerAction: GameMessage
    {
        public PlayerActionType Action;
        public MessagePlayerAction()
        {
            MessageType = MessageType.PlayerAction;
        }
    }
}
