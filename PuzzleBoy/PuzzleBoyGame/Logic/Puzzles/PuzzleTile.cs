using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PuzzleBoy
{
    enum PuzzleType
    {
        eWaterPuzzle,
        eElectricPuzzle,
        eSewerPuzzle
    }

    class PuzzleTile
    {
        public PuzzleType type;

        public TileTerrain collision;
        public Vector2 position;
        public int id;
        public bool puzzleCompleted;

        public PuzzleTile(int x, int y, PuzzleType type, int id)
        {
            collision = TileTerrain.Passable;
            position = new Vector2(x, y);
            this.type = type;
            this.id = id;
            puzzleCompleted = false;
        }

        public void SetPuzzleComplete()
        {
            puzzleCompleted = true;
        }
    }
}
