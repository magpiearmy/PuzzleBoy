using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace PuzzleBoy
{
    enum GameState
    {
        Level,
        Puzzle,
        PauseMenu,
        Dialog
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        SpriteFont _hudFont;

        Texture2D _puzzleCompleteOverlay;
        Texture2D _puzzleFailedOverlay;

        private KeyboardState _keyboardState;
        private GameState _gameState;

        private World _world;
        private DialogManager _dialogMgr;

        private int _levelIndex = -1;
        private const int _numberOfLevels = 1;
        private List<Puzzle> _puzzles = new List<Puzzle>();
        private int _currentPuzzleNum;

        private Stack<IGameState> _states = new Stack<IGameState>();

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = Tile.Width * 20;
            _graphics.PreferredBackBufferHeight = Tile.Height * 16;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _dialogMgr = new DialogManager(this);

            LoadNextLevel();

            LoadPuzzles();

            _world.Init();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            _puzzleCompleteOverlay = Content.Load<Texture2D>("Overlays/PuzzleComplete");
            _puzzleFailedOverlay = Content.Load<Texture2D>("Overlays/PuzzleFailed");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void LoadNextLevel()
        {
            // move to the next level
            _levelIndex = (_levelIndex + 1) % _numberOfLevels;

            _gameState = GameState.Level;

            // Unloads the content for the current level before loading the next one.
            if (_world != null)
                _world.Dispose();

            // Load the level
            string levelPath = string.Format("Content/Levels/{0}.txt", _levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                _world = new World(this, Services, fileStream, _levelIndex);
        }

        private void LoadPuzzles()
        {
            foreach (PuzzleTile pTile in _world.PuzzleTiles)
            {
                string puzzlePath = string.Format("Content/Puzzles/{0}.txt", pTile.id);

                if (pTile.type == PuzzleType.eWaterPuzzle)
                {
                    using (Stream fileStream = TitleContainer.OpenStream(puzzlePath))
                        _puzzles.Add(new PuzzleWater(Services, fileStream, GraphicsDevice.Viewport, pTile.id));
                }
                else if (pTile.type == PuzzleType.eElectricPuzzle)
                {
                    using (Stream fileStream = TitleContainer.OpenStream(puzzlePath))
                        _puzzles.Add(new PuzzleElectric(Services, fileStream, GraphicsDevice.Viewport, pTile.id));
                }
                else if (pTile.type == PuzzleType.eSewerPuzzle)
                {
                    using (Stream fileStream = TitleContainer.OpenStream(puzzlePath))
                        _puzzles.Add(new PuzzleSewer(Services, fileStream, GraphicsDevice.Viewport, pTile.id));
                }
            }

            _currentPuzzleNum = 0;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            _keyboardState = Keyboard.GetState();

            //if (_world.Player.InPuzzleMode)
            //{
            //    _currentPuzzleNum = _world.Player.CurrentPuzzleNum;
            //    _gameState = GameState.Puzzle;
            //}
            //else if (_world.inCharScreen)
            //{
            //    _gameState = GameState.PauseMenu;
            //}

            switch (_gameState)
            {
                case GameState.Level:

                    _world.Update(gameTime, _keyboardState);

                    break;

                case GameState.Puzzle:

                    _puzzles[_currentPuzzleNum].Update(gameTime, _keyboardState);
                    
                    if (_puzzles[_currentPuzzleNum].IsFinished)
                    {
                        if(_puzzles[_currentPuzzleNum].IsPuzzleComplete)
                        {
                            _world.PuzzleCompleted(_currentPuzzleNum);
                        }
                        else if (_puzzles[_currentPuzzleNum].PuzzleFailed)
                        {
                            _world.PuzzleFailed(_currentPuzzleNum);
                            _puzzles[_currentPuzzleNum].Reset();
                        }
                        _gameState = GameState.Level;
                    }

                    break;

                case GameState.PauseMenu:
                    if(_keyboardState.IsKeyUp(Keys.S))
                    {
                        _world.inCharScreen = false;
                        _gameState = GameState.Level;
                    }
                    break;
                
                case GameState.Dialog:
                    if (_keyboardState.IsKeyDown(Keys.X))
                    {
                        _gameState = GameState.Level;
                    }
                    else
                    {
                        _dialogMgr.GetDialog().Update(gameTime, _keyboardState);
                    }
                    break;
            }

            base.Update(gameTime);
        }

        public void InitDialog(String text)
        {
            _gameState = GameState.Dialog;
            _dialogMgr.InitDialog(text);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);

            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;

            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            switch (_gameState)
            {
                case GameState.Level:

                    _world.Draw(gameTime, _spriteBatch);
                    DrawHud();

                    break;
                case GameState.Puzzle:

                    Puzzle thisPuzzle = _puzzles[_currentPuzzleNum];

                    thisPuzzle.Draw(gameTime, _spriteBatch);

                    if (!thisPuzzle.IsFinished)
                    {
                        if (thisPuzzle.IsPuzzleComplete)
                        {
                            _spriteBatch.Begin();
                            Vector2 overlaySize = new Vector2(_puzzleCompleteOverlay.Width, _puzzleCompleteOverlay.Height);
                            _spriteBatch.Draw(_puzzleCompleteOverlay, center - overlaySize / 2, Color.White);
                            _spriteBatch.End();
                        }
                        else if (thisPuzzle.PuzzleFailed)
                        {
                            _spriteBatch.Begin();
                            Vector2 overlaySize = new Vector2(_puzzleFailedOverlay.Width, _puzzleFailedOverlay.Height);
                            _spriteBatch.Draw(_puzzleFailedOverlay, center - overlaySize / 2, Color.White);
                            _spriteBatch.End();
                        }
                    }

                    break;
                case GameState.PauseMenu:

                    _spriteBatch.Begin();

                    Vector2 levelLocation = new Vector2(20.0f, 20.0f);
                    Vector2 expLocation = new Vector2(20.0f, 50.0f);

                    string charLevel = "Lvl: " + _world.Player.AbilityLevel.ToString();
                    string expToLevelUp = "EXP to level up: " + _world.Player.ExpToNextLevel.ToString();

                    DrawShadowedString(_hudFont, charLevel, levelLocation, Color.White, 1.8f);
                    DrawShadowedString(_hudFont, expToLevelUp, expLocation, Color.White, 1.8f);

                    _spriteBatch.End();

                    break;
                case GameState.Dialog:
                    _world.Draw(gameTime, _spriteBatch);
                    _dialogMgr.GetDialog().Draw(gameTime, _spriteBatch);
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            _spriteBatch.Begin();

            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 currentLevelLocation = new Vector2(5.0f, 0.0f);
            Vector2 infoLocation = new Vector2(10.0f, GraphicsDevice.Viewport.Height - 24);
            
            Color hudColor = Color.White;

            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                        titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            string currentLevelString = "Player level: " + _world.Player.AbilityLevel.ToString();
            DrawShadowedString(_hudFont, currentLevelString, currentLevelLocation, Color.White, 1.0f);

            string infoString = "Press 's' for player data";
            DrawShadowedString(_hudFont, infoString, infoLocation, hudColor, 1.0f);

            _spriteBatch.End();
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color, float scale)
        {
            _spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black, 0.0f, new Vector2(1.0f, 1.0f), scale, SpriteEffects.None, 0.0f);
            _spriteBatch.DrawString(font, value, position, color, 0.0f, new Vector2(1.0f, 1.0f), scale, SpriteEffects.None, 0.0f);
        }
    }
}
