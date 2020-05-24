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
        Color ceilColor = Color.Blue;
        Color ignoreColor = Color.FromArgb(255, 152, 0, 136); 

        byte byteColorFloor = 224;
        byte byteColorCeil = 255;
        byte byteColorWhite = 255;
        byte byteColorIgnore = 0x54;


        Dictionary<int, Bitmap> wallTextures;
        Dictionary<int, int[]> wallTexturesPalettes;
        Dictionary<int, int[]> cachedPalettes;

        public int VerticalSegmentsAmount = 128; 
        public int HorizontalSegmentsAmount = 128;


        public void SetMap(TileMap map)
        {
            this.map = map;
        }

        public Render3D(TileMap map, Dictionary<int, Bitmap> wallTextures)
        {
            this.wallTextures = wallTextures;
            wallTexturesPalettes = new Dictionary<int, int[]>();
            foreach(int key in wallTextures.Keys)
            {
                wallTexturesPalettes.Add(key, getPalette(wallTextures[key]));
            }
            cachedPalettes = new Dictionary<int, int[]>();
            depthBuffer = new double[VerticalSegmentsAmount];
            this.map = map;
        }

        double countDistance(GameObject firstObject, GameObject secondObject)
        {
            return Math.Sqrt(Math.Pow(firstObject.Position.X - secondObject.Position.X, 2)
                + Math.Pow(secondObject.Position.Y - secondObject.Position.Y, 2));
        }

        public unsafe Bitmap DrawGameObject(GameObject obj, Player player, Bitmap 
            canvas, Bitmap texture, bool isLanded)
        {
            Bitmap bufferCanvas = canvas;// new Bitmap(canvas, VerticalSegmentsAmount, HorizontalSegmentsAmount);
            BitmapData bufferCanvasData = bufferCanvas.LockBits(new Rectangle(0, 0,
                bufferCanvas.Width, bufferCanvas.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int* ptrData = (int*)bufferCanvasData.Scan0.ToPointer();

            BitmapData bufferTextureData = texture.LockBits(new Rectangle(0, 0,
                texture.Width, texture.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            byte* ptrTextureData = (byte*)bufferTextureData.Scan0.ToPointer();
            if (!cachedPalettes.ContainsKey(texture.Palette.Flags))
                cachedPalettes.Add(texture.Palette.Flags, getPalette(texture));
            int[] texturePalette = cachedPalettes[texture.Palette.Flags];

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
            double distance = countDistance(obj, player);
            double step = distance / VerticalSegmentsAmount;
            if (step == 0)
                return canvas;

            for (double j = -obj.Size; j < obj.Size; j += step)
            {
                double alpha = Math.Atan2(j, distance) + objectAngle;
                while (alpha < 0)
                    alpha += Math.PI * 2;
                while (alpha > Math.PI * 2)
                    alpha -= Math.PI * 2;

                bool isVisual = false;
                if (angleLowBorder < 0)
                    if (alpha < Math.PI / 2.0 && alpha < angleHighBorder)
                        isVisual = true;
                    else if (alpha > Math.PI * 3 / 2 && alpha > angleLowBorder + Math.PI * 2)
                    {
                        isVisual = true;
                    }
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
                    int x = (int)(Math.Abs((alpha - angleLowBorder)) * VerticalSegmentsAmount
                        / (ViewAngle));
                    int textureX = (int)((j + obj.Size) * texture.Width / (obj.Size * 2));
                    ceiling += correctLanded;
                    floor += correctLanded;
                    if (x > 0 && x < depthBuffer.Length && 
                        distance < depthBuffer[x])
                    {
                        depthBuffer[x] = distance;
                        for (int y = 0; y < VerticalSegmentsAmount; y++)
                        {
                            if (y > ceiling && y < floor)
                            {
                                int textureY = (y - ceiling) * texture.Height / height;
                                byte shade = *(ptrTextureData + textureY * texture.Width + textureX);
                                if (texturePalette[shade] != Color.White.ToArgb() 
                                    && texturePalette[shade] != ignoreColor.ToArgb())
                                    *(ptrData + y * canvas.Width + x) = texturePalette[shade];
                            }
                        }
                    }
                }
            }
            bufferCanvas.UnlockBits(bufferCanvasData);
            texture.UnlockBits(bufferTextureData);
            canvas = bufferCanvas;// new Bitmap(bufferCanvas, canvas.Width, canvas.Height);
            return canvas;
        }

        public unsafe Bitmap DrawInterface(Bitmap canvas, Bitmap texture)
        {

            //Bitmap scaledTexture = new Bitmap(texture, canvas.Width, canvas.Height);
            Bitmap scaledTexture = texture;
            BitmapData canvasData = canvas.LockBits(new Rectangle(0, 0, canvas.Width, canvas.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int* ptrCanvasData = (int*)canvasData.Scan0.ToPointer();

            BitmapData textureData = scaledTexture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            byte* ptrTextureData = (byte*)textureData.Scan0.ToPointer();

            int[] texturePalette = getPalette(texture);

            int scaleWidth = canvas.Width / texture.Width;
            int scaleHeight = canvas.Height / texture.Height;

            for(int i = 0; i < scaledTexture.Height; i++)
                for(int j = 0; j < scaledTexture.Width; j++)
                {
                    int shade = texturePalette[*(ptrTextureData + i * scaledTexture.Width + j)];
                    if (shade != ignoreColor.ToArgb())
                    {
                        drawRectangle(ptrCanvasData, canvas.Width, j * scaleWidth, 
                            i * scaleHeight, scaleWidth, scaleHeight, shade);
                      //  *(ptrCanvasData + i * canvas.Width + j) = shade;
                    }
                }

            scaledTexture.UnlockBits(textureData);
            canvas.UnlockBits(canvasData);
            return canvas;
        }

        unsafe void drawRectangle(int* ptrCanvas, int canvasWidth, int x, int y, int width, int height, int color)
        {
            for(int i = y; i < y + height; i++)
                for(int j = x; j < x + width; j++)
                {
                    *(ptrCanvas + i * canvasWidth + j) = color;
                }
        }


        int[] getPalette(Bitmap source)
        {
            int[] argbTable = new int[256];
            Color[] colors = source.Palette.Entries.ToArray();
            int i = 0;
            foreach(Color color in colors)
            {
                argbTable[i] = color.ToArgb();
                i++;
            }
            return argbTable;
        }

        public unsafe Bitmap GetScaledImage(Bitmap source, int width, int height)
        {
            Bitmap scaledImage = new Bitmap(width, height);
            int scaleWidth = scaledImage.Width / source.Width;
            int scaleHeight = scaledImage.Height / source.Height;

            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, 
                source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int* ptrSourceData = (int*)sourceData.Scan0.ToPointer();

            BitmapData scaledImageData = scaledImage.LockBits(new Rectangle(0, 0,
                scaledImage.Width, scaledImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            int* ptrScaledImage = (int*)scaledImageData.Scan0.ToPointer();


            for(int i = 0; i < source.Height; i++)
                for(int j = 0; j < source.Width; j++)
                {
                    int color = *(ptrSourceData + i * source.Width + j);
                    drawRectangle(ptrScaledImage, scaledImage.Width, j * scaleWidth,
                        i * scaleHeight, scaleWidth, scaleHeight, color);
                }

            source.UnlockBits(sourceData);
            scaledImage.UnlockBits(scaledImageData);
            return scaledImage;
        }


        public unsafe Bitmap DrawWalls(Player player, Bitmap canvas)
        {
            Bitmap bufferCanvas = new Bitmap(VerticalSegmentsAmount, HorizontalSegmentsAmount, PixelFormat.Format32bppArgb);
            BitmapData bufferCanvasData = bufferCanvas.LockBits(new Rectangle(0,
                0, bufferCanvas.Width, bufferCanvas.Height), ImageLockMode.WriteOnly, bufferCanvas.PixelFormat);
            int* ptrData = (int*)bufferCanvasData.Scan0.ToPointer();

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
                int[] wallTexturePalette = wallTexturesPalettes[map.Tiles[testY, testX].TextureID];
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
                           wallTexturePalette[*(ptrWallData + textureY * wallTextureData.Width + textureX)];
                    }
                    else if (y > floor)
                    {
                        *(ptrData + y * bufferCanvasData.Width + x) = floorColor.ToArgb();
                    }
                    else
                    {
                        *(ptrData + y * bufferCanvasData.Width + x) = ceilColor.ToArgb();
                    }
                }
                wallTexture.UnlockBits(wallTextureData);
            }
            bufferCanvas.UnlockBits(bufferCanvasData);
            canvas = bufferCanvas;// new Bitmap(bufferCanvas.Width, bufferCanvas.Height, bufferCanvasData.Stride, 
              //  PixelFormat.Format8bppIndexed, bufferCanvasData.Scan0);
            //canvas = new Bitmap(bufferCanvas, canvas.Width, canvas.Height);
            return canvas;
        }
    }
}
