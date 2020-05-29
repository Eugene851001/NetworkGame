using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ClientGame
{
    class Animation
    {
        public int TimeForFrame;
        public Bitmap[] Frames;
        int accumulatedTime;
        int framePointer;
        public bool IsCycled = true;

        public Animation(int timeForFrame)
        {
            framePointer = 0;
            accumulatedTime = 0;
            TimeForFrame = timeForFrame;
        }
        
        public void UpdateAnimation(int elapsedTime)
        {
            accumulatedTime += elapsedTime;
            if (accumulatedTime > TimeForFrame)
            {
                accumulatedTime = 0;
                framePointer++;
                if (framePointer == Frames.Length)
                    if (IsCycled)
                        framePointer = 0;
                    else
                        framePointer--;

            }
        }



        public Bitmap GetCurrentFrame()
        {           
            return Frames[framePointer];
        }

    }
}
