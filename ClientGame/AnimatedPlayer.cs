using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using GameCommon;

namespace ClientGame
{
    class AnimatedPlayer: Player
    {
        public Animation AnimationWalkFront;
        public Animation AnimationWalkBack;
        public Animation AnimationAttack;
        public Animation AnimationDie;
        public Animation AnimationNoneFront;
        public Animation AnimationNoneBack;

        public int PlayerID;
        int timeLastStateUpdate;
        int physicalUpdateInterval;

        public AnimatedPlayer(int physicalUpdateInterval)
        {
            this.physicalUpdateInterval = physicalUpdateInterval;
            timeLastStateUpdate = 0;
        }

        double getAngle(Player player)
        {
            double result = Vector2D.GetAngle(player.Direction, this.Direction);
            while (result < 0)
                result += Math.PI * 2;
            while (result > Math.PI * 2)
                result += Math.PI * 2;
            return result;
        }

        public void UpdateAnimation(Player player, int elapsedTime)
        {

            if ((this.PlayerState & PlayerState.MoveFront) != 0 || (this.PlayerState & PlayerState.MoveBack) != 0)
            {
                double angle = getAngle(player);
                if (angle > Math.PI / 2 && angle < Math.PI * 3 / 2)
                    AnimationWalkFront.UpdateAnimation(elapsedTime);
                else
                    AnimationWalkBack.UpdateAnimation(elapsedTime);
            }
        }

        public void UpdatePlayer(Player playerToDraw, int time)
        {
            if(time - timeLastStateUpdate > physicalUpdateInterval)
            {
                timeLastStateUpdate = time;
                this.PlayerState = playerToDraw.PlayerState;
            }
            this.Position = playerToDraw.Position;
            this.Direction = playerToDraw.Direction;
            this.ViewAngle = playerToDraw.ViewAngle;
        }

        public Bitmap GetTexture(Player player)
        {
            Bitmap texture = null;
            double angle = getAngle(player);
            if ((this.PlayerState & PlayerState.MoveFront) != 0 
                || (this.PlayerState & PlayerState.MoveBack) != 0)
            {             
                if (angle > Math.PI / 2 && angle < Math.PI * 3 / 2)
                    texture = AnimationWalkFront.GetCurrentFrame();
                else
                    texture = AnimationWalkBack.GetCurrentFrame();
            }
            else if((this.PlayerState & PlayerState.Shoot) != 0)
            {
                texture = AnimationAttack.GetCurrentFrame();
            }
            else
            {
                if (angle > Math.PI / 2 && angle < Math.PI * 3 / 2)
                    texture = AnimationNoneFront.GetCurrentFrame();
                else
                    texture = AnimationNoneBack.GetCurrentFrame();
            }
            return texture;
        }

    }
}
