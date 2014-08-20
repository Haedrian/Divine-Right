using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;

namespace DRObjects.Items.Tiles.Global
{
    /// <summary>
    /// This is a special kind of map item tile which can be used for all global tiles.
    /// </summary>
    public class GlobalTile:
        MapItem
    {
        #region Constants

        private SpriteData BIGTREE = SpriteManager.GetSprite(GlobalSpriteName.BIGTREE);
        private SpriteData DEADTREE = SpriteManager.GetSprite(GlobalSpriteName.DEADTREE);
        private SpriteData DESERTTILE = SpriteManager.GetSprite(GlobalSpriteName.DESERTTILE);
        private SpriteData FORESTTILE = SpriteManager.GetSprite(GlobalSpriteName.FORESTTILE);
        private SpriteData GARIGUETILE = SpriteManager.GetSprite(GlobalSpriteName.GARIGUETILE);
        private SpriteData GRASSTILE = SpriteManager.GetSprite(GlobalSpriteName.GRASSTILE);
        private SpriteData HILLSLOPE = SpriteManager.GetSprite(GlobalSpriteName.HILLSLOPE);
        private SpriteData MOUNTAIN = SpriteManager.GetSprite(GlobalSpriteName.MOUNTAIN);
        private SpriteData RIVER = SpriteManager.GetSprite(GlobalSpriteName.RIVER);
        private SpriteData SNOWTILE = SpriteManager.GetSprite(GlobalSpriteName.SNOWTILE);
        private SpriteData SWAMPTILE = SpriteManager.GetSprite(GlobalSpriteName.SWAMPTILE);
        private SpriteData TREE = SpriteManager.GetSprite(GlobalSpriteName.TREE);
        private SpriteData TROPICALTREE = SpriteManager.GetSprite(GlobalSpriteName.TROPICALTREE);
        private SpriteData WATERTILE = SpriteManager.GetSprite(GlobalSpriteName.WATERTILE);

        private SpriteData BROWN = SpriteManager.GetSprite(ColourSpriteName.BROWN);
        private SpriteData GREEN = SpriteManager.GetSprite(ColourSpriteName.GREEN);
        private SpriteData INDIGO = SpriteManager.GetSprite(ColourSpriteName.INDIGO);
        private SpriteData MARBLEBLUE = SpriteManager.GetSprite(ColourSpriteName.MARBLEBLUE);
        private SpriteData ORANGE = SpriteManager.GetSprite(ColourSpriteName.ORANGE);
        private SpriteData PINK = SpriteManager.GetSprite(ColourSpriteName.PINK);
        private SpriteData PURPLE = SpriteManager.GetSprite(ColourSpriteName.PURPLE);
        private SpriteData RED = SpriteManager.GetSprite(ColourSpriteName.RED);
        private SpriteData WHITE = SpriteManager.GetSprite(ColourSpriteName.WHITE);
        private SpriteData YELLOW = SpriteManager.GetSprite(ColourSpriteName.YELLOW);

        #endregion

        #region members

        /// <summary>
        /// This represents the elevation of the tile, from -500 to 500.
        /// Anything less than 0, is underwater
        /// Anything less than 80 is Plain
        /// 250 or less is hilly
        /// anything above 250 is mountainous
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
        public bool HasHillSlope { get; set; }

        /// <summary>
        /// Temperature of the tile in Celcius
        /// </summary>
        public decimal ClimateTemperature { get; set; }

        /// <summary>
        /// The amount of rainfall this tile receives
        /// </summary>
        public decimal Rainfall { get; set; }

        /// <summary>
        /// The biome of the tile, as determined by its traits
        /// </summary>
        public GlobalBiome? Biome { get; set; }

        /// <summary>
        /// Determines how desirable the tile is, based on its temperature, elevation, and resources
        /// </summary>
        public int BaseDesirability { get; set; }

        /// <summary>
        /// If set to true, it will not be possible to colonise on this tile
        /// </summary>
        public bool IsBlockedForColonisation { get; set; }

        /// <summary>
        /// Which civilisation owns the tile - if any. Other civilisations will not colonise this tile.
        /// </summary>
        public int? Owner { get; set; }

        /// <summary>
        /// Whether it contains a resource (to save us time looking)
        /// </summary>
        public bool HasResource { get; set; }

        #endregion

        #region Properties

       

        public override List<SpriteData> Graphics
        {
            get
            {        
               //we need to determine what kind of items we have on the tile
                List<SpriteData> graphics = new List<SpriteData>();

                //check the elevation and determine which graphic we'll show

                if (Elevation < 0)
                {
                    graphics.Add(RIVER);
                }
                else if (Biome.HasValue)
                {
                    //The graphic will depend on the biome
                    switch (this.Biome)
                    {
                        case GlobalBiome.ARID_DESERT:
                            graphics.Add(DESERTTILE);
                            break;
                        case GlobalBiome.DENSE_FOREST:
                            graphics.Add(BIGTREE);
                            graphics.Add(GRASSTILE);
                            break;
                        case GlobalBiome.GARIGUE:
                            graphics.Add(GARIGUETILE);
                            break;
                        case GlobalBiome.GRASSLAND:
                            graphics.Add(GRASSTILE);
                            break;
                        case GlobalBiome.POLAR_DESERT:
                            graphics.Add(SNOWTILE);
                            break;
                        case GlobalBiome.RAINFOREST:
                            graphics.Add(TROPICALTREE);
                            graphics.Add(FORESTTILE);
                            break;
                        case GlobalBiome.WETLAND:
                            graphics.Add(SWAMPTILE);
                            break;
                        case GlobalBiome.WOODLAND:
                            graphics.Add(TREE);
                            graphics.Add(GRASSTILE);
                            break;
                        case GlobalBiome.POLAR_FOREST:
                            graphics.Add(DEADTREE);
                            graphics.Add(SNOWTILE);
                            break;

                    }
                }
                else
                {
                    //default
                    graphics.Add(GRASSTILE);
                }

                //Do we have a slope?

                if (HasHillSlope)
                {
                    graphics.Insert(0, HILLSLOPE);
                }

                //Do we have a river?

                if (HasRiver)
                {
                    graphics.Insert(0, RIVER);
                }

                //do we have a mountain?

                if (Elevation > 250)
                {
                    graphics.Insert(0, MOUNTAIN);
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

        public SpriteData GetGraphicsByOverlay(GlobalOverlay overlay)
        {
            if (overlay.Equals(GlobalOverlay.NONE))
            {
                return null;
            }

            #region Region Overlay
            else if (overlay.Equals(GlobalOverlay.REGION))
            {
                //if the elevation is underwater, nothing
                if (this.Elevation < 0)
                {
                    return null;
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

                return null;

            }
            #endregion
            #region Temperature Overlay
            else if (overlay.Equals(GlobalOverlay.TEMPERATURE))
            {

                //Don't overlay the sea
                if (this.Elevation < 0)
                {
                    return null;
                }

                if (this.ClimateTemperature > 30)
                {
                    return RED;
                }
                else if (this.ClimateTemperature > 20)
                {
                    return ORANGE;
                }
                else if (this.ClimateTemperature > 10)
                {
                    return YELLOW;
                }
                else if (this.ClimateTemperature > 0)
                {
                    return INDIGO;
                }
                else if (this.ClimateTemperature > -10)
                {
                    return MARBLEBLUE;
                }
                else if (this.ClimateTemperature < -10)
                {
                    return WHITE;
                }

                return null;
            }
            #endregion
            #region Rainfall Overlay
            else if (overlay.Equals(GlobalOverlay.RAINFALL))
            {

                if (Elevation < 0)
                {
                    return null;
                }

                if (Rainfall > 8)
                {
                    //really wet
                    return RED;

                }
                else if (Rainfall > 6)
                {
                    //wet
                    return ORANGE;
                }
                else if (Rainfall > 4)
                {
                    return YELLOW;
                }
                else if (Rainfall > 2)
                {
                    return INDIGO;
                }
                else if (Rainfall < 2)
                {
                    return null;
                }

            }
            #endregion
            #region Elevation Overlay
            else if (overlay == GlobalOverlay.ELEVATION)
            {
                if (Elevation > 250)
                {
                    //mountain
                    return RED;
                }
                else if (Elevation > 100)
                {
                    return ORANGE;
                }
                else if (Elevation > 50)
                {
                    return YELLOW;
                }
                else if (Elevation > 25)
                {
                    return INDIGO;
                }
                else if (Elevation > 0)
                {
                    return WHITE;
                }
                else
                {
                    return null;
                }
            }
            #endregion
            #region Desirability Overlay
            else if (overlay == GlobalOverlay.DESIRABILITY)
            {
                if (BaseDesirability > 10)
                {
                    return GREEN;
                }
                else if (BaseDesirability > 5)
                {
                    return YELLOW;
                }
                else if (BaseDesirability > 0)
                {
                    return ORANGE;
                }
                else if (BaseDesirability <= 0)
                {
                    return RED;
                }
            }
            #endregion
            #region Owner
            else if (overlay == GlobalOverlay.OWNER)
            {
                if (Owner == null)
                {
                    return null;
                }
                
                switch (Owner.Value)
                {
                    case 0: return BROWN;
                    case 1: return GREEN;
                    case 2: return ORANGE;
                    case 3: return PINK;
                    case 4: return PURPLE;
                    case 5: return RED;
                    case 6: return YELLOW;
                }
            }
            #endregion
            return null;

        }

        #endregion

        #region Constructor

        public GlobalTile()
        {
            Elevation = 0;
            Region = -1;
            base.MayContainItems = true;
            IsBlockedForColonisation = false;
            HasResource = false;
        }

        #endregion
    }
}
