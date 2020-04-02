using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class Map
    {
        private const String MAP_SEPARATOR = "===";
        private IRenderer _renderer;

        public Tile[,] _tiles;
        private TileFactory _tileFactory;
        public List<TileSet> _tileSets;

        public TileSetManager TileSetMgr
        {
            get { return _tileSetMgr; }
        }
        private TileSetManager _tileSetMgr;

        public int LayerCount
        {
            get { return _layers.Count; }
        }
        public List<MapLayer> _layers;

        public Point StartPoint
        {
            get { return _startPoint; }
            set { _startPoint = value; }
        }
        private Point _startPoint;

        // Objects
        public List<Entity> Entities
        {
            get { return _entities; }
        }
        List<Entity> _entities = new List<Entity>();

        public int Width
        {
            get { return _tiles.GetLength(0); }
        }

        public int Height
        {
            get { return _tiles.GetLength(1); }
        }

        public Map()
        {
            _renderer = new MapRenderer(this);
            _tileSets = new List<TileSet>();
            _layers = new List<MapLayer>();
            _tileSetMgr = new TileSetManager();
        }

        public void LoadMap(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<String> lines = new List<String>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                String line = reader.ReadLine();
                width = line.Length;
                while (line != null && !line.Equals(MAP_SEPARATOR))
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            _tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // Load each tile.
                    char tileType = lines[y][x];
                    if (tileType == 'P') // Special case for player start position
                    {
                        _startPoint = new Point(x, y);
                        _tiles[x, y] = _tileFactory.parseTile('.');
                    }
                    else
                        _tiles[x, y] = _tileFactory.parseTile(tileType);

                    //Entity obj = GetObjectAtPos(new Point(x, y));
                }
            }
        }

        public void AddLayer(MapLayer newLayer)
        {
            _layers.Add(newLayer);
        }

        public MapLayer GetLayer(int layerIdx)
        {
            return _layers[layerIdx];
        }

        public Tile GetTileAt(int x, int y)
        {
            return _layers.First().GetTile(x, y);
        }

        public TileTerrain GetTileCollision(int x, int y)
        {
            Tile tile = _layers[0].GetTile(x, y);
            if (tile == null) return TileTerrain.Blocked;
            if (_tileSetMgr.GetTileType(tile.TileId).Terrain != TileTerrain.Passable)
                return _tileSetMgr.GetTileType(tile.TileId).Terrain;

            tile = _layers[1].GetTile(x, y);
            if (tile == null) return TileTerrain.Passable;
            return _tileSetMgr.GetTileType(tile.TileId).Terrain;
        }

        public IRenderer GetRenderer()
        {
            return _renderer;
        }
    }
}
