using System.Drawing;
using GameCommon;

namespace ClientGame
{
    class FirstPersonAnimation
    {
        public Animation AnimationShoot;
        public Animation AnimationReady;

        PlayerState playerState;

        public void UpdatePlayer(Player player, int time)
        {
            playerState = player.PlayerState;
        }

        public void UpdateAnimation(int time)
        {
            if((playerState & PlayerState.Shoot) != 0)
            {
                AnimationShoot.UpdateAnimation(time);
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
