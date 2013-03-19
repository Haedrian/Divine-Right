using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

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
        public string PLAIN = @"";
        
        public string WATERTILE = @"Graphics/World/Tiles/WaterTile";
        public string GRASSTILE = @"Graphics/World/Tiles/GrassTile";

        public string RIVER = @"Graphics/World/River";

        public string CONTOUR = @"Graphics/World/Hill";
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

        /// <summary>
        /// Do we draw a contour on this tile?
        /// </summary>
        public bool HasContour { get; set; }
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

                //Do we draw a contour

                if (HasContour)
                {
                    graphics.Insert(0, CONTOUR);
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

        #region Functions

        public string GetGraphicsByOverlay(GlobalOverlay overlay)
        {
            string BROWN = @"Graphics/World/Overlay/Regions/Brown";
            string GREEN = @"Graphics/World/Overlay/Regions/Green";
            string ORANGE = @"Graphics/World/Overlay/Regions/Orange";
            string PINK = @"Graphics/World/Overlay/Regions/Pink";
            string PURPLE = @"Graphics/World/Overlay/Regions/Purple";
            string RED = @"Graphics/World/Overlay/Regions/Red";
            string YELLOW = @"Graphics/World/Overlay/Regions/Yellow";

            if (overlay.Equals(GlobalOverlay.NONE))
            {
                return "";
            }

            else if (overlay.Equals(GlobalOverlay.REGION))
            {
                //if the elevation is underwater, nothing
                if (this.Elevation < 0)
                {
                    return "";
                }

                //otherwise, depending on the region number, colour them
                switch (Region % 7)
                {
                    case 0: return BROWN;
                    case 1: return GREEN;
                    case 2: return ORANGE;
                    case 3: return PINK;
                    case 4: return PURPLE;
                    case 5: return RED;
                    case 6: return YELLOW;
                }

                return "";

            }

            else
            {
                //todo:
                return "";
            }



        }

        #endregion

    }
}
