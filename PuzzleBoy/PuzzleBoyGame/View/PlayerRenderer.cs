using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class PlayerRenderer : IRenderer
    {
        Player _player;

        Texture2D _textureFront;
        Texture2D _textureBack;
        Texture2D _textureLeft;
        Texture2D _textureRight;
        Vector2 _origin;

        Sprite4 sprite;

        public PlayerRenderer(Player player)
        {
            _player = player;
        }

        public void LoadContent(ContentManager content)
        {
            _textureFront   = content.Load<Texture2D>("Sprites/PlayerFront");
            _textureBack    = content.Load<Texture2D>("Sprites/PlayerBack");
            _textureLeft    = content.Load<Texture2D>("Sprites/PlayerLeft");
            _textureRight   = content.Load<Texture2D>("Sprites/PlayerRight");
            sprite = new Sprite4(content.Load<Texture2D>("Sprites/PlayerSprite"));
            _origin = new Vector2(0, _textureBack.Height-Tile.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.draw(spriteBatch, _player.Position, _player.Direction);
            /*Texture2D texture = null;
            switch (_player.Direction)
            {
                case Direction.dUp:
                    texture = _textureBack;
                    break;
                case Direction.dDown:
                    texture = _textureFront;
                    break;
                case Direction.dLeft:
                    texture = _textureLeft;
                    break;
                case Direction.dRight:
                    texture = _textureRight;
                    break;
            }
            spriteBatch.Draw(texture, _player.Position, null, Color.White, 0.0f, _origin, 1.0f, SpriteEffects.None, 0.0f);*/
        }
    }
}
