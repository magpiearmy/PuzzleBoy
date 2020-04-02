using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleBoy
{
    public class PuzzlePiece
    {
        protected Color initialColor;
        public Color color;

        public const int Width = 32;
        public const int Height = 32;

        protected PipeType initialType;
        public PipeType type;

        public bool isFilled;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public PuzzlePiece(Color color, PipeType pipeType)
        {
            this.color = color;
            this.type = pipeType;

            initialColor = color;
            initialType = type;
        }

        public void Reset()
        {
            color = initialColor;
            type = initialType;
            isFilled = false;
        }

        public virtual void Fill(Color fillColor)
        {
            color = fillColor;
            isFilled = true;
        }

        //Returns the new direction of travel if this pipe if applicable.
        //If this pipe is not connected, returns 0;
        public int GetDirection(int currentDirection)
        {
            switch (type)
            {
                case PipeType.pVert:
                    if (currentDirection == 1 || currentDirection == 3)
                        return currentDirection;
                    break;
                case PipeType.pHoriz:
                    if (currentDirection == 2 || currentDirection == 4)
                        return currentDirection;
                    break;
                case PipeType.pBottomLeftCorner:
                    if (currentDirection == 2)
                    {
                        return 3;
                    }
                    else if (currentDirection == 1)
                    {
                        return 4;
                    }
                    break;
                case PipeType.pBottomRightCorner:
                    if (currentDirection == 4)
                    {
                        return 3;
                    }
                    else if (currentDirection == 1)
                    {
                        return 2;
                    }
                    break;
                case PipeType.pTopLeftCorner:
                    if (currentDirection == 2)
                    {
                        return 1;
                    }
                    else if (currentDirection == 3)
                    {
                        return 4;
                    }
                    break;
                case PipeType.pTopRightCorner:
                    if (currentDirection == 4)
                    {
                        return 1;
                    }
                    else if (currentDirection == 3)
                    {
                        return 2;
                    }
                    break;
            }

            return 0;
        }

        //Rotates the pipe one move
        public void Rotate()
        {
            switch (type)
            {
                case PipeType.pVert:
                    type = PipeType.pHoriz;
                    break;
                case PipeType.pHoriz:
                    type = PipeType.pVert;
                    break;
                case PipeType.pBottomLeftCorner:
                    type = PipeType.pTopLeftCorner;
                    break;
                case PipeType.pBottomRightCorner:
                    type = PipeType.pBottomLeftCorner;
                    break;
                case PipeType.pTopLeftCorner:
                    type = PipeType.pTopRightCorner;
                    break;
                case PipeType.pTopRightCorner:
                    type = PipeType.pBottomRightCorner;
                    break;
            }
        }
    }
}
