using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class Weapon
    {
        string name;
        public string Name { get { return name; } }

        public int TimeReload;
        public int TimeLastShoot;

        public Weapon()
        {
            TimeReload = 500;
            TimeLastShoot = 0;
            name = "Default weapon";
        }

        public virtual Bullet Shoot(Vector2D direction, Vector2D position, int playerID, int currentTime)
        {
            if (currentTime - TimeLastShoot >= TimeReload)
            {
                Bullet bullet = new Bullet(playerID);
                bullet.Damage = 10;
                bullet.Speed = 0.01;
                bullet.Size = 0.2;
                bullet.Direction = new Vector2D(direction);
                bullet.Position = new Vector2D(position);
                return bullet;
            }
            else
                return null;
        }

    }
}
