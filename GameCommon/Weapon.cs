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

        public virtual Bullet Shoot(Player player, int currentTime)
        {
            if (currentTime - TimeLastShoot >= TimeReload)
            {
                Bullet bullet = new Bullet(player.PlayerID);
                bullet.Damage = 10;
                bullet.Speed = 0.001;
                bullet.Size = 0.2;
                bullet.Direction = new Vector2D(player.Direction);
                bullet.Position = new Vector2D(player.Position);
                bullet.Position.X += (player.Size + bullet.Size + 0.1) * bullet.Direction.X;
                bullet.Position.Y += (player.Size + bullet.Size + 0.1) * bullet.Direction.Y;
                return bullet;
            }
            else
                return null;
        }

    }
}
