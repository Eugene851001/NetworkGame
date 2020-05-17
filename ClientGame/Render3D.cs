using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameCommon;
using System.Threading;
using System.Drawing.Imaging;

namespace ClientGame
{
    class Render3D
    {
        TileMap map;

        double ViewAngle = Math.PI / 2;
        double checkStep = 0.1;

        double[] depthBuffer;

        Color floorColor = Color.RosyBrown;
        Color ignoreColor = Color.FromArgb(255, 152, 0, 136);

        byte byteColorFloor = 224;
        byte byteColorCeil = 255;


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

        public Bitmap DrawGameObject(GameObject obj, Player player, Bitmap 
            canvas, Bitmap texture, bool isLanded)
        {
            Bitmap bufferCanvas = new Bitmap(canvas, VerticalSegmentsAmount, HorizontalSegmentsAmount);
            while (player.ViewAngle < 0)
                player.ViewAngle += Math.PI * 2;
            while (player.ViewAngle > Math.PI * 2)
                player.ViewAngle -= Math.PI * 2;
            double angleLowBorder = player.ViewAngle - ViewAngle / 2;
            double angleHighBorder = player.ViewAngle + ViewAngle / 2;
            double objectAngle = Math.Atan2(obj.Position.Y - player.Position.Y,
                    obj.Position.X - player.Position.X);
            while (objectAngle < 0)
                objectAngle += Math.PI * 2;
            while (objectAngle > Math.PI * 2)
                objectAngle -= Math.PI * 2;
            double distance = countDistance(obj, player);
            double step = distance / VerticalSegmentsAmount;
            if (step == 0)
                return canvas;

            for (double j = -obj.Size; j < obj.Size; j += step)
            {
                double alpha = Math.Atan2(j, distance) + objectAngle;
                bool isVisual = false;
                if (angleLowBorder < 0)
                    if (alpha < Math.PI / 2.0f && alpha < angleHighBorder)
                        isVisual = true;
                    else
                        if (alpha > angleLowBorder + 2 * Math.PI)
                        isVisual = true;
                if ((alpha > angleLowBorder && alpha < angleHighBorder) || isVisual)
                {
                    int ceiling = (int)(HorizontalSegmentsAmount / 2 - HorizontalSegmentsAmount * obj.Size / distance);
                    int floor = HorizontalSegmentsAmount - ceiling;
                    int height = floor - ceiling;
                    if (height > HorizontalSegmentsAmount)
                        continue;
                    int correctLanded = 0;
                    if (isLanded)
                        correctLanded = (int)(HorizontalSegmentsAmount * obj.Size / (distance * 2));
                    int x = (int)((alpha - angleLowBorder) * VerticalSegmentsAmount
                        / (angleHighBorder - angleLowBorder));
                    int textureX = (int)((j + obj.Size) * texture.Width / (obj.Size * 2));
                    ceiling += correctLanded;
                    floor += correctLanded;
                    if (distance < depthBuffer[x])
                    {
                        depthBuffer[x] = distance;
                        for (int y = 0; y < VerticalSegmentsAmount; y++)
                        {
                            if (y > ceiling && y < floor)
                            {
                                int textureY = (y - ceiling) * texture.Height / height;
                                Color shade = texture.GetPixel(textureX, textureY);
                                if (shade != Color.FromArgb(255, 255, 255, 255)
                                    && shade != ignoreColor)
                                    bufferCanvas.SetPixel(x, y, shade);
                            }
                        }
                    }
                }
            }
            canvas = new Bitmap(bufferCanvas, canvas.Width, canvas.Height);
            return canvas;
        }

