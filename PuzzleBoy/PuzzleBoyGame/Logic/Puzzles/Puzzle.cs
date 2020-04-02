using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace PuzzleBoy
{
    public enum PuzzleState
    {
        eDefault,
        eCompleted,
        eFailed,
        eRouteConnected
    }

    public abstract class Puzzle : IGameState
    {
        protected PuzzlePiece[,] _pieces;
        protected int _puzzleId;
        public event StateEventHandler _exitEvent;

        public bool IsPuzzleComplete
        {
            get
            {
                return (_state == PuzzleState.eCompleted);
            }
        }

        public bool PuzzleFailed
        {
            get
            {
                if (_state == PuzzleState.eFailed)
                    return true;
                else
                    return false;
            }
        }

        public bool IsFinished
        {
            get { return _finished; }
        }
        protected bool _finished;

        public PuzzleState PuzzleState
        {
            get { return _state; }
        }
        protected PuzzleState _state;

        protected static readonly Vector2 InvalidPosition = new Vector2(-1, -1);
        protected Vector2 selectorPos;
        protected Vector2 startTile = InvalidPosition;
        protected Vector2 endTile = InvalidPosition;

        // Stores the elapsed time since the start of the puzzle
        protected TimeSpan timeSinceStart;
        protected TimeSpan timeSinceCompletion;
        protected const float completionWaitTime = 2000.0f;

        // Puzzle Textures
        protected Texture2D backgroundTexture;
        protected Texture2D selectorTexture;

        // Pipe Textures
        protected Texture2D pHorizTexture;
        protected Texture2D pVertTexture;
        protected Texture2D pTopLeftTexture;
        protected Texture2D pTopRightTexture;
        protected Texture2D pBottomLeftTexture;
        protected Texture2D pBottomRightTexture;

        protected Rectangle puzzleBounds;
        protected float leftMargin = 0.0f;
        protected float topMargin = 0.0f;

        protected KeyboardState oldKeyState;

        public int Width
        {
            get { return _pieces.GetLength(0); }
        }

        public int Height
        {
            get { return _pieces.GetLength(1); }
        }

        public ContentManager Content
        {
            get { return content; }
        }
        protected ContentManager content;

        #region Loading

        public virtual void Init()
        {
            foreach (PuzzlePiece pipe in _pieces)
            {
                pipe.Reset();
            }
        }

        protected void LoadBounds(Viewport viewport)
        {
            float trueWidth = Width * PuzzlePiece.Width;
            float trueHeight = Height * PuzzlePiece.Height;

            if (trueWidth < viewport.Width)
            {
                leftMargin = (viewport.Width - (Width * PuzzlePiece.Width)) / 2;
            }
            if (trueHeight < viewport.Height)
            {
                topMargin = (viewport.Height - (Height * PuzzlePiece.Height)) / 2;
            }

            puzzleBounds = new Rectangle((int)leftMargin, (int)topMargin, (int)trueWidth, (int)trueHeight);
        }

        protected virtual void LoadTextures()
        {
        }

        protected void LoadPieces(Stream fileStream)
        {
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            _pieces = new PuzzlePiece[width, lines.Count];

            // Loop over every Orb position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // Load each tile.
                    char tileType = lines[y][x];
                    _pieces[x, y] = ParsePiece(tileType, x, y);
                }
            }

            if (startTile == InvalidPosition)
                throw new NotSupportedException("A puzzle must have a starting point.");
            if (endTile == InvalidPosition)
                throw new NotSupportedException("A puzzle must have an exit.");
        }

        protected virtual PuzzlePiece ParsePiece(char orbType, int x, int y)
        {
            return null;
        }

        protected PuzzlePiece LoadPiece(Color color, PipeType type)
        {
            return new PuzzlePiece(color, type);
        }

        protected PuzzlePiece LoadStartPiece(int x, int y)
        {
            startTile = new Vector2(x, y);
            return LoadPiece(Color.PaleGreen, PipeType.pHoriz);
        }

        protected PuzzlePiece LoadExitPiece(int x, int y)
        {
            endTile = new Vector2(x, y);
            return LoadPiece(Color.FromNonPremultiplied(255, 0, 0, 255), PipeType.pHoriz);
        }

        #endregion Loading

        #region Updating

        public virtual void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            //KeyboardState newKeyState = keyboardState;

            //if (state == PuzzleState.eCompleted || state == PuzzleState.eFailed)
            //{
            //    if (!(completionWaitTime >= timeSinceCompletion.TotalMilliseconds))
            //    {
            //        finished = true;
            //    }
            //    else
            //    {
            //        timeSinceCompletion += gameTime.ElapsedGameTime;
            //    }
            //}
            //else
            //{
            //    if (state != PuzzleState.eRouteConnected)
            //    {
            //        CheckPathSuccess();

            //        UpdateInput(newKeyState);
            //    }

            //    //UpdateWaterFlow(gameTime);

            //    timeSinceStart += gameTime.ElapsedGameTime;

            //    oldKeyState = newKeyState;
            //}

        }

        protected void UpdateInput(KeyboardState newKeyState)
        {
            if (newKeyState.IsKeyDown(Keys.Z) && !oldKeyState.IsKeyDown(Keys.Z))
            {
                if (!_pieces[(int)selectorPos.X, (int)selectorPos.Y].isFilled)
                {
                    _pieces[(int)selectorPos.X, (int)selectorPos.Y].Rotate();
                }
            }

            if (newKeyState.IsKeyDown(Keys.Right) && !oldKeyState.IsKeyDown(Keys.Right))
            {
                selectorPos.X++;
            }
            else if (newKeyState.IsKeyDown(Keys.Left) && !oldKeyState.IsKeyDown(Keys.Left))
            {
                selectorPos.X--;
            }
            else if (newKeyState.IsKeyDown(Keys.Up) && !oldKeyState.IsKeyDown(Keys.Up))
            {
                selectorPos.Y--;
            }
            else if (newKeyState.IsKeyDown(Keys.Down) && !oldKeyState.IsKeyDown(Keys.Down))
            {
                selectorPos.Y++;
            }

            if (newKeyState.IsKeyDown(Keys.Escape))
            {
                _state = PuzzleState.eFailed;
                _finished = true;
            }

            ClampSelector();
        }

        protected void ClampSelector()
        {
            if (selectorPos.X < 0)
                selectorPos.X = 0;

            if (selectorPos.X > Width - 1)
                selectorPos.X = Width - 1;

            if (selectorPos.Y < 0)
                selectorPos.Y = 0;

            if (selectorPos.Y > Height - 1)
                selectorPos.Y = Height - 1;
        }

        protected static Vector2 GetNextTilePos(int direction, Vector2 currentTile)
        {
            switch (direction)
            {
                case 1:
                    return new Vector2(currentTile.X, currentTile.Y - 1);
                case 2:
                    return new Vector2(currentTile.X + 1, currentTile.Y);
                case 3:
                    return new Vector2(currentTile.X, currentTile.Y + 1);
                case 4:
                    return new Vector2(currentTile.X - 1, currentTile.Y);
                default:
                    return new Vector2(currentTile.X + 1, currentTile.Y);
            }
        }

        public void Reset()
        {
            Init();
        }

        #endregion Updating

        #region Drawing

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            //spriteBatch.Draw(backgroundTexture, new Vector2(0,0), Color.White);
            Rectangle sourceRect = new Rectangle(0, 0, puzzleBounds.Width + 1, puzzleBounds.Height + 1);
            spriteBatch.Draw(backgroundTexture, puzzleBounds, sourceRect, Color.White);

            spriteBatch.End();

            spriteBatch.Begin();
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = null;
                    switch (_pieces[x, y].type)
                    {
                        case PipeType.pVert:
                            texture = pVertTexture;
                            break;
                        case PipeType.pHoriz:
                            texture = pHorizTexture;
                            break;
                        case PipeType.pBottomLeftCorner:
                            texture = pBottomLeftTexture;
                            break;
                        case PipeType.pBottomRightCorner:
                            texture = pBottomRightTexture;
                            break;
                        case PipeType.pTopLeftCorner:
                            texture = pTopLeftTexture;
                            break;
                        case PipeType.pTopRightCorner:
                            texture = pTopRightTexture;
                            break;
                    }

                    if (texture != null)
                    {
                        Color color = _pieces[x, y].color;
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * PuzzlePiece.Size;
                        position += new Vector2(leftMargin, topMargin);
                        spriteBatch.Draw(texture, position, color);
                    }
                }
            }


            // Draw selector
            float selectorX = (selectorPos.X) * PuzzlePiece.Width;
            float selectorY = (selectorPos.Y) * PuzzlePiece.Height;
            spriteBatch.Draw(selectorTexture, new Vector2(selectorX + leftMargin, selectorY + topMargin), Color.White);

            spriteBatch.End();
        }

        #endregion Drawing


        public bool IsExiting() { return false; }

    }
}