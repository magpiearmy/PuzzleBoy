using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PuzzleBoy
{
    class PuzzleSewer : Puzzle
    {
        const float sludgeFlowDelay = 4000;
        float sludgeFlowSpeed;
        float timeSinceSludgeMoved;
        int sludgeFlowDirection;
        bool sludgeFlowing;
        Vector2 currentSludgePos;

        public PuzzleSewer(IServiceProvider serviceProvider, Stream fileStream, Viewport viewport, int puzzleId)
        {
            this._puzzleId = puzzleId;

            content = new ContentManager(serviceProvider, "Content");

            LoadTextures();
            LoadPieces(fileStream);
            LoadBounds(viewport);

            Init();

            selectorPos = new Vector2(0, 0);

            oldKeyState = Keyboard.GetState();
        }

        protected override void LoadTextures()
        {
            backgroundTexture = Content.Load<Texture2D>("Puzzles/Pieces/SewerBackground");
            selectorTexture = Content.Load<Texture2D>("Puzzles/Pieces/Selector");

            pHorizTexture = Content.Load<Texture2D>("Puzzles/Pieces/TunnelH");
            pVertTexture = Content.Load<Texture2D>("Puzzles/Pieces/TunnelV");
            pTopLeftTexture = Content.Load<Texture2D>("Puzzles/Pieces/TunnelTL");
            pTopRightTexture = Content.Load<Texture2D>("Puzzles/Pieces/TunnelTR");
            pBottomLeftTexture = Content.Load<Texture2D>("Puzzles/Pieces/TunnelBL");
            pBottomRightTexture = Content.Load<Texture2D>("Puzzles/Pieces/TunnelBR");

        }

        public void Init()
        {
            base.Init();

            _finished = false;
            _state = PuzzleState.eDefault;
            sludgeFlowing = false;
            sludgeFlowDirection = 2;
            sludgeFlowSpeed = 15;
            timeSinceStart = TimeSpan.Zero;
            timeSinceCompletion = TimeSpan.Zero;
        }

        protected override PuzzlePiece ParsePiece(char orbType, int x, int y)
        {
            switch (orbType)
            {
                case 'S':
                    return LoadStartPiece(x, y);

                case 'E':
                    return LoadExitPiece(x, y);

                case '-':
                    return LoadPiece(Color.White, PipeType.pHoriz);

                case '|':
                    return LoadPiece(Color.White, PipeType.pVert);

                case '1':
                    return LoadPiece(Color.White, PipeType.pTopLeftCorner);

                case '2':
                    return LoadPiece(Color.White, PipeType.pTopRightCorner);

                case '3':
                    return LoadPiece(Color.White, PipeType.pBottomLeftCorner);

                case '4':
                    return LoadPiece(Color.White, PipeType.pBottomRightCorner);

                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", orbType, x, y));
            }
        }

        public override void Update(GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyboardState)
        {
            base.Update(gameTime, keyboardState);

            KeyboardState newKeyState = keyboardState;

            if (_state == PuzzleState.eCompleted || _state == PuzzleState.eFailed)
            {
                if (!(completionWaitTime >= timeSinceCompletion.TotalMilliseconds))
                {
                    _finished = true;
                }
                else
                {
                    timeSinceCompletion += gameTime.ElapsedGameTime;
                }
            }
            else
            {
                if (_state != PuzzleState.eRouteConnected)
                {
                    CheckPathSuccess();

                    UpdateInput(newKeyState);
                }

                UpdateFlow(gameTime);

                timeSinceStart += gameTime.ElapsedGameTime;

                oldKeyState = newKeyState;
            }
        }

        private void CheckPathSuccess()
        {
            // Starting point is outside the grid, to the left of the start tile
            Vector2 currentTile = new Vector2(-1, startTile.Y);
            int currentDirection = 2;
            Vector2 nextTile;

            // Loop until direction is 0 (dead end) or a complete path is found
            while (currentDirection != 0 && _state != PuzzleState.eRouteConnected)
            {
                if (currentTile == endTile)
                {
                    _state = PuzzleState.eRouteConnected;
                    sludgeFlowSpeed = 300;
                    if (!sludgeFlowing)
                        BeginFlow();
                }
                else
                {
                    nextTile = GetNextTilePos(currentDirection, currentTile);

                    if (nextTile.X < 0 || nextTile.X > Width - 1 || nextTile.Y < 0 || nextTile.Y > Height - 1)
                    {
                        currentDirection = 0;
                    }
                    else
                    {
                        currentDirection = _pieces[(int)nextTile.X, (int)nextTile.Y].GetDirection(currentDirection);

                        currentTile = nextTile;
                    }
                }
            }
        }

        protected void UpdateFlow(GameTime gameTime)
        {
            if (!sludgeFlowing)
            {
                // If the time delay has passed, begin the flow of water
                if (timeSinceStart.TotalMilliseconds >= sludgeFlowDelay)
                {
                    sludgeFlowing = true;
                    timeSinceSludgeMoved = 0.0f;
                    currentSludgePos = startTile;
                }
            }
            else
            {
                timeSinceSludgeMoved += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                float timeForWaterMovement = PuzzlePiece.Width / sludgeFlowSpeed;
                if (timeSinceSludgeMoved >= timeForWaterMovement * 1000)
                {
                    currentSludgePos = GetNextTilePos(sludgeFlowDirection, currentSludgePos);
                    if (currentSludgePos == endTile)
                    {
                        sludgeFlowing = false;
                        _state = PuzzleState.eCompleted;
                    }
                    else
                    {
                        if (currentSludgePos.X < 0 || currentSludgePos.X > Width - 1 || currentSludgePos.Y < 0 || currentSludgePos.Y > Height - 1)
                        {
                            sludgeFlowDirection = 0;
                        }
                        else
                        {
                            sludgeFlowDirection = _pieces[(int)currentSludgePos.X, (int)currentSludgePos.Y].GetDirection(sludgeFlowDirection);
                        }
                    }
                    timeSinceSludgeMoved = 0.0f;
                }

                // Check if the water flow is still valid
                if (sludgeFlowDirection == 0)
                {
                    _state = PuzzleState.eFailed;
                }
                else
                {
                    _pieces[(int)currentSludgePos.X, (int)currentSludgePos.Y].Fill(Color.Green);
                }
            }
        }

        private void BeginFlow()
        {
            sludgeFlowing = true;
            timeSinceSludgeMoved = 0.0f;
            currentSludgePos = startTile;
        }
    }
}
