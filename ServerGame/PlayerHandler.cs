using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;
using System.Threading;

namespace ServerGame
{
    class PlayerHandler
    {
        const int timeStep = 100;
        public Player player;
        public delegate void PlayerUpdate(PlayerInfo playerInfo);
        public event PlayerUpdate PlayerUpdateEvent;

        public int PlayerID;

        public PlayerHandler(Player player, int playerID)
        {
            PlayerID = playerID;
            this.player = player;
        }

        public void StartUpdatePlayer()
        {
            Thread threadUpdate = new Thread(UpdatePlayer);
            threadUpdate.Start();
        }

        public Vector2D GetDirection()
        {
            return player.MoveDirection;
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
                player.Move(timeStep);
                Console.WriteLine("Move");
                Console.WriteLine("(" + player.MoveDirection.X +":" + player.MoveDirection.Y + ")");
                OnPlayerUpdate();
                player.ChangeDirection(new Vector2D(0, 0));
                Thread.Sleep(timeStep);
            }
        }

    }
}
