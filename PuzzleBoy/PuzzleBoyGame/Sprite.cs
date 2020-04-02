using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleBoy
{
    class Sprite4
    {
        private Texture2D img;
        private int spriteWidth, spriteHeight;

        public Sprite4(Texture2D img) {
            this.img = img;
            this.spriteWidth = img.Width / 4;
            this.spriteHeight = img.Height;
        }

        public void draw(SpriteBatch spriteBatch, Vector2 topLeft, Direction dir) {
            Rectangle rect = new Rectangle(spriteWidth * (int)dir, 0, spriteWidth, spriteHeight);
            Vector2 origin = new Vector2(0, spriteHeight - Tile.Height);
            spriteBatch.Draw(img, topLeft, rect, Color.White, 0, origin, 1.0f, SpriteEffects.None, 0);
        }

    }
}
