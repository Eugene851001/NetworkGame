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
        new enum Direction { Front, Back, Left, Right};

        public Animation AnimationWalkFront;
        public Animation AnimationWalkBack;
        public Animation AnimationWalkLeft;
        public Animation AnimationWalkRight;

        public Animation AnimationAttackFront;
        public Animation AnimationAttackBack;
        public Animation AnimationAttackRight;
        public Animation AnimationAttackLeft;
     
     //   public Animation Hurt;
        public Animation AnimationDie;

        public Animation AnimationNoneFront;
        public Animation AnimationNoneBack;
        public Animation AnimationNoneLeft;
        public Animation AnimationNoneRight;

        new public int PlayerID;
        int timeLastStateUpdate;
        int accumulatedTime;
        int physicalUpdateInterval;

        public AnimatedPlayer(int physicalUpdateInterval): base(-1)
        {
            this.physicalUpdateInterval = physicalUpdateInterval;
            timeLastStateUpdate = 0;
            accumulatedTime = 0;
        }


        Direction getDirection(double angle)
        {
            Direction result = Direction.Front;
            while (angle < -Math.PI)
                angle += Math.PI * 2;
            while(angle > Math.PI)
                angle -= Math.PI * 2;
            if(Math.Abs(angle) < Math.PI / 4)
            {
                result = Direction.Back;
            }
            else if(Math.Abs(angle) >= Math.PI * 3 / 4)
            {
                result = Direction.Front;
            }
            else if(angle  > 0)
            {
                result = Direction.Left;
            }
            else if(angle < 0)
            {
                result = Direction.Right;
            }
            return result;
        }

        double getAngle(Player player)
        {
             double result = Vector2D.GetAngle(base.Direction, player.Direction);
            while (result < 0)
                result += Math.PI * 2;
            while (result > Math.PI * 2)
                result += Math.PI * 2;
            return result;
        }

        public void UpdateAnimation(Player player, int elapsedTime)
        {
            if (player == null)
                return;
            double angle = getAngle(player);
            Direction reativeDirection = getDirection(angle);
            if ((this.PlayerState & PlayerState.Killed) != 0)
            {
                AnimationDie.UpdateAnimation(elapsedTime);
            }
            else if ((PlayerState & PlayerState.Shoot) != 0)
            {
                accumulatedTime += elapsedTime;
                if(accumulatedTime > getCurrentAnimationTime())
                {
                    accumulatedTime = 0;
                    PlayerState = PlayerState & ~PlayerState.Shoot;
                }
                switch (reativeDirection)
                {
                    case Direction.Back:
                        AnimationAttackBack.UpdateAnimation(elapsedTime);
                        break;
                    case Direction.Left:
                        AnimationAttackLeft.UpdateAnimation(elapsedTime);
                        break;
                    case Direction.Right:
                        AnimationAttackRight.UpdateAnimation(elapsedTime);
                        break;
                    case Direction.Front:
                        AnimationAttackFront.UpdateAnimation(elapsedTime);
                        break;
                }
            }
            else if ((this.PlayerState & PlayerState.MoveFront) != 0 || (this.PlayerState & PlayerState.MoveBack) != 0)
            {
                accumulatedTime += elapsedTime;
                if(accumulatedTime > getCurrentAnimationTime())
                {
                    accumulatedTime = 0;
                    PlayerState = PlayerState & ~(PlayerState.MoveFront | PlayerState.MoveBack);
                }
                switch (reativeDirection)
                {
                    case Direction.Front:
                        AnimationWalkFront.UpdateAnimation(elapsedTime);
                        break;
                    case Direction.Back:
                        AnimationWalkBack.UpdateAnimation(elapsedTime);
                        break;
                    case Direction.Left:
                        AnimationWalkLeft.UpdateAnimation(elapsedTime);
                        break;
                    case Direction.Right:
                        AnimationAttackRight.UpdateAnimation(elapsedTime);
                        break;
                }
            }

        }

        int getCurrentAnimationTime()
        {
            int result = 0;
            if((PlayerState & PlayerState.MoveFront) != 0 || (PlayerState & PlayerState.MoveBack) != 0)
            {
                result = AnimationWalkFront.Frames.Length * AnimationWalkFront.TimeForFrame;
            }
            if((PlayerState & PlayerState.Shoot) != 0)
            {
                result = AnimationAttackFront.Frames.Length * AnimationAttackFront.TimeForFrame;
            }
            return result;
        }

        public void UpdatePlayer(Player playerToDraw, int time)
        {
            if(time - timeLastStateUpdate > getCurrentAnimationTime())
            {
                timeLastStateUpdate = time;
                this.PlayerState = playerToDraw.PlayerState;
            }
            this.Position = playerToDraw.Position;
            base.Direction = playerToDraw.Direction;
            this.ViewAngle = playerToDraw.ViewAngle;
        }

        public Bitmap GetTexture(Player player)
        {
            if (player == null)
                return null;
            Bitmap texture = null;
            double angle = getAngle(player);
            Direction relativeDirection = getDirection(angle);
            if((this.PlayerState & PlayerState.Killed) != 0)
            {
                texture = AnimationDie.GetCurrentFrame();
            }
            else if ((this.PlayerState & PlayerState.Shoot) != 0)
            {
                switch(relativeDirection)
                {
                    case Direction.Left:
                        texture = AnimationAttackLeft.GetCurrentFrame();
                        break;
                    case Direction.Right:
                        texture = AnimationAttackRight.GetCurrentFrame();
                        break;
                    case Direction.Front:
                        texture = AnimationAttackFront.GetCurrentFrame();
                        break;
                    case Direction.Back:
                        texture = AnimationAttackBack.GetCurrentFrame();
                        break;
                }
            }
            else if ((this.PlayerState & PlayerState.MoveFront) != 0 
                || (this.PlayerState & PlayerState.MoveBack) != 0)
            {             
                switch(relativeDirection)
                {
                    case Direction.Front:
                        texture = AnimationWalkFront.GetCurrentFrame();
                        break;
                    case Direction.Back:
                        texture = AnimationWalkBack.GetCurrentFrame();
                        break;
                    case Direction.Left:
                        texture = AnimationWalkLeft.GetCurrentFrame();
                        break;
                    case Direction.Right:
                        texture = AnimationWalkRight.GetCurrentFrame();
                        break;
                }
            }
            else
            {
                switch(relativeDirection)
                {
                    case Direction.Front:
                        texture = AnimationNoneFront.GetCurrentFrame();
                        break;
                    case Direction.Back:
                        texture = AnimationNoneBack.GetCurrentFrame();
                        break;
                    case Direction.Left:
                        texture = AnimationNoneLeft.GetCurrentFrame();
                        break;
                    case Direction.Right:
                        texture = AnimationNoneRight.GetCurrentFrame();
                        break;
                }
            }
            return texture;
        }

    }
}
