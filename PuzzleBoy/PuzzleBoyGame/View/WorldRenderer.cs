using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class WorldRenderer : IRenderer
    {
        private World _world;
        private float _cameraPositionX;
        private float _cameraPositionY;

        public WorldRenderer(World world)
        {
            _world = world;
        }

        public void LoadContent(ContentManager content)
        {

        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            // Create camera transform
            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            Matrix cameraTransform = Matrix.CreateTranslation(-_cameraPositionX, -_cameraPositionY, 0.0f);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);

            DrawMapLayer(0, spriteBatch);

            // The middle layer is drawn line by line along with the entities
            MapLayer objectLayer = _world.Map.GetLayer(1);
            for (int y = 0; y < _world.Map.Height; ++y)
            {
                for (int x = 0; x < _world.Map.Width; ++x)
                {
                    // If there is a visible tile in that position, draw it!
                    Tile tile = objectLayer.GetTile(x, y);
                    if (tile != null)
                        _world.Map.TileSetMgr.GetTileSetContainingTileId(tile.TileId)
                                       .DrawTile(tile.TileId, spriteBatch, new Vector2(x, y) * Tile.Size);

                }

                // Draw any entities appearing on this row
                foreach (Entity entity in _world.Map.Entities)
                {
                    int entityY = (int)(entity.Position.Y - (entity.Position.Y % Tile.Height)) / Tile.Height;
                    if (entityY == y) entity.Draw(spriteBatch);
                }
            }

            DrawMapLayer(2, spriteBatch);

            //foreach (PuzzleTile puzzleTile in puzzleTiles)
            //{
            //    //Draw in screen space.
            //    Texture2D texture = puzzleTile.puzzleCompleted ? puzzleCompleteTexture : puzzleIncompleteTexture;
            //    Vector2 thisTilePos = puzzleTile.position * Tile.Size;
            //    spriteBatch.Draw(texture, thisTilePos, Color.White);
            //}

            spriteBatch.End();
        }

        public void DrawMapLayer(int layerIdx, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _world.Map.Height; ++y)
            {
                for (int x = 0; x < _world.Map.Width; ++x)
                {
                    // If there is a visible tile in that position, draw it!
                    Tile tile = _world.Map.GetLayer(layerIdx).GetTile(x, y);
                    if (tile != null)
                        _world.Map.TileSetMgr.GetTileSetContainingTileId(tile.TileId)
                                       .DrawTile(tile.TileId, spriteBatch, new Vector2(x, y) * Tile.Size);

                }
            }
        }

        private void ScrollCamera(Viewport viewport)
        {
            const float ViewMargin = 0.5f;

            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * ViewMargin;
            float marginLeft = _cameraPositionX + marginWidth;
            float marginRight = _cameraPositionX + viewport.Width - marginWidth;

            const float topMargin = 0.5f;
            const float bottomMargin = 0.5f;
            float marginTop = _cameraPositionY + viewport.Height * topMargin;
            float marginBottom = _cameraPositionY + viewport.Height - (viewport.Height * bottomMargin);

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovementX = 0.0f;
            if (_world.Player.Position.X < marginLeft)
                cameraMovementX = _world.Player.Position.X - marginLeft;
            else if (_world.Player.Position.X > marginRight)
                cameraMovementX = _world.Player.Position.X - marginRight;

            float cameraMovementY = 0.0f;
            if (_world.Player.Position.Y < marginTop) //above the top margin
                cameraMovementY = _world.Player.Position.Y - marginTop;
            else if (_world.Player.Position.Y > marginBottom) //below the bottom margin  
                cameraMovementY = _world.Player.Position.Y - marginBottom;

            // Update the camera position, but prevent scrolling off the boundaries of the level.
            float maxCameraPositionX = Tile.Width * _world.Map.Width - viewport.Width;
            float maxCameraPositionY = Tile.Height * _world.Map.Height - viewport.Height;
            _cameraPositionX = MathHelper.Clamp(_cameraPositionX + cameraMovementX, 0.0f, maxCameraPositionX);
            _cameraPositionY = MathHelper.Clamp(_cameraPositionY + cameraMovementY, 0.0f, maxCameraPositionY);
        }
    }
}
