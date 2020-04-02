using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class NonPlayableCharacter : Entity, IInteractable
    {
        Texture2D _textureFront, _textureBack, _textureLeft, _textureRight;

        IAIBehaviour _behaviour = null;

        // Movement data
        private Vector2 _previousPosition;
        private const float MAX_MOVE_TIME = 750f;
        private const float MAX_MOVE_DIST = Tile.Width;
        private float _distSinceMove;
        private TileTerrain _currentTerrain; // The terrain type currently walked on
        private bool _isMoving;

        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        Direction _direction = Direction.dDown;

        public NonPlayableCharacter(World world, Vector2 position) :
            base(world, position)
        {
        }

        public override void Init()
        {
        }

        public override void LoadContent(ContentManager content)
        {
            _textureFront = _world.Content.Load<Texture2D>("Sprites/NPCFront");
            _textureBack = _world.Content.Load<Texture2D>("Sprites/NPCBack");
            _textureLeft = _world.Content.Load<Texture2D>("Sprites/NPCLeft");
            _textureRight = _world.Content.Load<Texture2D>("Sprites/NPCRight");
        }

        public override void Update(GameTime gameTime)
        {
            if (_behaviour != null)
                _behaviour.Update(gameTime);

            if (_isMoving && _distSinceMove < MAX_MOVE_DIST)
            {
                // Calculate movement speed based on terrain
                float moveSpeed = (_currentTerrain == TileTerrain.Grass) ? MAX_MOVE_DIST / 500.0f : MAX_MOVE_DIST / 320.0f;

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

                // Update the position based on previous position +/- total distance moved
                switch (_direction)
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D textureToDraw;
            switch (_direction)
            {
                case Direction.dUp:     textureToDraw = _textureBack; break;
                case Direction.dLeft:   textureToDraw = _textureLeft; break;
                case Direction.dRight:  textureToDraw = _textureRight; break;
                case Direction.dDown:   textureToDraw = _textureFront; break;
                default:  textureToDraw = _textureFront; break;
            }
            Vector2 origin = new Vector2(0, _textureBack.Height - Tile.Height);
            spriteBatch.Draw(textureToDraw, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }

        public bool IsMoving()
        {
            return _isMoving;
        }

        public bool Move(Direction direction)
        {
            if (_isMoving) return false;

            Point destPt = _world.GetPointAdjacentToPosition(Position.X, Position.Y, direction);
            if (_world.GetTileCollision(destPt.X, destPt.Y) != TileTerrain.Blocked)
            {
                _previousPosition = _position;
                _distSinceMove = 0.0f;
                Point tilePos = _world.GetPointFromPosition(_position.X, _position.Y);
                _currentTerrain = _world.GetTileCollision(tilePos.X, tilePos.Y);
                _direction = direction;
                _isMoving = true;
            }
            return _isMoving;
        }

        public void SetAIBehaviour(IAIBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public void Interact()
        {
            if (_isMoving) return;
            switch(_world.Player.Direction)
            {
                case Direction.dUp:     _direction = Direction.dDown; break;
                case Direction.dDown:   _direction = Direction.dUp; break;
                case Direction.dLeft:   _direction = Direction.dRight; break;
                case Direction.dRight:  _direction = Direction.dLeft;  break;
            }
            _world.ShowText("Hi! I am an NPC!");
        }
    }
}
