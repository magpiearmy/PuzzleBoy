using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class MapLayer
    {
        public Tile[,] Tiles
        {
            get { return _tiles; }
        }
        private Tile[,] _tiles;
        public int Width { get { return _tiles.GetLength(0); } }
        public int Height { get { return _tiles.GetLength(1); } }

        public MapLayer(int width, int height)
        {
            _tiles = new Tile[width, height];
        }

        public void AddTile(int x, int y, Tile newTile)
        {
            _tiles[x, y] = newTile;
        }

        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }
    }
}
