using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameCommon;

namespace ClientGame
{
    class Render3D
    {
        TileMap map;

        double ViewAngle = Math.PI / 2;
        double checkStep = 0.1;

        double[] depthBuffer;

        Dictionary<int, Bitmap> wallTextures;

        public int VerticalSegmentsAmount = 300;
        public int HorizontalSegmentsAmount = 300;

        public Render3D(TileMap map, Dictionary<int, Bitmap> wallTextures)
        {
            this.wallTextures = wallTextures;
            depthBuffer = new double[VerticalSegmentsAmount];
            this.map = map;
        }

        double countDistance(GameObject firstObject, GameObject secondObject)
        {
            return Math.Sqrt(Math.Pow(firstObject.Position.X - secondObject.Position.X, 2)
                + Math.Pow(secondObject.Position.Y - secondObject.Position.Y, 2));
        }

        public Bitmap DrawGameObjects(List<GameObject> gameObjects, PlayerInfo player, Bitmap canvas, Bitmap texture)
        {
            while (player.ViewAngle < 0)
                player.ViewAngle += Math.PI * 2;
            while (player.ViewAngle > Math.PI * 2)
                player.ViewAngle -= Math.PI * 2;
            double angleLowBorder = player.ViewAngle - ViewAngle / 2;
            double angleHighBorder = player.ViewAngle + ViewAngle / 2;
            foreach(var obj in gameObjects)
            {
                double objectAngle = Math.Atan2(obj.Position.Y - player.Position.Y, 
                    obj.Position.X - player.Position.X);
                while (objectAngle < 0)
                    objectAngle += Math.PI * 2;
                while (objectAngle > Math.PI * 2)
                    objectAngle -= Math.PI * 2;
                double distance = countDistance(obj, player);
                double step = distance / VerticalSegmentsAmount;
                for(double j = -obj.Size; j < obj.Size; j+=step)
                {
                    double alpha = Math.Atan2(j, distance) + objectAngle;
                    if(alpha > angleLowBorder && alpha < angleHighBorder)
                    {
                        int ceiling = (int)(HorizontalSegmentsAmount / 2 - HorizontalSegmentsAmount * obj.Size / distance);
                        int floor = HorizontalSegmentsAmount - ceiling;
                        int height = floor - ceiling;
                        int x = (int)((alpha - angleLowBorder) * VerticalSegmentsAmount
                            / (angleHighBorder - angleLowBorder));
                        int textureX = (int)((j + obj.Size) * texture.Width / (obj.Size * 2));
                       
                        if (distance > depthBuffer[x])
                            continue;
                        for (int y = 0; y < VerticalSegmentsAmount; y++)
                        {
                            if (y > ceiling && y < floor)
                            {
                                int textureY = (y - ceiling) * texture.Height / height;
                                Color shade = texture.GetPixel(textureX, textureY);
                                if(shade != Color.FromArgb(255, 255, 255, 255))
                                    canvas.SetPixel(x, y, shade);
                            }
                        }
                    }
                }
            }
            return canvas;
        }

        public Bitmap DrawWalls(PlayerInfo player, Bitmap canvas)
        {
            for(int x = 0; x < VerticalSegmentsAmount; x++)
            {
                //float fRayAngle = (player.fAngle - fViewAngle / 2.0f) + ((float)x / ScreenWidth) * fViewAngle;
                double rayAngle = player.ViewAngle - ViewAngle / 2 + ((double)x / VerticalSegmentsAmount) * ViewAngle;
                double wallDistance = 0;
                bool isWall = false;
                int testX = 0, testY = 0;
                while(!isWall)
                {
                    wallDistance += checkStep;
                    testX = (int)(player.Position.X + Math.Cos(rayAngle) * wallDistance);
                    testY = (int)(player.Position.Y + Math.Sin(rayAngle) * wallDistance);
                    if(testX >= map.Width || testX < 0 || testY >= map.Height || testY < 0)
                    {
                        isWall = true;
                    }
                    else
                    {
                        isWall =  map.IsSolid(testX, testY);
                    }
                }

                Bitmap wallTexture = wallTextures[map.Tiles[testY, testX].TextureID];
                depthBuffer[x] = wallDistance;

                int ceiling = (int)(HorizontalSegmentsAmount / 2 - HorizontalSegmentsAmount / (wallDistance));
                int floor = HorizontalSegmentsAmount - ceiling;
                int height = floor - ceiling;

                double fractX = player.Position.X + Math.Cos(rayAngle) * wallDistance - testX;
                double fractY = player.Position.Y + Math.Sin(rayAngle) * wallDistance - testY;
                int textureX =(int)(fractX > fractY ? fractX * wallTexture.Width : fractY * wallTexture.Width);
                for(int y = 0; y < HorizontalSegmentsAmount; y++)
                {
                    if(y > ceiling && y < floor)
                    {
                        int textureY = (y - ceiling) * wallTexture.Height / height;
                        canvas.SetPixel(x, y, wallTexture.GetPixel(textureX, textureY));
                    }
                    else
                    {
                        canvas.SetPixel(x, y, Color.White);
                    }
                }
            }
            return canvas;
        }
    }
}
