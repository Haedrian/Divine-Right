using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Compare;

namespace DivineRightGame.MapFactory
{
    /// <summary>
    /// Class for reading and creating a map or maplet from the map data stored in a file
    /// </summary>
    public class MapFactoryManager
    {
        public MapBlock[,,] GetMap(string filename)
        {
            MapBlock[, ,] map = null;

            //First we have to read the actual file and determine how large the map is going to be

                MapFileReader reader = new MapFileReader();
                string[] fileData = reader.ReadFile(filename);

                //find the line which tells us the size of the map
                foreach (string line in fileData)
                {
                    if (line.StartsWith("-"))
                    {
                        if (line.ToLower().Contains("-mapsize"))
                        {
                            var cells = line.Split(',');

                            map = new MapBlock[Int32.Parse(cells[1]), Int32.Parse(cells[2]), Int32.Parse(cells[3])];
                            break;
                        }
                    }
                }

                if (map == null)
                {
                    //map was missing metainformation
                    throw new Exception("Can't parse Map. It is missing the Mapsize metainformation");
                }

                foreach (string s in fileData)
                {
                    if (!s.StartsWith("-"))
                    {
                            //split into its components    
                            var splitline = s.Split(',');

                            MapCoordinate coo = new MapCoordinate(Int32.Parse(splitline[0]), Int32.Parse(splitline[1]), Int32.Parse(splitline[2]),DRObjects.Enums.MapTypeEnum.LOCAL);

                            //now check what item we're putting

                            if (splitline[3].ToLower().Equals("t"))
                            {
                                //its a tile
                                MapBlock block = new MapBlock();

                                ItemFactory.ItemFactory itemFact = new ItemFactory.ItemFactory();

                                //TODO: STORAGE OF AN ITEM BY ITS PARAMETERS

                                block.Tile = itemFact.CreateItem(splitline[4], Int32.Parse(splitline[5]));
                                block.Tile.Coordinate =coo;

                                map[coo.X, coo.Y, coo.Z] = block;
                            }
                            else
                            {
                                //its an item
                                //find the block
                                MapBlock block = map[coo.X,coo.Y,coo.Z];
 
                                ItemFactory.ItemFactory itemFact = new ItemFactory.ItemFactory();

                                block.PutItemOnBlock(itemFact.CreateItem(splitline[4],Int32.Parse(splitline[5])));
                            }
                    }
                }

                return map;

    
        }

    }
}
