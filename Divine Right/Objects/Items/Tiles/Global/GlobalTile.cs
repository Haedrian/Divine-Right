using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Tiles.Global
{
    /// <summary>
    /// This is a special kind of map item tile which can be used for all global tiles.
    /// </summary>
    public class GlobalTile:
        MapItem
    {
        #region Constants
        /// <summary>
        /// The graphic for the mountain
        /// </summary>
        public string MOUNTAIN = @"Graphics/World/Mountain";
        public string HILL = @"Graphics/World/Hill";
        public string PLAIN = @"";
        
        public string WATERTILE = @"Graphics/World/Tiles/WaterTile";
        public string GRASSTILE = @"Graphics/World/Tiles/GrassTile";

        public string RIVER = @"Graphics/World/Tiles/WaterTile";
        #endregion
        #region members

        /// <summary>
        /// This represents the elevation of the tile, from -500 to 500.
        /// Anything less than 0, is underwater
        /// Anything less than 80 is Plain
        /// 255 or less is hilly
        /// anything above 255 is mountainous
        /// </summary>
        public int Elevation { get; set; }
        /// <summary>
        /// This represents the region this tile belongs in. Currently this is used by the world generation code only.
        /// </summary>
        public int Region { get; set; }

        /// <summary>
        /// Does it have a river?
        /// </summary>
        public bool HasRiver { get; set; }
        #endregion

        #region Properties

        public GlobalTile()
        {
            Elevation = 0;
            Region = -1;
        }

        public override List<string> Graphics
        {
            get
            {
                //we need to determine what kind of items we have on the tile
                List<string> graphics = new List<string>();

                //check the elevation and determine which graphic we'll show

                if (Elevation < 0)
                {
                    graphics.Add(WATERTILE);
                }
                else
                {
                    graphics.Add(GRASSTILE);
                }

                //now check whether its a mountain or a hill, and put it on top
                if (Elevation > 255)
                {
                    graphics.Insert(0,MOUNTAIN);
                }
                else if (Elevation <= 255 && Elevation > 80)
                {
                    graphics.Insert(0, HILL);
                }
                else if (Elevation >= 0)
                {
                    graphics.Insert(0,PLAIN);
                }

                //Do we have a river?

                if (HasRiver)
                {
                    graphics.Insert(0, RIVER);
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
