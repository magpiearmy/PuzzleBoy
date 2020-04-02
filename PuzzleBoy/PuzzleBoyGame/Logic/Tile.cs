using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PuzzleBoy
{
    class Tile
    {
        public int _tileId;
        public int TileId
        {
            get { return _tileId; }
        }

        public const int Width = 32;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(int tileId)
        {
            _tileId = tileId;
        }
    }
}
