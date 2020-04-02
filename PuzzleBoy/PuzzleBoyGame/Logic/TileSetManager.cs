using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBoy
{
    class TileSetManager
    {
        private List<TileSet> _tileSets = new List<TileSet>();

        public void AddTileSet(TileSet tileSet)
        {
            _tileSets.Add(tileSet);
        }

        public TileType GetTileType(int gid)
        {
            foreach (TileSet tileSet in _tileSets)
            {
                if (tileSet.HasTileWithGid(gid))
                {
                    return tileSet.GetTileType(gid);
                }
            }
            return null;
        }

        public TileSet GetTileSetContainingTileId(int gid)
        {
            foreach (TileSet tileSet in _tileSets)
            {
                if (tileSet.HasTileWithGid(gid))
                {
                    return tileSet;
                }
            }
            return null;
        }
    }
}
