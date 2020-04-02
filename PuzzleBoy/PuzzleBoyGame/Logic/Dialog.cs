using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    public class Dialog : IGameState
    {
        static Texture2D boxImg = null;
        static SpriteFont _font;

        String _text;

        public Dialog(ContentManager content, String text)
        {
            _text = text;
            if (boxImg == null)
            {
                boxImg = content.Load<Texture2D>("Overlays/DialogBox");
            }
            _font = content.Load<SpriteFont>("Fonts/Hud");
        }

        public void Init()
        {
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.X))
            {
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw dialog box background
            spriteBatch.Begin();
            spriteBatch.Draw(boxImg, new Vector2(0, 256), Color.White);
            spriteBatch.DrawString(_font, _text, new Vector2(16, 272), Color.Black);
            spriteBatch.End();
        }

        public void SetText(String text)
        {
            _text = text;
        }
    }
}
