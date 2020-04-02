using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace PuzzleBoy
{
    class World : IDisposable, IGameState
    {
        private Game _game;
        private IRenderer _renderer;
        public event StateEventHandler _exitEvent;

        public Map Map
        {
            get { return _map; }
        }
        private Map _map;

        public bool inCharScreen = false;

        public List<Boat> Boats
        {
            get { return boats; }
        }
        List<Boat> boats = new List<Boat>();

        public List<PuzzleTile> PuzzleTiles
        {
            get { return puzzleTiles; }
        }
        List<PuzzleTile> puzzleTiles = new List<PuzzleTile>();

        private Random random = new Random(354668);

        public Player Player
        {
            get { return _player; }
        }
        Player _player;

        public ContentManager Content
        {
            get { return _content; }
        }
        ContentManager _content;

        List<Map> _maps = new List<Map>();

        public World(Game game, IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            _game = game;
            _renderer = new WorldRenderer(this);
            _content = new ContentManager(serviceProvider, "Content");
        }

        public void Init()
        {
            //TEST: Load XML map
            TileMapParser parser = new TileMapParser(Path.Combine(Content.RootDirectory, "Levels/PuzzleBoyMap.tmx"), Content);
            _map = parser.LoadMap();
            SetStartPoint(_map.StartPoint);

            // For now we're just adding some objects into the world...
            Vector2 pos = new Vector2(4, 4) * Tile.Size;
            ObtainableItem item = new ObtainableItem(this, pos);
            _map.Entities.Add(item);

            pos = new Vector2(10, 5) * Tile.Size;
            NonPlayableCharacter npc1 = new NonPlayableCharacter(this, pos);
            npc1.SetAIBehaviour(new AIPacingBehaviour(npc1));
            _map.Entities.Add(npc1);

            // Add the player entity
            _map.Entities.Add(_player);

            foreach (Entity entity in _map.Entities)
            {
                entity.Init();
                entity.LoadContent(_content);
            }
        }

        public void SetStartPoint(Point pt)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            Vector2 start = new Vector2(pt.X, pt.Y) * Tile.Size;
            _player = new Player(this, start);
        }

        // Gets Collision of the tile at location (x, y)
        public TileTerrain GetTileCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= _map.Width)
                return TileTerrain.Blocked;
            if (y < 0 || y >= _map.Height)
                return TileTerrain.Blocked;

            Point tilePt = new Point(x, y);
            foreach (Entity entity in _map.Entities)
                if (GetPointFromPosition(entity.Position.X, entity.Position.Y) == tilePt) 
                    return TileTerrain.Blocked;

            return _map.GetTileCollision(x, y);
        }

        public Rectangle GetTileBounds(float x, float y)
        {
            return new Rectangle((int)x * Tile.Width, (int)y * Tile.Height, Tile.Width, Tile.Height);
        }

        public Point GetPointFromPosition(float absX, float absY)
        {
            int tileX = (int)absX / Tile.Width;
            int tileY = (int)absY / Tile.Height;
            return new Point(tileX, tileY);
        }
        public Point GetPointFromPosition(Vector2 absPos)
        {
            return GetPointFromPosition((int)absPos.X, (int)absPos.Y);
        }

        public Tile GetTileAtPoint(Point pt)
        {
            return _map.GetTileAt(pt.X, pt.Y);
        }

        public Point GetPointAdjacentToPosition(float absX, float absY, Direction dir)
        {
            Point pt = GetPointFromPosition(absX, absY);
            int tileX = pt.X;
            int tileY = pt.Y;

            switch (dir)
            {
            case Direction.dUp:
                tileY--;
                break;
            case Direction.dRight:
                tileX++;
                break;
            case Direction.dDown:
                tileY++;
                break;
            case Direction.dLeft:
                tileX--;
                break;
            }

            if (tileX >= 0 && tileX < _map.Width && tileY >= 0 && tileY < _map.Height)
            {
                pt.X = tileX;
                pt.Y = tileY;
            }
            else
            {
                pt.X = -1;
                pt.Y = -1;
            }
            return pt;
        }

        public Entity GetObjectAtPoint(Point tilePt)
        {
            foreach (Entity entity in _map.Entities)
            {
                if (entity.Position.Equals(new Vector2(tilePt.X, tilePt.Y) * Tile.Size))
                    return entity;
            }
            return null;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            UpdateInput(keyboardState);

            Player.HandleInput(keyboardState);

            foreach (Entity entity in _map.Entities)
            {
                entity.Update(gameTime);
            }
            //Player.Update(gameTime);

            // Once all entities have updated themselves we must check the state
            // of the world for any significant changes (e.g. has the player entered a building?)
            //DoPostUpdateChecks();
        }

        private void DoPostUpdateChecks()
        {
            if (_maps.Count == 1)
            {
                if (Player.IsAtTile(new Point(9, 3)))
                {
                    // Load a new room.
                    _map = new Map();
                    _map.LoadMap(TitleContainer.OpenStream(_content.RootDirectory + "/Levels/Room1.txt"));
                    _player.Position = new Vector2(_map.StartPoint.X, _map.StartPoint.Y) * Tile.Size;
                    _maps.Add(_map);
                }
            }
            else if (_maps.Count == 2)
            {
                if (Player.IsAtTile(new Point(5, 10)))
                {
                    _maps.RemoveAt(_maps.Count-1);
                    _map = _maps[_maps.Count-1];
                    _player.Position = new Vector2(_map.StartPoint.X, _map.StartPoint.Y) * Tile.Size;
                }
            }
        }

        private void UpdateInput(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.S))
            {
                inCharScreen = true;
            }
        }

        public void PuzzleCompleted(int puzzleNum)
        {
            /*puzzleTiles[puzzleNum].SetPuzzleComplete();
            
            CheckPuzzleStates(puzzleNum);

            for (int i = 0; i < boats.Count; i++)
            {
                if (boats[i].PuzzleId == puzzleNum + 1)
                {
                    boats[i].Start();
                }
            }

            _player.AddExp = 10;
            _player.InPuzzleMode = false;*/
        }

        public void PuzzleFailed(int puzzleNum)
        {
            Player.InPuzzleMode = false;
        }

        /*private void CheckPuzzleStates(int completedPuzzleNum)
        {
            if ((completedPuzzleNum == 0 && puzzleTiles[6].puzzleCompleted) || (completedPuzzleNum == 6 && puzzleTiles[0].puzzleCompleted))
            {
                _tiles[32, 4] = LoadTile("Bridge", TileTerrain.Passable);
                _tiles[33, 4] = LoadTile("Bridge", TileTerrain.Passable);
                _tiles[34, 4] = LoadTile("Bridge", TileTerrain.Passable);
                _tiles[35, 4] = LoadTile("Bridge", TileTerrain.Passable);

                _tiles[32, 5] = LoadTile("Bridge", TileTerrain.Passable);
                _tiles[33, 5] = LoadTile("Bridge", TileTerrain.Passable);
                _tiles[34, 5] = LoadTile("Bridge", TileTerrain.Passable);
                _tiles[35, 5] = LoadTile("Bridge", TileTerrain.Passable);
            }
            else if (completedPuzzleNum == 8)
            {
                _tiles[22, 18] = LoadTile("Path", TileTerrain.Passable);
                _tiles[22, 19] = LoadTile("Path", TileTerrain.Passable);
            }
        }*/

        public void ShowText(String text)
        {
            _game.InitDialog(text);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _renderer.Draw(spriteBatch);
        }

        public void Dispose()
        {
            Content.Unload();
        }

        public bool IsExiting() { return false; }

        public static Point PointFromPosition(int x, int y)
        {
            return new Point(x / Tile.Width, y / Tile.Height);
        }
        public static Point PointFromPosition(Vector2 position)
        {
            return PointFromPosition((int)position.X, (int)position.Y);
        }

    }
}