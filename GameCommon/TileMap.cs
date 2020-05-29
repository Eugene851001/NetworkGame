using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class TileMap
    {
        public int Height { get; }
        public int Width { get; }

        public Vector2D[] SpawnPoints { get; }

        public Tile[,] Tiles;

        public TileMap(int width, int height, Vector2D[] spawnPoints)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[height, width];
            SpawnPoints = spawnPoints;
        }

        public TileMap(Tile[,] tiles)
        {
            SpawnPoints = null;
            Height = tiles.GetUpperBound(0) + 1;
            Width = tiles.Length / Height;
            Tiles = tiles;
        }

        public TileMap(int width, int height, string map, Vector2D[] spawnPoints)
        {
            SpawnPoints = spawnPoints;
            Width = width;
            Height = height;
            Tiles = new Tile[height, width];
            for(int i = 0; i < map.Length; i++)
            {
                if (map[i] == '#')
                    Tiles[i / width, i % width] = new Tile() { IsSolid = true, TextureID = 1};
                else
                    Tiles[i / width, i % width] = new Tile() { IsSolid = false};
            }
        }

        public bool IsSolid(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return Tiles[y, x].IsSolid;
            else
                return true;
        }
    }
}
