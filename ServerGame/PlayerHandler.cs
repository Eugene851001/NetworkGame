using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;
using System.Threading;

namespace ServerGame
{
    class PlayerHandler : IPlayerInfo
    {
        const int timeUpdate = 40;
        Player player;
        public delegate void PlayerUpdate(IPlayerInfo playerInfo);
        public event PlayerUpdate PlayerUpdateEvent;

        public PlayerHandler(Player player)
        {
            this.player = player;
            Thread threadUpdate = new Thread(UpdatePlayer);
            threadUpdate.Start();
        }

        public Vector2D GetPosition()
        {
            return player.GetPosition();
        }

        public int GetHealth()
        {
            return player.GetHealth();
        }

        public void ChangePlayerDirection(Vector2D newDirection)
        {
            player.ChangeDirection(newDirection);
        }

        public void OnPlayerUpdate()
        {
            PlayerUpdateEvent(player);
        }

        public void UpdatePlayer()
        {
            while (true)
            {
                player.Move(40);
                Console.WriteLine("Move");
                OnPlayerUpdate();
                Thread.Sleep(timeUpdate);
            }
        }

    }
}
