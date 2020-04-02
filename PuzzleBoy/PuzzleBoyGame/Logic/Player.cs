using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PuzzleBoy
{
    class Player : Entity
    {
        private IRenderer _renderer;

        public int AbilityLevel
        {
            get { return abilityLevel; }
        }
        private int abilityLevel = 1;

        public int AddExp
        {
            set { currentLevelExp += value; }
        }
        private int currentLevelExp = 0;
        private int baseExpToLevelUp = 20;

        public int ExpToNextLevel
        {
            get { return baseExpToLevelUp * abilityLevel - currentLevelExp; }
        }

        public bool InPuzzleMode
        {
            get { return inPuzzleMode; }
            set { inPuzzleMode = value; }
        }
        bool inPuzzleMode;

        public int CurrentPuzzleNum
        {
            get { return _currentPuzzleNum; }
        }
        int _currentPuzzleNum;

        // Movement data
        private const float MAX_MOVE_TIME = 250.0f;
        private const float MAX_MOVE_SPEED = Tile.Width / MAX_MOVE_TIME;
        private const float MAX_MOVE_DIST = Tile.Width;
        private bool _isMoving;
        private float _distSinceMove;
        private TileTerrain _currentTerrain; // The terrain type currently walked on

        private const float TIME_FROM_KEY_PRESS_TO_MOVEMENT = 100.0f;
        private float _timeSinceNewKeyPress = 0.0f;
        private KeyboardState _prevKeyboard;

        private Vector2 _previousPosition;

        public Direction Direction
        {
            get { return _direction; }
        }
        Direction _direction = Direction.dDown;
        Direction _prevDirection;

        public Player(World world, Vector2 startPos) :
            base(world, startPos)
        {
            _world = world;
            _renderer = new PlayerRenderer(this);
        }

        public override void Init()
        {
        }

        public override void LoadContent(ContentManager content)
        {
            _renderer.LoadContent(content);
        }

        /*public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - _origin.X);
                int top = (int)Math.Round(Position.Y - _origin.Y);
                return new Rectangle(left, top, textureFront.Width, textureFront.Height);
            }
        }*/

        public override void Update(GameTime gameTime)
        {
            if (currentLevelExp >= (ExpToNextLevel+currentLevelExp))
            {
                abilityLevel++;
                currentLevelExp = 0;
            }
            
            if (_isMoving && _distSinceMove < MAX_MOVE_DIST)
            {
                // Calculate movement speed based on terrain
                float moveSpeed = (_currentTerrain == TileTerrain.Grass) ? MAX_MOVE_DIST / 320.0f : MAX_MOVE_DIST / 240.0f;

                // Calculate how far to move this frame
                float distanceThisFrame = moveSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                // Advance the total distance moved
                _distSinceMove += distanceThisFrame;

                // Check if we reached the target tile
                if (_distSinceMove > MAX_MOVE_DIST)
                {
                    _isMoving = false;
                    _distSinceMove = MAX_MOVE_DIST;
                }

                // Update the position based on previous position +/- distance moved
                switch(_direction)
                {
                    case Direction.dRight:
                        _position.X = (int)(_previousPosition.X + _distSinceMove);
                        break;
                    case Direction.dLeft:
                        _position.X = (int)(_previousPosition.X - _distSinceMove);
                        break;
                    case Direction.dDown:
                        _position.Y = (int)(_previousPosition.Y + _distSinceMove);
                        break;
                    case Direction.dUp:
                        _position.Y = (int)(_previousPosition.Y - _distSinceMove);
                        break;
                }
            }

            // Check for and resolve any collisions
            //CheckCollisions();
        }

        public void HandleInput(KeyboardState keyboardState)
        {
            if (_isMoving) return;

            _prevDirection = _direction;

            bool isKeyDown = false;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                _direction = Direction.dLeft;
                isKeyDown = true;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                _direction = Direction.dRight;
                isKeyDown = true;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                _direction = Direction.dUp;
                isKeyDown = true;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                _direction = Direction.dDown;
                isKeyDown = true;
            }

            if (isKeyDown)
            {
                if (_direction == _prevDirection)
                {
                    // Check that the tile we are trying to move to is clear
                    Point pt = _world.GetPointAdjacentToPosition(_position.X, _position.Y, _direction);

                    if (_world.GetTileCollision(pt.X, pt.Y) == TileTerrain.Blocked)
                        return;

                    _isMoving = true;
                    _distSinceMove = 0.0f;
                    Point tilePos = _world.GetPointFromPosition(_position.X, _position.Y);
                    _currentTerrain = _world.GetTileCollision(tilePos.X, tilePos.Y);
                    _previousPosition = _position;

                    MessageSystem._MessageSystem.sendMessage(MessageType.MSG_PLAYER_STARTED_MOVING, "");
                }
            }
            else // No movement, check for other buttons
            {
                // "Action" button
                if (keyboardState.IsKeyDown(Keys.Z))
                {
                    Point adjPt = _world.GetPointAdjacentToPosition(_position.X, _position.Y, _direction);
                    Entity entity = _world.GetObjectAtPoint(adjPt);
                    if (/*entity.IsInteractable() &&*/ entity != null)
                    {
                        IInteractable obj = null;
                        try {
                            obj = (IInteractable)entity;
                            obj.Interact();
                            //_world.ShowText("Interacted with object [" + entity.ToString() + "]");
                        }
                        catch{}
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.X))
                {
                    // Button2 pressed
                }
            }
        }

        public bool IsAtTile(Point tilePt)
        {
            if (_isMoving) return false;
            Point pt = _world.GetPointFromPosition(_position.X, _position.Y);
            if (pt == tilePt)
                return true;
            return false;
        }

        #region CollisionDetection
        /*private void CheckCollisions()
        {
            Rectangle bounds = BoundingRectangle;

            foreach (PuzzleTile puzzleTile in _world.PuzzleTiles)
            {
                if (!_isMoving)
                {
                    Rectangle tileBounds = _world.GetTileBounds((int)puzzleTile.position.X, (int)puzzleTile.position.Y);

                    if(BoundingRectangle.Intersects(tileBounds) && puzzleTile.puzzleCompleted == false)
                    {
                        currentPuzzleNum = puzzleTile.id;
                        inPuzzleMode = true;
                        _position = _previousPosition;
                    }
                }
            }

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
                    TileTerrain collision = _world.GetTileCollision(x, y);
                    if (collision != TileTerrain.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = _world.GetTileBounds(x, y);
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

                if (collision == TileTerrain.Blocked || collision == TileTerrain.Water) // Ignore platforms.  
                {
                    // Resolve the collision along the X axis.
                    if(_direction == Direction.dLeft || _direction == Direction.dRight)
                        Position = new Vector2(Position.X + depth.X, Position.Y);
                    else
                        Position = new Vector2(Position.X, Position.Y + depth.Y);

                    // Perform further collisions with the new bounds.  
                    bounds = BoundingRectangle;
                }
            }

            return bounds;
        }*/
        #endregion

        public override void Draw(/*GameTime gameTime, */SpriteBatch spriteBatch)
        {
            _renderer.Draw(spriteBatch);
        }
    }
}
