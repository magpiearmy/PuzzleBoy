using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    interface IAIBehaviour
    {
        void Update(GameTime gameTime);
    }

    abstract class BaseNPCBehaviour : IAIBehaviour
    {
        protected NonPlayableCharacter _npc;

        public BaseNPCBehaviour(NonPlayableCharacter npc)
        {
            _npc = npc;
        }

        public abstract void Update(GameTime gameTime);
    }

    class AITurnBehaviour : BaseNPCBehaviour
    {
        const float TIME_BETWEEN_TURNS = 1000.0f;
        float _timeSinceTurn = 0.0f;

        public AITurnBehaviour(NonPlayableCharacter npc) : base(npc)
        {
        }

        public override void Update(GameTime gameTime)
        {
            _timeSinceTurn += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_timeSinceTurn >= TIME_BETWEEN_TURNS)
            {
                switch (_npc.Direction)
                {
                    case Direction.dUp:
                        _npc.Direction = Direction.dRight; break;
                    case Direction.dRight:
                        _npc.Direction = Direction.dDown; break;
                    case Direction.dDown:
                        _npc.Direction = Direction.dLeft; break;
                    case Direction.dLeft:
                        _npc.Direction = Direction.dUp; break;
                }
                _timeSinceTurn -= TIME_BETWEEN_TURNS;
            }
        }
    }

    class AIPacingBehaviour : BaseNPCBehaviour
    {
        int steps = 0;
        bool reverse = false;
        public AIPacingBehaviour(NonPlayableCharacter npc) : base(npc)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!_npc.IsMoving())
            {
                if (steps < 4)
                {
                    if (_npc.Move( reverse ? Direction.dUp : Direction.dDown ))
                        steps++;
                }
                else
                {
                    steps = 0;
                    reverse = !reverse;
                }
            }
        }
    }
}
