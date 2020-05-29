using System.Drawing;
using GameCommon;

namespace ClientGame
{
    class FirstPersonAnimation
    {
        public Animation AnimationShoot;
        public Animation AnimationReady;

        PlayerState playerState;
        int timeLastStateUpdate;
        int accumulatedTime;

        public FirstPersonAnimation()
        {
            timeLastStateUpdate = 0;
            accumulatedTime = 0;
        }

        int getCurrentAnimationTime()
        {
            int result = 0;
            if((playerState & PlayerState.Shoot) != 0)
            {
                result = AnimationShoot.Frames.Length * AnimationShoot.TimeForFrame;
            }
            return result;
        }

        public void UpdatePlayer(Player player, int time)
        {
            if(time - timeLastStateUpdate > getCurrentAnimationTime())
            {
                timeLastStateUpdate = time;
                playerState = player.PlayerState;
            }
        }

        public void UpdateAnimation(int time)
        {
            if((playerState & PlayerState.Shoot) != 0)
            {
                accumulatedTime += time;
                if (accumulatedTime > getCurrentAnimationTime())
                {
                    accumulatedTime = 0;
                    playerState = playerState & ~PlayerState.Shoot;
                }
                else
                {
                    AnimationShoot.UpdateAnimation(time);
                }
            }
            else
            {
                    AnimationReady.UpdateAnimation(time);
            }
        }

        public Bitmap GetTexture()
        {
            Bitmap result = null;
            if((playerState & PlayerState.Shoot) != 0)
            {
                result = AnimationShoot.GetCurrentFrame();
            }
            else
            {
                result = AnimationReady.GetCurrentFrame();
            }
            return result;
        }
    }
}
