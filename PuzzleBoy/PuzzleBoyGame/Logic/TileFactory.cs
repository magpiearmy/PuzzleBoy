using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PuzzleBoy
{
    class TileFactory
    {
        public enum TileTypes { TILE_GRASS, TILE_PATH, TILE_WATER };
        private Random random = new Random(354668);
        private ContentManager _content;
        
        public TileFactory(ContentManager content)
        {
            _content = content;
        }

        public Tile parseTile(char tileType)
        {
            switch (tileType)
            {
                case '.':
                    return LoadTile("Path", TileTerrain.Passable);

                case '#':
                    return LoadTile("Grass", TileTerrain.Grass);

                case '~':
                    return LoadTile("Water", TileTerrain.Water);

                case '[':
                    return LoadTile("Water_LeftBank", TileTerrain.Water);

                case ']':
                    return LoadTile("Water_RightBank", TileTerrain.Water);

                case 'T':
                    return LoadVarietyTile("Tree", 3, TileTerrain.Blocked);

                /*
                case 'D':
                    return LoadDoorTile();
                    break;
                case 'E':
                    return LoadPuzzleTile(x, y, PuzzleType.eElectricPuzzle);

                case 'W':
                    return LoadPuzzleTile(x, y, PuzzleType.eWaterPuzzle);

                case 'S':
                    return LoadPuzzleTile(x, y, PuzzleType.eSewerPuzzle);

                case 'B':
                    return LoadBoat(x, y);
                */
                default:
                    return LoadTile("Path", TileTerrain.Passable);
                    //throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadTile(string name, TileTerrain collision)
        {
            TileType type = null;// new TileType(_content.Load<Texture2D>("Tiles/" + name), collision);
            return new Tile(1);//type);
        }

        private Tile LoadCustomTile(string baseName, TileTerrain collision)
        {
            return LoadTile(baseName, collision);
        }

        private Tile LoadVarietyTile(string baseName, int variationCount, TileTerrain collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }
    }
}
