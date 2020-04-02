using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class MapRenderer : IRenderer
    {
        private Map _map;

        public MapRenderer(Map map)
        {
            _map = map;
        }

        public void LoadContent(ContentManager content)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
