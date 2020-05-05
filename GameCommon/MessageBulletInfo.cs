using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MessageBulletInfo: GameMessage
    {
        public Bullet Bullet;

        public MessageBulletInfo()
        {
            MessageType = MessageType.BulletInfo;
        }
    }
}
