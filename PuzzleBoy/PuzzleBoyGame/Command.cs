using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    /*enum GameCommands
    {
        CMD_FACE_UP,
        CMD_FACE_DOWN,
        CMD_FACE_LEFT,
        CMD_FACE_RIGHT,

        CMD_MOVE_UP,
        CMD_MOVE_DOWN,
        CMD_MOVE_LEFT,
        CMD_MOVE_RIGHT
    }*/

    interface ICommand
    {
        void execute();
    }

    abstract class BaseCommand : ICommand
    {
        protected Entity _target;
        public BaseCommand(Entity target)
        {
            _target = target;
        }

        public abstract void execute();
    }

    class MoveCommand : BaseCommand
    {
        private Direction _dir;
        public MoveCommand(Entity target, Direction dir)
            : base(target)
        {
            _dir = dir;
        }

        public override void execute()
        {

        }
    }

    class ActionCommand : BaseCommand
    {
        public ActionCommand(Entity target)
            : base(target)
        {
        }
        public override void execute()
        {

        }
    }

    class CancelCommand : BaseCommand
    {
        public CancelCommand(Entity target)
            : base(target)
        {
        }

        public override void execute()
        {

        }
    }
}
