using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    interface IComponent
    {
        String GetComponentId();
        void OnMessage();
        void OnUpdate();
    }
}
