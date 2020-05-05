using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{

    public enum PlayerActionType {Move, Rotate, Shoot};
    [Serializable]
    public class MessagePlayerAction: GameMessage
    {
        public Vector2D Direction;
        public PlayerActionType Action;
        public double Angle;
        public MessagePlayerAction()
        {
            MessageType = MessageType.PlayerAction;
        }
    }
}
