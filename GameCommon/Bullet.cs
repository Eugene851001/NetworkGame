using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class Bullet: MovableGameObject
    {
        public int OwnerID;
        public int Damage;

        public Bullet(int ownerID)
        {
            OwnerID = ownerID;
            Damage = 10;
        }
    }
}
