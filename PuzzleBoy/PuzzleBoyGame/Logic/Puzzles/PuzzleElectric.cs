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
    class PuzzleElectric : Puzzle
    {
        const float elecFlowDelay = 6000;
        float elecFlowSpeed;
        float timeSinceElecMoved;
        int elecFlowDirection;
        bool elecFlowing;
        Vector2 currentElecPos;

        public PuzzleElectric(IServiceProvider serviceProvider, Stream fileStream, Viewport viewport, int puzzleId)
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
            backgroundTexture = Content.Load<Texture2D>("Puzzles/Pieces/ElectricBackground");
            selectorTexture = Content.Load<Texture2D>("Puzzles/Pieces/Selector");

            pHorizTexture = Content.Load<Texture2D>("Puzzles/Pieces/WireH");
            pVertTexture = Content.Load<Texture2D>("Puzzles/Pieces/WireV");
            pTopLeftTexture = Content.Load<Texture2D>("Puzzles/Pieces/WireTL");
            pTopRightTexture = Content.Load<Texture2D>("Puzzles/Pieces/WireTR");
            pBottomLeftTexture = Content.Load<Texture2D>("Puzzles/Pieces/WireBL");
            pBottomRightTexture = Content.Load<Texture2D>("Puzzles/Pieces/WireBR"); 
        }

        public new void Init()
        {
            base.Init();

            _finished = false;
            _state = PuzzleState.eDefault;
            elecFlowing = false;
            elecFlowDirection = 2;
            elecFlowSpeed = 50;
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

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
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
                    elecFlowSpeed = 300;

                    if (!elecFlowing)
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
            if (!elecFlowing)
            {
                // If the time delay has passed, begin the flow of water
                if (timeSinceStart.TotalMilliseconds >= elecFlowDelay)
                {
                    elecFlowing = true;
                    timeSinceElecMoved = 0.0f;
                    currentElecPos = startTile;
                }
            }
            else
            {
                timeSinceElecMoved += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                float timeForWaterMovement = PuzzlePiece.Width / elecFlowSpeed;
                if (timeSinceElecMoved >= timeForWaterMovement * 1000)
                {
                    currentElecPos = GetNextTilePos(elecFlowDirection, currentElecPos);
                    if (currentElecPos == endTile)
                    {
                        elecFlowing = false;
                        _state = PuzzleState.eCompleted;


                    }
                    else
                    {
                        if (currentElecPos.X < 0 || currentElecPos.X > Width - 1 || currentElecPos.Y < 0 || currentElecPos.Y > Height - 1)
                        {
                            elecFlowDirection = 0;
                        }
                        else
                        {
                            elecFlowDirection = _pieces[(int)currentElecPos.X, (int)currentElecPos.Y].GetDirection(elecFlowDirection);
                        }
                    }
                    timeSinceElecMoved = 0.0f;
                }

                // Check if the water flow is still valid
                if (elecFlowDirection == 0)
                {
                    _state = PuzzleState.eFailed;
                }
                else
                {
                    _pieces[(int)currentElecPos.X, (int)currentElecPos.Y].Fill(Color.Gold);
                }
            }
        }

        // Initiate the water flow
        private void BeginFlow()
        {
            elecFlowing = true;
            timeSinceElecMoved = 0.0f;
            currentElecPos = startTile;
        }
    }
}
