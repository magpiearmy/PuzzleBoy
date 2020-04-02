using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PuzzleBoy
{
    class TileMapParser
    {
        String _mapFilename;
        ContentManager _content;
        Map _map;

        public TileMapParser(String mapFilename, ContentManager content)
        {
            _mapFilename = mapFilename;
            _content = content;
        }

        public Map LoadMap()
        {
            _map = new Map();
            XmlDocument doc = new XmlDocument();
            doc.Load(_mapFilename);

            XmlNodeList mapList  = doc.GetElementsByTagName("map");
            foreach (XmlNode node in mapList[0].ChildNodes)
            {
                switch (node.Name)
                {
                    case "tileset":
                        ParseTileset(node); break;
                    case "layer":
                        ParseLayer(node); break;
                    case "objectgroup":
                        ParseObjectgroup(node); break;
                }
            }

            return _map;
        }

        public void ParseTileset(XmlNode tilesetNode)
        {
            XmlAttributeCollection attributes = tilesetNode.Attributes;
            string firstGid = attributes.GetNamedItem("firstgid").Value;
            string tileCount = attributes.GetNamedItem("tilecount").Value;
            bool isCollection = tilesetNode.FirstChild.Name == "tile";

            TileSet newTileSet = new TileSet(attributes.GetNamedItem("name").Value,
                                             Convert.ToInt32(firstGid),
                                             Convert.ToInt32(tileCount),
                                             isCollection);

            if (!isCollection)
            {
                String imageFilename = tilesetNode.FirstChild.Attributes["source"].Value;
                imageFilename = imageFilename.Substring(3, imageFilename.Length - 7);
                Texture2D image = _content.Load<Texture2D>(imageFilename);
                newTileSet.SetImage(image);

                foreach (XmlNode tileNode in tilesetNode.ChildNodes)
                {
                    if (tileNode.Name == "tile")
                    {
                        int tileTypeId = Convert.ToInt32(tileNode.Attributes["id"].Value);
                        TileType newTileType = new TileType(tileTypeId, TileTerrain.Passable);

                        foreach (XmlNode tileChildNode in tileNode.ChildNodes)
                        {
                            if (tileChildNode.Name == "properties")
                            {
                                foreach (XmlNode propertyNode in tileChildNode.ChildNodes)
                                {
                                    if (propertyNode.Attributes["name"].Value == "Collision")
                                    {
                                        TileTerrain newTerrain = TileTerrain.Passable;
                                        if (propertyNode.Attributes["value"].Value == "Blocked")
                                            newTerrain = TileTerrain.Blocked;
                                        else if (propertyNode.Attributes["value"].Value == "Water")
                                            newTerrain = TileTerrain.Water;
                                        newTileType.Terrain = newTerrain;
                                    }
                                }
                            }
                        }

                        newTileSet.AddTileType(newTileType);
                    }
                }
            }
            else
            {
                foreach (XmlNode tileNode in tilesetNode.ChildNodes)
                {
                    int tileTypeId = Convert.ToInt32(tileNode.Attributes["id"].Value);
                    TileType newTileType = new TileType(tileTypeId, TileTerrain.Passable);

                    foreach (XmlNode tileChildNode in tileNode.ChildNodes)
                    {
                        if (tileChildNode.Name == "image")
                        {
                            String imageFilename = tileChildNode.Attributes["source"].Value;
                            imageFilename = imageFilename.Substring(3, imageFilename.Length - 7);

                            newTileType.SetTexture(_content.Load<Texture2D>(imageFilename));
                        }
                        else if (tileChildNode.Name == "properties")
                        {
                            foreach (XmlNode propertyNode in tileChildNode.ChildNodes)
                            {
                                if (propertyNode.Attributes["name"].Value == "Collision")
                                {
                                    TileTerrain newTerrain = TileTerrain.Passable;
                                    if (propertyNode.Attributes["value"].Value == "Blocked")
                                        newTerrain = TileTerrain.Blocked;
                                    else if (propertyNode.Attributes["value"].Value == "Water")
                                        newTerrain = TileTerrain.Water;
                                    newTileType.Terrain = newTerrain;
                                }
                            }
                        }
                    }

                    newTileSet.AddTileType(newTileType);
                }
            }

            _map.TileSetMgr.AddTileSet(newTileSet);
        }

        public void ParseLayer(XmlNode layerNode)
        {
            int width = Convert.ToInt32(layerNode.Attributes["width"].Value);
            int height = Convert.ToInt32(layerNode.Attributes["height"].Value);
            _map._tiles = new Tile[width, height];

            MapLayer newLayer = new MapLayer(width, height);

            for (int i = 0; i < layerNode.FirstChild.ChildNodes.Count; i++)
            {
                XmlNode tileNode = layerNode.FirstChild.ChildNodes[i];

                string sGid = tileNode.Attributes["gid"].Value;
                if (sGid != "0")
                {
                    int gid = Convert.ToInt32(sGid);
                    Tile newTile = new Tile(gid);
                    newLayer.AddTile(i % width, i / width, newTile);
                }
            }

            _map.AddLayer(newLayer);
        }

        public void ParseObjectgroup(XmlNode objectgroupNode)
        {
            foreach (XmlNode objectNode in objectgroupNode)
            {
                int absX = Convert.ToInt32(objectNode.Attributes["x"].Value);
                int absY = Convert.ToInt32(objectNode.Attributes["y"].Value);
                switch (objectNode.Attributes["type"].Value)
                {
                    case "PlayerStart":
                        _map.StartPoint = World.PointFromPosition(absX, absY);
                        break;
                    case "NPC":
                        //_map.AddEntity("NPC", World.PointFromPosition(absX, absY));
                        break;
                }
            }
        }
    }
}
