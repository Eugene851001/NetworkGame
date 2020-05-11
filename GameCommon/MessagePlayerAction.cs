using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{

    public enum PlayerActionType {
        None = 0,
        MoveFront = 1, 
        MoveBack  = 2,
        RotateRight = 4, 
        RotateLeft = 8, 
        Shoot = 16,
        MoveFrontShoot = Shoot | MoveFront,
        MoveBackShoot = Shoot | MoveBack,
        MoveFrontRotateRight = MoveFront | RotateRight,
        MoveBackRotateRight = MoveBack | RotateRight,
        MoveFrontRotateLeft = MoveFront | RotateLeft,
        MoveBackRotateLeft = MoveBack | RotateLeft
    };
    [Serializable]
    public class MessagePlayerAction: GameMessage
    {
        public PlayerActionType Action;
        public int DeltaTime;
        public int InputNumber;
        public MessagePlayerAction()
        { 
            MessageType = MessageType.PlayerAction;
        }
    }
}
