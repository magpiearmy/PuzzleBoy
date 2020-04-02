using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace PuzzleBoy
{
    class Boat
    {
        Texture2D texture;
        World level;

        // Boats use an electric puzzle to power them
        public int PuzzleId
        {
            get { return puzzleId; }
        }
        private int puzzleId;

        public int RequiredLevel
        {
            get { return requiredLevel; }
            set { requiredLevel = value; }
        }
        private int requiredLevel;

        Vector2 origin;
        Vector2 position;
        bool playerInBoat;
        bool moving;
        Direction direction;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(position.X - origin.X);
                int top = (int)Math.Round(position.Y - origin.Y);

                return new Rectangle(left, top, texture.Width, texture.Height);
            }
        }

        public Boat(int x, int y, World level, int puzzleId)
        {
            this.level = level;
            this.puzzleId = puzzleId;
            direction = Direction.dDown;

            Vector2 start = RectangleExtensions.GetCenter(level.GetTileBounds(x, y));
            position = start;

            playerInBoat = false;
            moving = false;

            LoadContent();
        }

        public void LoadContent()
        {
            texture = level.Content.Load<Texture2D>("Tiles/Boat");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
        }

        public void Update(GameTime gameTime)
        {
            if (moving)
            {
                if (playerInBoat)
                {

                }

                float moveSpeed = 0.05f;
                float moveDistance = moveSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                position.Y += moveDistance;                
            }

            if(moving)
                CheckCollisions();
        }

        public void Start()
        {
            moving = true;
            position.Y += 32;
        }

        private void CheckCollisions()
        {
            Rectangle bounds = BoundingRectangle;

            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // For each potentially colliding tile...
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileTerrain collision = level.GetTileCollision(x, y);
                    if (collision != TileTerrain.Water)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = level.GetTileBounds(x, y);
                        bounds = HandleCollision(bounds, collision, tileBounds);
                    }
                }
            }
        }


        private Rectangle HandleCollision(Rectangle bounds, TileTerrain collision, Rectangle tileBounds)
        {
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                if (collision == TileTerrain.Passable) // Ignore platforms.  
                {
                    // Resolve the collision along the X axis.
                    if (direction == Direction.dLeft || direction == Direction.dRight)
                        position = new Vector2(position.X + depth.X, position.Y);
                    else
                        position = new Vector2(position.X, position.Y + depth.Y);

                    // Perform further collisions with the new bounds.  
                    bounds = BoundingRectangle;
                }
            }

            return bounds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
