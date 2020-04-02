using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    public delegate void StateEventHandler();

    interface IGameState
    {
        void Init();
        void Update(GameTime gameTime, KeyboardState keyboardState);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
