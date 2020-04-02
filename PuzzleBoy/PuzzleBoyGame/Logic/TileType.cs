using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class TileType
    {
        private int _id;

        public Texture2D Texture
        {
            get { return _texture; }
        }
        private Texture2D _texture;

        public TileTerrain Terrain
        {
            get { return _terrain; }
            set { _terrain = value; }
        }
        private TileTerrain _terrain;

        /**
         * Constructor
         */
        public TileType(int id, TileTerrain terrain)
        {
            _id = id;
            _terrain = terrain;
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
        }
    }
}
