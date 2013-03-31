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
            MapBlock[, , ] map = new MapBlock[20, 20, 1];

            //First we read the actual file

            try
            {
                MapFileReader reader = new MapFileReader();
                string[] filedata = reader.ReadFile(filename);

                //for now we only care about the coordiantaes

                foreach (string s in filedata)
                {
                    if (!s.StartsWith("-"))
                    {
                        try
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

                                block.Tile = itemFact.CreateItem(splitline[4], splitline[5]);
                                block.Tile.Coordinate =coo;

                                map[coo.X, coo.Y, coo.Z] = block;
                            }
                            else
                            {
                                //its an item
                                //find the block
                                MapBlock block = map[coo.X,coo.Y,coo.Z];
 
                                ItemFactory.ItemFactory itemFact = new ItemFactory.ItemFactory();

                                block.PutItemOnBlock(itemFact.CreateItem(splitline[4],splitline[5]));
                            }
                            
                        }
                        catch (Exception e)
                        {

                        }

                    }
                }

                return map;

            }
            catch (Exception ex)
            {

            }

            return map;

        }

    }
}
