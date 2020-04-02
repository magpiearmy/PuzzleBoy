using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class InputHandler
    {
        Player player;
        public List<ICommand> _commands = new List<ICommand>();
        
        public void GetInput(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Z))
            {
                _commands.Add(new MoveCommand(null, Direction.dDown));
            }
        }
    }
}
