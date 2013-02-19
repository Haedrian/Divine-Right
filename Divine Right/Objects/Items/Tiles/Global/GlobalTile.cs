using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Tiles.Global
{
    /// <summary>
    /// This is a special kind of map item tile which can be used for all global tiles.
    /// </summary>
    class GlobalTile:
        MapItem
    {
        #region Constants
        /// <summary>
        /// The graphic for the mountain
        /// </summary>
        public string MOUNTAIN = "/Graphics/World/Mountain";
        public string HILL = "/Graphics/World/Hill";
        public string PLAIN = "";
        
        public string WATERTILE = "/Graphics/World/Tiles/WaterTile";
        public string GRASSTILE = "/Graphics/World/Tiles/GrassTile";
        #endregion
        #region members

        /// <summary>
        /// This represents the elevation of the tile, from -500 to 500.
        /// Anything less than 0, is underwater
        /// Anything less than 80 is Plain
        /// 255 or less is hilly
        /// anything above 255 is mountainous
        /// </summary>
        int elevation = 0;
        #endregion

        #region Properties

        public override List<string> Graphics
        {
            get
            {
                //we need to determine what kind of items we have on the tile
                List<string> graphics = new List<string>();

                //check the elevation and determine which graphic we'll show

                if (elevation < 0)
                {
                    graphics.Add(WATERTILE);
                }
                else
                {
                    graphics.Add(GRASSTILE);
                }

                //now check whether its a mountain or a hill, and put it on top
                if (elevation > 255)
                {
                    graphics.Insert(0,MOUNTAIN);
                }
                else if (elevation <= 255 && elevation > 80)
                {
                    graphics.Insert(0, HILL);
                }
                else if (elevation >= 0)
                {
                    graphics.Insert(0,PLAIN);
                }

                return graphics;
            }
            set
            {
                base.Graphics = value;
            }
        }

        #endregion


    }
}
