using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class TileSet
    {
        String _name;
        int _tileCount = 0;
        bool _isCollection = false;

        public int FirstGid
        {
            get { return _firstGid; }
        }
        int _firstGid;

        Texture2D _tilesetImage;
        List<TileType> _tileTypes;

        public TileSet(String name, int firstGid, int tileCount, bool isCollection)
        {
            _name = name;
            _firstGid = firstGid;
            _tileCount = tileCount;
            _isCollection = isCollection;

            _tileTypes = new List<TileType>();
        }

        public bool HasTileWithGid(int gid)
        {
            return (gid >= _firstGid && gid <= _firstGid + _tileCount);
        }

        public void AddTileType(TileType tileType)
        {
            _tileTypes.Add(tileType);
            if (_isCollection) _tileCount++;
        }

        public TileType GetTileType(int tileId)
        {
            int localId = tileId - _firstGid;
            if (_tileTypes.Count <= localId) return new TileType(-1, TileTerrain.Passable);
            return _tileTypes[localId];
        }

        public void SetImage(Texture2D texture)
        {
            if (_isCollection) throw new Exception("TileSet not expecting a tileset image");
            _tilesetImage = texture;
            _tileCount = (_tilesetImage.Width / Tile.Width) * (_tilesetImage.Height / Tile.Height);
        }

        public void DrawTile(int gid, SpriteBatch spriteBatch, Vector2 position)
        {
            int localId = gid - _firstGid;
            if (_isCollection)
            {
                Texture2D texture = _tileTypes[localId].Texture;
                position.Y -= texture.Height - Tile.Height;
                spriteBatch.Draw(texture, position, Color.White);
            }
            else
            {
                int tilesPerRow = _tilesetImage.Width / Tile.Width;
                int col = localId % tilesPerRow;
                int row = localId / tilesPerRow;
                Rectangle srcRect = new Rectangle(col * Tile.Width, row * Tile.Height, Tile.Width, Tile.Height);
                spriteBatch.Draw(_tilesetImage, position, srcRect, Color.White);
            }   
        }
    }
}