        public Bitmap DrawGameObjects(List<GameObject> gameObjects, Player player, 
            Bitmap canvas, Bitmap texture, bool isLanded)
        {
            Bitmap bufferCanvas = new Bitmap(canvas, VerticalSegmentsAmount, HorizontalSegmentsAmount);
            while (player.ViewAngle < 0)
                player.ViewAngle += Math.PI * 2;
            while (player.ViewAngle > Math.PI * 2)
                player.ViewAngle -= Math.PI * 2;
            double angleLowBorder = player.ViewAngle - ViewAngle / 2;
            double angleHighBorder = player.ViewAngle + ViewAngle / 2;
            foreach (var obj in gameObjects)
            {
                double objectAngle = Math.Atan2(obj.Position.Y - player.Position.Y,
                   obj.Position.X - player.Position.X);
                while (objectAngle < 0)
                    objectAngle += Math.PI * 2;
                while (objectAngle > Math.PI * 2)
                    objectAngle -= Math.PI * 2;
                double distance = countDistance(obj, player);
                double step = distance / VerticalSegmentsAmount;
                if (step == 0)
                    return canvas;
                for (double j = -obj.Size; j < obj.Size; j += step)
                {
                    double alpha = Math.Atan2(j, distance) + objectAngle;
                    if (alpha > angleLowBorder && alpha < angleHighBorder)
                    {
                        int ceiling = (int)(HorizontalSegmentsAmount / 2 -
                            HorizontalSegmentsAmount * obj.Size / distance);
                        int floor = HorizontalSegmentsAmount - ceiling;
                        int height = floor - ceiling;
                        if (height > HorizontalSegmentsAmount)
                            continue;
                        int correctLanded = 0;
                        if (isLanded)
                            correctLanded = (int)(HorizontalSegmentsAmount * obj.Size / (distance * 2));
                        int x = (int)((alpha - angleLowBorder) * VerticalSegmentsAmount
                            / (angleHighBorder - angleLowBorder));
                        int textureX = (int)((j + obj.Size) * texture.Width / (obj.Size * 2));
                        ceiling += correctLanded;
                        floor += correctLanded;
                        if (distance > depthBuffer[x])
                            continue;
                        depthBuffer[x] = distance;
                        for (int y = 0; y < VerticalSegmentsAmount; y++)
                        {
                            if (y > ceiling && y < floor)
                            {
                                int textureY = (y - ceiling) * texture.Height / height;
                                Color shade = texture.GetPixel(textureX, textureY);
                                if (shade != Color.FromArgb(255, 255, 255, 255))
                                    bufferCanvas.SetPixel(x, y, shade);
                            }
                        }
                    }
                }
            }
            canvas = new Bitmap(bufferCanvas, canvas.Width, canvas.Height);
            return canvas;
        }

        public unsafe Bitmap DrawWalls(Player player, Bitmap canvas)
        {
            Bitmap bufferCanvas = new Bitmap(VerticalSegmentsAmount, HorizontalSegmentsAmount, PixelFormat.Format8bppIndexed);
            BitmapData bufferCanvasData = bufferCanvas.LockBits(new Rectangle(0,
                0, bufferCanvas.Width, bufferCanvas.Height), ImageLockMode.WriteOnly, bufferCanvas.PixelFormat);
            byte* ptrData = (byte*)bufferCanvasData.Scan0.ToPointer();

            for(int x = 0; x < VerticalSegmentsAmount; x++)
            {
                double rayAngle = player.ViewAngle - ViewAngle / 2 + ((double)x / VerticalSegmentsAmount) * ViewAngle;
                double wallDistance = 0;
                bool isWall = false;
                int testX = 0, testY = 0;
                while (!isWall)
                {
                    wallDistance += checkStep;
                    testX = (int)(player.Position.X + Math.Cos(rayAngle) * wallDistance);
                    testY = (int)(player.Position.Y + Math.Sin(rayAngle) * wallDistance);
                    if (testX >= map.Width || testX < 0 || testY >= map.Height || testY < 0)
                    {
                        isWall = true;
                    }
                    else
                    {
                        isWall = map.IsSolid(testX, testY);
                    }
                }

                Bitmap wallTexture = wallTextures[map.Tiles[testY, testX].TextureID];
                BitmapData wallTextureData = wallTexture.LockBits(new Rectangle(0, 0,
                    wallTexture.Width, wallTexture.Height), ImageLockMode.ReadOnly, wallTexture.PixelFormat);
                byte* ptrWallData = (byte*)wallTextureData.Scan0.ToPointer();
                depthBuffer[x] = wallDistance;

                int ceiling = (int)(HorizontalSegmentsAmount / 2 - HorizontalSegmentsAmount / (wallDistance));
                int floor = HorizontalSegmentsAmount - ceiling;
                int height = floor - ceiling;

                double fractX = player.Position.X + Math.Cos(rayAngle) * wallDistance - testX;
                double fractY = player.Position.Y + Math.Sin(rayAngle) * wallDistance - testY;
                int textureX = (int)(fractX > fractY ? fractX * wallTexture.Width : fractY * wallTexture.Width);
                int textureY;
                for (int y = 0; y < HorizontalSegmentsAmount; y++)
                {
                    if (y > ceiling && y < floor)
                    {
                        textureY = (y - ceiling) * wallTexture.Height / height;
                        *(ptrData + y * bufferCanvasData.Width + x) =
                           *(ptrWallData + textureY * wallTextureData.Width + textureX);
                        //bufferCanvas.SetPixel(x, y, wallTexture.GetPixel(textureX, textureY));
                    }
                    else if (y > floor)
                    {
                        *(ptrData + y * bufferCanvasData.Width + x) = byteColorFloor;
                      //  bufferCanvas.SetPixel(x, y, floorColor);
                    }
                    else
                    {
                        *(ptrData + y * bufferCanvasData.Width + x) = byteColorCeil;
                        //bufferCanvas.SetPixel(x, y, Color.White);
                    }
                }
                wallTexture.UnlockBits(wallTextureData);
            }
            bufferCanvas.UnlockBits(bufferCanvasData);
            canvas = new Bitmap(bufferCanvas, canvas.Width, canvas.Height);
            return canvas;
        }
    }
}
